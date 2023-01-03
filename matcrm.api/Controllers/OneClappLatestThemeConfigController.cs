using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
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
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class OneClappLatestThemeConfigController : Controller
    {
        private readonly IOneClappLatestThemeConfigService _oneClappLatestThemeConfigService;
        private readonly IOneClappLatestThemeService _oneClappLatestThemeService;
        private readonly IOneClappLatestThemeLayoutService _oneClappLatestThemeLayoutService;
        private readonly IOneClappLatestThemeSchemeService _oneClappLatestThemeSchemeService;
        private readonly IUserService _userService;
        public IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public OneClappLatestThemeConfigController(IOneClappLatestThemeConfigService oneClappLatestThemeConfigService, IMapper mapper,
        IOneClappLatestThemeService oneClappLatestTheme, IOneClappLatestThemeSchemeService oneClappLatestThemeSchemeService,
        IOneClappLatestThemeLayoutService oneClappLatestThemeLayoutService, IUserService userService)
        {
            _mapper = mapper;
            _userService = userService;
            _oneClappLatestThemeConfigService = oneClappLatestThemeConfigService;
            _oneClappLatestThemeService = oneClappLatestTheme;
            _oneClappLatestThemeSchemeService = oneClappLatestThemeSchemeService;
            _oneClappLatestThemeLayoutService = oneClappLatestThemeLayoutService;
        }

        #region OneClappLatestThemeConfig
        [Authorize]
        [HttpPost]
        public async Task<OperationResult<OneClappLatestThemeConfigAddUpdateResponse>> AddUpdate([FromBody] OneClappLatestThemeConfigAddUpdateRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var requestmodel = _mapper.Map<OneClappLatestThemeConfigDto>(model);
            requestmodel.UserId = UserId;
            requestmodel.CreatedBy = UserId;
            
            if (!string.IsNullOrEmpty(requestmodel.Theme))
            {
                var oneClapplatestTheme = _oneClappLatestThemeService.GetByName(requestmodel.Theme);
                if (oneClapplatestTheme != null)
                {
                    requestmodel.OneClappLatestThemeId = oneClapplatestTheme.Id;
                }
            }
            if (!string.IsNullOrEmpty(requestmodel.Scheme))
            {
                var oneClapplatestThemeScheme = _oneClappLatestThemeSchemeService.GetByName(requestmodel.Scheme);
                if (oneClapplatestThemeScheme != null)
                {
                    requestmodel.OneClappLatestThemeSchemeId = oneClapplatestThemeScheme.Id;
                }
            }
            if (!string.IsNullOrEmpty(requestmodel.Layout))
            {
                var oneClapplatestThemeLayout = _oneClappLatestThemeLayoutService.GetByName(requestmodel.Layout);
                if (oneClapplatestThemeLayout != null)
                {
                    requestmodel.OneClappLatestThemeLayoutId = oneClapplatestThemeLayout.Id;
                }
            }
            var oneClappLatestThemeConfigObj = await _oneClappLatestThemeConfigService.CheckInsertOrUpdate(requestmodel);
            if (requestmodel.UserId != null)
            {
                var userObj = _userService.GetUserById(UserId);
                userObj.OneClappLatestThemeConfigId = oneClappLatestThemeConfigObj.Id;
                var ADDUpdate = await _userService.UpdateUser(userObj, false);
            }

            requestmodel = _mapper.Map<OneClappLatestThemeConfigDto>(oneClappLatestThemeConfigObj);
            var responsemodel = _mapper.Map<OneClappLatestThemeConfigAddUpdateResponse>(requestmodel);
            if (requestmodel.Id > 0)
                return new OperationResult<OneClappLatestThemeConfigAddUpdateResponse>(true, System.Net.HttpStatusCode.OK,"Updated successfully", responsemodel);
            return new OperationResult<OneClappLatestThemeConfigAddUpdateResponse>(false, System.Net.HttpStatusCode.OK,"Added successfully.", responsemodel);
        }
   
        [HttpDelete]
        public async Task<OperationResult<OneClappLatestThemeConfigDto>> Remove([FromBody] OneClappLatestThemeConfigDto model)
        {
            if (model.Id != null)
            {
                var oneClappLatestThemeConfigObj = _oneClappLatestThemeConfigService.DeleteOneClappLatestThemeConfig(model);
                model = _mapper.Map<OneClappLatestThemeConfigDto>(oneClappLatestThemeConfigObj);
                return new OperationResult<OneClappLatestThemeConfigDto>(true, System.Net.HttpStatusCode.OK, "OneClappLatestThemeConfig deleted successfully.", model);
            }
            else
            {
                return new OperationResult<OneClappLatestThemeConfigDto>(false, System.Net.HttpStatusCode.OK, "Please provide oneClappLatestThemeConfigId.", model);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<OneClappLatestThemeConfigDto>>> List()
        {
            List<OneClappLatestThemeConfigDto> oneClappLatestThemeConfigDtoList = new List<OneClappLatestThemeConfigDto>();
            var oneClappLatestThemeConfigList = new List<OneClappLatestThemeConfig>();
            oneClappLatestThemeConfigList = _oneClappLatestThemeConfigService.GetAll();
            oneClappLatestThemeConfigDtoList = _mapper.Map<List<OneClappLatestThemeConfigDto>>(oneClappLatestThemeConfigList);

            var latestTheme = _oneClappLatestThemeService.GetAll();
            var latestThemeScheme = _oneClappLatestThemeSchemeService.GetAll();
            var latestThemeLayout = _oneClappLatestThemeLayoutService.GetAll();
            var users = _userService.GetAll();

            if (oneClappLatestThemeConfigDtoList != null && oneClappLatestThemeConfigDtoList.Count() > 0)
            {
                foreach (var item in oneClappLatestThemeConfigDtoList)
                {
                    if (latestTheme != null)
                    {
                        var theme = latestTheme.Where(d => d.Id == item.OneClappLatestThemeId).FirstOrDefault();
                        item.LatestTheme = _mapper.Map<OneClappLatestThemeDto>(theme);
                    }

                    if (latestThemeScheme != null)
                    {
                        var themeScheme = latestThemeScheme.Where(d => d.Id == item.OneClappLatestThemeSchemeId).FirstOrDefault();
                        item.LatestThemeScheme = _mapper.Map<OneClappLatestThemeSchemeDto>(themeScheme);
                    }

                    if (latestThemeLayout != null)
                    {
                        var themeLayout = latestThemeLayout.Where(d => d.Id == item.OneClappLatestThemeLayoutId).FirstOrDefault();
                        item.LatestThemeLayout = _mapper.Map<OneClappLatestThemeLayoutDto>(themeLayout);
                    }

                    if (users != null)
                    {
                        var User = users.Where(d => d.OneClappLatestThemeConfigId == item.Id).FirstOrDefault();
                        item.User = _mapper.Map<UserDto>(User);
                    }
                }
            }
            return new OperationResult<List<OneClappLatestThemeConfigDto>>(true, System.Net.HttpStatusCode.OK, "Found successfully", oneClappLatestThemeConfigDtoList);
        }
        
        [HttpGet("{Id}")]
        public async Task<OperationResult<OneClappLatestThemeConfigGetByIdResponse>> Detail(long Id)
        {
            OneClappLatestThemeConfigDto oneClappLatestThemeConfigDto = new OneClappLatestThemeConfigDto();
            var oneClappLatestThemeConfigObj = _oneClappLatestThemeConfigService.GetOneClappLatestThemeConfigById(Id);
            oneClappLatestThemeConfigDto = _mapper.Map<OneClappLatestThemeConfigDto>(oneClappLatestThemeConfigObj);

            var latestTheme = _oneClappLatestThemeService.GetAll();
            var latestThemeScheme = _oneClappLatestThemeSchemeService.GetAll();
            var latestThemeLayout = _oneClappLatestThemeLayoutService.GetAll();
            var users = _userService.GetAll();

            if (oneClappLatestThemeConfigDto != null)
            {
                if (latestTheme != null)
                {
                    var theme = latestTheme.Where(d => d.Id == oneClappLatestThemeConfigDto.OneClappLatestThemeId).FirstOrDefault();
                    oneClappLatestThemeConfigDto.LatestTheme = _mapper.Map<OneClappLatestThemeDto>(theme);
                    if (theme != null)
                    {
                        oneClappLatestThemeConfigDto.Theme = theme.Name;
                    }
                    else
                    {
                        oneClappLatestThemeConfigDto.Theme = "default";
                    }
                }

                if (latestThemeScheme != null)
                {
                    var themeScheme = latestThemeScheme.Where(d => d.Id == oneClappLatestThemeConfigDto.OneClappLatestThemeSchemeId).FirstOrDefault();
                    oneClappLatestThemeConfigDto.LatestThemeScheme = _mapper.Map<OneClappLatestThemeSchemeDto>(themeScheme);
                    if (themeScheme != null)
                    {
                        oneClappLatestThemeConfigDto.Scheme = themeScheme.Name;
                    }
                    else
                    {
                        oneClappLatestThemeConfigDto.Scheme = "auto";
                    }
                }

                if (latestThemeLayout != null)
                {
                    var themeLayout = latestThemeLayout.Where(d => d.Id == oneClappLatestThemeConfigDto.OneClappLatestThemeLayoutId).FirstOrDefault();
                    if (themeLayout != null)
                    {
                        oneClappLatestThemeConfigDto.Layout = themeLayout.Name;
                    }
                    else
                    {
                        oneClappLatestThemeConfigDto.Layout = "thin";
                    }

                    oneClappLatestThemeConfigDto.LatestThemeLayout = _mapper.Map<OneClappLatestThemeLayoutDto>(themeLayout);
                }

                if (users != null)
                {
                    var User = users.Where(d => d.OneClappLatestThemeConfigId == oneClappLatestThemeConfigDto.Id).FirstOrDefault();
                    oneClappLatestThemeConfigDto.User = _mapper.Map<UserDto>(User);
                }
            }
            var ResponseThemeConfigDtos = _mapper.Map<OneClappLatestThemeConfigGetByIdResponse>(oneClappLatestThemeConfigDto);    
            return new OperationResult<OneClappLatestThemeConfigGetByIdResponse>(true, System.Net.HttpStatusCode.OK,"", ResponseThemeConfigDtos);
        }

        [HttpGet("{UserId}")]
        public async Task<OperationResult<OneClappLatestThemeConfigDto>> BasedOnUser(int UserId)
        {
            OneClappLatestThemeConfigDto oneClappLatestThemeConfigDto = new OneClappLatestThemeConfigDto();
            var userObj = _userService.GetUserById(UserId);
            if (userObj.OneClappLatestThemeConfigId != null)
            {
                var configId = userObj.OneClappLatestThemeConfigId.Value;
                oneClappLatestThemeConfigDto = new OneClappLatestThemeConfigDto();
                var OneClappLatestThemeConfigObj = _oneClappLatestThemeConfigService.GetOneClappLatestThemeConfigById(configId);
                oneClappLatestThemeConfigDto = _mapper.Map<OneClappLatestThemeConfigDto>(OneClappLatestThemeConfigObj);

                var latestTheme = _oneClappLatestThemeService.GetAll();
                var latestThemeScheme = _oneClappLatestThemeSchemeService.GetAll();
                var latestThemeLayout = _oneClappLatestThemeLayoutService.GetAll();
                var users = _userService.GetAll();

                if (oneClappLatestThemeConfigDto != null)
                {
                    if (latestTheme != null)
                    {
                        var theme = latestTheme.Where(d => d.Id == oneClappLatestThemeConfigDto.OneClappLatestThemeId).FirstOrDefault();
                        oneClappLatestThemeConfigDto.LatestTheme = _mapper.Map<OneClappLatestThemeDto>(theme);
                        oneClappLatestThemeConfigDto.Theme = theme.Name;
                    }

                    if (latestThemeScheme != null)
                    {
                        var themeScheme = latestThemeScheme.Where(d => d.Id == oneClappLatestThemeConfigDto.OneClappLatestThemeSchemeId).FirstOrDefault();
                        oneClappLatestThemeConfigDto.LatestThemeScheme = _mapper.Map<OneClappLatestThemeSchemeDto>(themeScheme);

                        oneClappLatestThemeConfigDto.Scheme = themeScheme.Name;
                    }

                    if (latestThemeLayout != null)
                    {
                        var themeLayout = latestThemeLayout.Where(d => d.Id == oneClappLatestThemeConfigDto.OneClappLatestThemeLayoutId).FirstOrDefault();
                        oneClappLatestThemeConfigDto.LatestThemeLayout = _mapper.Map<OneClappLatestThemeLayoutDto>(themeLayout);
                        oneClappLatestThemeConfigDto.Layout = themeLayout.Name;
                    }

                    if (users != null)
                    {
                        var User = users.Where(d => d.OneClappLatestThemeConfigId == oneClappLatestThemeConfigDto.Id).FirstOrDefault();
                        oneClappLatestThemeConfigDto.User = _mapper.Map<UserDto>(User);
                    }
                }
            }

            return new OperationResult<OneClappLatestThemeConfigDto>(true, System.Net.HttpStatusCode.OK, "", oneClappLatestThemeConfigDto);
        }

        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        
        [HttpGet]
        public async Task<OperationResult<OneClappLatestLayoutSettingResponse>> Layout()
        {
            OneClappLatestLayoutSettingDto oneClappLatestLayoutSettingDto = new OneClappLatestLayoutSettingDto();

            var themes = _oneClappLatestThemeService.GetAll();
            var schemes = _oneClappLatestThemeSchemeService.GetAll();
            var layouts = _oneClappLatestThemeLayoutService.GetAll();

            oneClappLatestLayoutSettingDto.ThemeList = _mapper.Map<List<OneClappLatestThemeDto>>(themes);
            oneClappLatestLayoutSettingDto.SchemeList = _mapper.Map<List<OneClappLatestThemeSchemeDto>>(schemes);
            oneClappLatestLayoutSettingDto.LayoutList = _mapper.Map<List<OneClappLatestThemeLayoutDto>>(layouts);
            var responseLayoutsettingDto = _mapper.Map<OneClappLatestLayoutSettingResponse>(oneClappLatestLayoutSettingDto);
            return new OperationResult<OneClappLatestLayoutSettingResponse>(true, System.Net.HttpStatusCode.OK,"", responseLayoutsettingDto);           
        }
        #endregion
    }
}