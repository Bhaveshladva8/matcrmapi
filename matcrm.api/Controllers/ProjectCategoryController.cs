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
using Microsoft.AspNetCore.Hosting;
using System.IO;
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ProjectCategoryController : Controller
    {
        private readonly IProjectCategoryService _projectCategoryService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IMapper _mapper;
        private Common Common;
        private int UserId = 0;
        private int TenantId = 0;
        public ProjectCategoryController(IProjectCategoryService projectCategoryService,
        IHostingEnvironment hostingEnvironment,
        IMapper mapper)
        {
            _projectCategoryService = projectCategoryService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            Common = new Common();
        }

        [HttpPost]
        public async Task<OperationResult<ProjectCategoryAddUpdateResponse>> Add([FromForm] ProjectCategoryAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ProjectCategory>(requestmodel);
            if (model.Id == 0)
            {
                model.CreatedBy = UserId;
            }
            var filePath = "";
            if (requestmodel.FileName != null)
            {
                model.Icon = requestmodel.FileName;
            }
            if (requestmodel.File != null)
            {
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ProjectCategoryIconDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestmodel.File.FileName),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestmodel.File.FileName)
                            );
                filePath = dirPath + "\\" + fileName;


                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }

                model.Icon = fileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestmodel.File);
                    if (fileStatus)
                    {
                        return new OperationResult<ProjectCategoryAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await requestmodel.File.CopyToAsync(oStream);
                }
            }
            var projectCategoryObj = await _projectCategoryService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ProjectCategoryAddUpdateResponse>(projectCategoryObj);
            return new OperationResult<ProjectCategoryAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Added successfully.", responseObj);
        }

        [HttpPut]
        public async Task<OperationResult<ProjectCategoryAddUpdateResponse>> Update([FromForm] ProjectCategoryAddUpdateRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            var model = _mapper.Map<ProjectCategory>(requestmodel);
            // if (model.Id > 0)
            // {
            //     model.UpdatedBy = UserId;
            // }
            var filePath = "";
            if (requestmodel.FileName != null)
            {
                model.Icon = requestmodel.FileName;
            }
            if (requestmodel.File != null)
            {
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ProjectCategoryIconDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(requestmodel.File.FileName),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(requestmodel.File.FileName)
                            );
                filePath = dirPath + "\\" + fileName;


                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }

                model.Icon = fileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(requestmodel.File);
                    if (fileStatus)
                    {
                        return new OperationResult<ProjectCategoryAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await requestmodel.File.CopyToAsync(oStream);
                }
            }
            var projectCategoryObj = await _projectCategoryService.CheckInsertOrUpdate(model);
            var responseObj = _mapper.Map<ProjectCategoryAddUpdateResponse>(projectCategoryObj);
            return new OperationResult<ProjectCategoryAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "Updated successfully.", responseObj);
        }

        [HttpDelete("{Id}")]
        public async Task<OperationResult> Remove(int Id)
        {
            if (Id > 0)
            {
                var projectCategoryObj = await _projectCategoryService.DeleteById(Id);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Deleted successfully", Id);
            }
            else
            {
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "Please provide Id", Id);
            }
        }

        [HttpGet]
        public async Task<OperationResult<List<ProjectCategoryListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var projectCategoryList = _projectCategoryService.GetByTenant(TenantId);
            List<ProjectCategoryListResponse> responseList = new List<ProjectCategoryListResponse>();
            if (projectCategoryList != null && projectCategoryList.Count > 0)
            {
                foreach (var item in projectCategoryList)
                {
                    ProjectCategoryListResponse responseObj = new ProjectCategoryListResponse();
                    responseObj.Id = item.Id;
                    responseObj.Name = item.Name;
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (item.Icon != null)
                    {
                        responseObj.IconURL = OneClappContext.CurrentURL + "ProjectCategory/Icon/" + responseObj.Id + "?" + Timestamp;
                    }
                    else
                    {
                        responseObj.IconURL = null;
                    }
                    responseList.Add(responseObj);
                }
            }
            return new OperationResult<List<ProjectCategoryListResponse>>(true, System.Net.HttpStatusCode.OK, "", responseList);
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> Icon(int Id)
        {
            var projectCategoryObj = _projectCategoryService.GetById(Id);
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ProjectCategoryIconDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (projectCategoryObj != null && !string.IsNullOrEmpty(projectCategoryObj.Icon))
            {
                filePath = dirPath + "\\" + projectCategoryObj.Icon;
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    return File(bytes, Common.GetMimeTypes(projectCategoryObj.Icon), projectCategoryObj.Icon);
                }
            }
            return null;
        }

        [HttpGet("{Id}")]
        public async Task<OperationResult<ProjectCategoryDetailResponse>> Detail(long Id)
        {
            var projectCategoryObj = _projectCategoryService.GetById(Id);
            var ResponseObj = _mapper.Map<ProjectCategoryDetailResponse>(projectCategoryObj);
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (projectCategoryObj.Icon != null)
            {
                ResponseObj.FileName = projectCategoryObj.Icon;
                ResponseObj.IconURL = OneClappContext.CurrentURL + "ProjectCategory/Icon/" + ResponseObj.Id + "?" + Timestamp;
            }
            else
            {
                ResponseObj.FileName = null;
                ResponseObj.IconURL = null;
            }
            return new OperationResult<ProjectCategoryDetailResponse>(true, System.Net.HttpStatusCode.OK, "", ResponseObj);
        }
    }
}