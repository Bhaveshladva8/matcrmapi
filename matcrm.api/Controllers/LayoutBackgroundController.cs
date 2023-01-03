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
    [Authorize]
    [Route("[controller]/[action]")]
    public class LayoutBackgroundController : Controller
    {
        private readonly IOneClappFormLayoutBackgroundService _oneClappFormLayoutBackgroundService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IMapper _mapper;
        private Common Common;
        private readonly OneClappContext _context;
        public LayoutBackgroundController(
            IOneClappFormLayoutBackgroundService oneClappFormLayoutBackgroundService,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper,
            OneClappContext context
        )
        {
            _oneClappFormLayoutBackgroundService = oneClappFormLayoutBackgroundService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _context = context;
            Common = new Common();
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<LayoutBackgroundAddUpdateResponse>> Add([FromForm] LayoutBackgroundAddUpdateRequest model)
        {
            var requestmodel = _mapper.Map<OneClappFormLayoutBackgroundDto>(model);
            if (requestmodel.CustomBackgroundImageFile != null)
            {
                var layoutFile = requestmodel.CustomBackgroundImageFile;
                //var layoutDirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormLayout";
                var layoutDirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormLayoutDirPath;

                if (!Directory.Exists(layoutDirPath))
                {
                    Directory.CreateDirectory(layoutDirPath);
                }

                if (System.IO.File.Exists(layoutDirPath))
                {
                    System.IO.File.Delete(Path.Combine(layoutDirPath));
                }

                var layoutFileName = string.Concat(
                    Path.GetFileNameWithoutExtension(layoutFile.FileName),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(layoutFile.FileName)
                );
                var LayoutFilePath = layoutDirPath + "\\" + layoutFileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(layoutFile);
                    if (fileStatus)
                    {
                        return new OperationResult<LayoutBackgroundAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }
                using (var oStream = new FileStream(LayoutFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await layoutFile.CopyToAsync(oStream);
                }

                requestmodel.CustomBackgroundImage = layoutFileName;
            }
            var AddUpdate = await _oneClappFormLayoutBackgroundService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<OneClappFormLayoutBackgroundDto>(AddUpdate);
            var responsemodel = _mapper.Map<LayoutBackgroundAddUpdateResponse>(requestmodel);
            return new OperationResult<LayoutBackgroundAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        [Authorize]
        [HttpPut]
        public async Task<OperationResult<LayoutBackgroundAddUpdateResponse>> Update([FromForm] LayoutBackgroundAddUpdateRequest model)
        {
            var requestmodel = _mapper.Map<OneClappFormLayoutBackgroundDto>(model);
            if (requestmodel.CustomBackgroundImageFile != null)
            {
                var layoutFile = requestmodel.CustomBackgroundImageFile;
                //var layoutDirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormLayout";
                var layoutDirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormLayoutDirPath;

                if (!Directory.Exists(layoutDirPath))
                {
                    Directory.CreateDirectory(layoutDirPath);
                }

                if (System.IO.File.Exists(layoutDirPath))
                {
                    System.IO.File.Delete(Path.Combine(layoutDirPath));
                }

                var layoutFileName = string.Concat(
                    Path.GetFileNameWithoutExtension(layoutFile.FileName),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(layoutFile.FileName)
                );
                var LayoutFilePath = layoutDirPath + "\\" + layoutFileName;
                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(layoutFile);
                    if (fileStatus)
                    {
                        return new OperationResult<LayoutBackgroundAddUpdateResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(LayoutFilePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await layoutFile.CopyToAsync(oStream);
                }

                requestmodel.CustomBackgroundImage = layoutFileName;
            }
            var AddUpdate = await _oneClappFormLayoutBackgroundService.CheckInsertOrUpdate(requestmodel);
            requestmodel = _mapper.Map<OneClappFormLayoutBackgroundDto>(AddUpdate);
            var responsemodel = _mapper.Map<LayoutBackgroundAddUpdateResponse>(requestmodel);
            return new OperationResult<LayoutBackgroundAddUpdateResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> File(long Id)
        {
            var layoutBackgroundObj = _oneClappFormLayoutBackgroundService.GetById(Id);
            //var dirPath = _hostingEnvironment.WebRootPath + "\\DynamicFormImages\\FormLayout";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DynamicFormLayoutDirPath;
            var filePath = dirPath + "\\" + "default-profile.jpg";
            if (layoutBackgroundObj != null && !string.IsNullOrEmpty(layoutBackgroundObj.CustomBackgroundImage))
            {
                filePath = dirPath + "\\" + layoutBackgroundObj.CustomBackgroundImage;

                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    // return File (b,GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
                    return File(bytes, Common.GetMimeTypes(layoutBackgroundObj.CustomBackgroundImage), layoutBackgroundObj.CustomBackgroundImage);
                }
                else
                {
                    filePath = dirPath + "\\" + "default-profile.jpg";
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    // return File (b,GetMimeTypes(userDetailsObj.ProfileImage), userDetailsObj.ProfileImage);
                    return File(bytes, Common.GetMimeTypes(layoutBackgroundObj.CustomBackgroundImage), layoutBackgroundObj.CustomBackgroundImage);
                }
                //  Byte[] b = System.IO.File.ReadAllBytes (filePath);

            }
            return null;

        }

        [AllowAnonymous]
        [HttpGet("{Name}")]
        public async Task<FileResult> DefaultFile(string Name)
        {
            //var dirPath = _hostingEnvironment.WebRootPath + "\\DefaultLayout";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.DefaultLayoutDirPath;
            var filePath = dirPath + "\\" + Name;

            if (System.IO.File.Exists(filePath))
            {
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(bytes, Common.GetMimeTypes(Name), Name);
            }
            else
            {
                filePath = dirPath + "\\" + "default-profile.jpg";
                var bytes = await System.IO.File.ReadAllBytesAsync(filePath);
                return File(bytes, Common.GetMimeTypes(Name), Name);
            }
            //  Byte[] b = System.IO.File.ReadAllBytes (filePath);
        }        

        // [Authorize]
        // [HttpGet("GetAll")]
        // public async Task<OperationResult<List<BorderStyleDto>>> GetAll()
        // {
        //     var BorderStyleList = _BorderStyleService.GetAll();
        //     var BorderStyleListDto = _mapper.Map<List<BorderStyleDto>>(BorderStyleList);
        //     return new OperationResult<List<BorderStyleDto>>(true, "", BorderStyleListDto);
        // }

        //[Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost("Remove")]
        // public async Task<OperationResult<BorderStyleDto>> DeleteBorderStyle(BorderStyleDto model)
        // {
        //     if (model.Id != null)
        //     {
        //         var BorderStyleObj = _BorderStyleService.DeleteBorderStyle(model.Id.Value);
        //         if (BorderStyleObj != null)
        //         {
        //             model = _mapper.Map<BorderStyleDto>(BorderStyleObj);
        //         }
        //         return new OperationResult<BorderStyleDto>(true, "", model);
        //     }
        //     else
        //     {
        //         return new OperationResult<BorderStyleDto>(false, "Id can not pass null", model);
        //     }

        // }
    }
}