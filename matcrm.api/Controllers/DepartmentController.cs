using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public DepartmentController(IDepartmentService departmentService,
        IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }
        
        [HttpPost]
        public async Task<OperationResult<DepartmentAddUpdateResponse>> Add([FromBody] DepartmentAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            
            var model = _mapper.Map<Department>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var departmentObj = await _departmentService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<DepartmentAddUpdateResponse>(departmentObj);
            return new OperationResult<DepartmentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responseObj);
        }

        [HttpPut]
        public async Task<OperationResult<DepartmentAddUpdateResponse>> Update([FromBody] DepartmentAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            
            var model = _mapper.Map<Department>(requestmodel);
            if (model.Id > 0)
            {
                model.UpdatedBy = UserId;
            }
            var departmentObj = await _departmentService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<DepartmentAddUpdateResponse>(departmentObj);
            return new OperationResult<DepartmentAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", responseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            if (Id > 0)
            {
                var departmentObj = await _departmentService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<DepartmentListResponse>>> List()
        {
            var departmentList = _departmentService.GetAll();
            var departmentListResponses = _mapper.Map<List<DepartmentListResponse>>(departmentList);
            return new OperationResult<List<DepartmentListResponse>>(true, System.Net.HttpStatusCode.OK, "", departmentListResponses);
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<DepartmentDetailResponse>> Detail(long Id)
        {
            var departmentObj = _departmentService.GetById(Id);
            var ResponseObj = _mapper.Map<DepartmentDetailResponse>(departmentObj);
            return new OperationResult<DepartmentDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }

        [HttpGet]
        public async Task<OperationResult<List<DepartmentDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var departmentList = _departmentService.GetByTenant(TenantId);
            var departmentListResponses = _mapper.Map<List<DepartmentDropDownListResponse>>(departmentList);
            return new OperationResult<List<DepartmentDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", departmentListResponses);
        }        
    }
}