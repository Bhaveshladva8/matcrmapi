using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class LabelController : Controller
    {

        private readonly ILabelCategoryService _labelCategoryService;
        private readonly ILabelService _labelService;
        private readonly ICustomerLabelService _customerLabelService;
        private readonly IOrganizationLabelService _organizationLabelService;
        private readonly ILeadLabelService _leadLabelService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public LabelController(
            ILabelCategoryService labelCategoryService,
            ILabelService labelService,
            ICustomerLabelService customerLabelService,
            IOrganizationLabelService organizationLabelService,
            ILeadLabelService leadLabelService,
            IMapper mapper)
        {
            _labelCategoryService = labelCategoryService;
            _labelService = labelService;
            _customerLabelService = customerLabelService;
            _organizationLabelService = organizationLabelService;
            _leadLabelService = leadLabelService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<LabelGetAllLabelResponse>>> List([FromBody] LabelGetAllLabelRequest model)
        {
            List<LabelDto> labelDtoList = new List<LabelDto>();
            var requestmodel = _mapper.Map<LabelDto>(model);
            var labelCategoryObj = _labelCategoryService.GetByName(requestmodel.CategoryName);

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            if (labelCategoryObj != null)
            {
                var labelList = _labelService.GetByUserAndDefault(UserId, labelCategoryObj.Id);
                labelDtoList = _mapper.Map<List<LabelDto>>(labelList);
            }
            var responselabel = _mapper.Map<List<LabelGetAllLabelResponse>>(labelDtoList);
            return new OperationResult<List<LabelGetAllLabelResponse>>(true, System.Net.HttpStatusCode.OK, "", responselabel);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpGet]
        public async Task<OperationResult<List<LabelDto>>> BasedOnTenant([FromBody] LabelDto model)
        {
            List<LabelDto> labelDtoList = new List<LabelDto>();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var labelCategoryObj = _labelCategoryService.GetByName(model.CategoryName);
            if (labelCategoryObj != null)
            {
                var labelList = _labelService.GetByTenant(TenantId);
                labelDtoList = _mapper.Map<List<LabelDto>>(labelList);
            }
            return new OperationResult<List<LabelDto>>(true, System.Net.HttpStatusCode.OK, "", labelDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpGet]
        public async Task<OperationResult<List<LabelGetAllResponse>>> All()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<LabelDto> labelDtoList = new List<LabelDto>();
            var labelList = _labelService.GetByTenant(TenantId);
            labelDtoList = _mapper.Map<List<LabelDto>>(labelList);
            var responseLabellist = _mapper.Map<List<LabelGetAllResponse>>(labelDtoList);
            return new OperationResult<List<LabelGetAllResponse>>(true, System.Net.HttpStatusCode.OK, "", responseLabellist);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<LabelAddUpdateResponse>> AddUpdate([FromBody] LabelAddUpdateRequest model)
        {
            Label labelObj = new Label();
            // if (!string.IsNullOrEmpty(model.CategoryName))
            // {
            //     var labelCategory = _labelCategoryService.GetByName(model.CategoryName);
            //     if (labelCategory != null)
            //     {
            //         model.LabelCategoryId = labelCategory.Id;
            //         var LabelObj = await _labelService.CheckInsertOrUpdate(model);
            //         return new OperationResult<Label>(true, "", LabelObj);
            //     }
            //     return new OperationResult<Label>(false, "", labelObj);
            // }
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<LabelDto>(model);
            if (requestmodel.Id == null)
            {
                requestmodel.CreatedBy = UserId;
            }
            else
            {
                requestmodel.UpdatedBy = UserId;
            }
            requestmodel.TenantId = TenantId;
            if (requestmodel.Categories != null && requestmodel.Categories.Count() > 0)
            {
                foreach (var CategoryName in requestmodel.Categories)
                {
                    var labelCategory = _labelCategoryService.GetByName(CategoryName);
                    if (labelCategory != null)
                    {
                        requestmodel.CategoryName = CategoryName;
                        requestmodel.LabelCategoryId = labelCategory.Id;
                        
                        if(requestmodel.Categories.Count() > 1)
                        {
                        var existingItem = _labelService.CheckExistOrNot(requestmodel);
                        if (existingItem == null)
                        {
                            requestmodel.Id = null;
                        }
                        }

                        var AddUpdate = await _labelService.CheckInsertOrUpdate(requestmodel);
                        labelObj = AddUpdate;
                        // return new OperationResult<Label>(true, "", LabelObj);
                    }
                }
                // var labelCategory = _labelCategoryService.GetByName(model.CategoryName);
                // if (labelCategory != null)
                // {
                //     model.LabelCategoryId = labelCategory.Id;
                //     var LabelObj = await _labelService.CheckInsertOrUpdate(model);
                //     return new OperationResult<Label>(true, "", LabelObj);
                // }
                // return new OperationResult<Label>(false, "", labelObj);
            }
            var responsemodel = _mapper.Map<LabelAddUpdateResponse>(labelObj);
            return new OperationResult<LabelAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<LabelDeleteResponse>> Remove([FromBody] LabelDeleteRequest model)
        {
            var requestmodel = _mapper.Map<LabelDto>(model);
            if (requestmodel.Id != null)
            {
                if (requestmodel.CategoryName == "Person")
                {
                    await _customerLabelService.DeleteByLabel(requestmodel.Id.Value);
                }
                else if (requestmodel.CategoryName == "Organization")
                {
                    await _organizationLabelService.DeleteByLabel(requestmodel.Id.Value);
                }
                else if (requestmodel.CategoryName == "Lead")
                {
                    await _leadLabelService.DeleteByLabel(requestmodel.Id.Value);
                }
                var labelObj = _labelService.DeleteLabel(requestmodel);
                var responseObj = _mapper.Map<LabelDeleteResponse>(labelObj);
                return new OperationResult<LabelDeleteResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
            }
            else
            {
                return new OperationResult<LabelDeleteResponse>(false, System.Net.HttpStatusCode.OK, "Please provide id");
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> RemoveLeadLabel(long Id)
        {
            if (Id != null && Id > 0)
            {
                var data = await _leadLabelService.DeleteLeadLabel(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);                
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide id", Id);                
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<Label>> Detail(int Id)
        {
            Label labelObj = new Label();
            labelObj = _labelService.GetById(Id);
            return new OperationResult<Label>(true, System.Net.HttpStatusCode.OK, "", labelObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<LabelCategoryResponse>>> LabelCategories()
        {
            List<LabelCategoryDto> labelCategoryDtoList = new List<LabelCategoryDto>();
            var labelCategories = _labelCategoryService.GetAll();
            labelCategoryDtoList = _mapper.Map<List<LabelCategoryDto>>(labelCategories);
            var responseLabelCategory = _mapper.Map<List<LabelCategoryResponse>>(labelCategoryDtoList);
            return new OperationResult<List<LabelCategoryResponse>>(true, System.Net.HttpStatusCode.OK, "", responseLabelCategory);
        }

    }
}