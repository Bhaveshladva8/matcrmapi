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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Context;
using matcrm.service.Utility;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class ImportContactAttachmentController : Controller
    {
        private readonly IImportContactAttachmentService _importContactAttachmentService;
        private readonly ICustomModuleService _customModuleService;
        private readonly IUserService _userService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICustomTableService _customTableService;
        private readonly OneClappContext _context;
        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;
        public ImportContactAttachmentController(
            IImportContactAttachmentService importContactAttachmentService,
            ICustomModuleService customModuleService,
            IUserService userService,
            IHostingEnvironment hostingEnvironment,
            IMapper mapper,
            ICustomTableService customTableService,
            OneClappContext context)
        {
            _importContactAttachmentService = importContactAttachmentService;
            _customModuleService = customModuleService;
            _userService = userService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _customTableService = customTableService;
            _context = context;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<ImportContactAttachmentResponse>> UploadFile([FromForm] ImportContactAttachmentRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;

            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var requestmodel = _mapper.Map<ImportContactAttachmentDto>(model);
            ImportContactAttachmentResponse responsemodel = new ImportContactAttachmentResponse();

            if (!string.IsNullOrEmpty(requestmodel.ModuleName))
            {
                //var customModuleObj = _customModuleService.GetByName(model.ModuleName);
                CustomModule? customModuleObj = null;
                var customTable = _customTableService.GetByName(requestmodel.ModuleName);
                if (customTable != null)
                {
                    customModuleObj = _customModuleService.GetByCustomTable(customTable.Id);
                }

                if (requestmodel.File == null) throw new Exception("File is null");
                if (requestmodel.File.Length == 0) throw new Exception("File is empty");

                var file = requestmodel.File;
                // full path to file in temp location
                //var dirPath = _hostingEnvironment.WebRootPath + "\\ImportContact";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ImportContactDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                    Path.GetFileNameWithoutExtension(file.FileName),
                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                    Path.GetExtension(file.FileName)
                );
                var filePath = dirPath + "\\" + fileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(file);
                    if (fileStatus)
                    {
                        return new OperationResult<ImportContactAttachmentResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await file.CopyToAsync(oStream);
                }

                ImportContactAttachmentDto importContactAttachmentDto = new ImportContactAttachmentDto();
                importContactAttachmentDto.FileName = fileName;



                importContactAttachmentDto.CreatedBy = UserId;
                importContactAttachmentDto.TenantId = TenantId;
                if (customModuleObj != null)
                {
                    importContactAttachmentDto.ModuleId = customModuleObj.Id;
                }
                var addedItem = await _importContactAttachmentService.CheckInsertOrUpdate(importContactAttachmentDto);

                requestmodel = _mapper.Map<ImportContactAttachmentDto>(addedItem);
                responsemodel = _mapper.Map<ImportContactAttachmentResponse>(requestmodel);
                return new OperationResult<ImportContactAttachmentResponse>(true, System.Net.HttpStatusCode.OK, "", responsemodel);
            }
            else
            {
                responsemodel = _mapper.Map<ImportContactAttachmentResponse>(requestmodel);
                return new OperationResult<ImportContactAttachmentResponse>(false, System.Net.HttpStatusCode.OK, "Please select entity", responsemodel);
            }
        }
    }
}