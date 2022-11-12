using Message_app_backend.Dto.Auth;
using Message_app_backend.Entities;
using Message_app_backend.Repository;
using Message_app_backend.Service;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Runtime.Intrinsics.Arm;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace Message_app_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        public IConfiguration configuration;
        private JwtTokenService jwtTokenService;
        private UserService userService;

        public AuthenticationController(
            IConfiguration config,
            UserService userService,
            JwtTokenService jwtTokenService)
        {
            configuration = config;
            this.userService = userService;
            this.jwtTokenService = jwtTokenService;
        }

        [HttpPost]
        [Route("/auth")]
        public MessageResponse<AuthResponseDto> Authenticate(AuthDto authDto)
        {
            try
            {
                var data = jwtTokenService.Authenticate(authDto);

                return new MessageResponse<AuthResponseDto> { Code = HttpStatusCode.OK, Data = data };
            }
            catch (Exception ex)
            {
                return new MessageResponse<AuthResponseDto> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        [HttpPost]
        [Route("/reconnect")]
        public MessageResponse<bool> Reconnect(ReconnectDto reconnectDto)
        {
            try
            {
                var isValid = jwtTokenService.Reconnect(reconnectDto.AccessToken);
                if (isValid)
                {
                    return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "Login success" };
                }
                else
                {
                    return new MessageResponse<bool> { Code = HttpStatusCode.NotFound, Message = "Access token invalid" };
                }
            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }

        [HttpGet]
        [Route("/GetAccessToken")]
        public MessageResponse<object> GetAccessToken(string refreshToken)
        {
            try
            {
                var accessToken = jwtTokenService.GetAccesToken(refreshToken);
                if (accessToken != "")
                {
                    var data = new { accessToken = accessToken };
                    return new MessageResponse<object> { Code = HttpStatusCode.OK, Data = data };
                }
                return new MessageResponse<object> { Code = HttpStatusCode.NotFound, Message= "Can not get access token",Data = "" };
            }
            catch (Exception ex)
            {
                return new MessageResponse<object> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }

        }

        [HttpPost]
        [Authorize]
        [Route("/logout")]
        public MessageResponse<bool> Logout(LogoutDto logoutDto)
        {
            try
            {
                if (jwtTokenService.Logout(logoutDto))
                {
                    return new MessageResponse<bool> { Code = HttpStatusCode.OK, Message = "Logout success" };
                }

                return new MessageResponse<bool> { Code = HttpStatusCode.MethodNotAllowed, Message = "Logout failed" };

            }
            catch (Exception ex)
            {
                return new MessageResponse<bool> { Code = HttpStatusCode.Forbidden, Message = ex.Message };
            }
        }


    }

}
