using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using matcrm.service.Services;
using AutoMapper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using matcrm.service.Common;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TaxRateController : Controller
    {
        private readonly ITaxRateService _taxRateService;
        private int UserId = 0;
        private int TenantId = 0;
        public TaxRateController(
        ITaxRateService taxRateService,
        IMapper mapper)
        {
            _taxRateService = taxRateService;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<TaxRate>> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TaxRate? taxRateObj = null;
            if (Id != null && Id > 0)
            {
                int deletedby = UserId;
                taxRateObj = await _taxRateService.DeleteTaxRate(Id, deletedby);

                return new OperationResult<TaxRate>(true, System.Net.HttpStatusCode.OK, "", taxRateObj);
            }
            else
            {
                return new OperationResult<TaxRate>(false, System.Net.HttpStatusCode.OK, "Please provide id", taxRateObj);
            }
        }
    }
}