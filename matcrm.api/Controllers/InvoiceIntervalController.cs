using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using matcrm.data.Models.Tables;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    [Authorize]
    public class InvoiceIntervalController : Controller
    {
        private readonly IInvoiceIntervalService _invoiceIntervalService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public InvoiceIntervalController(IInvoiceIntervalService invoiceIntervalService,
        IMapper mapper)
        {
            _invoiceIntervalService = invoiceIntervalService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpGet]
        public async Task<OperationResult<List<AddUpdateInvoiceIntervalRequestResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var InvoiceListData = _invoiceIntervalService.GetByTenant(TenantId).Select(t => new AddUpdateInvoiceIntervalRequestResponse
            {
                Id = t.Id,
                Name = t.Name,
                Interval = t.Interval
            }).ToList();

            return new OperationResult<List<AddUpdateInvoiceIntervalRequestResponse>>(true, System.Net.HttpStatusCode.OK, "", InvoiceListData);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateInvoiceIntervalRequestResponse>> Add([FromBody] AddUpdateInvoiceIntervalRequestResponse requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            AddUpdateInvoiceIntervalRequestResponse addUpdateInvoiceIntervalRequestResponse = new AddUpdateInvoiceIntervalRequestResponse();

            var invoiceData = _mapper.Map<InvoiceInterval>(requestModel);
            invoiceData.CreatedBy = UserId;
            var AddedInvoice = await _invoiceIntervalService.CheckInsertOrUpdate(invoiceData, TenantId);
            if (AddedInvoice != null)
            {
                requestModel.Id = AddedInvoice.Id;
            }
            return new OperationResult<AddUpdateInvoiceIntervalRequestResponse>(true, System.Net.HttpStatusCode.OK, "", requestModel);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPut]
        public async Task<OperationResult<AddUpdateInvoiceIntervalRequestResponse>> Update([FromBody] AddUpdateInvoiceIntervalRequestResponse requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            if (requestModel.Id != null)
            {
                var invoiceData = _mapper.Map<InvoiceInterval>(requestModel);
                invoiceData.CreatedBy = UserId;
                var AddedInvoice = await _invoiceIntervalService.CheckInsertOrUpdate(invoiceData, TenantId);
                if (AddedInvoice != null)
                {
                    requestModel.Id = AddedInvoice.Id;
                }
            }
            else
            {
                return new OperationResult<AddUpdateInvoiceIntervalRequestResponse>(true, System.Net.HttpStatusCode.OK, "", requestModel);
            }
            return new OperationResult<AddUpdateInvoiceIntervalRequestResponse>(true, System.Net.HttpStatusCode.OK, "Please provide id", requestModel);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult<AddUpdateInvoiceIntervalRequestResponse>> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var AddedInvoice = await _invoiceIntervalService.DeleteInvoiceInterval(Id);
            var invoiceData = _mapper.Map<AddUpdateInvoiceIntervalRequestResponse>(AddedInvoice);

            return new OperationResult<AddUpdateInvoiceIntervalRequestResponse>(true, System.Net.HttpStatusCode.OK, "", invoiceData);

        }

        [HttpGet]
        public async Task<OperationResult<List<InvoiceIntervalDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var InvoiceListData = _invoiceIntervalService.GetByTenant(TenantId).Select(t => new InvoiceIntervalDropDownListResponse
            {
                Id = t.Id,
                Name = t.Name,
                Interval = t.Interval
            }).ToList();

            return new OperationResult<List<InvoiceIntervalDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", InvoiceListData);
        }
    }
}