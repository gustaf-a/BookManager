using Contracts;
using Microsoft.FeatureManagement;
using Service.Contracts;
using Service.EF;
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
        if(_featureManager.IsEnabledAsync(FeatureFlags.UseSqlDatabase).Result)
            return ResolveServiceManager(typeof(ServiceSqlManager));
        else
            return ResolveServiceManager(typeof(ServiceEfManager));
    }

    private IServiceManager ResolveServiceManager(Type serviceManagerType)
    {
        _logger.LogInfo($"Using {serviceManagerType.Name}");

        IServiceManager serviceManager;

        try
        {
            serviceManager = (IServiceManager)_serviceProvider.GetService(serviceManagerType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error when resolving ServiceManager: {serviceManagerType.Name}");

            throw;
        }

        return serviceManager ?? throw new Exception($"Failed to resolve ServiceManager '{serviceManagerType.Name}'. Ensure services are registered with DI.");
    }
       
}
