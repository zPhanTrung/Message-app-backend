using AutoMapper.Execution;
using Message_app_backend.Dto.Auth;
using Message_app_backend.Entities;
using Message_app_backend.Repository;
using Message_app_backend.Shared;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace Message_app_backend.Service
{
    public class JwtTokenService : BaseService
    {
        private UserInfoRepository userInfoRepository;
        private IConnectionMultiplexer redis;
        private IConfiguration configuration;
        public JwtTokenService(UserInfoRepository userInfoRepository, IConnectionMultiplexer redis, IConfiguration configuration)
        {
            this.userInfoRepository = userInfoRepository;
            this.configuration = configuration;
            this.redis = redis;
        }

        public AuthResponseDto Authenticate(AuthDto authDto)
        {
            try
            {
                if (authDto.PhoneNumber != null && authDto.Password != null)
                {
                    var userInfo = userInfoRepository.FindByPhoneNumber(authDto.PhoneNumber);

                    if (userInfo != null && userInfo.Password.Equals(authDto.Password))
                    {
                        //create claims details based on the user information
                        string accessToken = CreateAccessToken(userInfo);

                        string refreshToken = CreateRefreshToken(userInfo, authDto);

                        SaveAccessToken(userInfo.UserId, accessToken);

                        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = refreshToken };
                    }

                    throw new Exception("Phone number or password incorrect");
                }

                throw new Exception("Missing phone number or password");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CreateAccessToken(UserInfo userInfo)
        {
            try
            {
                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", userInfo.UserId.ToString()),
                        new Claim("PhoneNumber", userInfo.PhoneNumber),
                    };

                var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:AccessToken_key"]));
                var signIn = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:AccessToken_expired"])),
                    signingCredentials: signIn);

                return new JwtSecurityTokenHandler().WriteToken(token).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string CreateRefreshToken(UserInfo userInfo, AuthDto authDto)
        {
            try
            {
                var deviceInfoClaims = new HashSet<DeviceInfoClaim>();
                if (userInfo.RefreshToken != string.Empty)
                {
                    deviceInfoClaims = GetDeviceInfoClaim(userInfo.RefreshToken);
                }

                var deviceInfo = new DeviceInfoClaim() { DeviceModel = authDto.DeviceModel, DeviceName = authDto.DeviceName };
                deviceInfoClaims.Add(deviceInfo);

                //Create refresh token for server
                var refreshToken = CreateRefreshTokenForServer(userInfo, deviceInfoClaims);
                SaveRefreshToken(userInfo, refreshToken);

                //Create refresh token for client
                return CreateRefreshTokenForClient(userInfo, deviceInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private string CreateRefreshTokenForServer(UserInfo userInfo, HashSet<DeviceInfoClaim> deviceInfos)
        {
            try
            {
                var deviceInfoClaimForServer = JsonSerializer.Serialize(deviceInfos);

                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", userInfo.UserId.ToString()),
                        new Claim("PhoneNumber", userInfo.PhoneNumber),
                        new Claim("DeviceInfo", deviceInfoClaimForServer)
                    };

                var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:RefreshToken_key"]));
                var signIn = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:RefreshToken_expired"])),
                    signingCredentials: signIn);

                return new JwtSecurityTokenHandler().WriteToken(token).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private string CreateRefreshTokenForClient(UserInfo userInfo, DeviceInfoClaim deviceInfo)
        {
            try
            {
                var deviceInfoClaimForClient = JsonSerializer.Serialize(deviceInfo);

                var claims = new[] {
                        new Claim(JwtRegisteredClaimNames.Sub, configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", userInfo.UserId.ToString()),
                        new Claim("PhoneNumber", userInfo.PhoneNumber),
                        new Claim("DeviceInfo", deviceInfoClaimForClient)
                    };

                var tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:RefreshToken_key"]));
                var signIn = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    configuration["Jwt:Issuer"],
                    configuration["Jwt:Audience"],
                    claims,
                    expires: DateTime.UtcNow.AddMinutes(double.Parse(configuration["Jwt:RefreshToken_expired"])),
                    signingCredentials: signIn);

                return new JwtSecurityTokenHandler().WriteToken(token).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private HashSet<DeviceInfoClaim>? GetDeviceInfoClaim(string refreshToken)
        {
            try
            {
                var claims = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken).Claims
                   .Where(claim => claim.Type.Equals("DeviceInfo")).First().Value;

                var deviceInfos = JsonSerializer.Deserialize<HashSet<DeviceInfoClaim>>(claims);

                return deviceInfos;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void SaveAccessToken(int userId, string accessToken)
        {
            try
            {
                var db = redis.GetDatabase();
                var expToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken).Claims
                    .Where(claim => claim.Type.Equals("exp")).First().Value;
                var key = "accessToken_" + userId.ToString() + "_" + expToken;

                var expired = double.Parse(configuration["Jwt:AccessToken_expired"]);
                var hour = (int)expired / 60;
                var minute = (int)(expired - hour * 60);
                var second = (int)(expired - Math.Truncate(expired)) * 60;
                TimeSpan exp = new TimeSpan(hour, minute, second);
                db.StringSet(key, accessToken);
                db.KeyExpire(key, exp);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void RemoveAccessToken(int userId)
        {
            try
            {
                var db = redis.GetDatabase();
                var keys = redis.GetServer(configuration["Redis:Endpoint"]).Keys(database: db.Database, pattern: "accessToken_" + userId + "*").ToArray();
                foreach (var key in keys)
                {
                    db.KeyDelete(key);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void SaveRefreshToken(UserInfo userInfo, string refreshToken)
        {
            try
            {
                userInfo.RefreshToken = refreshToken;
                userInfoRepository.Update(userInfo);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Reconnect(string accessToken)
        {
            try
            {
                var db = redis.GetDatabase();
                var userId = new JwtSecurityTokenHandler().ReadJwtToken(accessToken).Claims
                    .Where(claims => claims.Type.Equals("UserId")).First().Value;
                var expToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken).Claims
                    .Where(claim => claim.Type.Equals("exp")).First().Value;
                var key = "accessToken_" + userId + "_" + expToken;
                var value = db.StringGet(key);

                if (value.HasValue && value.Equals(accessToken))
                {
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetAccesToken(string refreshToken)
        {
            try
            {
                var userId = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken).Claims
                    .Where(claims => claims.Type.Equals("UserId")).First().Value;
                var exp = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken).Claims
                    .Where(claims => claims.Type.Equals("exp")).First().Value;
                var user = userInfoRepository.FindById(int.Parse(userId));

                if (user == null)
                {
                    throw new Exception("User not exist");
                }

                var deviceInfo_str = new JwtSecurityTokenHandler().ReadJwtToken(refreshToken).Claims
                    .Where(claims => claims.Type.Equals("DeviceInfo")).First().Value;

                var deviceInfo = JsonSerializer.Deserialize<DeviceInfoClaim>(deviceInfo_str);

                var deviceInfoClaims = GetDeviceInfoClaim(user.RefreshToken);

                var expired = DateTimeOffset.FromUnixTimeSeconds(long.Parse(exp)).UtcDateTime;

                var a = deviceInfoClaims.Contains<DeviceInfoClaim>(deviceInfo);


                if (deviceInfo != null && deviceInfoClaims.Contains(deviceInfo) && expired > DateTime.UtcNow)
                {
                    var accessToken = CreateAccessToken(user);
                    SaveAccessToken(int.Parse(userId), accessToken);
                    return accessToken;
                }

                return "";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public bool Logout(LogoutDto logoutDto)
        {
            try
            {
                var deviceInfoClaim_str = new JwtSecurityTokenHandler().ReadJwtToken(logoutDto.RefreshToken).Claims
                    .Where(claims => claims.Type.Equals("DeviceInfo")).First().Value;

                var deviceInfoClaim = JsonSerializer.Deserialize<DeviceInfoClaim>(deviceInfoClaim_str);

                var userId = new JwtSecurityTokenHandler().ReadJwtToken(logoutDto.RefreshToken).Claims
                        .Where(claims => claims.Type.Equals("UserId")).First().Value;

                var userInfo = userInfoRepository.FindById(int.Parse(userId));

                if (userInfo != null)
                {
                    var deviceInfoClaims = GetDeviceInfoClaim(userInfo.RefreshToken);
                    deviceInfoClaims.Remove(deviceInfoClaim);
                    var newRefreshToken = CreateRefreshTokenForServer(userInfo, deviceInfoClaims);
                    SaveRefreshToken(userInfo, newRefreshToken);
                    RemoveAccessToken(userInfo.UserId);
                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
