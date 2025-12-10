using Microsoft.AspNetCore.Mvc;
using Streamflix.Api.DTOs;
using Streamflix.Infrastructure.Entities;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        if (subscription == null)
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
    public async Task<ActionResult<Subscription?>> ChangeSubscription(int subscriptionId, [FromBody] ChangeSubscriptionDto dto)
    {
        if (subscriptionId != dto.SubscriptionId)
        {
            return BadRequest("SubscriptionId mismatch");
        }

        var updated = await _service.ChangeSubscriptionAsync(dto);

        if (updated == null)
        {
            return NotFound("Subscription not found or inactive");
        }

        return Ok(updated);
    }

    [HttpDelete("{subscriptionId}")]
    public async Task<ActionResult> CancelSubscription(int subscriptionId)
    {
        var success = await _service.CancelSubscriptionAsync(subscriptionId);
        if (!success)
        {
            return NotFound("Subscription not found");
        }

        return NoContent();
    }

    [HttpPost("renew/{subscriptionId}")]
    public async Task<ActionResult<Subscription?>> RenewSubscription(int subscriptionId)
    {
        var renewed = await _service.RenewSubscriptionAsync(subscriptionId);
        if (renewed == null)
        {
            return NotFound();
        }

        return Ok(renewed);
    }

    [HttpGet("plans")]
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
