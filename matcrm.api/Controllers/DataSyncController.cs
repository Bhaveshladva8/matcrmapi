using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Context;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class DataSyncController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly IERPSystemColumnMapService _eRPSystemColumnMapService;
        private readonly IWeClappUserService _weClappUserService;
        private readonly IWeClappService _weClappService;
        private int UserId = 0;
        private int TenantId = 0;

        public DataSyncController(ICustomerService customerService,
        IERPSystemColumnMapService eRPSystemColumnMapService,
        IWeClappUserService weClappUserService,
        IWeClappService weClappService)
        {
            _customerService = customerService;
            _eRPSystemColumnMapService = eRPSystemColumnMapService;
            _weClappUserService = weClappUserService;
            _weClappService = weClappService;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<CustomerDto>> SyncData()
        {
            CustomerDto customerDto = new CustomerDto();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (UserId != null)
            {
                var weclappUserCredentialObj = _weClappUserService.GetByUser(UserId);
                if (weclappUserCredentialObj != null)
                {
                    // List<CustomerVM> Customers = new List<CustomerVM>();
                    if (!string.IsNullOrEmpty(weclappUserCredentialObj.ApiKey))
                    {
                        var customers = await _weClappService.GetCustomers(weclappUserCredentialObj.ApiKey, weclappUserCredentialObj.TenantName);

                        if (customers != null && customers.Count() > 0)
                        {
                            var columnMappingList = _eRPSystemColumnMapService.GetByUser(UserId);
                            // Customers = Customers.Where(t => !string.IsNullOrEmpty(t.FirstName) && !string.IsNullOrEmpty(t.Email)).ToList();
                            // var CustomerObj = Customers.Last();
                            foreach (var CustomerObj in customers)
                            {
                                var customerType = typeof(CustomerDto);
                                CustomerDto customerDto1 = new CustomerDto();
                                if (columnMappingList != null && columnMappingList.Count() > 0)
                                {
                                    foreach (var item in columnMappingList)
                                    {
                                        var propertyInfo = customerDto1.GetType().GetProperty(item.DestinationColumnName);
                                        var value = CustomerObj.GetType().GetProperty(item.SourceColumnName).GetValue(CustomerObj, null);
                                        customerType.GetProperty(item.DestinationColumnName).SetValue(customerDto1, value);
                                    }
                                }
                                if (customerDto1.Name != null)
                                {
                                    customerDto1.CreatedBy = UserId;
                                    customerDto1.TenantId = TenantId;
                                    var AddUpdate = await _customerService.CheckInsertOrUpdate(customerDto1);
                                }
                            }

                        }
                    }
                }
                return new OperationResult<CustomerDto>(true, System.Net.HttpStatusCode.OK, "", customerDto);
            }
            else
            {
                return new OperationResult<CustomerDto>(false, System.Net.HttpStatusCode.OK, "Please provide userid", customerDto);
            }
        }
    }
}