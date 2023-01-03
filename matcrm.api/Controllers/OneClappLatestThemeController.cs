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
    public class OneClappLatestThemeController : Controller
    {
        private readonly IOneClappLatestThemeService _oneClappLatestThemeService;

        public IMapper _mapper;

        public OneClappLatestThemeController(IOneClappLatestThemeService oneClappLatestThemeService, IMapper mapper)
        {
            _mapper = mapper;
            _oneClappLatestThemeService = oneClappLatestThemeService;
        }
        #region OneClappLatestTheme
        [HttpPost]
        public async Task<OperationResult<OneClappLatestThemeDto>> AddUpdate([FromBody] OneClappLatestThemeDto model)
        {
            var oneClappLatestThemeObj = await _oneClappLatestThemeService.CheckInsertOrUpdate(model);
            model = _mapper.Map<OneClappLatestThemeDto>(oneClappLatestThemeObj);
            if (model.Id > 0)
                return new OperationResult<OneClappLatestThemeDto>(true, System.Net.HttpStatusCode.OK,"Updated successfully", model);
            return new OperationResult<OneClappLatestThemeDto>(false, System.Net.HttpStatusCode.OK,"Added successfully.", model);
        }

        [HttpDelete]
        public async Task<OperationResult<OneClappLatestThemeDto>> Remove([FromBody] OneClappLatestThemeDto model)
        {
            if (model.Id != null)
            {
                var oneClappLatestThemeObj = _oneClappLatestThemeService.DeleteOneClappLatestTheme(model);
                model = _mapper.Map<OneClappLatestThemeDto>(oneClappLatestThemeObj);
                return new OperationResult<OneClappLatestThemeDto>(true, System.Net.HttpStatusCode.OK,"OneClappLatestTheme deleted successfully.", model);
            }
            else
            {
                return new OperationResult<OneClappLatestThemeDto>(false, System.Net.HttpStatusCode.OK,"Please provide oneClappLatestThemeId.", model);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<OneClappLatestThemeDto>>> List()
        {
            List<OneClappLatestThemeDto> oneClappLatestThemeDtoList = new List<OneClappLatestThemeDto>();
            var oneClappLatestThemeList = new List<OneClappLatestTheme>();
            oneClappLatestThemeList = _oneClappLatestThemeService.GetAll();
            oneClappLatestThemeDtoList = _mapper.Map<List<OneClappLatestThemeDto>>(oneClappLatestThemeList);

            return new OperationResult<List<OneClappLatestThemeDto>>(true, System.Net.HttpStatusCode.OK,"Found successfully", oneClappLatestThemeDtoList);
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<OneClappLatestThemeDto>> Detail(long Id)
        {
            OneClappLatestThemeDto oneClappLatestThemeDto = new OneClappLatestThemeDto();
            var OneClappLatestThemeObj = _oneClappLatestThemeService.GetOneClappLatestThemeById(Id);
            oneClappLatestThemeDto = _mapper.Map<OneClappLatestThemeDto>(OneClappLatestThemeObj);

            return new OperationResult<OneClappLatestThemeDto>(true, System.Net.HttpStatusCode.OK,"", oneClappLatestThemeDto);
        }
        #endregion
    }
}