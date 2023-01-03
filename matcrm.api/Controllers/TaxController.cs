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
    public class TaxController : Controller
    {
        private readonly ITaxService _taxService;
        private readonly ITaxRateService _taxRateService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public TaxController(ITaxService taxService,
        ITaxRateService taxRateService,
        IMapper mapper)
        {
            _taxService = taxService;
            _taxRateService = taxRateService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<TaxAddResponse>> Add([FromBody] TaxAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<Tax>(requestmodel);
            if (model.Id == null || model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            model.TenantId = TenantId;

            var taxObj = await _taxService.CheckInsertOrUpdate(model);
            TaxAddResponse taxAddResponseObj = new TaxAddResponse();
            taxAddResponseObj = _mapper.Map<TaxAddResponse>(taxObj);
            long Percentage = 0;
            if (taxObj != null && requestmodel.TaxRates != null && requestmodel.TaxRates.Count() > 0)
            {
                foreach (var item in requestmodel.TaxRates)
                {
                    var taxRateObj = _mapper.Map<TaxRate>(item);

                    taxRateObj.TaxId = taxObj.Id;
                    taxRateObj.TenantId = TenantId;

                    if (taxRateObj.Id != null && taxRateObj.Id > 0)
                    {
                        taxRateObj.UpdatedBy = UserId;
                    }
                    else
                    {
                        taxRateObj.CreatedBy = UserId;
                    }
                    var AddUpdate = await _taxRateService.CheckInsertOrUpdate(taxRateObj);
                    if (item.Percentage == null || item.Percentage == 0)
                    {
                        Percentage = 0;
                    }
                    else
                    {
                        Percentage += item.Percentage.Value;
                    }
                    TaxRateAddResponse taxRateAddResponseObj = new TaxRateAddResponse();
                    taxRateAddResponseObj = _mapper.Map<TaxRateAddResponse>(AddUpdate);
                    taxAddResponseObj.TaxRates.Add(taxRateAddResponseObj);
                }
            }

            if (requestmodel.TaxRates != null && requestmodel.TaxRates.Count() > 0)
            {
                taxAddResponseObj.Percentage = Convert.ToString(Percentage);
            }
            else
            {
                taxAddResponseObj.Percentage = "NA";
            }
            return new OperationResult<TaxAddResponse>(true, System.Net.HttpStatusCode.OK, "Tax added successfully", taxAddResponseObj);
        }


        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<TaxAddResponse>> Update([FromBody] TaxAddRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var model = _mapper.Map<Tax>(requestmodel);
            if (model.Id != null && model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            model.TenantId = TenantId;

            var taxObj = await _taxService.CheckInsertOrUpdate(model);
            TaxAddResponse taxAddResponseObj = new TaxAddResponse();
            taxAddResponseObj = _mapper.Map<TaxAddResponse>(taxObj);
            long Percentage = 0;
            if (taxObj != null && requestmodel.TaxRates != null && requestmodel.TaxRates.Count() > 0)
            {
                foreach (var item in requestmodel.TaxRates)
                {
                    var taxRateObj = _mapper.Map<TaxRate>(item);

                    taxRateObj.TaxId = taxObj.Id;
                    taxRateObj.TenantId = TenantId;
                    
                    if (taxRateObj.Id != null && taxRateObj.Id > 0)
                    {
                        taxRateObj.UpdatedBy = UserId;
                    }
                    else
                    {
                        taxRateObj.CreatedBy = UserId;
                    }
                    var AddUpdate = await _taxRateService.CheckInsertOrUpdate(taxRateObj);
                    if (item.Percentage == null || item.Percentage == 0)
                    {
                        Percentage = 0;
                    }
                    else
                    {
                        Percentage += item.Percentage.Value;
                    }
                    TaxRateAddResponse taxRateAddResponseObj = new TaxRateAddResponse();
                    taxRateAddResponseObj = _mapper.Map<TaxRateAddResponse>(AddUpdate);
                    taxAddResponseObj.TaxRates.Add(taxRateAddResponseObj);
                }
            }

            if (requestmodel.TaxRates != null && requestmodel.TaxRates.Count() > 0)
            {
                taxAddResponseObj.Percentage = Convert.ToString(Percentage);
            }
            else
            {
                taxAddResponseObj.Percentage = "NA";
            }
            return new OperationResult<TaxAddResponse>(true, System.Net.HttpStatusCode.OK, "Tax updated successfully", taxAddResponseObj);
        }


        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<TaxListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var taxList = _taxService.GetByTenant(TenantId);

            var taxListResponses = _mapper.Map<List<TaxListResponse>>(taxList);

            if (taxListResponses != null && taxListResponses.Count() > 0)
            {
                foreach (var item in taxListResponses)
                {
                    long? Percentage = 0;
                    var taxRateList = _taxRateService.GetByTaxId(item.Id);
                    if (taxRateList != null && taxRateList.Count() > 0)
                    {
                        foreach (var taxRateItem in taxRateList)
                        {
                            if (taxRateItem.Percentage != null && taxRateItem.Percentage > 0)
                            {
                                Percentage += taxRateItem.Percentage;
                            }
                            else
                            {
                                Percentage = 0;
                            }
                            item.Percentage = Convert.ToString(Percentage);
                        }
                    }
                    else
                    {
                        item.Percentage = "NA";
                    }
                    List<TaxRateListResponse> taxRateListResponse = new List<TaxRateListResponse>();
                    taxRateListResponse= _mapper.Map<List<TaxRateListResponse>>(taxRateList);
                    item.TaxRates = taxRateListResponse;
                }
            }
            return new OperationResult<List<TaxListResponse>>(true, System.Net.HttpStatusCode.OK, "", taxListResponses);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<TaxDetailResponse>> Detail(int Id)
        {
            Tax taxObj = new Tax();
            TaxDetailResponse taxDetailResponseObj = new TaxDetailResponse();
            List<TaxRate> taxRateList = new List<TaxRate>();
            List<TaxRateDetailResponse> taxRateDetailResponseList = new List<TaxRateDetailResponse>();
            taxObj = _taxService.GetById(Id);
            taxRateList = _taxRateService.GetByTaxId(Id);
            taxDetailResponseObj = _mapper.Map<TaxDetailResponse>(taxObj);
            taxRateDetailResponseList = _mapper.Map<List<TaxRateDetailResponse>>(taxRateList);
            taxDetailResponseObj.taxRates = taxRateDetailResponseList;
            return new OperationResult<TaxDetailResponse>(true, System.Net.HttpStatusCode.OK, "", taxDetailResponseObj);
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
                var taxRateList = await _taxRateService.DeleteByTaxId(Id, deletedby);

                var taxObj = await _taxService.DeleteTax(Id, deletedby);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<TaxDropdownListResponse>>> DropdownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var taxList = _taxService.GetByTenant(TenantId);

            var taxDropdownListResponseList = _mapper.Map<List<TaxDropdownListResponse>>(taxList);

            return new OperationResult<List<TaxDropdownListResponse>>(true, System.Net.HttpStatusCode.OK, "", taxDropdownListResponseList);
        }


    }
}