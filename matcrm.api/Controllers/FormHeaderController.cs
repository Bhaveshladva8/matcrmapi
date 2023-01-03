using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using matcrm.authentication.jwt;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Utility;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    public class FormHeaderController : Controller
    {
        private readonly IOneClappFormHeaderService _formHeaderService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private Common Common;
        public FormHeaderController(
            IOneClappFormHeaderService formHeaderService,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper,
            OneClappContext context
        )
        {
            _formHeaderService = formHeaderService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _context = context;
            Common = new Common();
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<FormHeaderAddUpdateResponse>> Add([FromForm] FormHeaderAddUpdateResquest model)
        {
            var requestmodel = _mapper.Map<OneClappFormHeaderDto>(model);
            if (requestmodel.CustomHeaderFile != null)
            {
                var headerFile = requestmodel.CustomHeaderFile;
                //var headerDirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormHeader";
                var headerDirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormHeaderDirPath;

                if (!Directory.Exists(headerDirPath))
                {
                    Directory.CreateDirectory(headerDirPath);
                }

                if (System.IO.File.Exists(headerDirPath))
                {
                    System.IO.File.Delete(Path.Combine(headerDirPath));
                }

                var headerFileName = string.Concat(
                    Path.GetFileNameWithoutExtension(headerFile.FileName),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(headerFile.FileName)
                );
                var HeaderFilePath = headerDirPath + "\\" + headerFileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(headerFile);
                    if (fileStatus)
                    {
                        return new OperationResult<FormHeaderAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(HeaderFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await headerFile.CopyToAsync(oStream);
                }

                requestmodel.CustomHeaderImage = headerFileName;
            }
            var AddUpdate = await _formHeaderService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<OneClappFormHeaderDto>(AddUpdate);
            var responsemodel = _mapper.Map<FormHeaderAddUpdateResponse>(requestmodel);
            return new OperationResult<FormHeaderAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<FormHeaderAddUpdateResponse>> Update([FromForm] FormHeaderAddUpdateResquest model)
        {
            var requestmodel = _mapper.Map<OneClappFormHeaderDto>(model);
            if (requestmodel.CustomHeaderFile != null)
            {
                var headerFile = requestmodel.CustomHeaderFile;
                //var headerDirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormHeader";
                var headerDirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormHeaderDirPath;

                if (!Directory.Exists(headerDirPath))
                {
                    Directory.CreateDirectory(headerDirPath);
                }

                if (System.IO.File.Exists(headerDirPath))
                {
                    System.IO.File.Delete(Path.Combine(headerDirPath));
                }

                var headerFileName = string.Concat(
                    Path.GetFileNameWithoutExtension(headerFile.FileName),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(headerFile.FileName)
                );
                var HeaderFilePath = headerDirPath + "\\" + headerFileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(headerFile);
                    if (fileStatus)
                    {
                        return new OperationResult<FormHeaderAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(HeaderFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await headerFile.CopyToAsync(oStream);
                }

                requestmodel.CustomHeaderImage = headerFileName;
            }
            var AddUpdate = await _formHeaderService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<OneClappFormHeaderDto>(AddUpdate);
            var responsemodel = _mapper.Map<FormHeaderAddUpdateResponse>(requestmodel);
            return new OperationResult<FormHeaderAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> File(long Id)
        {
            var headerBackgroundObj = _formHeaderService.GetById(Id);
            //var dirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormHeader";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormHeaderDirPath;
            var filePath = dirPath + "\\" + "default-profile.jpg";
            if (headerBackgroundObj != null && !string.IsNullOrEmpty(headerBackgroundObj.CustomHeaderImage))
            {
                filePath = dirPath + "\\" + headerBackgroundObj.CustomHeaderImage;

                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    // return File (b,GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
                    return File(bytes, Common.GetMimeTypes(headerBackgroundObj.CustomHeaderImage), headerBackgroundObj.CustomHeaderImage);
                }
                else
                {
                    filePath = dirPath + "\\" + "default-profile.jpg";
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    // return File (b,GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
                    return File(bytes, Common.GetMimeTypes(headerBackgroundObj.CustomHeaderImage), headerBackgroundObj.CustomHeaderImage);
                }
                //  Byte[] b = System.IO.File.ReadAllBytes (filePath);

            }
            return null;

        }       

    }
}