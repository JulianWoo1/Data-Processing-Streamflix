using Microsoft.AspNetCore.Mvc;
using Streamflix.Api.Services;
using Streamflix.Api.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Streamflix.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReferralController : ControllerBase
{
    private readonly IReferralService _service;

    public ReferralController(IReferralService service)
    {
        _service = service;
    }

    [HttpPost("createInvitation")]
    public async Task<IActionResult> Create([FromBody] CreateInvitationDto dto)
    {
        var referral = await _service.CreateInvitationAsync(dto);
        return Ok(referral);
    }

    [HttpPost("acceptInvitation")]
    public async Task<IActionResult> Accept([FromBody] AcceptInvitationDto dto)
    {
        var success = await _service.AcceptInvitationAsync(dto);
        return success ? Ok("Referral accepted") : BadRequest("Invalid referral code");
    }

    [HttpGet("getReferralStatus/{invitationCode}")]
    public async Task<IActionResult> GetStatus(string invitationCode)
    {
        var status = await _service.GetReferralStatusAsync(invitationCode);
        return Ok(new { invitationCode, status });
    }

    [HttpGet("getDiscount/{accountId}")]
    public async Task<IActionResult> GetDiscount(int accountId)
    {
        var discount = await _service.GetDiscountAsync(accountId);
        return Ok(discount);
    }
}
