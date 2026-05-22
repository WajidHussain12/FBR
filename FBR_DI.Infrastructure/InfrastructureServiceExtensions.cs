using FBR_DI.Application.Interfaces;
using FBR_DI.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;

namespace FBR_DI.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services, IConfiguration configuration)
    {
        bool useMock = configuration.GetValue<bool>("FbrApi:UseMockService", true);

        // Always register IHttpClientFactory so SettingsController can validate
        // PRAL tokens via direct HTTP calls regardless of the API mode in use.
        services.AddHttpClient();

        if (useMock)
        {
            services.AddScoped<IFbrApiService, MockFbrApiService>();
        }
        else
        {
            var retryPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .WaitAndRetryAsync(
                    configuration.GetValue<int>("FbrApi:RetryCount", 3),
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

            var circuitBreakerPolicy = HttpPolicyExtensions
                .HandleTransientHttpError()
                .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));

            services.AddHttpClient("FbrApiClient", client =>
            {
                client.Timeout = TimeSpan.FromSeconds(
                    configuration.GetValue<int>("FbrApi:TimeoutSeconds", 30));
            })
            .AddPolicyHandler(retryPolicy)
            .AddPolicyHandler(circuitBreakerPolicy);

            services.AddScoped<IFbrApiService, FbrApiService>();
        }

        services.AddSingleton<IQrCodeService, QrCodeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}
