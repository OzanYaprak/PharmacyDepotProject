namespace Application.Rules;

// Bu dosya, tüm iş kuralı (Business Rules) sınıflarının türemesi gereken temel sınıfı tanımlar.
// Amaç: DI container'ında otomatik tespiti mümkün kılmak.
// ApplicationServiceRegistration, bu sınıfın alt sınıflarını Reflection ile bulup Scoped kaydeder.
// İleride tüm kural sınıflarına uygulanacak ortak davranış (logging, audit) buraya eklenebilir.

/// <summary>
/// Tüm *BusinessRules sınıflarının türemesi zorunlu temel sınıf.
/// DI otomatik kayıt için işaretleyici (marker) görevi görür.
/// </summary>
public class BaseBusinessRules
{
}
