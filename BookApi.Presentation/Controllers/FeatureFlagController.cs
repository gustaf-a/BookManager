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

    [HttpGet(FeatureFlags.UseSqlDatabase)]
    public async Task<IActionResult> UseSqlDatabaseStatus()
    {
        if (await _featureManager.IsEnabledAsync(FeatureFlags.UseSqlDatabase))
        {
            return Ok($"Feature {FeatureFlags.UseSqlDatabase} enabled");
        }
        else
        {
            return BadRequest($"Feature {FeatureFlags.UseSqlDatabase} not enabled");
        }
    }
}
