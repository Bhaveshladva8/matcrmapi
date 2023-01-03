using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using matcrm.data.Models.Dto;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;

namespace matcrm.api.Controllers
{
    // [Authorize]
    [Route("[controller]/[action]")]
    [ApiController]
    public class WeClappController : Controller
    {
        private readonly IWeClappService _weClappService;
        private readonly IConfiguration _config;
        private readonly IProxyService _proxyService;
        private readonly IWeClappUserService _weClappUserService;
        private IMapper _mapper;
        private int UserId = 0;
        public WeClappController(IWeClappService weClappService,
            IConfiguration config,
            IProxyService proxyService,
            IWeClappUserService weClappUserService,
            IMapper mapper)
        {
            _weClappService = weClappService;
            _config = config;
            _proxyService = proxyService;
            _weClappUserService = weClappUserService;
            _mapper = mapper;
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<WeClappDropdownResponse>> Dropdown(WeClappDropdownRequest currentUser)
        {
            DropDownVM dropDownVMObj = new DropDownVM();
            WeClappDropdownResponse responseDropdown = new WeClappDropdownResponse();
            var requestcurrentUser = _mapper.Map<UserVM>(currentUser);
            if (!string.IsNullOrEmpty(requestcurrentUser.ApiKey))
            {
                var customers = await _weClappService.GetCustomers(requestcurrentUser.ApiKey, requestcurrentUser.Tenant);
                var ticketCategories = await _weClappService.GetTicketCategories(requestcurrentUser.ApiKey, requestcurrentUser.Tenant);
                var ticketTypes = await _weClappService.GetTicketTypes(requestcurrentUser.ApiKey, requestcurrentUser.Tenant);

                dropDownVMObj.Customers = customers;
                dropDownVMObj.TicketCategories = ticketCategories;
                dropDownVMObj.TicketTypes = ticketTypes;
                responseDropdown = _mapper.Map<WeClappDropdownResponse>(dropDownVMObj);
                return new OperationResult<WeClappDropdownResponse>(true, System.Net.HttpStatusCode.OK,"", responseDropdown);              

            }
            else
            {
                responseDropdown = _mapper.Map<WeClappDropdownResponse>(dropDownVMObj);
                return new OperationResult<WeClappDropdownResponse>(false, System.Net.HttpStatusCode.OK,"", responseDropdown);                
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<Ticket>>> SaveTicket(TicketDto ticket)
        {
            List<Ticket> ticktList = new List<Ticket>();
            if (!string.IsNullOrEmpty(ticket.ApiKey))
            {
                var tickets = await _weClappService.GetTickets(ticket.ApiKey, ticket.Tenant);
                tickets = tickets.Where(t => t.CustomerId == ticket.CustomerId.ToString()).ToList();
                ticktList = tickets;
                return new OperationResult<List<Ticket>>(true, System.Net.HttpStatusCode.OK, "", ticktList);
            }
            else
            {
                return new OperationResult<List<Ticket>>(false, System.Net.HttpStatusCode.OK, "", ticktList);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<Ticket>> UpdateTicketStatus(TicketDto model)
        {
            Ticket ticketObj = new Ticket();

            if (!string.IsNullOrEmpty(model.ApiKey))
            {
                var ticketDto = _mapper.Map<TicketVM>(model);
                var ticket = await _weClappService.UpdateTicketStatus(model.ApiKey, model.Tenant, ticketDto);

                if (ticket != null)
                {
                    ticketObj = ticket;
                    return new OperationResult<Ticket>(true, System.Net.HttpStatusCode.OK, "", ticketObj);
                }
            }
            return new OperationResult<Ticket>(false, System.Net.HttpStatusCode.OK, "", ticketObj);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPut]
        public async Task<OperationResult<Ticket>> AssignTicket(TicketDto model)
        {
            Ticket ticketObj = new Ticket();

            if (!string.IsNullOrEmpty(model.ApiKey))
            {
                var ticketDto = _mapper.Map<TicketVM>(model);
                var ticket = await _weClappService.UpdateTicketStatus(model.ApiKey, model.Tenant, ticketDto);

                if (ticket != null)
                {
                    ticketObj = ticket;
                    ticketObj.Message = "Ticket assigned.";
                }
                else
                {
                    ticketObj.Message = "Ticket status not assigned.";
                    return new OperationResult<Ticket>(false, System.Net.HttpStatusCode.OK, ticketObj.Message, ticketObj);
                }
                return new OperationResult<Ticket>(true, System.Net.HttpStatusCode.OK, "", ticketObj);
            }
            else
            {
                return new OperationResult<Ticket>(false, System.Net.HttpStatusCode.OK, "", ticketObj);
            }
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<List<weClappGetTicketsResponse>>> Tickets(weClappGetTicketsRequest currentUser)
        {
            List<Ticket> ticktList = new List<Ticket>();
            var requestcurrentUser = _mapper.Map<UserVM>(currentUser);
            if (!string.IsNullOrEmpty(requestcurrentUser.ApiKey))
            {
                var tickets = await _weClappService.GetTickets(requestcurrentUser.ApiKey, requestcurrentUser.Tenant);
                tickets = tickets.Where(t => t.CustomerId == requestcurrentUser.CustomerId.ToString()).ToList();
                var responseTicketList = _mapper.Map<List<weClappGetTicketsResponse>>(tickets);
                return new OperationResult<List<weClappGetTicketsResponse>>(true, System.Net.HttpStatusCode.OK,"", responseTicketList);                
            }
            var reponseticktList = _mapper.Map<List<weClappGetTicketsResponse>>(ticktList);
            return new OperationResult<List<weClappGetTicketsResponse>>(false, System.Net.HttpStatusCode.OK,"", reponseticktList);            
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<TicketInfoVM>> TicketInfo([FromBody] TicketDto model)
        {
            TicketInfoVM ticketInfoVMObj = new TicketInfoVM();
            if (!string.IsNullOrEmpty(model.ApiKey))
            {
                var timeRecords = await _weClappService.GetTimeRecords(model.ApiKey, model.Tenant);
                timeRecords = timeRecords.Where(t => t.ticketNumber == model.TicketNumber).ToList();

                var tickets = await _weClappService.GetTickets(model.ApiKey, model.Tenant);
                if (tickets != null)
                {
                    var selectedTicket = tickets.Where(t => t.TicketNumber == model.TicketNumber.ToString()).FirstOrDefault();
                    ticketInfoVMObj.TimeTecords = timeRecords;
                    ticketInfoVMObj.Ticket = selectedTicket;
                }
                return new OperationResult<TicketInfoVM>(true, System.Net.HttpStatusCode.OK, "", ticketInfoVMObj);
            }
            return new OperationResult<TicketInfoVM>(false, System.Net.HttpStatusCode.OK, "", ticketInfoVMObj);
        }

        // Save Time Record
        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpPost]
        public async Task<OperationResult<PostTimeRecord>> Add([FromForm] JobDto job)
        {

            if (!string.IsNullOrEmpty(job.ApiKey))
            {
                PostTimeRecord postTimeRecordObj = new PostTimeRecord()
                {
                    description = job.Description,
                    startDate = job.StartAt,
                    ticketNumber = job.Ticket,
                    durationSeconds = job.Duration
                };

                var result = await _weClappService.AddJob(job.ApiKey, job.Tenant, postTimeRecordObj);

                if (result != null)
                {
                    return new OperationResult<PostTimeRecord>(true, System.Net.HttpStatusCode.OK, "Ticket saved successfully.", postTimeRecordObj);
                }
            }
            PostTimeRecord postTimeRecord = new PostTimeRecord();
            return new OperationResult<PostTimeRecord>(false, System.Net.HttpStatusCode.OK, "Internal error occured.", postTimeRecord);
        }

        [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]
        public async Task<OperationResult<List<CustomerVM>>> Customers()
        {
            UserVM userVMObj = new UserVM();
            userVMObj.Tenant = "testit";
            userVMObj.ApiKey = "7a970695-5f65-4056-ab0e-9c6fd40ad7e6";

            List<CustomerVM> customerVMList = new List<CustomerVM>();
            if (!string.IsNullOrEmpty(userVMObj.ApiKey))
            {
                customerVMList = await _weClappService.GetCustomers(userVMObj.ApiKey, userVMObj.Tenant);

                return new OperationResult<List<CustomerVM>>(true, System.Net.HttpStatusCode.OK, "", customerVMList,customerVMList.Count());
            }
            else
            {
                return new OperationResult<List<CustomerVM>>(false, System.Net.HttpStatusCode.OK, "", customerVMList);
            }
        }

        // [Authorize(Roles = "Admin,TenantManager,TenantAdmin, TenantUser, ExternalUser")]
        [HttpGet]

        public async Task<OperationResult<CustomerCountResult>> CustomerCount()
        {
            UserVM userVMObj = new UserVM();
            userVMObj.Tenant = "testit";
            userVMObj.ApiKey = "7a970695-5f65-4056-ab0e-9c6fd40ad7e6";
            CustomerCountResult CustomerCountResult = new CustomerCountResult();
            if (!string.IsNullOrEmpty(userVMObj.ApiKey))
            {
                var count = await _weClappService.GetCustomerCount(userVMObj.ApiKey, userVMObj.Tenant);
                CustomerCountResult.Result = count;
                return new OperationResult<CustomerCountResult>(true, System.Net.HttpStatusCode.OK, "", CustomerCountResult);
            }
            else
            {
                return new OperationResult<CustomerCountResult>(false, System.Net.HttpStatusCode.OK, "", CustomerCountResult);
            }
        }


        [HttpGet]
        public async Task<string> CustomerColumn()
        {
            UserVM userVMObj = new UserVM();

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            // currentUser.Tenant = "testit";
            // currentUser.ApiKey = "7a970695-5f65-4056-ab0e-9c6fd40ad7e6";
            var WeClappUserObj = _weClappUserService.GetByUser(UserId);
            if (WeClappUserObj != null)
            {
                string weClappUrl = "https://*check*.weclapp.com/webapp/api/v1/customer?page=1&pageSize=1000";
                weClappUrl = weClappUrl.Replace("*check*", WeClappUserObj.TenantName);

                List<CustomerVM> customerVMList = new List<CustomerVM>();
                if (!string.IsNullOrEmpty(WeClappUserObj.ApiKey))
                {
                    var data = await _proxyService.GetProxy(weClappUrl, WeClappUserObj.ApiKey);

                    return data;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }


        }


        //[Authorize(Roles = "Admin, TenantAdmin, TenantManager, TenantUser, ExternalUser")]
        // [HttpPost ("job/document")]
        // public async Task<OperationResult<FinalizeVM>> Document ([FromForm] FinalizeVM finalize) {
        //     // Check if a given license key string is valid.
        //     bool isLicensed = IronPdf.License.IsValidLicense (_config.GetValue<string> ("IronPdf.LicenseKey"));
        //     // Check if IronPdf is licensed sucessfully 
        //     bool is_licensed = IronPdf.License.IsLicensed;

        //     // UserVM currentUser = AuthData.GetAll (this.User);
        //     // if (currentUser == null) return Json (new { success = false, message = "Authentication failed." });

        //     var Renderer = new HtmlToPdf ();

        //     // Save form data to PDF
        //     //var finalPdf = "Leistungsnachweis" + "-Ticket-" + DateTime.Now.Minute + "-" + DateTime.Now.Second + finialize.TicketId + ".pdf";
        //     var finalPdf = "Leistungsnachweis" + "-Ticket-" + finalize.TicketNumber + ".pdf";

        //     var finalPath = Path.Combine (Directory.GetCurrentDirectory (), @"wwwroot\Output\", finalPdf);
        //     Renderer.PrintOptions.CustomCssUrl = "https://pop.mateit.io/css/style.css";
        //     Renderer.PrintOptions.FitToPaperWidth = true;
        //     Renderer.PrintOptions.PrintHtmlBackgrounds = true;
        //     Renderer.PrintOptions.PaperOrientation = PdfPrintOptions.PdfPaperOrientation.Landscape;
        //     Renderer.PrintOptions.PaperSize = PdfPrintOptions.PdfPaperSize.A4;
        //     Renderer.PrintOptions.MarginLeft = 0;
        //     Renderer.PrintOptions.MarginRight = 0;
        //     Renderer.PrintOptions.MarginTop = 0;
        //     Renderer.PrintOptions.MarginBottom = 0;
        //     Renderer.PrintOptions.CssMediaType = PdfPrintOptions.PdfCssMediaType.Screen;
        //     Renderer.PrintOptions.ViewPortWidth = 1280;

        //     Renderer.PrintOptions.Header = new HtmlHeaderFooter () {
        //         Height = 40,
        //             HtmlFragment = "<div class='row text-center' style='text-align: center;margin: 0 !important;'>" +
        //             "<img alt='logo' src='" + finalize.HeaderImageUrl + "' height='150' width='100%'>" +
        //             "</div>",
        //             DrawDividerLine = false
        //     };

        //     Renderer.PrintOptions.Footer = new HtmlHeaderFooter () {
        //         Height = 30,
        //             HtmlFragment = "<div class='row col-12 text-center' style='text-align: center;margin: 0 !important;'>" +
        //             "<img alt='logo' src='" + finalize.FooterImageUrl + "' height='100' width='100%'>" +
        //             "</div>",
        //             DrawDividerLine = false
        //     };

        //     var doc = "<!DOCTYPE html>" +
        //         "<html>" +
        //         "<head>" +
        //         "<link rel = 'stylesheet' href = 'https://pop.mateit.io/css/style.css'/>" +
        //         (string.IsNullOrWhiteSpace (finalize.Fonts) ? "" : "<link href = 'https://fonts.googleapis.com/css2?family=" + finalize.Fonts + "&amp;display=swap' rel = 'stylesheet' />") +
        //         "</head>" +
        //         "<body>" +
        //         finalize.Doc +
        //         "</body>" +
        //         "</html>";

        //     var PDF = Renderer.RenderHtmlAsPdf (doc);

        //     if (!string.IsNullOrEmpty (finalize.ApiKey)) {
        //         var result = await _weClappService.PostDocument (finalize.ApiKey, finalize.Tenant, finalPdf, finalize.TicketId, PDF.BinaryData);
        //         if (result)
        //             return new OperationResult<FinalizeVM> (true, "Document posted successfully", finalize);
        //     }
        //     finalize = null;
        //     return new OperationResult<FinalizeVM> (false, "Internal error occured.", finalize);
        // }

    }
}