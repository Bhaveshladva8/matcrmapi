using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OneClappLatestThemeLayoutController : Controller
    {
        private readonly IOneClappLatestThemeLayoutService _oneClappLatestThemeLayoutService;

        public IMapper _mapper;
        public OneClappLatestThemeLayoutController(IMapper mapper,
        IOneClappLatestThemeLayoutService oneClappLatestThemeLayoutService
        )
        {
            _mapper = mapper;
            _oneClappLatestThemeLayoutService = oneClappLatestThemeLayoutService;
        }

        #region OneClappLatestThemeLayout
        [HttpPost]
        public async Task<OperationResult<OneClappLatestThemeLayoutDto>> AddUpdate([FromBody] OneClappLatestThemeLayoutDto model)
        {
            var oneClappLatestThemeLayoutObj = await _oneClappLatestThemeLayoutService.CheckInsertOrUpdate(model);
            model = _mapper.Map<OneClappLatestThemeLayoutDto>(oneClappLatestThemeLayoutObj);
            if (model.Id > 0)
                return new OperationResult<OneClappLatestThemeLayoutDto>(true, System.Net.HttpStatusCode.OK,"Updated successfully", model);
            return new OperationResult<OneClappLatestThemeLayoutDto>(false, System.Net.HttpStatusCode.OK,"Added successfully.", model);
        }

        [HttpDelete]
        public async Task<OperationResult<OneClappLatestThemeLayoutDto>> Remove([FromBody] OneClappLatestThemeLayoutDto model)
        {
            if (model.Id != null)
            {
                var oneClappLatestThemeLayoutObj = _oneClappLatestThemeLayoutService.DeleteOneClappLatestThemeLayout(model);
                model = _mapper.Map<OneClappLatestThemeLayoutDto>(oneClappLatestThemeLayoutObj);
                return new OperationResult<OneClappLatestThemeLayoutDto>(true, System.Net.HttpStatusCode.OK,"OneClappLatestThemeLayout deleted successfully.", model);
            }
            else
            {
                return new OperationResult<OneClappLatestThemeLayoutDto>(false, System.Net.HttpStatusCode.OK,"Please provide oneClappLatestThemeLayoutId.", model);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<OneClappLatestThemeLayoutDto>>> List()
        {
            List<OneClappLatestThemeLayoutDto> oneClappLatestThemeLayoutDtoList = new List<OneClappLatestThemeLayoutDto>();
            var oneClappLatestThemeLayoutList = new List<OneClappLatestThemeLayout>();
            oneClappLatestThemeLayoutList = _oneClappLatestThemeLayoutService.GetAll();
            oneClappLatestThemeLayoutDtoList = _mapper.Map<List<OneClappLatestThemeLayoutDto>>(oneClappLatestThemeLayoutList);

            return new OperationResult<List<OneClappLatestThemeLayoutDto>>(true, System.Net.HttpStatusCode.OK,"Found successfully", oneClappLatestThemeLayoutDtoList);
        }
        //[Authorize (Roles = "Admin")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<OneClappLatestThemeLayoutDto>> Detail(long Id)
        {
            OneClappLatestThemeLayoutDto oneClappLatestThemeLayoutDto = new OneClappLatestThemeLayoutDto();
            var OneClappLatestThemeLayoutObj = _oneClappLatestThemeLayoutService.GetOneClappLatestThemeLayoutById(Id);
            oneClappLatestThemeLayoutDto = _mapper.Map<OneClappLatestThemeLayoutDto>(OneClappLatestThemeLayoutObj);

            return new OperationResult<OneClappLatestThemeLayoutDto>(true, System.Net.HttpStatusCode.OK,"", oneClappLatestThemeLayoutDto);
        }
        #endregion  
    }
}