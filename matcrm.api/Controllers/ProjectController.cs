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
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.BusinessLogic;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using matcrm.data.Context;
using matcrm.service.Utility;
using Microsoft.AspNetCore.SignalR;
using matcrm.api.SignalR;
using Swashbuckle.AspNetCore.Annotations;

namespace matcrm.api.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class ProjectController : Controller
    {
        private readonly IEmployeeProjectService _employeeProjectService;
        private readonly IUserService _userService;
        private readonly IEmployeeProjectStatusService _employeeProjectStatusService;
        private readonly IEmployeeTaskService _employeeTaskService;
        private readonly ISectionService _sectionService;
        private readonly ISectionActivityService _sectionActivityService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserService;
        private readonly IEmployeeTaskTimeRecordService _employeeTaskTimeRecordService;
        private readonly IEmployeeTaskAttachmentService _employeeTaskAttachmentService;
        private readonly IEmployeeTaskCommentService _employeeTaskCommentService;
        private readonly IEmployeeTaskActivityService _employeeTaskActivityService;
        private readonly IEmployeeSubTaskService _employeeSubTaskService;
        private readonly IEmployeeChildTaskService _employeeChildTaskService;
        private readonly IEmployeeTaskUserSerivce _employeeTaskUserSerivce;
        private readonly IEmployeeSubTaskUserService _employeeSubTaskUserService;
        private readonly IEmployeeChildTaskUserService _employeeChildTaskUserService;
        private readonly IEmployeeSubTaskTimeRecordService _employeeSubTaskTimeRecordService;
        private readonly IEmployeeChildTaskTimeRecordService _employeeChildTaskTimeRecordService;
        private readonly IWeClappService _weClappService;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IEmployeeSubTaskAttachmentService _employeeSubTaskAttachmentService;
        private readonly IEmployeeChildTaskAttachmentService _employeeChildTaskAttachmentService;
        private readonly IEmployeeSubTaskActivityService _employeeSubTaskActivityService;
        private readonly IEmployeeChildTaskActivityService _employeeChildTaskActivityService;
        private readonly IEmployeeSubTaskCommentService _employeeSubTaskCommentService;
        private readonly IEmployeeChildTaskCommentService _employeeChildTaskCommentService;
        private readonly IEmployeeTaskStatusService _employeeTaskStatusService;
        private readonly ICustomModuleService _customModuleService;
        private CustomFieldLogic customFieldLogic;
        private readonly ICustomControlService _customControlService;
        private readonly ICustomControlOptionService _customControlOptionService;
        private readonly ICustomFieldService _customFieldService;
        private readonly IModuleFieldService _moduleFieldService;
        private readonly ITenantModuleService _tenantModuleService;
        private readonly ICustomTenantFieldService _customTenantFieldService;
        private readonly ICustomTableService _customTableService;
        private readonly ICustomFieldValueService _customFieldValueService;
        private readonly IModuleRecordCustomFieldService _moduleRecordCustomFieldService;
        private readonly ICustomTableColumnService _customTableColumnService;
        private readonly IEmployeeProjectUserService _employeeProjectUserService;
        private readonly IEmployeeProjectActivityService _employeeProjectActivityService;
        private readonly IStatusService _statusService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private readonly OneClappContext _context;
        private readonly IMateProjectTimeRecordService _mateProjectTimeRecordService;
        private readonly IMateTimeRecordService _mateTimeRecordService;
        private readonly IProjectContractService _projectContractService;
        private readonly IContractService _contractService;
        private readonly IContractActivityService _contractActivityService;
        private readonly IClientService _clientService;
        private readonly IMateCategoryService _mateCategoryService;
        private readonly IEmployeeProjectTaskService _employeeProjectTaskService;
        private readonly IEmployeeClientTaskService _employeeClientTaskService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private readonly IMateProjectTicketService _mateProjectTicketService;
        private readonly IMateTicketActivityService _mateTicketActivityService;
        private readonly IMateTicketTaskService _mateTicketTaskService;
        private SendEmail sendEmail;
        private Common Common;

        private IMapper _mapper;
        private int UserId = 0;
        private int TenantId = 0;

        public ProjectController(
            IEmployeeProjectService employeeProjectService,
            IEmployeeProjectStatusService employeeProjectStatusService,
            IUserService userService,
            IEmployeeTaskService employeeTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserService,
            IEmployeeTaskTimeRecordService employeeTaskTimeRecordService,
            IEmployeeTaskAttachmentService employeeTaskAttachmentService,
            IEmployeeTaskCommentService employeeTaskCommentService,
            IEmployeeTaskActivityService employeeTaskActivityService,
            IEmployeeSubTaskService employeeSubTaskService,
            IEmployeeChildTaskService employeeChildTaskService,
            IEmployeeTaskUserSerivce employeeTaskUserSerivce,
            IEmployeeSubTaskUserService employeeSubTaskUserService,
            IEmployeeChildTaskUserService employeeChildTaskUserService,
            IEmployeeSubTaskTimeRecordService employeeSubTaskTimeRecordService,
            IEmployeeChildTaskTimeRecordService employeeChildTaskTimeRecordService,
            IWeClappService weClappService,
            IHostingEnvironment hostingEnvironment,
            IEmployeeSubTaskAttachmentService employeeSubTaskAttachmentService,
            IEmployeeChildTaskAttachmentService employeeChildTaskAttachmentService,
            IEmployeeSubTaskActivityService employeeSubTaskActivityService,
            IEmployeeChildTaskActivityService employeeChildTaskActivityService,
            IEmployeeSubTaskCommentService employeeSubTaskCommentService,
            IEmployeeChildTaskCommentService employeeChildTaskCommentService,
            IEmployeeTaskStatusService employeeTaskStatusService,
            IMapper mapper,
            ICustomModuleService customModuleService,
            ICustomControlService customControlService,
            ICustomControlOptionService customControlOptionService,
            ICustomFieldService customFieldService,
            IModuleFieldService moduleFieldService,
            ITenantModuleService tenantModuleService,
            ICustomTenantFieldService customTenantFieldService,
            ICustomTableService customTableService,
            ICustomFieldValueService customFieldValueService,
            IModuleRecordCustomFieldService moduleRecordCustomFieldService,
            ICustomTableColumnService customTableColumnService,
            ISectionService sectionService,
            ISectionActivityService sectionActivityService,
            IEmployeeProjectUserService employeeProjectUserService,
            IEmployeeProjectActivityService employeeProjectActivityService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            IStatusService statusService,
            OneClappContext context,
            IMateProjectTimeRecordService mateProjectTimeRecordService,
            IMateTimeRecordService mateTimeRecordService,
            IProjectContractService projectContractService,
            IContractService contractService,
            IContractActivityService contractActivityService,
            IClientService clientService,
            IMateCategoryService mateCategoryService,
            IEmployeeProjectTaskService employeeProjectTaskService,
            IEmployeeClientTaskService employeeClientTaskService,
            IHubContext<BroadcastHub, IHubClient> hubContext,
            IMateProjectTicketService mateProjectTicketService,
            IMateTicketActivityService mateTicketActivityService,
            IMateTicketTaskService mateTicketTaskService
        )
        {
            _employeeProjectService = employeeProjectService;
            _employeeProjectStatusService = employeeProjectStatusService;
            _userService = userService;
            _employeeTaskService = employeeTaskService;
            _employeeTaskUserService = employeeTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeTaskAttachmentService = employeeTaskAttachmentService;
            _employeeTaskCommentService = employeeTaskCommentService;
            _employeeTaskActivityService = employeeTaskActivityService;
            _employeeSubTaskService = employeeSubTaskService;
            _employeeChildTaskService = employeeChildTaskService;
            _employeeTaskUserSerivce = employeeTaskUserSerivce;
            _employeeSubTaskUserService = employeeSubTaskUserService;
            _employeeChildTaskUserService = employeeChildTaskUserService;
            _employeeTaskTimeRecordService = employeeTaskTimeRecordService;
            _employeeSubTaskTimeRecordService = employeeSubTaskTimeRecordService;
            _employeeChildTaskTimeRecordService = employeeChildTaskTimeRecordService;
            _weClappService = weClappService;
            _hostingEnvironment = hostingEnvironment;
            _employeeSubTaskAttachmentService = employeeSubTaskAttachmentService;
            _employeeChildTaskAttachmentService = employeeChildTaskAttachmentService;
            _employeeSubTaskActivityService = employeeSubTaskActivityService;
            _employeeChildTaskActivityService = employeeChildTaskActivityService;
            _userService = userService;
            _employeeSubTaskCommentService = employeeSubTaskCommentService;
            _employeeChildTaskCommentService = employeeChildTaskCommentService;
            _employeeTaskStatusService = employeeTaskStatusService;
            _hostingEnvironment = hostingEnvironment;
            _mapper = mapper;
            _customModuleService = customModuleService;
            _customControlService = customControlService;
            _customControlOptionService = customControlOptionService;
            _customFieldService = customFieldService;
            _customModuleService = customModuleService;
            _moduleFieldService = moduleFieldService;
            _tenantModuleService = tenantModuleService;
            _customTenantFieldService = customTenantFieldService;
            _customTableService = customTableService;
            _customFieldValueService = customFieldValueService;
            _moduleRecordCustomFieldService = moduleRecordCustomFieldService;
            _customTableColumnService = customTableColumnService;
            _sectionActivityService = sectionActivityService;
            _sectionService = sectionService;
            _employeeProjectUserService = employeeProjectUserService;
            _employeeProjectActivityService = employeeProjectActivityService;
            _statusService = statusService;
            _context = context;
            _mateProjectTimeRecordService = mateProjectTimeRecordService;
            _mateTimeRecordService = mateTimeRecordService;
            _projectContractService = projectContractService;
            _contractService = contractService;
            _contractActivityService = contractActivityService;
            _clientService = clientService;
            _mateCategoryService = mateCategoryService;
            _employeeProjectTaskService = employeeProjectTaskService;
            _employeeClientTaskService = employeeClientTaskService;
            Common = new Common();
            _hubContext = hubContext;
            _mateProjectTicketService = mateProjectTicketService;
            _mateTicketActivityService = mateTicketActivityService;
            _mateTicketTaskService = mateTicketTaskService;
            customFieldLogic = new CustomFieldLogic(customControlService, customControlOptionService, customFieldService,
                customModuleService, moduleFieldService, tenantModuleService, customTenantFieldService, customTableService, customFieldValueService, mapper);
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<AddUpdateEmployeeProjectResponse>> Add([FromForm] AddUpdateEmployeeProjectRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            var employeeProjectDto = _mapper.Map<EmployeeProjectDto>(requestModel);

            if (!string.IsNullOrEmpty(employeeProjectDto.Duration))
            {
                var data = employeeProjectDto.Duration.Split(":");
                var count = data.Count();
                if (count == 3)
                {
                    var hours = Convert.ToInt16(data[0]);
                    var minutes = Convert.ToInt16(data[1]);
                    var seconds = Convert.ToInt16(data[2]);
                    employeeProjectDto.EstimatedTime = new TimeSpan(hours, minutes, seconds);
                }
            }

            employeeProjectDto.CreatedBy = UserId;
            employeeProjectDto.TenantId = TenantId;
            var filePath = "";

            if (employeeProjectDto.File != null)
            {
                //var dirPath = _hostingEnvironment.WebRootPath + "\\ProjectLogo";
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ProjectLogoDirPath;

                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }

                var fileName = string.Concat(
                                Path.GetFileNameWithoutExtension(employeeProjectDto.File.FileName),
                                DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                Path.GetExtension(employeeProjectDto.File.FileName)
                            );
                filePath = dirPath + "\\" + fileName;


                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(Path.Combine(filePath));
                }

                EmployeeProject employeeProjectObj = new EmployeeProject();
                var employeeProject = _mapper.Map<EmployeeProject>(employeeProjectDto);

                employeeProjectDto.Logo = fileName;

                if (OneClappContext.ClamAVServerIsLive)
                {
                    ScanDocument scanDocumentObj = new ScanDocument();
                    bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(employeeProjectDto.File);
                    if (fileStatus)
                    {
                        return new OperationResult<AddUpdateEmployeeProjectResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                    }
                }

                using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                {
                    await employeeProjectDto.File.CopyToAsync(oStream);
                }
            }

            var AddUpdate = await _employeeProjectService.CheckInsertOrUpdate(employeeProjectDto);

            EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
            employeeProjectActivityObj.ProjectId = AddUpdate.Id;
            employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_Created.ToString().Replace("_", " ");
            employeeProjectActivityObj.UserId = employeeProjectDto.CreatedBy;
            var projectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);

            //start assign user for project
            if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
            {
                foreach (var userObj in requestModel.AssignedUsers)
                {
                    EmployeeProjectUserDto employeeProjectUserDto = new EmployeeProjectUserDto();
                    employeeProjectUserDto.EmployeeProjectId = AddUpdate.Id;
                    employeeProjectUserDto.UserId = userObj.UserId;
                    employeeProjectUserDto.CreatedBy = UserId;
                    var isExist = _employeeProjectUserService.IsExistOrNot(employeeProjectUserDto);
                    if (!isExist)
                    {
                        var employeeProjectUserObj = await _employeeProjectUserService.CheckInsertOrUpdate(employeeProjectUserDto);
                        if (employeeProjectUserObj != null)
                        {
                            userObj.Id = employeeProjectUserObj.Id;
                        }

                        if (employeeProjectUserDto.UserId != null)
                        {
                            var userAssignDetails = _userService.GetUserById(employeeProjectUserDto.UserId.Value);
                            if (userAssignDetails != null)
                                await sendEmail.SendEmailEmployeeProjectUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, AddUpdate.Name, TenantId, AddUpdate.Id);
                            EmployeeProjectActivity employeeProjectUserActivityObj = new EmployeeProjectActivity();
                            employeeProjectUserActivityObj.ProjectId = AddUpdate.Id;
                            employeeProjectUserActivityObj.UserId = UserId;
                            employeeProjectUserActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_assigned_to_user.ToString().Replace("_", " ");
                            var AddUpdate1 = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectUserActivityObj);
                        }
                    }
                }
                employeeProjectDto.AssignedUsers = new List<EmployeeProjectUser>();
            }
            //end assign user for project

            //start project Contract
            if (requestModel.ContractId != null && AddUpdate.Id > 0)
            {
                ProjectContract projectContractObj = new ProjectContract();
                projectContractObj.ProjectId = AddUpdate.Id;
                projectContractObj.ContractId = requestModel.ContractId;
                var AddUpdateProjectContract = await _projectContractService.CheckInsertOrUpdate(projectContractObj);
                if (AddUpdateProjectContract != null)
                {
                    //project activity
                    EmployeeProjectActivity ProjectContractActivityObj = new EmployeeProjectActivity();
                    ProjectContractActivityObj.ProjectId = projectContractObj.ProjectId;
                    ProjectContractActivityObj.UserId = UserId;
                    ProjectContractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_added_into_contract.ToString().Replace("_", " ");
                    var AddUpdateProjectContractActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(ProjectContractActivityObj);

                    //contract activity
                    // var contractObj = _contractService.GetById(projectContractObj.ContractId.Value);
                    // ContractActivity contractActivityObj = new ContractActivity();
                    // contractActivityObj.ContractId = projectContractObj.ContractId;
                    // contractActivityObj.ClientId = contractObj.ClientId;
                    // contractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_contract_created.ToString().Replace("_", " ");
                    // var contractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);

                }
            }
            //end project Contract

            // //start for custom field
            // CustomModule? customModuleObj = null;
            // var employeeProjectTableObj = _customTableService.GetByName("Project");
            // if (employeeProjectTableObj != null)
            // {
            //     customModuleObj = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
            // }

            // if (employeeProjectDto.CustomFields != null && employeeProjectDto.CustomFields.Count() > 0)
            // {
            //     foreach (var item in employeeProjectDto.CustomFields)
            //     {
            //         if (item != null)
            //         {
            //             CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
            //             customFieldValueDto.FieldId = item.Id;
            //             if (customModuleObj != null)
            //             {
            //                 customFieldValueDto.ModuleId = customModuleObj.Id;
            //             }
            //             customFieldValueDto.RecordId = AddUpdate.Id;
            //             var controlType = "";
            //             if (item.CustomControl != null)
            //             {
            //                 controlType = item.CustomControl.Name;
            //                 customFieldValueDto.ControlType = controlType;
            //             }
            //             customFieldValueDto.Value = item.Value;
            //             customFieldValueDto.CreatedBy = UserId;
            //             customFieldValueDto.TenantId = TenantId;
            //             if (item.CustomControlOptions != null && item.CustomControlOptions.Count() > 0)
            //             {

            //                 var selectedOptionList = item.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
            //                 if (controlType == "Checkbox")
            //                 {
            //                     var deletedList = await _customFieldValueService.DeleteList(customFieldValueDto);
            //                 }
            //                 if (selectedOptionList != null && selectedOptionList.Count() > 0)
            //                 {
            //                     foreach (var option in selectedOptionList)
            //                     {
            //                         customFieldValueDto.OptionId = option.Id;
            //                         var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
            //                     }
            //                 }
            //             }
            //             else
            //             {
            //                 var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
            //             }
            //         }

            //     }
            // }
            // //end for custom field



            var employeeProjectAddUpdateResponse = _mapper.Map<AddUpdateEmployeeProjectResponse>(AddUpdate);
            var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
            if (employeeProjectAddUpdateResponse.Logo != null)
            {
                employeeProjectAddUpdateResponse.LogoURL = OneClappContext.CurrentURL + "Project/Logo/" + employeeProjectAddUpdateResponse.Id + "?" + Timestamp;
            }
            if (requestModel.StatusId != null)
            {
                var statusObj = _statusService.GetById(requestModel.StatusId.Value);
                if (statusObj != null)
                {
                    employeeProjectAddUpdateResponse.StatusId = statusObj.Id;
                    employeeProjectAddUpdateResponse.StatusName = statusObj.Name;
                }
            }
            if (requestModel.MateCategoryId != null)
            {
                var mateCategoryObj = _mateCategoryService.GetById(requestModel.MateCategoryId.Value);
                if (mateCategoryObj != null)
                {
                    employeeProjectAddUpdateResponse.MateCategoryId = mateCategoryObj.Id;
                    employeeProjectAddUpdateResponse.MateCategoryName = mateCategoryObj.Name;
                }
            }
            await _hubContext.Clients.All.OnProjectModuleEvent(employeeProjectAddUpdateResponse.Id, TenantId);
            return new OperationResult<AddUpdateEmployeeProjectResponse>(true, System.Net.HttpStatusCode.OK, "Project add successfully", employeeProjectAddUpdateResponse);
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<AddUpdateEmployeeProjectResponse>> Update([FromForm] AddUpdateEmployeeProjectRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            var employeeProjectDto = _mapper.Map<EmployeeProjectDto>(requestModel);

            employeeProjectDto.UpdatedBy = UserId;
            employeeProjectDto.TenantId = TenantId;

            if (employeeProjectDto.Duration != null)
            {
                var data = employeeProjectDto.Duration.Split(":");
                var count = data.Count();
                if (count == 3)
                {
                    var hours = Convert.ToInt16(data[0]);
                    var minutes = Convert.ToInt16(data[1]);
                    var seconds = Convert.ToInt16(data[2]);
                    employeeProjectDto.EstimatedTime = new TimeSpan(hours, minutes, seconds);
                }
            }
            EmployeeProject employeeProjectObj = new EmployeeProject();
            if (employeeProjectDto.Id != null)
            {
                var filePath = "";
                if (employeeProjectDto.File != null)
                {
                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ProjectLogo";
                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ProjectLogoDirPath;

                    if (!Directory.Exists(dirPath))
                    {
                        Directory.CreateDirectory(dirPath);
                    }

                    var fileName = string.Concat(
                                    Path.GetFileNameWithoutExtension(employeeProjectDto.File.FileName),
                                    DateTime.Now.ToString("yyyyMMdd_HHmmss"),
                                    Path.GetExtension(employeeProjectDto.File.FileName)
                                );
                    filePath = dirPath + "\\" + fileName;


                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(Path.Combine(filePath));
                    }

                    //EmployeeProject employeeProjectObj = new EmployeeProject();
                    //var employeeProject = _mapper.Map<EmployeeProject>(employeeProjectDto);

                    employeeProjectDto.Logo = fileName;
                    if (OneClappContext.ClamAVServerIsLive)
                    {
                        ScanDocument scanDocumentObj = new ScanDocument();
                        bool fileStatus = await scanDocumentObj.ScanDocumentWithClam(employeeProjectDto.File);
                        if (fileStatus)
                        {
                            return new OperationResult<AddUpdateEmployeeProjectResponse>(false, System.Net.HttpStatusCode.OK, "Virus Found!");
                        }
                    }
                    using (var oStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite))
                    {
                        await employeeProjectDto.File.CopyToAsync(oStream);
                    }
                }

                var existingItem = _employeeProjectService.GetEmployeeProjectById(employeeProjectDto.Id.Value);
                if (existingItem != null)
                {
                    existingItem.Name = employeeProjectDto.Name;
                    existingItem.UpdatedBy = employeeProjectDto.UpdatedBy;
                    var AddUpdate = await _employeeProjectService.CheckInsertOrUpdate(employeeProjectDto);

                    EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                    employeeProjectActivityObj.ProjectId = AddUpdate.Id;
                    employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_Updated.ToString().Replace("_", " ");
                    employeeProjectActivityObj.UserId = employeeProjectDto.UpdatedBy;
                    var ProjectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);

                    //start assign user for project
                    if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
                    {
                        foreach (var userObj in requestModel.AssignedUsers)
                        {
                            EmployeeProjectUserDto employeeProjectUserDto = new EmployeeProjectUserDto();
                            employeeProjectUserDto.EmployeeProjectId = AddUpdate.Id;
                            employeeProjectUserDto.UserId = userObj.UserId;
                            employeeProjectUserDto.CreatedBy = UserId;
                            var isExist = _employeeProjectUserService.IsExistOrNot(employeeProjectUserDto);
                            if (!isExist)
                            {
                                var employeeProjectUserObj = await _employeeProjectUserService.CheckInsertOrUpdate(employeeProjectUserDto);
                                if (employeeProjectUserObj != null)
                                {
                                    userObj.Id = employeeProjectUserObj.Id;
                                }

                                if (employeeProjectUserDto.UserId != null)
                                {
                                    var userAssignDetails = _userService.GetUserById(employeeProjectUserDto.UserId.Value);
                                    if (userAssignDetails != null)
                                        await sendEmail.SendEmailEmployeeProjectUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, AddUpdate.Name, TenantId, AddUpdate.Id);
                                    EmployeeProjectActivity employeeProjectUserActivityObj = new EmployeeProjectActivity();
                                    employeeProjectUserActivityObj.ProjectId = AddUpdate.Id;
                                    employeeProjectUserActivityObj.UserId = UserId;
                                    employeeProjectUserActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_assigned_to_user.ToString().Replace("_", " ");
                                    var AddUpdate1 = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectUserActivityObj);
                                }
                            }
                        }
                        employeeProjectDto.AssignedUsers = new List<EmployeeProjectUser>();
                    }
                    //end assign user for project

                    //start project Contract
                    if (requestModel.ContractId != null && AddUpdate.Id > 0)
                    {
                        ProjectContract projectContractObj = new ProjectContract();
                        projectContractObj.ProjectId = AddUpdate.Id;
                        projectContractObj.ContractId = requestModel.ContractId;
                        var AddUpdateProjectContract = await _projectContractService.CheckInsertOrUpdate(projectContractObj);
                        if (AddUpdateProjectContract != null)
                        {
                            //project activity
                            EmployeeProjectActivity ProjectContractActivityObj = new EmployeeProjectActivity();
                            ProjectContractActivityObj.ProjectId = projectContractObj.ProjectId;
                            ProjectContractActivityObj.UserId = UserId;
                            ProjectContractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_added_into_contract.ToString().Replace("_", " ");
                            var AddUpdateProjectContractActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(ProjectContractActivityObj);

                            //contract activity
                            // var contractObj = _contractService.GetById(projectContractObj.ContractId.Value);
                            // ContractActivity contractActivityObj = new ContractActivity();
                            // contractActivityObj.ContractId = projectContractObj.ContractId;
                            // contractActivityObj.ClientId = contractObj.ClientId;
                            // contractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_contract_created.ToString().Replace("_", " ");
                            // var contractActivity = await _contractActivityService.CheckInsertOrUpdate(contractActivityObj);

                        }
                    }
                    //end project Contract

                    // //start custom module
                    // CustomModule? customModuleObj = null;
                    // var employeeProjectTableObj = _customTableService.GetByName("Project");
                    // if (employeeProjectTableObj != null)
                    // {
                    //     customModuleObj = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
                    // }

                    // if (employeeProjectDto.CustomFields != null && employeeProjectDto.CustomFields.Count() > 0)
                    // {
                    //     foreach (var item in employeeProjectDto.CustomFields)
                    //     {
                    //         if (item != null)
                    //         {
                    //             CustomFieldValueDto customFieldValueDto = new CustomFieldValueDto();
                    //             customFieldValueDto.FieldId = item.Id;
                    //             if (customModuleObj != null)
                    //             {
                    //                 customFieldValueDto.ModuleId = customModuleObj.Id;
                    //             }
                    //             customFieldValueDto.RecordId = AddUpdate.Id;
                    //             var controlType = "";
                    //             if (item.CustomControl != null)
                    //             {
                    //                 controlType = item.CustomControl.Name;
                    //                 customFieldValueDto.ControlType = controlType;
                    //             }
                    //             customFieldValueDto.Value = item.Value;
                    //             customFieldValueDto.CreatedBy = UserId;
                    //             customFieldValueDto.TenantId = TenantId;
                    //             if (item.CustomControlOptions != null && item.CustomControlOptions.Count() > 0)
                    //             {
                    //                 var selectedOptionList = item.CustomControlOptions.Where(t => t.IsChecked == true).ToList();
                    //                 if (controlType == "Checkbox")
                    //                 {
                    //                     var deletedList = await _customFieldValueService.DeleteList(customFieldValueDto);
                    //                 }
                    //                 if (selectedOptionList != null && selectedOptionList.Count() > 0)
                    //                 {
                    //                     foreach (var option in selectedOptionList)
                    //                     {
                    //                         customFieldValueDto.OptionId = option.Id;
                    //                         var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                    //                     }
                    //                 }
                    //             }
                    //             else
                    //             {
                    //                 var customFieldAddUpdate = await _customFieldValueService.CheckInsertOrUpdate(customFieldValueDto);
                    //             }
                    //         }

                    //     }
                    // }
                    // //end custom module
                    var employeeProjectAddUpdateResponse = _mapper.Map<AddUpdateEmployeeProjectResponse>(AddUpdate);
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (employeeProjectAddUpdateResponse.Logo != null)
                    {
                        employeeProjectAddUpdateResponse.LogoURL = OneClappContext.CurrentURL + "Project/Logo/" + employeeProjectAddUpdateResponse.Id + "?" + Timestamp;
                    }
                    if (requestModel.StatusId != null)
                    {
                        var statusObj = _statusService.GetById(requestModel.StatusId.Value);
                        if (statusObj != null)
                        {
                            employeeProjectAddUpdateResponse.StatusId = statusObj.Id;
                            employeeProjectAddUpdateResponse.StatusName = statusObj.Name;
                        }
                    }
                    if (requestModel.MateCategoryId != null)
                    {
                        var mateCategoryObj = _mateCategoryService.GetById(requestModel.MateCategoryId.Value);
                        if (mateCategoryObj != null)
                        {
                            employeeProjectAddUpdateResponse.MateCategoryId = mateCategoryObj.Id;
                            employeeProjectAddUpdateResponse.MateCategoryName = mateCategoryObj.Name;
                        }
                    }
                    await _hubContext.Clients.All.OnProjectModuleEvent(employeeProjectAddUpdateResponse.Id, TenantId);
                    return new OperationResult<AddUpdateEmployeeProjectResponse>(true, System.Net.HttpStatusCode.OK, "Project Updated successfully", employeeProjectAddUpdateResponse);
                }
            }
            return new OperationResult<AddUpdateEmployeeProjectResponse>(false, System.Net.HttpStatusCode.OK, "Error");
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpDelete]
        public async Task<OperationResult> Remove([FromBody] EmployeeProjectDeleteRequest requestmodel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            //EmployeeProjectDto model = new EmployeeProjectDto();
            if (requestmodel.Id != null && requestmodel.Id > 0)
            {
                var employeeProjectId = requestmodel.Id;

                var tasks = _employeeProjectTaskService.GetAllTaskByProjectId(employeeProjectId);

                if (requestmodel.IsKeepTasks == true)
                {
                    if (tasks != null && tasks.Count() > 0)
                    {
                        foreach (var taskObj in tasks)
                        {
                            var employeeProjectTaskObj = await _employeeProjectTaskService.DeleteByTaskId(taskObj.Id);

                            EmployeeTaskActivity EmployeeTaskActivityObj = new EmployeeTaskActivity();
                            EmployeeTaskActivityObj.EmployeeTaskId = taskObj.EmployeeTaskId;
                            EmployeeTaskActivityObj.UserId = UserId;
                            // EmployeeTaskActivity.ProjectId = ProjectId;
                            EmployeeTaskActivityObj.Activity = "Removed this task from Project";
                            var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(EmployeeTaskActivityObj);
                            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(taskObj.EmployeeTaskId, TenantId);
                        }
                    }
                }
                else
                {
                    if (tasks != null && tasks.Count() > 0)
                    {
                        foreach (var taskObj in tasks)
                        {
                            var employeeTaskId = taskObj.Id;

                            var subTasks = _employeeSubTaskService.GetAllSubTaskByTask(employeeTaskId);

                            if (subTasks != null && subTasks.Count() > 0)
                            {
                                foreach (var subTask in subTasks)
                                {
                                    var subTaskId = subTask.Id;

                                    var childTasks = _employeeChildTaskService.GetAllChildTaskBySubTask(subTaskId);

                                    if (childTasks != null && childTasks.Count() > 0)
                                    {
                                        foreach (var item in childTasks)
                                        {
                                            var childTaskId = item.Id;

                                            var childDocuments = await _employeeChildTaskAttachmentService.DeleteAttachmentByChildTaskId(childTaskId);

                                            // Remove child task documents from folder
                                            if (childDocuments != null && childDocuments.Count() > 0)
                                            {
                                                foreach (var childTaskDoc in childDocuments)
                                                {

                                                    //var dirPath = _hostingEnvironment.WebRootPath + "\\ChildTaskUpload";
                                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ChildTaskUploadDirPath;
                                                    var filePath = dirPath + "\\" + childTaskDoc.Name;

                                                    if (System.IO.File.Exists(filePath))
                                                    {
                                                        System.IO.File.Delete(Path.Combine(filePath));
                                                    }
                                                }
                                            }

                                            var childComments = await _employeeChildTaskCommentService.DeleteCommentByChildTaskId(childTaskId);

                                            var childTimeRecords = await _employeeChildTaskTimeRecordService.DeleteTimeRecordByEmployeeChildTaskId(childTaskId);

                                            var childTaskUsers = await _employeeChildTaskUserService.DeleteByChildTaskId(childTaskId);

                                            EmployeeChildTaskActivity employeeChildTaskActivityObj = new EmployeeChildTaskActivity();
                                            employeeChildTaskActivityObj.EmployeeChildTaskId = childTaskId;
                                            employeeChildTaskActivityObj.UserId = UserId;
                                            employeeChildTaskActivityObj.Activity = "Removed the task";
                                            var AddUpdate1 = await _employeeChildTaskActivityService.CheckInsertOrUpdate(employeeChildTaskActivityObj);

                                            var childTaskActivities = await _employeeChildTaskActivityService.DeleteByEmployeeChildTaskId(childTaskId);

                                            var childTaskToDelete = await _employeeChildTaskService.Delete(childTaskId);
                                        }
                                    }

                                    var subDocuments = await _employeeSubTaskAttachmentService.DeleteAttachmentByEmployeeSubTaskId(subTaskId);

                                    // Remove sub task documents from folder
                                    if (subDocuments != null && subDocuments.Count() > 0)
                                    {
                                        foreach (var subTaskDoc in subDocuments)
                                        {

                                            //var dirPath = _hostingEnvironment.WebRootPath + "\\SubTaskUpload";
                                            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.SubTaskUploadDirPath;
                                            var filePath = dirPath + "\\" + subTaskDoc.Name;

                                            if (System.IO.File.Exists(filePath))
                                            {
                                                System.IO.File.Delete(Path.Combine(filePath));
                                            }
                                        }
                                    }

                                    var subComments = await _employeeSubTaskCommentService.DeleteCommentByEmployeeSubTaskId(subTaskId);

                                    var subTimeRecords = await _employeeSubTaskTimeRecordService.DeleteTimeRecordBySubTaskId(subTaskId);

                                    var subTaskUsers = await _employeeSubTaskUserService.DeleteBySubTaskId(subTaskId);

                                    EmployeeSubTaskActivity employeeSubTaskActivityObj = new EmployeeSubTaskActivity();
                                    employeeSubTaskActivityObj.EmployeeSubTaskId = subTaskId;
                                    employeeSubTaskActivityObj.UserId = UserId;
                                    employeeSubTaskActivityObj.Activity = "Removed the task";
                                    var AddUpdate2 = await _employeeSubTaskActivityService.CheckInsertOrUpdate(employeeSubTaskActivityObj);

                                    var subTaskActivities = await _employeeSubTaskActivityService.DeleteByEmployeeSubTaskId(subTaskId);

                                    var subTaskToDelete = await _employeeSubTaskService.Delete(subTaskId);
                                }
                            }

                            var documents = await _employeeTaskAttachmentService.DeleteAttachmentByTaskId(employeeTaskId);

                            // Remove task documents from folder
                            if (documents != null && documents.Count() > 0)
                            {
                                foreach (var taskDoc in documents)
                                {

                                    //var dirPath = _hostingEnvironment.WebRootPath + "\\EmployeeTaskUpload";
                                    var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.EmployeeTaskUploadDirPath;
                                    var filePath = dirPath + "\\" + taskDoc.Name;

                                    if (System.IO.File.Exists(filePath))
                                    {
                                        System.IO.File.Delete(Path.Combine(filePath));
                                    }
                                }
                            }

                            var comments = await _employeeTaskCommentService.DeleteCommentByEmployeeTaskId(employeeTaskId);

                            var timeRecords = await _employeeTaskTimeRecordService.DeleteTimeRecordByTaskId(employeeTaskId);

                            //for EmployeeProjectTask
                            var employeeProjectTaskObj = await _employeeProjectTaskService.DeleteByTaskId(taskObj.Id);

                            EmployeeTaskActivity ProjectTaskActivityObj = new EmployeeTaskActivity();
                            ProjectTaskActivityObj.EmployeeTaskId = taskObj.EmployeeTaskId;
                            ProjectTaskActivityObj.UserId = UserId;
                            // EmployeeTaskActivity.ProjectId = ProjectId;
                            ProjectTaskActivityObj.Activity = "Removed this task from Project";
                            var AddUpdateProjectTask = await _employeeTaskActivityService.CheckInsertOrUpdate(ProjectTaskActivityObj);

                            //for EmployeeClientTask
                            var employeeClientTaskObj = await _employeeClientTaskService.DeleteByTaskId(employeeTaskId);
                            if (employeeClientTaskObj != null)
                            {
                                EmployeeTaskActivity clientTaskActivityObj = new EmployeeTaskActivity();
                                clientTaskActivityObj.EmployeeTaskId = employeeTaskId;
                                clientTaskActivityObj.UserId = UserId;
                                clientTaskActivityObj.Activity = Enums.EmployeeTaskActivityEnum.Unassign_client_from_task.ToString().Replace("_", " ");
                                var AddUpdateClientTask = await _employeeTaskActivityService.CheckInsertOrUpdate(clientTaskActivityObj);
                            }

                            //Ticket task
                            var mateTicketTaskObj = await _mateTicketTaskService.DeleteByTaskId(employeeTaskId);
                            if (mateTicketTaskObj != null)
                            {
                                MateTicketActivity mateTicketTaskActivityObj = new MateTicketActivity();
                                mateTicketTaskActivityObj.EmployeeTaskId = employeeTaskId;
                                mateTicketTaskActivityObj.MateTicketId = mateTicketTaskObj.MateTicketId;
                                mateTicketTaskActivityObj.CreatedBy = UserId;
                                mateTicketTaskActivityObj.Activity = Enums.MateTicketActivityEnum.Task_removed_from_ticket.ToString().Replace("_", " ");
                                var AddUpdateTicketTaskActivity = await _mateTicketActivityService.CheckInsertOrUpdate(mateTicketTaskActivityObj);
                                await _hubContext.Clients.All.OnMateTicketModuleEvent(mateTicketTaskObj.MateTicketId, TenantId);
                            }

                            var taskUsers = await _employeeTaskUserService.DeleteByEmployeeTaskId(employeeTaskId);
                            EmployeeTaskActivity employeeTaskActivityObj = new EmployeeTaskActivity();
                            employeeTaskActivityObj.EmployeeTaskId = employeeTaskId;
                            employeeTaskActivityObj.UserId = UserId;
                            employeeTaskActivityObj.Activity = "Removed this task";
                            var AddUpdateActivity = await _employeeTaskActivityService.CheckInsertOrUpdate(employeeTaskActivityObj);
                            await _hubContext.Clients.All.OnEmployeeTaskModuleEvent(taskObj.EmployeeTaskId, TenantId);
                            //var taskActivities = await _employeeTaskActivityService.DeleteByEmployeeTaskId(employeeTaskId);

                            var taskToDelete = await _employeeTaskService.Delete(employeeTaskId);
                        }
                    }
                }

                //for delete project time record
                var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetByProjectId(employeeProjectId);
                if (mateProjectTimeRecordList != null && mateProjectTimeRecordList.Count > 0)
                {
                    foreach (var projectTimeRecord in mateProjectTimeRecordList)
                    {
                        if (projectTimeRecord != null && projectTimeRecord.MateTimeRecordId != null)
                        {
                            var mateTimeRecordObj = await _mateTimeRecordService.DeleteMateTimeRecord(projectTimeRecord.MateTimeRecordId.Value);
                            if (mateTimeRecordObj != null)
                            {
                                //project time record activity
                                EmployeeProjectActivity projectTimeRecordActivityObj = new EmployeeProjectActivity();
                                projectTimeRecordActivityObj.ProjectId = projectTimeRecord.ProjectId;
                                projectTimeRecordActivityObj.UserId = UserId;
                                projectTimeRecordActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_time_record_removed.ToString().Replace("_", " ");
                                var AddUpdateProjectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(projectTimeRecordActivityObj);
                                //project time record activity  
                            }
                        }
                    }
                }

                //for delete project time record

                //for project ticket
                var mateProjectTicketList = _mateProjectTicketService.GetAllByProjectId(employeeProjectId, TenantId);
                if (mateProjectTicketList != null && mateProjectTicketList.Count > 0)
                {
                    foreach (var projectTicketObj in mateProjectTicketList)
                    {
                        var mateProjectTicketObj = await _mateProjectTicketService.DeleteById(projectTicketObj.Id);
                        if (mateProjectTicketObj != null)
                        {
                            MateTicketActivity projectTicketActivityObj = new MateTicketActivity();
                            projectTicketActivityObj.MateTicketId = mateProjectTicketObj.MateTicketId;
                            projectTicketActivityObj.EmployeeProjectId = employeeProjectId;
                            projectTicketActivityObj.CreatedBy = UserId;
                            projectTicketActivityObj.Activity = Enums.MateTicketActivityEnum.Unassign_project_from_ticket.ToString().Replace("_", " ");
                            var AddUpdateProjectTicket = await _mateTicketActivityService.CheckInsertOrUpdate(projectTicketActivityObj);
                            await _hubContext.Clients.All.OnMateTicketModuleEvent(mateProjectTicketObj.MateTicketId, TenantId);
                        }
                    }
                }

                //start for delete project contract
                var checkIsExist = _projectContractService.GetByProjectId(employeeProjectId);
                if (checkIsExist != null)
                {
                    var projectContractObj = await _projectContractService.DeleteByProjectId(employeeProjectId);
                    if (projectContractObj != null)
                    {
                        //project activity
                        EmployeeProjectActivity projectContractActivityObj = new EmployeeProjectActivity();
                        projectContractActivityObj.ProjectId = projectContractObj.ProjectId;
                        projectContractActivityObj.UserId = UserId;
                        projectContractActivityObj.Activity = Enums.ProjectContractActivityEnum.Project_removed_from_contract.ToString().Replace("_", " ");
                        var projectContractActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(projectContractActivityObj);
                    }
                }
                //end for delete project contract

                var ProjectUsers = await _employeeProjectUserService.DeleteByEmployeeProjectId(employeeProjectId);

                EmployeeProjectActivity employeeProjectUserActivityObj = new EmployeeProjectActivity();
                employeeProjectUserActivityObj.ProjectId = employeeProjectId;
                employeeProjectUserActivityObj.UserId = UserId;
                employeeProjectUserActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Assign_user_removed.ToString().Replace("_", " ");
                var employeeProjectUserObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectUserActivityObj);

                //project
                var AddUpdate = await _employeeProjectService.DeleteEmployeeProject(employeeProjectId);

                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                employeeProjectActivityObj.ProjectId = employeeProjectId;
                employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_Removed.ToString().Replace("_", " ");
                employeeProjectActivityObj.UserId = UserId;
                var ProjectActivityObj = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);

                // //start for custom module    
                // CustomModule? customModuleObj = null;
                // var employeeProjectTableObj = _customTableService.GetByName("Project");
                // if (employeeProjectTableObj != null)
                // {
                //     customModuleObj = _customModuleService.GetByCustomTable(employeeProjectTableObj.Id);
                // }

                // if (customModuleObj != null)
                // {

                //     var moduleFields = _moduleFieldService.GetAllModuleField(customModuleObj.Id);

                //     var moduleFieldIdList = moduleFields.Select(t => t.Id).ToList();

                //     var moduleRecordFieldList = _moduleRecordCustomFieldService.GetByModuleFieldIdList(moduleFieldIdList);
                //     if (moduleRecordFieldList != null && moduleRecordFieldList.Count() > 0)
                //     {
                //         foreach (var moduleRecordField in moduleRecordFieldList)
                //         {
                //             if (moduleRecordField.RecordId == employeeProjectId)
                //             {
                //                 var DeletedModuleRecordField = await _moduleRecordCustomFieldService.DeleteById(moduleRecordField.Id);

                //                 var moduleFieldId = moduleRecordField.ModuleFieldId;
                //                 long? CustomFieldId1 = null;
                //                 if (moduleRecordField.ModuleField.CustomField != null)
                //                 {
                //                     CustomFieldId1 = moduleRecordField.ModuleField.CustomField.Id;
                //                 }

                //                 if (moduleFieldId != null)
                //                 {
                //                     var DeleteModuleField = await _moduleFieldService.Delete(moduleFieldId.Value);
                //                 }

                //                 if (CustomFieldId1 != null && AddUpdate.TenantId != null)
                //                 {
                //                     var DeleteTenantField = await _customTenantFieldService.DeleteTenantField(CustomFieldId1.Value, AddUpdate.TenantId.Value);
                //                 }

                //                 CustomTableColumnDto customTableColumnDto = new CustomTableColumnDto();
                //                 customTableColumnDto.Name = moduleRecordField.ModuleField.CustomField.Name;
                //                 customTableColumnDto.ControlId = moduleRecordField.ModuleField.CustomField.ControlId;
                //                 customTableColumnDto.IsDefault = false;
                //                 customTableColumnDto.TenantId = AddUpdate.TenantId;
                //                 if (CustomFieldId1 != null)
                //                 {
                //                     customTableColumnDto.CustomFieldId = CustomFieldId1;
                //                 }

                //                 customTableColumnDto.MasterTableId = customModuleObj.Id;

                //                 var deleteTableColumns = await _customTableColumnService.DeleteCustomFields(customTableColumnDto);

                //                 if (CustomFieldId1 != null)
                //                 {
                //                     var deleteTableColumns1 = _customFieldService.DeleteById(CustomFieldId1.Value);
                //                 }

                //             }
                //         }
                //     }
                // }
                // //end for custom module

                //var responseDelete = _mapper.Map<EmployeeProjectDeleteResponse>(model);
                await _hubContext.Clients.All.OnProjectModuleEvent(employeeProjectId, TenantId);
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "", requestmodel.Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", requestmodel.Id);
            }
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeProjectId}")]
        public async Task<OperationResult<ProjectDetailResponse>> Detail(long EmployeeProjectId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            //ProjectVM projectVMObj = new ProjectVM();
            ProjectDetailResponse projectDetailResponseObj = new ProjectDetailResponse();
            var projectObj = _employeeProjectService.GetEmployeeProjectById(EmployeeProjectId);
            var AllUsers = _userService.GetAll();

            if (projectObj != null)
            {
                //projectVMObj = _mapper.Map<ProjectVM>(projectObj);

                // var y = 60 * 60 * 1000;
                // if (projectVMObj.Id != null)
                // {
                //     var projectId = projectVMObj.Id.Value;

                var taskList = _employeeProjectTaskService.GetAllTaskByProjectId(EmployeeProjectId);
                var ticketList = _mateProjectTicketService.GetAllByProjectId(EmployeeProjectId, TenantId);

                //     projectVMObj.TaskCount = taskList.Count();
                //     if (taskList != null && taskList.Count() > 0)
                //     {
                //         projectVMObj.Tasks = new List<EmployeeTaskVM>();
                //         var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(taskList);
                //         foreach (var taskObj in taskVMList)
                //         {
                //             if (taskObj.Id != null)
                //             {
                //                 var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
                //                 if (assignTaskUsers.Count > 0)
                //                 {
                //                     taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
                //                     var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
                //                     taskObj.AssignedUsers = assignUsersVMList;
                //                 }
                //             }
                //         }
                //         projectVMObj.Tasks = taskVMList;
                //     }

                //     // var sectionList = _sectionService.GetByProject(EmployeeProjectId);

                //     // if (sectionList != null && sectionList.Count() > 0)
                //     // {
                //     //     projectVMObj.Sections = new List<EmployeeSectionVM>();
                //     //     var sectionVMList = _mapper.Map<List<EmployeeSectionVM>>(sectionList);
                //     //     //SectionVM sectionVMObj = new SectionVM();
                //     //     foreach (var sectionObj in sectionVMList)
                //     //     {
                //     //         if (sectionObj.Id != null)
                //     //         {
                //     //             var sectionId = sectionObj.Id.Value;

                //     //             var sectiontaskList = _employeeTaskService.GetAllTaskBySection(sectionId);

                //     //             if (sectiontaskList != null && sectiontaskList.Count() > 0)
                //     //             {
                //     //                 sectionObj.Tasks = new List<EmployeeTaskVM>();
                //     //                 var taskVMList = _mapper.Map<List<EmployeeTaskVM>>(taskList);

                //     //                 foreach (var taskObj in taskVMList)
                //     //                 {
                //     //                     if (taskObj.Id != null)
                //     //                     {
                //     //                         var taskTotalDuration = _employeeTaskTimeRecordService.GetTotalEmployeeTaskTimeRecord(taskObj.Id.Value);
                //     //                         taskObj.Duration = taskTotalDuration;

                //     //                         var h = taskTotalDuration / y;
                //     //                         var m = (taskTotalDuration - (h * y)) / (y / 60);
                //     //                         var s = (taskTotalDuration - (h * y) - (m * (y / 60))) / 1000;

                //     //                         taskObj.Seconds = s;
                //     //                         taskObj.Minutes = m;
                //     //                         taskObj.Hours = h;
                //     //                         var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskObj.Id.Value);
                //     //                         if (assignTaskUsers.Count > 0)
                //     //                         {
                //     //                             taskObj.AssignedUsers = new List<EmployeeTaskUserDto>();
                //     //                             var assignUsersVMList = _mapper.Map<List<EmployeeTaskUserDto>>(assignTaskUsers);
                //     //                             taskObj.AssignedUsers = assignUsersVMList;
                //     //                         }
                //     //                     }
                //     //                 }
                //     //                 sectionObj.Tasks = taskVMList;
                //     //             }
                //     //         }
                //     //     }
                //     //     projectVMObj.Sections = sectionVMList;
                //     // }

                // }
                projectDetailResponseObj = _mapper.Map<ProjectDetailResponse>(projectObj);
                if (projectDetailResponseObj.EndDate != null && projectDetailResponseObj.EndDate < DateTime.UtcNow)
                {
                    projectDetailResponseObj.DueDate = projectDetailResponseObj.EndDate;
                }

                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                var employeeProjectUsersObj = _employeeProjectUserService.GetAssignUsersByEmployeeProject(EmployeeProjectId);

                if (employeeProjectUsersObj != null && employeeProjectUsersObj.Count > 0)
                {
                    //var assignUserVMList = _mapper.Map<List<EmployeeProjectUserRequestResponse>>(employeeProjectUsersObj);

                    if (projectDetailResponseObj.AssignedUsers == null)
                    {
                        projectDetailResponseObj.AssignedUsers = new List<EmployeeProjectUserRequestResponse>();
                    }

                    foreach (var assignUser in employeeProjectUsersObj)
                    {
                        EmployeeProjectUserRequestResponse employeeProjectUserRequestResponseObj = new EmployeeProjectUserRequestResponse();
                        employeeProjectUserRequestResponseObj.Id = assignUser.Id;
                        employeeProjectUserRequestResponseObj.EmployeeProjectId = EmployeeProjectId;
                        employeeProjectUserRequestResponseObj.UserId = assignUser.User.Id;
                        employeeProjectUserRequestResponseObj.FirstName = assignUser.User.FirstName;
                        employeeProjectUserRequestResponseObj.LastName = assignUser.User.LastName;
                        employeeProjectUserRequestResponseObj.Email = assignUser.User.Email;

                        //var userdirPath = _hostingEnvironment.WebRootPath + "\\ProfileImageUpload\\Original";
                        var userdirPath = _hostingEnvironment.WebRootPath + OneClappContext.OriginalUserProfileDirPath;
                        var userfilePath = "";

                        if (assignUser.User.ProfileImage == null)
                        {
                            userfilePath = "assets/images/default-profile.jpg";
                        }
                        else
                        {
                            userfilePath = OneClappContext.CurrentURL + "User/ProfileImageView/" + assignUser.User.Id + "?" + Timestamp;
                        }

                        employeeProjectUserRequestResponseObj.ProfileImageURL = userfilePath;
                        projectDetailResponseObj.AssignedUsers.Add(employeeProjectUserRequestResponseObj);
                    }

                }
                //for project time record
                var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetTimeRecordByProjectId(EmployeeProjectId);
                if (mateProjectTimeRecordList != null && mateProjectTimeRecordList.Count > 0)
                {
                    foreach (var projectTimeRecord in mateProjectTimeRecordList)
                    {
                        MateProjectTimeRecordProjectDetailResponse mateProjectTimeRecordObj = new MateProjectTimeRecordProjectDetailResponse();
                        if (projectTimeRecord != null && projectTimeRecord.MateTimeRecord != null)
                        {
                            mateProjectTimeRecordObj.MateTimeRecordId = projectTimeRecord.MateTimeRecord.Id;
                            mateProjectTimeRecordObj.Comment = projectTimeRecord.MateTimeRecord.Comment;
                            mateProjectTimeRecordObj.CreatedOn = projectTimeRecord.MateTimeRecord.CreatedOn;
                            mateProjectTimeRecordObj.Duration = projectTimeRecord.MateTimeRecord.Duration;
                            mateProjectTimeRecordObj.IsBillable = projectTimeRecord.MateTimeRecord.IsBillable;
                        }
                        projectDetailResponseObj.ProjectTimeRecords.Add(mateProjectTimeRecordObj);
                    }
                }
                //for project time record

                if (projectObj.Logo == null)
                {
                    projectDetailResponseObj.LogoURL = "assets/images/default-project.png";
                }
                else
                {
                    projectDetailResponseObj.LogoURL = OneClappContext.CurrentURL + "Project/Logo/" + projectDetailResponseObj.Id + "?" + Timestamp;
                }
                if (projectObj.StatusId != null)
                {
                    projectDetailResponseObj.StatusName = projectObj.Status.Name;
                }
                if (projectObj.MateCategoryId != null)
                {
                    projectDetailResponseObj.ProjectCategoryName = projectObj.MateCategory?.Name;
                }
                if (projectObj.CreatedBy != null)
                {
                    var userObj = AllUsers.Where(t => t.Id == projectObj.CreatedBy).FirstOrDefault();
                    var UserName = userObj.FirstName + " " + userObj.LastName;
                    if (!String.IsNullOrEmpty(UserName))
                    {
                        projectDetailResponseObj.UserName = userObj.FirstName + " " + userObj.LastName;
                    }
                    else
                    {
                        projectDetailResponseObj.UserName = userObj.Email;
                    }

                    var userfilePath = "";

                    if (userObj.ProfileImage == null)
                    {
                        userfilePath = "assets/images/default-profile.jpg";
                    }
                    else
                    {
                        userfilePath = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj.Id + "?" + Timestamp;
                    }
                    projectDetailResponseObj.UserProfileURL = userfilePath;
                }
                if (projectObj.ClientId != null)
                {
                    projectDetailResponseObj.ClientName = projectObj.Client.Name;
                }
                projectDetailResponseObj.TaskCount = taskList.Count();
                projectDetailResponseObj.TicketCount = ticketList.Count();
                //for chart
                if (taskList != null && taskList.Count > 0)
                {
                    List<ProjectTaskDetailResponse> statusTaskList = new List<ProjectTaskDetailResponse>();
                    var AllStatus = _statusService.GetByTenant(TenantId);

                    //statusTicketList.Add(new MateTicketStatusListResponse { StatusId = 0, StatusName = "All Tickets", StatusColor = "Yellow", TotalCount = ticketList.Count() });

                    var TicketKeyValueData = taskList.GroupBy(t => t?.EmployeeTask.StatusId);
                    foreach (var TicketData in TicketKeyValueData)
                    {
                        ProjectTaskDetailResponse statusTaskObj = new ProjectTaskDetailResponse();

                        statusTaskObj.TotalCount = TicketData.ToList().Count();
                        if (TicketData.Key != null)
                        {
                            var StatusObj = AllStatus.Where(t => t.Id == TicketData.Key).FirstOrDefault();
                            if (StatusObj != null)
                            {
                                statusTaskObj.StatusId = StatusObj.Id;
                                statusTaskObj.StatusName = StatusObj.Name;
                            }
                        }
                        statusTaskList.Add(statusTaskObj);
                    }
                    projectDetailResponseObj.Tasks = statusTaskList;
                }
                return new OperationResult<ProjectDetailResponse>(true, System.Net.HttpStatusCode.OK, "", projectDetailResponseObj);
            }
            else
            {
                return new OperationResult<ProjectDetailResponse>(false, System.Net.HttpStatusCode.OK, "Project not found", projectDetailResponseObj);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeProjectListResponse>>> List([FromBody] EmployeeProjectListRequest model)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
            List<EmployeeProject> employeeProjectList = new List<EmployeeProject>();

            employeeProjectList = _employeeProjectService.GetAllByTenant(TenantId);

            var AllStatus = _statusService.GetAll();
            var AllCategory = _mateCategoryService.GetAll();
            int totalCount = 0;
            if (model.PageNumber > 1)
            {
                //if (!string.IsNullOrEmpty(model.CategoryId))
                if (model.CategoryId != null && model.CategoryId > 0)
                {
                    employeeProjectList = employeeProjectList.Where(t => t?.MateCategoryId == model.CategoryId).ToList();
                }
            }
            var employeeProjectListResponse = _mapper.Map<List<EmployeeProjectListResponse>>(employeeProjectList);
            if (employeeProjectListResponse != null && employeeProjectListResponse.Count() > 0)
            {
                var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ProjectLogoDirPath;
                foreach (var item in employeeProjectListResponse)
                {
                    if (item.StatusId != null)
                    {
                        var statusObj = AllStatus.Where(t => t.Id == item.StatusId).FirstOrDefault();
                        if (statusObj != null)
                        {
                            item.StatusId = statusObj.Id;
                            item.StatusName = statusObj.Name;
                        }
                    }
                    var projectObj = employeeProjectList.Where(t => t.Id == item.Id).FirstOrDefault();
                    if (projectObj != null)
                    {
                        var overDueStatusObj = AllStatus.Where(t => t.Name.ToLower() == "overdue").FirstOrDefault();
                        if (projectObj.EndDate != null && projectObj.EndDate < DateTime.UtcNow)
                        {
                            item.StatusId = overDueStatusObj?.Id;
                            item.StatusName = overDueStatusObj?.Name;
                        }
                    }
                    if (item.MateCategoryId != null)
                    {
                        var mateCategoryObj = AllCategory.Where(t => t.Id == item.MateCategoryId).FirstOrDefault();
                        if (mateCategoryObj != null)
                        {
                            item.MateCategoryId = mateCategoryObj?.Id;
                            item.MateCategoryName = mateCategoryObj?.Name;
                        }
                    }
                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                    if (item.Logo == null)
                    {
                        item.LogoURL = "assets/images/default-project.png";
                    }
                    else
                    {
                        item.LogoURL = OneClappContext.CurrentURL + "Project/Logo/" + item.Id + "?" + Timestamp;
                    }
                    //project mate time record
                    //MateProjectTimeRecordListResponse mateProjectTimeRecordListResponse = new MateProjectTimeRecordListResponse();

                    if (item.Id != null)
                    {
                        var mateProjectTimeRecordList = _mateProjectTimeRecordService.GetByProjectIdAndUserId(item.Id.Value, UserId);
                        var mateProjectTimeRecordAscList = mateProjectTimeRecordList.OrderBy(t => t.MateTimeRecord.CreatedOn).ToList();
                        var mateProjectTimeRecordLast = mateProjectTimeRecordAscList.LastOrDefault();
                        long ProjectTotalDuration = 0;
                        if (mateProjectTimeRecordList != null && mateProjectTimeRecordList.Count > 0)
                        {
                            foreach (var projectTimeRecord in mateProjectTimeRecordList)
                            {
                                if (projectTimeRecord.MateTimeRecord != null)
                                {
                                    if (projectTimeRecord.MateTimeRecord.Duration != null)
                                    {
                                        ProjectTotalDuration = ProjectTotalDuration + projectTimeRecord.MateTimeRecord.Duration.Value;

                                        TimeSpan timeSpan = TimeSpan.FromMinutes(ProjectTotalDuration);

                                        item.TotalDuration = timeSpan.ToString(@"hh\:mm");
                                    }
                                    if (mateProjectTimeRecordLast != null)
                                    {
                                        item.Enddate = mateProjectTimeRecordLast.MateTimeRecord.CreatedOn;
                                    }

                                }
                            }
                            item.TimeRecordCount = mateProjectTimeRecordList.Count;
                        }
                    }
                    //project mate time record
                }                
                totalCount = employeeProjectListResponse.Count();
                var SkipValue = model.PageSize * (model.PageNumber - 1);
                if (!string.IsNullOrEmpty(model.SearchString))
                {
                    employeeProjectListResponse = employeeProjectListResponse.Where(t => (!string.IsNullOrEmpty(t.Name) && t.Name.ToLower().Contains(model.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.MateCategoryName) && t.MateCategoryName.ToLower().Contains(model.SearchString.ToLower())) || (!string.IsNullOrEmpty(t.StatusName) && t.StatusName.ToLower().Contains(model.SearchString.ToLower()))).ToList();
                    employeeProjectListResponse = employeeProjectListResponse.Skip(SkipValue).Take(model.PageSize).ToList();
                }
                else
                {
                    employeeProjectListResponse = employeeProjectListResponse.Skip(SkipValue).Take(model.PageSize).ToList();
                }
            }
            return new OperationResult<List<EmployeeProjectListResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeProjectListResponse, totalCount);
        }

        [AllowAnonymous]
        [HttpGet("{Id}")]
        public async Task<FileResult> Logo(int Id)
        {
            var projectDetailsObj = _employeeProjectService.GetEmployeeProjectById(Id);
            //var dirPath = _hostingEnvironment.WebRootPath + "\\ProjectLogo";
            var dirPath = _hostingEnvironment.WebRootPath + OneClappContext.ProjectLogoDirPath;
            var filePath = dirPath + "\\" + "default.png";
            if (projectDetailsObj != null && !string.IsNullOrEmpty(projectDetailsObj.Logo))
            {
                filePath = dirPath + "\\" + projectDetailsObj.Logo;
                if (System.IO.File.Exists(filePath))
                {
                    var bytes = await System.IO.File.ReadAllBytesAsync(filePath);

                    return File(bytes, Common.GetMimeTypes(projectDetailsObj.Logo), projectDetailsObj.Logo);
                }
            }
            return null;
        }

        [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        [HttpGet("{EmployeeProjectId}")]
        public async Task<OperationResult<List<EmployeeProjectActivityDto>>> History(long EmployeeProjectId)
        {
            List<EmployeeProjectActivityDto> employeeProjectActivityDtoList = new List<EmployeeProjectActivityDto>();
            var AllUsers = _userService.GetAll();
            var activities = _employeeProjectActivityService.GetAllByProjectId(EmployeeProjectId);
            employeeProjectActivityDtoList = _mapper.Map<List<EmployeeProjectActivityDto>>(activities);
            if (employeeProjectActivityDtoList != null && employeeProjectActivityDtoList.Count() > 0)
            {
                foreach (var item in employeeProjectActivityDtoList)
                {
                    var userObj = AllUsers.Where(t => t.Id == item.UserId).FirstOrDefault();
                    if (userObj != null)
                    {
                        item.FirstName = userObj.FirstName;
                        item.LastName = userObj.LastName;
                        item.Email = userObj.Email;
                        if (item.FirstName != null)
                        {
                            item.ShortName = item.FirstName.Substring(0, 1);
                        }
                        if (item.LastName != null)
                        {
                            item.ShortName = item.ShortName + item.LastName.Substring(0, 1);
                        }
                        var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                        if (userObj.ProfileImage == null)
                        {
                            item.ProfileUrl = "assets/images/default-profile.jpg";
                        }
                        else
                        {
                            item.ProfileUrl = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj.Id + "?" + Timestamp;
                        }
                    }
                }
            }
            return new OperationResult<List<EmployeeProjectActivityDto>>(true, System.Net.HttpStatusCode.OK, "", employeeProjectActivityDtoList);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeProjectUserRequestResponse>>> Assign([FromBody] AssignUserEmployeeProjectRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            List<EmployeeProjectUserRequestResponse> assignUserEmployeeProjectResponseObj = new List<EmployeeProjectUserRequestResponse>();

            //start assign user for project
            if (requestModel.AssignedUsers != null && requestModel.AssignedUsers.Count() > 0)
            {
                var existingItem = _employeeProjectService.GetEmployeeProjectById(requestModel.EmployeeProjectId);

                foreach (var userObj in requestModel.AssignedUsers)
                {
                    EmployeeProjectUserDto employeeProjectUserDto = new EmployeeProjectUserDto();
                    employeeProjectUserDto.EmployeeProjectId = requestModel.EmployeeProjectId;
                    employeeProjectUserDto.UserId = userObj;
                    employeeProjectUserDto.CreatedBy = UserId;
                    var isExist = _employeeProjectUserService.IsExistOrNot(employeeProjectUserDto);
                    var employeeProjectUserObj = await _employeeProjectUserService.CheckInsertOrUpdate(employeeProjectUserDto);

                    if (!isExist)
                    {
                        if (employeeProjectUserDto.UserId != null)
                        {
                            var userAssignDetails = _userService.GetUserById(employeeProjectUserDto.UserId.Value);
                            if (userAssignDetails != null)
                                await sendEmail.SendEmailEmployeeProjectUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, existingItem.Name, TenantId, requestModel.EmployeeProjectId);
                            EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                            employeeProjectActivityObj.ProjectId = requestModel.EmployeeProjectId;
                            employeeProjectActivityObj.UserId = UserId;
                            employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Project_assigned_to_user.ToString().Replace("_", " ");
                            var AddUpdateEmployeeProjectActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                        }
                    }
                }

                var assignUsers = _employeeProjectUserService.GetAssignUsersByEmployeeProject(requestModel.EmployeeProjectId);
                var AllUsers = _userService.GetAll();

                if (assignUsers != null && assignUsers.Count > 0)
                {
                    var assignProjectUserVMList = _mapper.Map<List<EmployeeProjectUserRequestResponse>>(assignUsers);

                    if (assignProjectUserVMList != null && assignProjectUserVMList.Count() > 0)
                    {
                        foreach (var assignUser in assignProjectUserVMList)
                        {
                            if (AllUsers != null)
                            {
                                var userObj = AllUsers.Where(t => t.Id == assignUser.UserId).FirstOrDefault();
                                if (userObj != null)
                                {
                                    assignUser.EmployeeProjectId = requestModel.EmployeeProjectId;
                                    assignUser.UserId = userObj.Id;
                                    assignUser.FirstName = userObj.FirstName;
                                    assignUser.LastName = userObj.LastName;
                                    assignUser.Email = userObj.Email;

                                    var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();

                                    if (userObj.ProfileImage == null)
                                    {
                                        assignUser.ProfileImageURL = "assets/images/default-profile.jpg";
                                    }
                                    else
                                    {
                                        assignUser.ProfileImageURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj.Id + "?" + Timestamp;
                                    }
                                }
                            }
                        }
                    }
                    assignUserEmployeeProjectResponseObj = assignProjectUserVMList;
                }
                await _hubContext.Clients.All.OnProjectModuleEvent(requestModel.EmployeeProjectId, TenantId);
            }
            //end assign user for project
            return new OperationResult<List<EmployeeProjectUserRequestResponse>>(true, System.Net.HttpStatusCode.OK, "User assigned successfully", assignUserEmployeeProjectResponseObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpDelete("{Id}")]
        public async Task<OperationResult> UnAssign(long Id)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            if (Id != null)
            {
                var employeeProjectUserObj = await _employeeProjectUserService.UnAssign(Id);
                if (employeeProjectUserObj != null)
                {
                    if (employeeProjectUserObj.EmployeeProjectId != null && employeeProjectUserObj.UserId != null)
                    {
                        var userAssignDetails = _userService.GetUserById(employeeProjectUserObj.UserId.Value);
                        var existingItem = _employeeProjectService.GetEmployeeProjectById(employeeProjectUserObj.EmployeeProjectId.Value);
                        if (userAssignDetails != null)
                            await sendEmail.SendEmailRemoveEmployeeProjectUserAssignNotification(userAssignDetails.Email, userAssignDetails.FirstName + ' ' + userAssignDetails.LastName, existingItem.Name, TenantId);

                        EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                        employeeProjectActivityObj.ProjectId = employeeProjectUserObj.EmployeeProjectId.Value;
                        employeeProjectActivityObj.UserId = UserId;
                        employeeProjectActivityObj.Activity = Enums.EmployeeProjectActivityEnum.Assign_user_removed.ToString().Replace("_", " ");
                        var AddUpdateEmployeeProjectActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
                        await _hubContext.Clients.All.OnProjectModuleEvent(employeeProjectUserObj.EmployeeProjectId, TenantId);
                    }
                }
                return new OperationResult(true, System.Net.HttpStatusCode.OK, "User unassigned", Id);
            }
            else
            {
                return new OperationResult(false, System.Net.HttpStatusCode.OK, "Please provide id", Id);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<List<EmployeeProjectTaskListResponse>>> TaskList([FromBody] EmployeeProjectTaskListRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            int totalCount = 0;

            var taskList = _employeeProjectTaskService.GetAllTaskListByProjectId(requestModel);
            totalCount = taskList.Count();

            var pagedTaskList = taskList.Skip((requestModel.PageNumber - 1) * requestModel.PageSize).Take(requestModel.PageSize).ToList();

            var orderTaskList = ShortTaskByColumn(requestModel.ShortColumnName, requestModel.SortType, pagedTaskList);

            var AllUsers = _userService.GetAll();
            var AllStatus = _statusService.GetAll();
            //var employeeProjectTaskListResponse = _mapper.Map<List<EmployeeProjectTaskListResponse>>(orderTaskList);
            List<EmployeeProjectTaskListResponse> employeeProjectTaskListResponseList = new List<EmployeeProjectTaskListResponse>();
            if (orderTaskList != null && orderTaskList.Count() > 0)
            {
                foreach (var item in orderTaskList)
                {
                    EmployeeProjectTaskListResponse taskListObj = new EmployeeProjectTaskListResponse();
                    if (item.EmployeeTask != null)
                    {
                        if (item.EmployeeTaskId != null)
                        {
                            taskListObj.Id = item.EmployeeTaskId.Value;
                        }
                        taskListObj.TaskNo = item.EmployeeTask.TaskNo;
                        taskListObj.Name = item.EmployeeTask?.Name;
                        if (item.EmployeeTask?.StatusId != null)
                        {
                            var statusObj = AllStatus.Where(t => t.Id == item.EmployeeTask?.StatusId).FirstOrDefault();
                            if (statusObj != null)
                            {
                                taskListObj.Status = statusObj.Name;
                            }
                            // var statusObj = taskList.Where(t => t.EmployeeTask.Status.Id == item.EmployeeTask?.StatusId).FirstOrDefault();
                            // if (statusObj != null)
                            // {
                            //     taskListObj.Status = statusObj.EmployeeTask.Status.Name;
                            // }
                        }
                        if (item.EmployeeTask?.EndDate != null)
                        {
                            var todayDate = DateTime.UtcNow;
                            if (item.EmployeeTask?.EndDate < todayDate)
                            {
                                var overdueStatusObj = AllStatus.Where(t => t.Name.ToLower() == "overdue").FirstOrDefault();
                                taskListObj.Status = overdueStatusObj?.Name;
                            }
                        }
                        var employeeClientTaskObj = _employeeClientTaskService.GetByTaskId(taskListObj.Id);
                        if (employeeClientTaskObj != null && employeeClientTaskObj.ClientId != null)
                        {
                            var clientObj = _clientService.GetById(employeeClientTaskObj.ClientId.Value);
                            if (clientObj != null)
                            {
                                taskListObj.ClientName = clientObj.FirstName + " " + clientObj.LastName;
                            }
                        }
                        taskListObj.CreatedOn = item.EmployeeTask?.CreatedOn;
                        var assignTaskUsers = _employeeTaskUserService.GetAssignUsersByEmployeeTask(taskListObj.Id);
                        if (assignTaskUsers != null && assignTaskUsers.Count > 0)
                        {
                            List<EmployeeProjectTaskUserListResponse> assignTaskUserVMList = new List<EmployeeProjectTaskUserListResponse>();
                            // if (item.AssignedUsers == null)
                            // {
                            //     item.AssignedUsers = new List<EmployeeProjectTaskUserListResponse>();
                            // }
                            foreach (var assignTaskUserObj in assignTaskUsers)
                            {
                                EmployeeProjectTaskUserListResponse employeeProjectTaskUserListResponseObj = new EmployeeProjectTaskUserListResponse();
                                var Timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
                                if (AllUsers != null)
                                {
                                    var userObj2 = AllUsers.Where(t => t.Id == assignTaskUserObj.UserId).FirstOrDefault();
                                    if (userObj2 != null)
                                    {
                                        employeeProjectTaskUserListResponseObj.UserId = assignTaskUserObj.UserId;
                                        if (userObj2.ProfileImage == null)
                                        {
                                            employeeProjectTaskUserListResponseObj.ProfileURL = null;
                                        }
                                        else
                                        {
                                            employeeProjectTaskUserListResponseObj.ProfileURL = OneClappContext.CurrentURL + "User/ProfileImageView/" + userObj2.Id + "?" + Timestamp;
                                        }
                                    }
                                    assignTaskUserVMList.Add(employeeProjectTaskUserListResponseObj);
                                }
                            }
                            taskListObj.AssignedUsers = assignTaskUserVMList;
                        }
                        employeeProjectTaskListResponseList.Add(taskListObj);
                    }
                }
            }
            return new OperationResult<List<EmployeeProjectTaskListResponse>>(true, System.Net.HttpStatusCode.OK, "", employeeProjectTaskListResponseList, totalCount);
        }

        private List<EmployeeProjectTask> ShortTaskByColumn(string ShortColumn, string ShortOrder, List<EmployeeProjectTask> taskList)
        {
            List<EmployeeProjectTask> employeeTaskVMList = new List<EmployeeProjectTask>();
            employeeTaskVMList = taskList;
            if (ShortColumn != "" && ShortColumn != null)
            {
                if (ShortColumn.ToLower() == "description")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        employeeTaskVMList = taskList.OrderBy(t => t.EmployeeTask.Description).ToList();
                    }
                    else
                    {
                        employeeTaskVMList = taskList.OrderByDescending(t => t.EmployeeTask.Description).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "createdon")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        employeeTaskVMList = taskList.OrderBy(t => t.EmployeeTask.CreatedOn).ToList();
                    }
                    else
                    {
                        employeeTaskVMList = taskList.OrderByDescending(t => t.EmployeeTask.CreatedOn).ToList();
                    }
                }
                else if (ShortColumn.ToLower() == "status")
                {
                    if (ShortOrder.ToLower() == "asc")
                    {
                        employeeTaskVMList = taskList.OrderBy(t => t?.EmployeeTask?.Status?.Name).ToList();
                    }
                    else
                    {
                        employeeTaskVMList = taskList.OrderByDescending(t => t?.EmployeeTask?.Status?.Name).ToList();
                    }
                }
                else
                {
                    employeeTaskVMList = taskList.OrderByDescending(t => t.EmployeeTask.CreatedOn).ToList();
                }
            }

            return employeeTaskVMList;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost]
        public async Task<OperationResult<EmployeeProjectAssignClientResponse>> AssignClient([FromBody] EmployeeProjectAssignClientRequest requestModel)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            EmployeeProjectDto employeeProjectDto = new EmployeeProjectDto();
            employeeProjectDto.Id = requestModel.EmployeeProjectId;
            employeeProjectDto.TenantId = TenantId;
            employeeProjectDto.ClientId = requestModel.ClientId;
            employeeProjectDto.UpdatedBy = UserId;

            var employeeProjectObj = await _employeeProjectService.CheckInsertOrUpdate(employeeProjectDto);

            if (employeeProjectObj != null)
            {
                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
                employeeProjectActivityObj.ProjectId = requestModel.EmployeeProjectId;
                employeeProjectActivityObj.UserId = UserId;
                employeeProjectActivityObj.Activity = "Project assigned to client";

                var AddUpdateProjectActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
            }
            var employeeProjectAssignClientResponse = _mapper.Map<EmployeeProjectAssignClientResponse>(employeeProjectObj);
            employeeProjectAssignClientResponse.EmployeeProjectId = employeeProjectObj.Id;
            await _hubContext.Clients.All.OnProjectModuleEvent(requestModel.EmployeeProjectId, TenantId);
            return new OperationResult<EmployeeProjectAssignClientResponse>(true, System.Net.HttpStatusCode.OK, "Client assigned successfully", employeeProjectAssignClientResponse);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpPost("{EmployeeProjectId}")]
        public async Task<OperationResult> UnAssignClient(long EmployeeProjectId)
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);

            EmployeeProjectDto employeeProjectDto = new EmployeeProjectDto();
            employeeProjectDto.Id = EmployeeProjectId;
            employeeProjectDto.TenantId = TenantId;
            employeeProjectDto.ClientId = null;

            var employeeProjectObj = await _employeeProjectService.CheckInsertOrUpdate(employeeProjectDto);

            if (employeeProjectObj != null)
            {
                EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity()
                {
                    ProjectId = EmployeeProjectId,
                    UserId = UserId,
                    Activity = "Project unassigned to client"
                };
                var AddUpdateProjectActivity = await _employeeProjectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
            }
            await _hubContext.Clients.All.OnProjectModuleEvent(EmployeeProjectId, TenantId);
            return new OperationResult(true, System.Net.HttpStatusCode.OK, "Client unassigned", EmployeeProjectId);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, ExternalUser, TenantUser")]
        [HttpGet]
        [SwaggerOperation(Description = "api used for get list for assign project to logged in user")]
        public async Task<OperationResult<List<ProjectDropDownListResponse>>> DropDownList()
        {
            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
            List<ProjectDropDownListResponse> projectResponseList = new List<ProjectDropDownListResponse>();
            var projectList = _employeeProjectUserService.GetByUserId(UserId);
            if (projectList != null && projectList.Count > 0)
            {
                foreach (var item in projectList)
                {
                    if (item != null)
                    {
                        ProjectDropDownListResponse projectObj = new ProjectDropDownListResponse();
                        projectObj = _mapper.Map<ProjectDropDownListResponse>(item?.EmployeeProject);
                        projectResponseList.Add(projectObj);
                    }
                }
            }
            return new OperationResult<List<ProjectDropDownListResponse>>(true, System.Net.HttpStatusCode.OK, "", projectResponseList);
        }

        // [Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPut]
        // public async Task<OperationResult<EmployeeProjectDto>> Priority([FromBody] EmployeeProjectPriorityRequest requestModel)
        // {
        //     ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //     UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
        //     TenantId = Convert.ToInt32(user.FindFirst("TenantId").Value);
        //     EmployeeProjectDto employeeProjectDtoObj = new EmployeeProjectDto();
        //     EmployeeProjectActivity employeeProjectActivityObj = new EmployeeProjectActivity();
        //     if (requestModel.Id != null)
        //     {
        //         // start logic for Update Current task with priority
        //         var projectObj = _employeeProjectService.GetEmployeeProjectById(requestModel.Id.Value);
        //         projectObj.Priority = requestModel.CurrentPriority;
        //         projectObj.UpdatedBy = UserId;

        //         // employeeProjectActivityObj.Activity = "Priority changed for this project. ";
        //         employeeProjectActivityObj.Activity = Enums.ProjectActivityEnum.Priority_changed_for_this_project.ToString().Replace("_", " ");

        //         var projectAddUpdate = await _employeeProjectService.UpdateEmployeeProject(projectObj, projectObj.Id);

        //         employeeProjectActivityObj.ProjectId = requestModel.Id;
        //         employeeProjectActivityObj.UserId = UserId;

        //         var AddUpdate = await _projectActivityService.CheckInsertOrUpdate(employeeProjectActivityObj);
        //         // End Logic


        //         // start logic for task move in with out section list
        //         var employeeProjectLsit = _employeeProjectService.GetAllByTenant(TenantId);

        //         if (requestModel.CurrentPriority < employeeProjectLsit.Count())
        //         {
        //             if (requestModel.CurrentPriority != requestModel.PreviousPriority)
        //             {
        //                 if (requestModel.PreviousPriority < requestModel.CurrentPriority)
        //                 {
        //                     var projects = employeeProjectLsit.Where(t => t.Priority > requestModel.PreviousPriority && t.Priority <= requestModel.CurrentPriority && t.Id != requestModel.Id).ToList();
        //                     if (projects != null && projects.Count() > 0)
        //                     {
        //                         foreach (var item in projects)
        //                         {
        //                             item.Priority = item.Priority - 1;
        //                             await _employeeProjectService.UpdateEmployeeProject(item, item.Id);
        //                         }
        //                     }
        //                 }
        //                 else if (requestModel.PreviousPriority > requestModel.CurrentPriority)
        //                 {
        //                     var projects = employeeProjectLsit.Where(t => t.Priority < requestModel.PreviousPriority && t.Priority >= requestModel.CurrentPriority && t.Id != requestModel.Id).ToList();
        //                     if (projects != null && projects.Count() > 0)
        //                     {
        //                         foreach (var item in projects)
        //                         {
        //                             item.Priority = item.Priority + 1;
        //                             await _employeeProjectService.UpdateEmployeeProject(item, item.Id);
        //                         }
        //                     }
        //                 }
        //             }

        //             // end
        //         }
        //     }
        //     return new OperationResult<EmployeeProjectDto>(true, System.Net.HttpStatusCode.OK, "", employeeProjectDtoObj);
        // }

    }

}