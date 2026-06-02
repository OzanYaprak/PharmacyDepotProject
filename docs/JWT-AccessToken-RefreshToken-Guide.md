# JWT ile Access Token & Refresh Token Uygulaması
### .NET Clean Architecture — Kapsamlı Geliştirici Rehberi

---

## İçindekiler

1. [Temel Kavramlar](#1-temel-kavramlar)
2. [Proje Yapısı](#2-proje-yapısı)
3. [Gerekli NuGet Paketleri](#3-gerekli-nuget-paketleri)
4. [appsettings Konfigürasyonu](#4-appsettings-konfigürasyonu)
5. [Entity Modelleri](#5-entity-modelleri)
6. [Şifre Hashleme](#6-şifre-hashleme)
7. [Token Üretimi — JwtHelper](#7-token-üretimi--jwthelper)
8. [Kayıt (Register) Akışı](#8-kayıt-register-akışı)
9. [Giriş (Login) Akışı](#9-giriş-login-akışı)
10. [Token Yenileme (Refresh) Akışı](#10-token-yenileme-refresh-akışı)
11. [Token İptal (Revoke) Akışı](#11-token-i̇ptal-revoke-akışı)
12. [Middleware — JWT Doğrulama](#12-middleware--jwt-doğrulama)
13. [Claim'lere Erişim](#13-claimlere-erişim)
14. [Güvenlik İpuçları](#14-güvenlik-i̇puçları)
15. [Tam Akış Diyagramı](#15-tam-akış-diyagramı)

---

## 1. Temel Kavramlar

| Kavram | Açıklama |
|---|---|
| **Access Token** | Kısa ömürlü (5–60 dk), her istekte `Authorization: Bearer` başlığında gönderilir |
| **Refresh Token** | Uzun ömürlü (7–30 gün), access token yenilemek için kullanılır, DB'de saklanır |
| **Claim** | Token içine gömülen kullanıcı bilgisi (id, email, rol vb.) |
| **Signing Key** | Token'ı imzalamak için kullanılan gizli anahtar (min. 32 karakter) |
| **Salt** | Şifre hash'ini benzersiz kılmak için kullanılan rastgele byte dizisi |

---

## 2. Proje Yapısı

```
Solution/
├── Domain/
│   └── Entities/Base/BaseEntity.cs
├── Security/                          ← Bu rehberin odak katmanı
│   ├── Encryption/
│   │   ├── SecurityKeyHelper.cs
│   │   └── SigningCredentialsHelper.cs
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── RefreshToken.cs
│   │   └── OperationClaim.cs
│   ├── Extensions/
│   │   ├── ClaimsExtension.cs
│   │   └── ClaimPrincipalExtensions.cs
│   ├── Hashing/
│   │   └── HashingHelper.cs
│   └── Jwt/
│       ├── ITokenHelper.cs
│       ├── JwtHelper.cs
│       ├── AccessToken.cs
│       └── TokenOptions.cs
├── Application/
│   └── Features/Auth/
│       ├── Commands/Login/
│       ├── Commands/Register/
│       └── Commands/RefreshToken/
├── Persistence/
│   └── Repositories/RefreshTokenRepository.cs
└── WebAPI/
	└── Controllers/AuthController.cs
```

---

## 3. Gerekli NuGet Paketleri

```xml
<!-- Security projesi -->
<PackageReference Include="Microsoft.IdentityModel.Tokens" Version="8.*" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="8.*" />

<!-- WebAPI projesi -->
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="10.*" />
```

---

## 4. appsettings Konfigürasyonu

```json
{
  "TokenOptions": {
	"Audience": "PharmacyDepot",
	"Issuer": "PharmacyDepot",
	"AccessTokenExpiration": 30,
	"RefreshTokenTTL": 7,
	"SecurityKey": "PharmacyDepotSuperSecretKeyMinimum32Chars!"
  }
}
```

> ⚠️ `SecurityKey` production'da **User Secrets** veya **Azure Key Vault**'ta saklanmalıdır, asla kaynak koduna eklenmemelidir.

---

## 5. Entity Modelleri

### TokenOptions.cs
```csharp
namespace Security.Jwt;

public class TokenOptions
{
	public string Audience { get; set; } = string.Empty;
	public string Issuer { get; set; } = string.Empty;
	public int AccessTokenExpiration { get; set; }   // dakika
	public string SecurityKey { get; set; } = string.Empty;
	public int RefreshTokenTTL { get; set; }          // gün
}
```

### AccessToken.cs
```csharp
namespace Security.Jwt;

public class AccessToken
{
	public string Token { get; set; } = string.Empty;
	public DateTime Expiration { get; set; }
}
```

### User.cs
```csharp
namespace Security.Entities;

public class User : BaseEntity<int>
{
	public string FirstName { get; set; } = string.Empty;
	public string LastName { get; set; } = string.Empty;
	public string Email { get; set; } = string.Empty;
	public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
	public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
	public bool IsActive { get; set; } = true;

	public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
```

### RefreshToken.cs
```csharp
namespace Security.Entities;

public class RefreshToken : BaseEntity<int>
{
	public int UserId { get; set; }
	public string Token { get; set; } = string.Empty;
	public DateTime Expires { get; set; }
	public string CreatedIp { get; set; } = string.Empty;

	// Revoke bilgileri — null ise token hâlâ aktiftir
	public DateTime? Revoked { get; set; }
	public string? RevokedIp { get; set; }
	public string? ReplacedToken { get; set; }  // hangi token ile değiştirildi
	public string? RevokeReason { get; set; }

	// Hesaplanan özellikler
	public bool IsExpired => DateTime.UtcNow >= Expires;
	public bool IsRevoked => Revoked != null;
	public bool IsActive => !IsRevoked && !IsExpired;

	public virtual User User { get; set; } = null!;
}
```

---

## 6. Şifre Hashleme

HMAC-SHA512 algoritması kullanılır. Her kullanıcı için rastgele bir `salt` üretilir; böylece aynı şifreye sahip iki kullanıcının hash değerleri birbirinden farklı olur.

```csharp
using System.Security.Cryptography;
using System.Text;

namespace Security.Hashing;

public static class HashingHelper
{
	/// <summary>Yeni kayıt için şifre hash + salt üretir.</summary>
	public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
	{
		using HMACSHA512 hmac = new HMACSHA512();

		passwordSalt = hmac.Key;                                        // rastgele salt
		passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
	}

	/// <summary>Giriş sırasında şifreyi doğrular.</summary>
	public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
	{
		using HMACSHA512 hmac = new HMACSHA512(passwordSalt);           // aynı salt ile

		byte[] computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
		return computed.SequenceEqual(passwordHash);
	}
}
```

**Neden HMACSHA512?**
- Tek yönlüdür: hash'ten şifreye dönülemez.
- Salt sayesinde rainbow table saldırılarına karşı koruma sağlar.
- SHA512 = 512 bit çıktı → brute-force maliyeti çok yüksektir.

---

## 7. Token Üretimi — JwtHelper

### Yardımcı sınıflar

```csharp
// SecurityKeyHelper.cs
public static class SecurityKeyHelper
{
	public static SecurityKey CreateSecurityKey(string securityKey) =>
		new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
}

// SigningCredentialsHelper.cs
public static class SigningCredentialsHelper
{
	public static SigningCredentials CreateSigningCredentials(SecurityKey key) =>
		new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
}
```

### ClaimsExtension.cs
```csharp
public static class ClaimsExtension
{
	public static void AddEmail(this ICollection<Claim> claims, string email) =>
		claims.Add(new Claim(JwtRegisteredClaimNames.Email, email));

	public static void AddName(this ICollection<Claim> claims, string name) =>
		claims.Add(new Claim(JwtRegisteredClaimNames.Name, name));

	public static void AddNameIdentifier(this ICollection<Claim> claims, string id) =>
		claims.Add(new Claim(ClaimTypes.NameIdentifier, id));

	public static void AddRoles(this ICollection<Claim> claims, string[] roles) =>
		roles.ToList().ForEach(role => claims.Add(new Claim(ClaimTypes.Role, role)));
}
```

### ITokenHelper.cs
```csharp
public interface ITokenHelper
{
	AccessToken CreateToken(User user, IList<OperationClaim> operationClaims);
	RefreshToken CreateRefreshToken(User user, string ipAddress);
}
```

### JwtHelper.cs
```csharp
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Security.Encryption;
using Security.Entities;
using Security.Extensions;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Security.Jwt;

public class JwtHelper : ITokenHelper
{
	private readonly TokenOptions _tokenOptions;

	public JwtHelper(IConfiguration configuration)
	{
		_tokenOptions = configuration
			.GetSection("TokenOptions")
			.Get<TokenOptions>()
			?? throw new InvalidOperationException("TokenOptions configuration is missing.");
	}

	public AccessToken CreateToken(User user, IList<OperationClaim> operationClaims)
	{
		DateTime expiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);

		SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
		SigningCredentials credentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);

		JwtSecurityToken jwt = new JwtSecurityToken(
			issuer: _tokenOptions.Issuer,
			audience: _tokenOptions.Audience,
			expires: expiration,
			notBefore: DateTime.Now,
			claims: BuildClaims(user, operationClaims),
			signingCredentials: credentials
		);

		string tokenString = new JwtSecurityTokenHandler().WriteToken(jwt);

		return new AccessToken(tokenString, expiration);
	}

	public RefreshToken CreateRefreshToken(User user, string ipAddress)
	{
		return new RefreshToken(
			userId: user.Id,
			token: GenerateRandomToken(),
			expires: DateTime.UtcNow.AddDays(_tokenOptions.RefreshTokenTTL),
			createdIp: ipAddress
		);
	}

	private static IEnumerable<Claim> BuildClaims(User user, IList<OperationClaim> claims)
	{
		List<Claim> claimList = [];

		claimList.AddNameIdentifier(user.Id.ToString());
		claimList.AddEmail(user.Email);
		claimList.AddName($"{user.FirstName} {user.LastName}");
		claimList.AddRoles(claims.Select(c => c.Name).ToArray());

		return claimList;
	}

	private static string GenerateRandomToken()
	{
		byte[] bytes = new byte[32];
		RandomNumberGenerator.Fill(bytes);
		return Convert.ToBase64String(bytes);
	}
}
```

---

## 8. Kayıt (Register) Akışı

```
Client → POST /api/auth/register { firstName, lastName, email, password }
	↓
1. Email daha önce kayıtlı mı? → Varsa hata fırlat
	↓
2. HashingHelper.CreatePasswordHash(password, out hash, out salt)
	↓
3. User { ..., PasswordHash, PasswordSalt } → DB'ye kaydet
	↓
4. (Opsiyonel) Otomatik giriş → CreateToken + CreateRefreshToken
	↓
Client ← { accessToken, refreshToken } veya { message: "Kayıt başarılı" }
```

### Register Command Handler (CQRS örneği)
```csharp
public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
	private readonly IUserRepository _userRepository;
	private readonly ITokenHelper _tokenHelper;

	public RegisterCommandHandler(IUserRepository userRepository, ITokenHelper tokenHelper)
	{
		_userRepository = userRepository;
		_tokenHelper = tokenHelper;
	}

	public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
	{
		bool emailExists = await _userRepository.AnyAsync(u => u.Email == request.Email, cancellationToken);
		if (emailExists)
			throw new InvalidOperationException("Bu e-posta zaten kayıtlı.");

		HashingHelper.CreatePasswordHash(request.Password, out byte[] hash, out byte[] salt);

		User user = new(request.FirstName, request.LastName, request.Email, salt, hash, isActive: true);
		await _userRepository.AddAsync(user, cancellationToken);

		AccessToken accessToken = _tokenHelper.CreateToken(user, []);
		RefreshToken refreshToken = _tokenHelper.CreateRefreshToken(user, request.IpAddress);
		await _userRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);

		return new AuthResponseDto(accessToken.Token, accessToken.Expiration, refreshToken.Token);
	}
}
```

---

## 9. Giriş (Login) Akışı

```
Client → POST /api/auth/login { email, password }
	↓
1. User email ile DB'den bulunur → bulunamazsa hata
	↓
2. HashingHelper.VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)
   → Eşleşmiyorsa hata
	↓
3. Kullanıcının rolleri (OperationClaim listesi) DB'den çekilir
	↓
4. JwtHelper.CreateToken(user, operationClaims)
   a) expiration = DateTime.Now + AccessTokenExpiration(dk)
   b) SecurityKey → SymmetricSecurityKey
   c) SigningCredentials → HMAC-SHA512
   d) Claims: NameIdentifier, Email, Name, Roles
   e) JwtSecurityToken → WriteToken() → string
   → AccessToken { Token, Expiration }
	↓
5. JwtHelper.CreateRefreshToken(user, ipAddress)
   → 32 rastgele byte → Base64
   → RefreshToken { UserId, Token, Expires(+7gün), CreatedIp }
	↓
6. Eski geçersiz refresh token'lar DB'den temizlenir (opsiyonel)
7. Yeni RefreshToken DB'ye kaydedilir
	↓
Client ← { accessToken, refreshToken }
```

### Login Command Handler
```csharp
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
	private readonly IUserRepository _userRepository;
	private readonly ITokenHelper _tokenHelper;

	public LoginCommandHandler(IUserRepository userRepository, ITokenHelper tokenHelper)
	{
		_userRepository = userRepository;
		_tokenHelper = tokenHelper;
	}

	public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
	{
		User? user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
		if (user is null)
			throw new UnauthorizedAccessException("Geçersiz e-posta veya şifre.");

		bool passwordValid = HashingHelper.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt);
		if (!passwordValid)
			throw new UnauthorizedAccessException("Geçersiz e-posta veya şifre.");

		IList<OperationClaim> roles = await _userRepository.GetRolesAsync(user.Id, cancellationToken);

		AccessToken accessToken = _tokenHelper.CreateToken(user, roles);
		RefreshToken refreshToken = _tokenHelper.CreateRefreshToken(user, request.IpAddress);
		await _userRepository.AddRefreshTokenAsync(refreshToken, cancellationToken);

		return new AuthResponseDto(accessToken.Token, accessToken.Expiration, refreshToken.Token);
	}
}
```

---

## 10. Token Yenileme (Refresh) Akışı

Access token süresi dolduğunda client, refresh token göndererek yeni bir çift alır. **Rotation** prensibi uygulanır: her yenilemede eski refresh token iptal edilir, yerine yeni bir tane oluşturulur.

```
Client → POST /api/auth/refresh { refreshToken, ipAddress }
	↓
1. RefreshToken DB'den bulunur → bulunamazsa 401
	↓
2. token.IsRevoked → true ise "Token zaten iptal edilmiş" → 401
	↓
3. token.IsExpired → true ise "Token süresi dolmuş" → 401
	↓
4. Yeni AccessToken üretilir → JwtHelper.CreateToken()
	↓
5. Yeni RefreshToken üretilir → JwtHelper.CreateRefreshToken()
	↓
6. Eski RefreshToken revoke edilir:
   → Revoked = DateTime.UtcNow
   → RevokedIp = ipAddress
   → ReplacedToken = yeniToken
   → RevokeReason = "Replaced by new token"
	↓
7. Yeni RefreshToken DB'ye kaydedilir
	↓
Client ← { yeni accessToken, yeni refreshToken }
```

### RefreshToken Command Handler
```csharp
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
	private readonly IRefreshTokenRepository _refreshTokenRepository;
	private readonly IUserRepository _userRepository;
	private readonly ITokenHelper _tokenHelper;

	public RefreshTokenCommandHandler(
		IRefreshTokenRepository refreshTokenRepository,
		IUserRepository userRepository,
		ITokenHelper tokenHelper)
	{
		_refreshTokenRepository = refreshTokenRepository;
		_userRepository = userRepository;
		_tokenHelper = tokenHelper;
	}

	public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
	{
		RefreshToken? existingToken = await _refreshTokenRepository
			.GetByTokenAsync(request.Token, cancellationToken);

		if (existingToken is null)
			throw new UnauthorizedAccessException("Geçersiz refresh token.");

		if (existingToken.IsRevoked)
			throw new UnauthorizedAccessException("Bu token daha önce iptal edilmiştir.");

		if (existingToken.IsExpired)
			throw new UnauthorizedAccessException("Refresh token süresi dolmuş, lütfen tekrar giriş yapın.");

		IList<OperationClaim> roles = await _userRepository.GetRolesAsync(existingToken.UserId, cancellationToken);

		AccessToken newAccessToken = _tokenHelper.CreateToken(existingToken.User, roles);
		RefreshToken newRefreshToken = _tokenHelper.CreateRefreshToken(existingToken.User, request.IpAddress);

		// Rotation: eski token'ı revoke et
		existingToken.Revoked = DateTime.UtcNow;
		existingToken.RevokedIp = request.IpAddress;
		existingToken.ReplacedToken = newRefreshToken.Token;
		existingToken.RevokeReason = "Replaced by new token";

		await _refreshTokenRepository.UpdateAsync(existingToken, cancellationToken);
		await _refreshTokenRepository.AddAsync(newRefreshToken, cancellationToken);

		return new AuthResponseDto(newAccessToken.Token, newAccessToken.Expiration, newRefreshToken.Token);
	}
}
```

---

## 11. Token İptal (Revoke) Akışı

Kullanıcı çıkış yaptığında veya güvenlik ihlali tespit edildiğinde tüm refresh token'lar iptal edilir.

```csharp
public async Task RevokeAllTokensAsync(int userId, string ipAddress, string reason, CancellationToken ct)
{
	IList<RefreshToken> tokens = await _refreshTokenRepository
		.GetActiveTokensByUserIdAsync(userId, ct);

	foreach (RefreshToken token in tokens)
	{
		token.Revoked = DateTime.UtcNow;
		token.RevokedIp = ipAddress;
		token.RevokeReason = reason;
		await _refreshTokenRepository.UpdateAsync(token, ct);
	}
}
```

---

## 12. Middleware — JWT Doğrulama

### Program.cs
```csharp
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Token servisini DI'ye kaydet
builder.Services.AddScoped<ITokenHelper, JwtHelper>();

// JWT Authentication
builder.Services
	.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
	.AddJwtBearer(options =>
	{
		TokenOptions tokenOptions = builder.Configuration
			.GetSection("TokenOptions")
			.Get<TokenOptions>()!;

		options.TokenValidationParameters = new TokenValidationParameters
		{
			ValidateIssuer = true,
			ValidateAudience = true,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,

			ValidIssuer = tokenOptions.Issuer,
			ValidAudience = tokenOptions.Audience,
			IssuerSigningKey = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(tokenOptions.SecurityKey)
			),
			ClockSkew = TimeSpan.Zero  // Süre toleransı sıfır — tam dakikasında süresi dolar
		};
	});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseAuthentication();   // Önce authentication
app.UseAuthorization();    // Sonra authorization

app.Run();
```

### Controller'da Kullanım
```csharp
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
	[HttpGet]
	[Authorize]                          // Sadece giriş yapmış kullanıcılar
	public IActionResult GetAll() { ... }

	[HttpDelete("{id}")]
	[Authorize(Roles = "Admin")]         // Sadece Admin rolü
	public IActionResult Delete(int id) { ... }
}
```

---

## 13. Claim'lere Erişim

```csharp
public static class ClaimPrincipalExtensions
{
	public static int GetUserId(this ClaimsPrincipal principal)
	{
		string? value = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
		return int.TryParse(value, out int id) ? id : throw new UnauthorizedAccessException();
	}

	public static string GetEmail(this ClaimsPrincipal principal) =>
		principal.FindFirst(JwtRegisteredClaimNames.Email)?.Value
		?? throw new UnauthorizedAccessException();

	public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal) =>
		principal.FindAll(ClaimTypes.Role).Select(c => c.Value);
}

// Controller'da:
int currentUserId = User.GetUserId();
string email = User.GetEmail();
```

---

## 14. Güvenlik İpuçları

| Konu | Öneri |
|---|---|
| **SecurityKey uzunluğu** | En az 32 karakter (256 bit) kullanın |
| **HTTPS** | Refresh token'ı yalnızca HTTPS üzerinden gönderin |
| **HttpOnly Cookie** | Refresh token'ı `HttpOnly; Secure; SameSite=Strict` cookie'de saklayın |
| **Token Rotation** | Her yenilemede refresh token değiştirilmeli (tek kullanımlık) |
| **IP Kontrolü** | Refresh token ile IP değişikliği tespit edilirse revoke edin |
| **TTL Temizliği** | Süresi dolmuş token'ları periyodik olarak DB'den silin |
| **ClockSkew** | `ClockSkew = TimeSpan.Zero` ile token süresini kesinleştirin |
| **Secrets** | SecurityKey'i `User Secrets` veya `Azure Key Vault`'a taşıyın |
| **Rate Limiting** | Login endpoint'ine brute-force koruması ekleyin |

---

## 15. Tam Akış Diyagramı

```
┌─────────────┐        ┌─────────────────────┐        ┌──────────┐
│   Client    │        │      WebAPI          │        │    DB    │
└──────┬──────┘        └──────────┬──────────┘        └────┬─────┘
	   │                          │                         │
	   │  POST /auth/login        │                         │
	   │─────────────────────────>│                         │
	   │                          │  SELECT user            │
	   │                          │────────────────────────>│
	   │                          │<────────────────────────│
	   │                          │  VerifyPasswordHash     │
	   │                          │  CreateToken()          │
	   │                          │  CreateRefreshToken()   │
	   │                          │  INSERT RefreshToken    │
	   │                          │────────────────────────>│
	   │  { accessToken,          │                         │
	   │    refreshToken }        │                         │
	   │<─────────────────────────│                         │
	   │                          │                         │
	   │  GET /api/products       │                         │
	   │  Authorization: Bearer   │                         │
	   │─────────────────────────>│                         │
	   │                          │  Middleware: JWT doğrula│
	   │  200 OK { data }         │  (DB sorgusu yok!)      │
	   │<─────────────────────────│                         │
	   │                          │                         │
	   │  [AccessToken süresi doldu]                        │
	   │                          │                         │
	   │  POST /auth/refresh      │                         │
	   │  { refreshToken }        │                         │
	   │─────────────────────────>│                         │
	   │                          │  SELECT RefreshToken    │
	   │                          │────────────────────────>│
	   │                          │  IsActive? ✅           │
	   │                          │  Revoke old token       │
	   │                          │  INSERT new token       │
	   │                          │────────────────────────>│
	   │  { yeni accessToken,     │                         │
	   │    yeni refreshToken }   │                         │
	   │<─────────────────────────│                         │
	   │                          │                         │
	   │  POST /auth/logout       │                         │
	   │─────────────────────────>│                         │
	   │                          │  Revoke all tokens      │
	   │                          │────────────────────────>│
	   │  204 No Content          │                         │
	   │<─────────────────────────│                         │
```

---

*Bu doküman PharmacyDepot projesinin Security katmanı baz alınarak hazırlanmıştır.*
*Yazar: GitHub Copilot — .NET 10 / Clean Architecture*
