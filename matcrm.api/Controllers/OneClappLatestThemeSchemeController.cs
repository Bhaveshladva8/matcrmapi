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
    public class OneClappLatestThemeSchemeController : Controller
    {
        private readonly IOneClappLatestThemeSchemeService _oneClappLatestThemeSchemeService;
        public IMapper _mapper;
        public OneClappLatestThemeSchemeController(IOneClappLatestThemeSchemeService oneClappLatestThemeSchemeService, IMapper mapper)
        {
            _mapper = mapper;
            _oneClappLatestThemeSchemeService = oneClappLatestThemeSchemeService;
        }

        #region OneClappLatestScheme
        [HttpPost]
        public async Task<OperationResult<OneClappLatestThemeSchemeDto>> AddUpdate([FromBody] OneClappLatestThemeSchemeDto model)
        {
            var oneClappLatestThemeSchemeObj = await _oneClappLatestThemeSchemeService.CheckInsertOrUpdate(model);
            model = _mapper.Map<OneClappLatestThemeSchemeDto>(oneClappLatestThemeSchemeObj);
            if (model.Id > 0)
                return new OperationResult<OneClappLatestThemeSchemeDto>(true, System.Net.HttpStatusCode.OK, "Updated successfully", model);
            return new OperationResult<OneClappLatestThemeSchemeDto>(false, System.Net.HttpStatusCode.OK, "Added successfully.", model);
        }

        [HttpDelete]
        public async Task<OperationResult<OneClappLatestThemeSchemeDto>> Remove([FromBody] OneClappLatestThemeSchemeDto model)
        {
            if (model.Id != null)
            {
                var oneClappLatestThemeSchemeObj = _oneClappLatestThemeSchemeService.DeleteOneClappLatestThemeScheme(model);
                model = _mapper.Map<OneClappLatestThemeSchemeDto>(oneClappLatestThemeSchemeObj);
                return new OperationResult<OneClappLatestThemeSchemeDto>(true, System.Net.HttpStatusCode.OK, "OneClappLatestThemeScheme deleted successfully.", model);
            }
            else
            {
                return new OperationResult<OneClappLatestThemeSchemeDto>(false, System.Net.HttpStatusCode.OK, "Please provide oneClappLatestThemeSchemeId.", model);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<OneClappLatestThemeSchemeDto>>> List()
        {
            List<OneClappLatestThemeSchemeDto> oneClappLatestThemeSchemeDtoList = new List<OneClappLatestThemeSchemeDto>();
            var oneClappLatestThemeSchemeList = new List<OneClappLatestThemeScheme>();
            oneClappLatestThemeSchemeList = _oneClappLatestThemeSchemeService.GetAll();
            oneClappLatestThemeSchemeDtoList = _mapper.Map<List<OneClappLatestThemeSchemeDto>>(oneClappLatestThemeSchemeList);

            return new OperationResult<List<OneClappLatestThemeSchemeDto>>(true, System.Net.HttpStatusCode.OK, "Found successfully", oneClappLatestThemeSchemeDtoList);
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<OneClappLatestThemeSchemeDto>> Detail(long Id)
        {
            OneClappLatestThemeSchemeDto oneClappLatestThemeSchemeDto = new OneClappLatestThemeSchemeDto();
            var OneClappLatestThemeSchemeObj = _oneClappLatestThemeSchemeService.GetOneClappLatestThemeSchemeById(Id);
            oneClappLatestThemeSchemeDto = _mapper.Map<OneClappLatestThemeSchemeDto>(OneClappLatestThemeSchemeObj);

            return new OperationResult<OneClappLatestThemeSchemeDto>(true, System.Net.HttpStatusCode.OK, "", oneClappLatestThemeSchemeDto);
        }
        #endregion
    }
}