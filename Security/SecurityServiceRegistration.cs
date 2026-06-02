using Microsoft.Extensions.DependencyInjection;
using Security.Jwt;
using Security.OtpAuthenticator.Interfaces;
using Security.OtpAuthenticator.OtpNet;

namespace Security;

public static class SecurityServiceRegistration
{
    public static IServiceCollection AddSecurityServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenHelper, JwtHelper>();
        //services.AddScoped<IEmailAuthenticator, EmailAuthenticator>();
        services.AddScoped<IOtpAuthenticatorHelper, OtpNetOtpAuthenticatorHelper>();

        return services;
    }
}
