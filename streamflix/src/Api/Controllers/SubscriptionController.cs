using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Data;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("subscription")]
public class SubscriptionController : ControllerBase
{
    private readonly ISubscriptionService _service;

    public SubscriptionController(ISubscriptionService service)
    {
        _service = service;
    }

    [HttpGet("{accountId}")]
    public async Task<ActionResult<Subscription>> GetSubscription(int accountId)
    {
        var subscription = await _service.GetSubscriptionAsync(accountId);
        if(subscription == null)
        {
            return NotFound("Subscription not found");
        }

        return Ok(subscription);
    }

    [HttpPost]
    public async Task<ActionResult<Subscription>> CreateSubscription([FromBody] CreateSubscriptionDto dto)
    {
        var sub = await _service.CreateSubscriptionAsync(dto);
        return CreatedAtAction(nameof(GetSubscription), new 
        {
             accountId = sub.AccountId 
        }, sub);
    }

    [HttpPut("{subscriptionId}")]
    public async Task<ActionResult<Subscription?>> UpgradeSubscription(int subscriptionId, [FromBody] ChangeSubscriptionDto dto)
    {
        if (subscriptionId != dto.SubscriptionId)
            return BadRequest("SubscriptionId mismatch");

        var updated = await _service.ChangeSubscriptionAsync(dto);
        if (updated == null)
            return NotFound("Subscription not found or inactive");

        return Ok(updated);
    }

    [HttpDelete("{subscriptionId}")]
    public async Task<ActionResult> CancelSubscription(int subscriptionId)
    {
        var success = await _service.CancelSubscriptionAsync(subscriptionId);
        if(!success)
        {
            return NotFound("subscription not found");
        }

        return NoContent();
    }

    [HttpGet("plans")]
    public ActionResult<IEnumerable<string>> GetAvailablePlans()
    {
        var plans = new []
        {
            "SD",
            "HD",
            "UHD"
        };

        return Ok(plans);
    }
}
