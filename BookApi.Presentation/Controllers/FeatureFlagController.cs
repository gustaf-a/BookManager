using Microsoft.FeatureManagement;
using Microsoft.AspNetCore.Mvc;
using Shared;

namespace BookApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FeatureFlagController : ControllerBase
{
    private readonly IFeatureManager _featureManager;

    public FeatureFlagController(IFeatureManager featureManager)
    {
        _featureManager = featureManager;
    }

    [HttpGet(FeatureFlags.UsqSqlDatabase)]
    public async Task<IActionResult> UseSqlDatabaseStatus()
    {
        if (await _featureManager.IsEnabledAsync(FeatureFlags.UsqSqlDatabase))
        {
            return Ok($"Feature {FeatureFlags.UsqSqlDatabase} enabled");
        }
        else
        {
            return BadRequest($"Feature {FeatureFlags.UsqSqlDatabase} not enabled");
        }
    }
}
