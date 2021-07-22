using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ChipsMMO.Services;
using ChipsMMO.Models.Request;
using ChipsMMO.Models.Exceptions;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

namespace ChipsMMO.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        private readonly TokenService _tokenService;

        public AccountController(AccountService accountService, TokenService tokenService)
        {
            _accountService = accountService;
            _tokenService = tokenService;
        }

        [HttpPost]
        [Route("new-account")]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest createAccountRequest)
        {
            try
            {
                await _accountService.CreateAccount(createAccountRequest);
                return Ok();
            }
            catch (AccountAlreadyExists)
            {
                return BadRequest("Account already exists");
            }
        }

        [HttpGet]
        [Route("info")]
        [Authorize]
        public async Task<IActionResult> GetAccountInfo()
        {
            string username = _tokenService.GetUserNameFromBearerToken(Request.Headers[HeaderNames.Authorization].ToString());

            try
            {
                return Ok(await _accountService.GetAccountInfo(username));
            } catch (AccountNotFound)
            {
                return BadRequest("Account not found");
            }
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            try
            {
                return Ok(await _accountService.Login(loginRequest));
            } catch (InvalidLoginCredentials)
            {
                return Unauthorized("Incorrect username/password");
            }
        }

        [HttpDelete]
        [Route("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            string username = _tokenService.GetUserNameFromBearerToken(Request.Headers[HeaderNames.Authorization].ToString());

            await _accountService.Logout(username);
            return NoContent();
        }

        [HttpPost]
        [Route("token")]
        public async Task<IActionResult> UpdateAccessToken(RefreshTokenRequest refreshTokenRequest)
        {
            try
            {
                return Ok(await _accountService.UpdateAccessToken(refreshTokenRequest));
            } catch (RefreshTokenDoesNotMatch)
            {
                return BadRequest("Refresh token does not match");
            } catch (RefreshTokenInvalid)
            {
                return BadRequest("Refresh token invalid");
            }
        }
    }
}
