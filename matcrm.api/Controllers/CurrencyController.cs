using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using matcrm.service.Common;
using matcrm.data.Models.Tables;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class CurrencyController : Controller
    {
        private readonly ICurrencyService _currencyService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public CurrencyController(ICurrencyService currencyService,
        IMapper mapper)
        {
            _currencyService = currencyService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CurrencyAddResponse>> Add([FromBody] CurrencyAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<Currency>(requestmodel);
            if (model.Id == null || model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            model.TenantId = TenantId;

            var currencyObj = await _currencyService.CheckInsertOrUpdate(model);
            CurrencyAddResponse currencyAddResponseObj = new CurrencyAddResponse();
            currencyAddResponseObj = _mapper.Map<CurrencyAddResponse>(currencyObj);

            return new OperationResult<CurrencyAddResponse>(true, System.Net.HttpStatusCode.OK, "Currency added successfully", currencyAddResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<CurrencyAddResponse>> Update([FromBody] CurrencyAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<Currency>(requestmodel);
            if (model.Id != null && model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;
            var currencyObj = await _currencyService.CheckInsertOrUpdate(model);
            CurrencyAddResponse currencyAddResponseObj = new CurrencyAddResponse();
            currencyAddResponseObj = _mapper.Map<CurrencyAddResponse>(currencyObj);

            return new OperationResult<CurrencyAddResponse>(true, System.Net.HttpStatusCode.OK, "Currency updated successfully", currencyAddResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CurrencyListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var currencyList = _currencyService.GetAll();
            var currencyListResponses = _mapper.Map<List<CurrencyListResponse>>(currencyList);
            return new OperationResult<List<CurrencyListResponse>>(true, System.Net.HttpStatusCode.OK, "", currencyListResponses);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<CurrencyDetailResponse>> Detail(int Id)
        {
            Currency currencyObj = new Currency();
            CurrencyDetailResponse currencyDetailResponseObj = new CurrencyDetailResponse();
            currencyObj = _currencyService.GetById(Id);
            currencyDetailResponseObj = _mapper.Map<CurrencyDetailResponse>(currencyObj);
            return new OperationResult<CurrencyDetailResponse>(true, System.Net.HttpStatusCode.OK, "", currencyDetailResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null && Id > 0)
            {
                int deletedby = UserId;
                var currencyObj = await _currencyService.DeleteCurrency(Id, deletedby);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CurrencyDropDownListResponse>>> DropdownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var currencyList = _currencyService.GetByTenant(TenantId);

            var currencyDropDownListResponseList = _mapper.Map<List<CurrencyDropDownListResponse>>(currencyList);
            if (currencyDropDownListResponseList != null && currencyDropDownListResponseList.Count() > 0)
            {
                foreach (var item in currencyDropDownListResponseList)
                {
                    var currencyObj = currencyList.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (currencyObj != null)
                    {
                        item.Name = currencyObj.Code + "(" + currencyObj.Symbol + ")";
                    }
                }
            }
            return new OperationResult<List<CurrencyDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", currencyDropDownListResponseList);
        }

    }
}