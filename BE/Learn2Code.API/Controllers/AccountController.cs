using Learn2Code.Application.Base;
using Learn2Code.Application.DTOs;
using Learn2Code.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Learn2Code.API.Controllers;

[ApiController]
[Route("api/accounts")]
// [Authorize(Roles = "Admin")] // Uncomment when roles are fully setup
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _accountService.GetAllAccountsAsync();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _accountService.GetAccountByIdAsync(id);
        return result.Success ? Ok(result) : NotFound(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountRequest request)
    {
        var result = await _accountService.CreateAccountAsync(request);
        return result.Success ? StatusCode(201, result) : BadRequest(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateAccountRequest request)
    {
        var result = await _accountService.UpdateAccountAsync(id, request);
        return result.Success ? Ok(result) : BadRequest(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _accountService.DeleteAccountAsync(id);
        return result.Success ? Ok(result) : BadRequest(result);
    }
}
