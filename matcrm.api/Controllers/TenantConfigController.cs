using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Common;
using matcrm.service.Services;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class TenantConfigController : Controller
    {

        private readonly ITenantConfigService _tenantConfigService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly OneClappContext _context;
        private int UserId = 0;
        private int TenantId = 0;
        public TenantConfigController(
            ITenantConfigService tenantConfigService,
            IHostingEnvironment hostingEnvironment,
            OneClappContext context
        )
        {
            _tenantConfigService = tenantConfigService;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<TenantConfig>> AddUpdate([FromForm] TenantConfigDto config)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            config.UserId = UserId;
            config.TenantId = TenantId;
            TenantConfig tenantConfigObj = new TenantConfig();

            if (config.IsUploadBgImg == true)
            {
                if (config.BgImgFile == null) throw new Exception("Backgroung Image File is null");

                var file = config.BgImgFile;
                //var dirPathForBgImgFile = _hostingEnvironment.WebRootPath + "\\BgImage";
                var dirPathForBgImgFile = _hostingEnvironment.WebRootPath + OneClappContext.BgImageDirPath;

                if (!Directory.Exists(dirPathForBgImgFile))
                {
                    Directory.CreateDirectory(dirPathForBgImgFile);
                }

                var bgImageName = string.Concat(
                    Path.GetFileNameWithoutExtension(file.FileName),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(file.FileName)
                );
                var bgImagefilePath = dirPathForBgImgFile + "\\" + bgImageName;
                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                    if (fileStatus)
                    {
                        return new OperationResult<TenantConfig>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(bgImagefilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }
                config.BackgroundImage = bgImageName;
            }

            if (config.IsUploadLogoImg == true)
            {
                if (config.LogoImgFile == null) throw new Exception("Logo Image File is null");

                var file = config.LogoImgFile;

                //var dirPathForLogoImgFile = _hostingEnvironment.WebRootPath + "\\LogoImage";
                var dirPathForLogoImgFile = _hostingEnvironment.WebRootPath + OneClappContext.LogoImageDirPath;

                if (!Directory.Exists(dirPathForLogoImgFile))
                {
                    Directory.CreateDirectory(dirPathForLogoImgFile);
                }

                var LogoImageName = string.Concat(
                    Path.GetFileNameWithoutExtension(file.FileName),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(file.FileName)
                );

                var LogoImagefilePath = dirPathForLogoImgFile + "\\" + LogoImageName;
                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                    if (fileStatus)
                    {
                        return new OperationResult<TenantConfig>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(LogoImagefilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                config.LogoImage = LogoImageName;
            }

            tenantConfigObj = await _tenantConfigService.CheckInsertOrUpdate(config);

            return new OperationResult<TenantConfig>(true, System.Net.HttpStatusCode.OK, "", tenantConfigObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{ConfigId}")]
        public async Task<OperationResult<TenantConfig>> Detail(long ConfigId)
        {

            var tenantConfigObj = _tenantConfigService.GetTenantConfigById(ConfigId);
            return new OperationResult<TenantConfig>(true, System.Net.HttpStatusCode.OK, "", tenantConfigObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]

        public async Task<OperationResult<TenantConfig>> UpdateLogoImage([FromForm] TenantConfigDto model)
        {
            TenantConfig tenantConfigObj = new TenantConfig();
            if (model.LogoImgFile != null)
            {
                if (model.Id != null)
                {
                    var configObj = _tenantConfigService.GetTenantConfigById(model.Id.Value);
                    if (configObj != null)
                    {
                        var logoImage = configObj.LogoImage;
                        //var dirPath = _hostingEnvironment.WebRootPath + "\\LogoImage";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.LogoImageDirPath;
                        var filePath = dirPath + "\\" + configObj.LogoImage;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        var file = model.LogoImgFile;
                        var LogoImageName = string.Concat(
                            Path.GetFileNameWithoutExtension(file.FileName),
                            DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                            Path.GetExtension(file.FileName)
                        );

                        var LogoImagefilePath = dirPath + "\\" + LogoImageName;

                        using (var oStream = new FileStream(LogoImagefilePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            await file.CopyToAsync(oStream);
                        }
                        configObj.IsUploadLogoImg = true;
                        configObj.LogoImage = LogoImageName;
                        configObj.UpdatedOn = DateTime.UtcNow;
                        var AddUpdate = await _tenantConfigService.UpdateTenantConfig(configObj, configObj.Id);

                        return new OperationResult<TenantConfig>(true, System.Net.HttpStatusCode.OK, "", AddUpdate);
                    }
                    else
                    {
                        return new OperationResult<TenantConfig>(false, System.Net.HttpStatusCode.OK, "Config not found!", tenantConfigObj);
                    }
                }
            }
            return new OperationResult<TenantConfig>(false, System.Net.HttpStatusCode.OK, "File not found!", tenantConfigObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]

        public async Task<OperationResult<TenantConfig>> UpdateBgImage([FromForm] TenantConfigDto model)
        {
            TenantConfig tenantConfigObj = new TenantConfig();
            if (model.BgImgFile != null)
            {
                if (model.Id != null)
                {
                    var configObj = _tenantConfigService.GetTenantConfigById(model.Id.Value);
                    if (configObj != null)
                    {
                        var BgImage = configObj.BackgroundImage;
                        //var dirPath = _hostingEnvironment.WebRootPath + "\\BgImage";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.BgImageDirPath;
                        var filePath = dirPath + "\\" + BgImage;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                        if (!Directory.Exists(dirPath))
                        {
                            Directory.CreateDirectory(dirPath);
                        }

                        var file = model.BgImgFile;
                        var BgImageName = string.Concat(
                            Path.GetFileNameWithoutExtension(file.FileName),
                            DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                            Path.GetExtension(file.FileName)
                        );

                        var BgImagefilePath = dirPath + "\\" + BgImageName;

                        using (var oStream = new FileStream(BgImagefilePath, FileMode.Create, FileAccess.ReadWrite))
                        {
                            await file.CopyToAsync(oStream);
                        }

                        configObj.BackgroundImage = BgImageName;
                        configObj.IsUploadBgImg = true;
                        configObj.UpdatedOn = DateTime.UtcNow;
                        var AddUpdate = await _tenantConfigService.UpdateTenantConfig(configObj, configObj.Id);

                        return new OperationResult<TenantConfig>(true, System.Net.HttpStatusCode.OK, "Update background image successfully.", AddUpdate);
                    }
                    else
                    {
                        return new OperationResult<TenantConfig>(false, System.Net.HttpStatusCode.OK, "Config not found!", tenantConfigObj);
                    }
                }
            }
            return new OperationResult<TenantConfig>(false, System.Net.HttpStatusCode.OK, "File not found!", tenantConfigObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult<TenantConfig>> Remove([FromBody] TenantConfigDto model)
        {
            TenantConfig tenantConfigObj = new TenantConfig();
            if (model.Id != null)
            {
                var ConfigObj = _tenantConfigService.GetTenantConfigById(model.Id.Value);
                if (ConfigObj != null)
                {
                    if (ConfigObj.IsUploadLogoImg == true)
                    {
                        var logoImage = ConfigObj.LogoImage;
                        //var dirPath = _hostingEnvironment.WebRootPath + "\\LogoImage";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.LogoImageDirPath;
                        var filePath = dirPath + "\\" + logoImage;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                    }

                    if (ConfigObj.IsUploadBgImg == true)
                    {
                        var BgImage = ConfigObj.BackgroundImage;
                        //var dirPath = _hostingEnvironment.WebRootPath + "\\BgImage";
                        var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.BgImageDirPath;
                        var filePath = dirPath + "\\" + BgImage;

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(Path.Combine(filePath));
                        }
                    }
                }
                tenantConfigObj = _tenantConfigService.DeleteTenantConfig(model.Id.Value);
                return new OperationResult<TenantConfig>(true, System.Net.HttpStatusCode.OK, "Config deleted successfully", tenantConfigObj);
            }
            else
                return new OperationResult<TenantConfig>(false, System.Net.HttpStatusCode.OK, "Config not found!", tenantConfigObj);
        }

    }
}