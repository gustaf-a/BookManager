using Contracts;
using Microsoft.FeatureManagement;
using Service.Contracts;
using Service.SQL;
using Shared;

namespace Service;

public class ServiceManagerFactory : IServiceManagerFactory
{
    private readonly ILoggerManager _logger;
    private readonly IFeatureManager _featureManager;
    private readonly IServiceProvider _serviceProvider;

    public ServiceManagerFactory(IFeatureManager featureManager, IServiceProvider serviceProvider, ILoggerManager logger)
    {
        _logger = logger;
        _featureManager = featureManager;
        _serviceProvider = serviceProvider;
    }

    public IServiceManager GetServiceManager()
    {
        if(_featureManager.IsEnabledAsync(FeatureFlags.UsqSqlDatabase).Result)
        {
            return (IServiceManager)_serviceProvider.GetService(typeof(ServiceSqlManager)) 
                ?? throw new Exception($"Failed to resolve {nameof(ServiceSqlManager)}. Ensure DI registration is correct.");
        }
        else
        {
            throw new NotImplementedException($"Support for alternative to feature {nameof(FeatureFlags.UsqSqlDatabase)} not implemented. Please set feature flag in appsettings.json to true.");
        }
    }
}
