using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using matcrm.service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AutoMapper;
using matcrm.service.Common;
using matcrm.data.Models.Response;
using matcrm.data.Models.Request;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using matcrm.data.Models.Tables;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using matcrm.data.Context;
using matcrm.service.Utility;
using matcrm.data.Models.Dto;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class MateCategoryController : Controller
    {
        private readonly IMateCategoryService _mateCategoryService;
        private readonly ICustomTableService _customTableService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private Common Common;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public MateCategoryController(IMateCategoryService mateCategoryService,
        ICustomTableService customTableService,
        IHostingEnvironment hostingEnvironment,
        IMapper mapper)
        {
            _mateCategoryService = mateCategoryService;
            _customTableService = customTableService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            Common = new Common();
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<MateCategoryAddResponse>>> Add([FromForm] MateCategoryAddRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            MateCategoryAddResponse categoryResponseObj = new MateCategoryAddResponse();

            List<MateCategoryAddResponse> categoryResponseList = new List<MateCategoryAddResponse>();

            var mateCategoryDtoObj = _mapper.Map<MateCategoryDto>(requestModel);

            if (mateCategoryDtoObj.Id > 0)
            {
                mateCategoryDtoObj.UpdatedBy = UserId;
            }
            else
            {
                mateCategoryDtoObj.CreatedBy = UserId;
            }

            var filePath = "";
            
            if (requestModel.Icon != null)
            {
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCategoryIconDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestModel.Icon.FileName),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestModel.Icon.FileName)
                            );
                filePath = dirPath + "\\" + fileName;


                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }

                mateCategoryDtoObj.Icon = fileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestModel.Icon);
                    if (fileStatus)
                    {
                        return new OperationResult<List<MateCategoryAddResponse>>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await requestModel.Icon.CopyToAsync(oStream);
                }
            }


            if (requestModel.CustomTableIds != null && requestModel.CustomTableIds.Count() > 0)
            {
                foreach (var item in requestModel.CustomTableIds)
                {
                    mateCategoryDtoObj.CustomTableId = item;
                    //MateCategory mateCategoryModel = new MateCategory();
                    //mateCategoryModel = _mapper.Map<>
                    //model.TenantId = TenantId;
                    //var testDTo = _mapper.Map<MatecategoryDto>(mateCategotymodel);
                    var AddUpdate = await _mateCategoryService.CheckInsertOrUpdate(mateCategoryDtoObj, TenantId);
                    categoryResponseObj = _mapper.Map<MateCategoryAddResponse>(AddUpdate);

                    categoryResponseObj.CustomTableId = item;

                    var customModuleObj = _customTableService.GetById(item);
                    if (customModuleObj != null)
                    {
                        categoryResponseObj.CustomTableName = customModuleObj.Name;
                    }
                    if (AddUpdate.Icon != null)
                    {
                        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        categoryResponseObj.IconName = AddUpdate.Icon;
                        categoryResponseObj.IconURL = OneClappContext.CurrentURL + "MateCategory/Icon/" + categoryResponseObj.Id + "?" + Timestamp;
                    }
                    categoryResponseList.Add(categoryResponseObj);
                }
            }
            //responseAddupdate.CustomTableIds = requestModel.CustomTableIds;
            return new OperationResult<List<MateCategoryAddResponse>>(true, System.Net.HttpStatusCode.OK, "Category added successfully", categoryResponseList);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<MateCategoryUpdateResponse>> Update([FromForm] MateCategoryUpdateRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            MateCategoryUpdateResponse categoryResponseObj = new MateCategoryUpdateResponse();

            var mateCategoryDtoObj = _mapper.Map<MateCategoryDto>(requestModel);

            if (mateCategoryDtoObj.Id > 0)
            {
                mateCategoryDtoObj.UpdatedBy = UserId;
            }
            else
            {
                mateCategoryDtoObj.CreatedBy = UserId;
            }

            var filePath = "";

            if (requestModel.IconName != null)
            {
                mateCategoryDtoObj.Icon = requestModel.IconName;
            }
            if (requestModel.Icon != null)
            {
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCategoryIconDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestModel.Icon.FileName),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestModel.Icon.FileName)
                            );
                filePath = dirPath + "\\" + fileName;


                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }

                mateCategoryDtoObj.Icon = fileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestModel.Icon);
                    if (fileStatus)
                    {
                        return new OperationResult<MateCategoryUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await requestModel.Icon.CopyToAsync(oStream);
                }
            }
            var AddUpdate = await _mateCategoryService.CheckInsertOrUpdate(mateCategoryDtoObj, TenantId);
            categoryResponseObj = _mapper.Map<MateCategoryUpdateResponse>(AddUpdate);
            if (categoryResponseObj != null)
            {
                var customModuleObj = _customTableService.GetById(requestModel.CustomTableId);
                if (customModuleObj != null)
                {
                    categoryResponseObj.CustomTableName = customModuleObj.Name;
                }
                if (requestModel.Icon != null)
                {
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    
                    categoryResponseObj.IconURL = OneClappContext.CurrentURL + "MateCategory/Icon/" + categoryResponseObj.Id + "?" + Timestamp;
                }
                if(requestModel.IconName!=null)
                {
                    categoryResponseObj.IconName = AddUpdate.Icon;
                }
            }
            return new OperationResult<MateCategoryUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Category updated successfully", categoryResponseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            if (Id != null)
            {
                var mateCategoryObj = await _mateCategoryService.DeleteById(Id);

                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpGet]
        public async Task<OperationResult<List<MateCategoryListResponse>>> List()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var mateCategoryList = _mateCategoryService.GetByTenant(TenantId);
            //var categoryResponseList = _mapper.Map<List<MateCategoryListResponse>>(mateCategoryList);
            List<MateCategoryListResponse> categoryResponseList = new List<MateCategoryListResponse>();
            if (mateCategoryList != null && mateCategoryList.Count() > 0)
            {
                foreach (var item in mateCategoryList)
                {
                    MateCategoryListResponse responseObj = new MateCategoryListResponse();
                    responseObj = _mapper.Map<MateCategoryListResponse>(item);
                    //var customTableObj = mateCategoryList.Where(t => t.CustomTable.Id == item?.CustomTableId).FirstOrDefault();
                    if (item.CustomTableId != null)
                    {
                        responseObj.CustomTableName = item.CustomTable?.Name;
                    }
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (item.Icon != null)
                    {
                        responseObj.IconURL = OneClappContext.CurrentURL + "MateCategory/Icon/" + item.Id + "?" + Timestamp;
                    }
                    else
                    {
                        responseObj.IconURL = null;
                    }
                    categoryResponseList.Add(responseObj);
                }
            }
            return new OperationResult<List<MateCategoryListResponse>>(true, System.Net.HttpStatusCode.OK, "", categoryResponseList);
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> Icon(int Id)
        {
            var mateCategoryObj = _mateCategoryService.GetById(Id);
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.MateCategoryIconDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (mateCategoryObj != null && !string.IsNullOrEmpty(mateCategoryObj.Icon))
            {
                filePath = dirPath + "\\" + mateCategoryObj.Icon;
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    return File(bytes, Common.GetMimeTypes(mateCategoryObj.Icon), mateCategoryObj.Icon);
                }
            }
            return null;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet("{Id}")]
        public async Task<OperationResult<MateCategoryDetailResponse>> Detail(long Id)
        {
            var mateCategoryObj = _mateCategoryService.GetById(Id);
            var responseObj = _mapper.Map<MateCategoryDetailResponse>(mateCategoryObj);
            if (mateCategoryObj != null)
            {
                if (mateCategoryObj.Icon != null)
                {
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    responseObj.IconName = mateCategoryObj.Icon;
                    responseObj.IconURL = OneClappContext.CurrentURL + "MateCategory/Icon/" + mateCategoryObj.Id + "?" + Timestamp;
                }
                if (mateCategoryObj.CustomTableId != null)
                {
                    responseObj.CustomTableName = mateCategoryObj.CustomTable?.Name;
                }
            }
            return new OperationResult<MateCategoryDetailResponse>(true, System.Net.HttpStatusCode.OK, "", responseObj);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{CustomTableName}")]
        public async Task<OperationResult<List<MateCategoryListByTableResponse>>> ListByTable(string CustomTableName)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<MateCategory> mateCategoryList = new List<MateCategory>();

            var customTable = _customTableService.GetByName(CustomTableName);
            if (customTable != null)
            {
                mateCategoryList = _mateCategoryService.GetByCustomTableName(customTable.Id, TenantId);
            }

            var responseStatusList = _mapper.Map<List<MateCategoryListByTableResponse>>(mateCategoryList);
            return new OperationResult<List<MateCategoryListByTableResponse>>(true, System.Net.HttpStatusCode.OK, "", responseStatusList);
        }
    }
}