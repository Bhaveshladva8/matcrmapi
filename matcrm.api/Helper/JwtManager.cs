using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.authentication.jwt;
using matcrm.data.Models.Request;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;
using matcrm.api.SignalR;
using matcrm.api.Helper;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.Mollie.Payment;
using matcrm.service.Utility;
using matcrm.data.Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace matcrm.api.Helper
{
    public class JwtManager
    {
        private Dictionary<string, string> claims = new Dictionary<string, string>();
        private readonly IUserService _userService;
        private readonly ITenantService _tenantService;
        private readonly IRoleService _roleService;

        public JwtManager(
        IUserService userService,
        ITenantService tenantService,
        IRoleService roleService)
        {
            _userService = userService;
            _tenantService = tenantService;
            _roleService = roleService;
        }
        public async Task<Tokens> GenerateToken(string email , bool IsSubscribed)
        {
            return await GenerateJWTTokens(email, IsSubscribed);
        }

        public async Task<Tokens> GenerateRefreshToken(string email , bool IsSubscribed)
        {
            return await GenerateJWTTokens(email, IsSubscribed);
        }

        public async Task<Tokens> GenerateJWTTokens(string email, bool IsSubscribed)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = System.Text.Encoding.UTF8.GetBytes(OneClappContext.SecretKey);
                User userObj = _userService.GetUserByEmail(email);
                string TenantName = "";
                string RoleName = "";
                if (userObj != null)
                {
                    if (userObj.TenantId != null)
                    {
                        var tenantObj = _tenantService.GetTenantById(userObj.TenantId.Value);
                        if (tenantObj != null)
                        {
                            TenantName = tenantObj?.TenantName;
                        }
                    }

                    if (userObj.RoleId != null)
                    {
                        var roleObj = _roleService.GetRoleById(userObj.RoleId.Value);
                        if (roleObj != null)
                        {
                            RoleName = roleObj?.RoleName;
                        }
                    }

                }
                var claims = GetClaim(userObj, TenantName, RoleName, IsSubscribed);
                List<Claim> ClaimList = new List<Claim>();
                foreach (var item in claims)
                {
                    var ClaimObj = new Claim(item.Key, item.Value);
                    ClaimList.Add(ClaimObj);
                }
                // var[] ClaimArr = new Array();
                // if (ClaimList != null)
                // {
                   var ClaimArr = ClaimList.ToArray();
                // }

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    //     Subject = new ClaimsIdentity(new Claim[]
                    //   {
                    //  new Claim(ClaimTypes.Name, email)
                    //   }),
                    Subject = new ClaimsIdentity(ClaimArr),
                    // Subject = new Claim(JwtRegisteredClaimNames.Sub, this.subject),
                    Issuer = OneClappContext.CurrentURL,
                    Audience = OneClappContext.AppURL,
                    Expires = DateTime.Now.AddMinutes(Convert.ToDouble(OneClappContext.TokenExpireMinute)),
                    // Claims = ClaimArr?.ToArray(),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var refreshToken = GenerateRefreshToken();
                await _userService.UpdateRefreshToken(userObj, refreshToken);
                return new Tokens { Access_Token = tokenHandler.WriteToken(token), Refresh_Token = refreshToken };
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        private Dictionary<string, string> GetClaim(User userObj, string Tenant, string RoleName, bool IsSubscribed = false)
        {
            var claim = new Dictionary<string, string>();
            claim.Add("Id", userObj.Id.ToString());
            claim.Add("Sid", userObj.Id.ToString());
            if (Tenant != "" || Tenant != null)
            {
                claim.Add("Tenant", Tenant.ToString());
            }
            if (userObj.TenantId != null)
            {
                claim.Add("TenantId", userObj.TenantId.ToString());
            }
            if (userObj.WeClappUserId != null)
            {
                claim.Add("WeClappUserId", userObj.WeClappUserId.ToString());
            }
            if (userObj.WeClappToken != null)
            {
                claim.Add("WeClappToken", userObj.WeClappToken);
            }
            if (RoleName != "")
            {
                claim.Add(ClaimTypes.Role, RoleName);
            }
            if (RoleName != "")
            {
                claim.Add("RoleName", RoleName);
            }
            if (userObj != null && !string.IsNullOrEmpty(userObj.Email))
            {
                claim.Add("Email", userObj.Email);
            }

            claim.Add("IsSubscribed", IsSubscribed.ToString());

            return claim;
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var Key = Encoding.UTF8.GetBytes(OneClappContext.SecretKey);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Key),
                ClockSkew = TimeSpan.Zero
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            JwtSecurityToken jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }


            return principal;
        }
    }
}