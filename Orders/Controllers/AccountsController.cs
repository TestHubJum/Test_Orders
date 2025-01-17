﻿using Application.Abstractions;
using Application.Models.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Orders.Controllers
{
    [Route("accounts")]
    public class AccountsController(IAuthService authService) : ApiBaseController
    {
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var result = await authService.Login(userLoginDto);

            return Ok(result);
        }        

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserRegisterDto userRegisterDto)
        {
            var result = await authService.Register(userRegisterDto);

            return Ok(result);
        }
    }
}
