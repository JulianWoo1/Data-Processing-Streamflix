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
}
