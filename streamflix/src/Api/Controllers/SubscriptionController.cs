using Microsoft.AspNetCore.Mvc;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionController(ISubscriptionService service)
    {
        _service = service;
    }

    private int GetCurrentAccountId()
    {
        var claim = User.FindFirst(JwtRegisteredClaimNames.Sub) 
            ?? User.FindFirst(ClaimTypes.NameIdentifier);
        if (claim == null) throw new InvalidOperationException("No account id claim present.");
        return int.Parse(claim.Value);
    }

    [HttpGet("{accountId}")]
    public async Task<ActionResult<Subscription>> GetMySubscription()
    {
        var accountId = GetCurrentAccountId();
        var subscription = await _service.GetSubscriptionAsync(accountId);

        if (subscription == null)
        {
            return NotFound("Subscription not found");
        }

        return Ok(subscription);
    }

    [HttpPost]
    public async Task<ActionResult<Subscription>> CreateSubscription([FromBody] CreateSubscriptionDto dto)
    {
        var accountId = GetCurrentAccountId();

        var subscription = await _service.CreateSubscriptionAsync(accountId, dto);

        return CreatedAtAction(nameof(GetMySubscription), subscription);
    }

    [HttpPut("{subscriptionId}")]
    public async Task<ActionResult<Subscription?>> ChangeSubscription(int subscriptionId, [FromBody] ChangeSubscriptionDto dto)
    {
        var accountId = GetCurrentAccountId();

        var updated = await _service.ChangeSubscriptionAsync(accountId, subscriptionId, dto);

        if (updated == null)
        {
            return NotFound("Subscription not found or inactive");
        }

        return Ok(updated);
    }

    [HttpDelete("{subscriptionId}")]
    public async Task<ActionResult> CancelSubscription(int subscriptionId)
    {
        var accountId = GetCurrentAccountId();
        var success = await _service.CancelSubscriptionAsync(accountId, subscriptionId);

        if (!success)
        {
            return NotFound("Subscription not found");
        }

        return NoContent();
    }

    [HttpPost("renew/{subscriptionId}")]
    public async Task<ActionResult<Subscription?>> RenewSubscription(int subscriptionId)
    {
        var accountId = GetCurrentAccountId();

        var renewed = await _service.RenewSubscriptionAsync(accountId, subscriptionId);

        if (renewed == null)
        {
            return NotFound();
        }

        return Ok(renewed);
    }

    [HttpGet("plans")]
    [AllowAnonymous]
    public ActionResult<IEnumerable<object>> GetPlans()
    {
        return Ok(new[]
        {
            new { Type = "SD",  Price = 7.99 },
            new { Type = "HD",  Price = 10.99 },
            new { Type = "UHD", Price = 13.99 }
        });
    }
}
