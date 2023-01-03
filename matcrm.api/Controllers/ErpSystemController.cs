using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using AutoMapper;
using System.Collections.Generic;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ERPSystemController : Controller
    {

        private readonly IERPSystemService _erpSystemService;
        private IMapper _mapper;
        public ERPSystemController(IERPSystemService erpSystemService, IMapper mapper)
        {
            _erpSystemService = erpSystemService;
            _mapper = mapper;
        }

        // GetAll Email Provider Method
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<ERPSystemDto>>> List()
        {
            var eRPSystemObj = _erpSystemService.GetAll();

            List<ERPSystemDto> eRPSystemDtoList = new List<ERPSystemDto>();

            if (eRPSystemObj != null)
            {
                eRPSystemDtoList = _mapper.Map<List<ERPSystemDto>>(eRPSystemObj);

                return new OperationResult<List<ERPSystemDto>>(true, System.Net.HttpStatusCode.OK, "ERP system found successfully", eRPSystemDtoList);
            }

            return new OperationResult<List<ERPSystemDto>>(false, System.Net.HttpStatusCode.OK, "ERP system not found", eRPSystemDtoList);

        }

        // GetAll Email Provider Method
        [Authorize(Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<ERPSystemDto>> Detail(int Id)
        {
            var eRPSystemObj = _erpSystemService.GetERPSystemById(Id);

            if (eRPSystemObj != null)
            {
                var eRPSystemDto = _mapper.Map<ERPSystemDto>(eRPSystemObj);
                return new OperationResult<ERPSystemDto>(true, System.Net.HttpStatusCode.OK, "ERP system found successfully", eRPSystemDto);
            }
            else
            {
                return new OperationResult<ERPSystemDto>(false, System.Net.HttpStatusCode.OK, "ERP system not found");
            }

        }

        // Add Updated Email Provider Method
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<OperationResult<ERPSystemDto>> AddUpdate([FromBody] ERPSystemDto model)
        {
            var eRPSystemObj = await _erpSystemService.CheckInsertOrUpdate(model);

            if (eRPSystemObj != null)
            {
                var eRPSystemDto = _mapper.Map<ERPSystemDto>(eRPSystemObj);
                return new OperationResult<ERPSystemDto>(true, System.Net.HttpStatusCode.OK, "ERP system found successfully", eRPSystemDto);
            }
            else
            {
                return new OperationResult<ERPSystemDto>(false, System.Net.HttpStatusCode.OK, "Somehting went wrong");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{ErpSystemId}")]
        public async Task<OperationResult<ERPSystemDto>> Remove(int ErpSystemId)
        {
            var deleteErpSystemObj = _erpSystemService.DeleteERPSystem(ErpSystemId);

            if (deleteErpSystemObj != null)
            {
                var deleteErpSystemDtoObj = _mapper.Map<ERPSystemDto>(deleteErpSystemObj);
                return new OperationResult<ERPSystemDto>(true, System.Net.HttpStatusCode.OK, "ERP system deleted successfully", deleteErpSystemDtoObj);
            }
            else
            {
                return new OperationResult<ERPSystemDto>(false, System.Net.HttpStatusCode.OK, "Somehting went wrong");
            }
        }
    }
}