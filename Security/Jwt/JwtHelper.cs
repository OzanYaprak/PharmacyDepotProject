using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Security.Encryption;
using Security.Entities;
using Security.Extensions;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Security.Jwt;

public class JwtHelper : ITokenHelper
{
    public IConfiguration Configuration { get; }
    private readonly TokenOptions _tokenOptions;
    private DateTime _accessTokenExpiration;

    public JwtHelper(IConfiguration configuration)
    {
        const string configurationSection = "TokenOptions";
        Configuration = configuration;
        _tokenOptions = Configuration.GetSection(configurationSection).Get<TokenOptions>() ?? throw new NullReferenceException($"{configurationSection} configuration is missing.");
    }

    public RefreshToken CreateRefreshToken(User user, string ipAddress)
    {
        RefreshToken refreshToken = new RefreshToken()
        {
            UserId = user.Id,
            Token = RandomRefreshToken(),
            Expires = DateTime.UtcNow.AddDays(7),
            CreatedIp = ipAddress
        };

        return refreshToken;
    }

    public AccessToken CreateToken(User user, IList<OperationClaim> operationClaims)
    {
        _accessTokenExpiration = DateTime.Now.AddMinutes(_tokenOptions.AccessTokenExpiration);
        
        SecurityKey securityKey = SecurityKeyHelper.CreateSecurityKey(_tokenOptions.SecurityKey);
        
        SigningCredentials signingCredentials = SigningCredentialsHelper.CreateSigningCredentials(securityKey);

        JwtSecurityToken jwtSecurityToken = CreateJwtSecurityToken(_tokenOptions, user, signingCredentials, operationClaims);
        
        string token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        
        return new AccessToken { Token = token , Expiration = _accessTokenExpiration };
    }


    public JwtSecurityToken CreateJwtSecurityToken(TokenOptions tokenOptions, User user, SigningCredentials signingCredentials, IList<OperationClaim> operationClaims)
    {
        JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
            issuer: tokenOptions.Issuer,
            audience: tokenOptions.Audience,
            expires: _accessTokenExpiration,
            notBefore: DateTime.Now,
            claims: GetClaims(user, operationClaims),
            signingCredentials: signingCredentials
        );
        return jwtSecurityToken;
    }

    private IEnumerable<Claim> GetClaims(User user, IList<OperationClaim> operationClaims)
    {
        List<Claim> claims = new List<Claim>();

        claims.AddNameIdentifier(user.Id.ToString());
        claims.AddEmail(user.Email);
        claims.AddName($"{user.FirstName} {user.LastName}");
        claims.AddRoles(operationClaims.Select(c => c.Name).ToArray());

        return claims;
    }

    private string RandomRefreshToken()
    {
        byte[] randomBytes = new byte[32];

        using var randomNumber = RandomNumberGenerator.Create();

        randomNumber.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

}
