using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Dto.Mollie;
using matcrm.data.Models.MollieModel;
using matcrm.data.Models.MollieModel.Customer;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.MollieModel.Subscription;
using matcrm.data.Models.Tables;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.Mollie.Customer;
using matcrm.service.Services.Mollie.Payment;
using matcrm.service.Services.Mollie.Subscription;
using matcrm.authentication.jwt;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json.Linq;
using matcrm.api.SignalR;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using matcrm.api.SignalR;
using matcrm.api.Helper;
using matcrm.authentication.jwt;
using matcrm.data.Context;
using matcrm.data.Models.Dto;
using matcrm.data.Models.MollieModel.Payment.Response;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.BusinessLogic;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Services.Mollie.Payment;
using matcrm.service.Utility;
using matcrm.data.Models.Request;
using matcrm.data.Models.Response;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace matcrm.api.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class MollieController : Controller
    {
        // private readonly ICustomerClient _customerClient;
        private readonly ICustomerOverviewClient _customerOverviewClient;
        private readonly ICustomerStorageClient _customerStorageClient;
        private readonly IPaymentOverviewClient _paymentOverviewClient;
        private readonly IPaymentStorageClient _paymentStorageClient;
        private readonly ISubscriptionOverviewClient _subscriptionOverviewClient;
        private readonly ISubscriptionStorageClient _subscriptionStorageClient;

        private readonly ISubscriptionPlanService _subscriptionPlanService;
        private readonly ISubscriptionPlanDetailService _subscriptionPlanDetailService;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly IMollieCustomerService _mollieCustomerService;
        private readonly IMollieSubscriptionService _mollieSubscriptionService;
        private readonly IUserService _userService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly ITenantService _tenantService;
        private readonly IRoleService _roleService;
        private readonly IHubContext<BroadcastHub, IHubClient> _hubContext;
        private IMapper _mapper;
        private SendEmail sendEmail;
        private int UserId = 0;
        private JwtManager jwtManager;



        public MollieController(
            // ICustomerClient customerClient,
            ICustomerOverviewClient customerOverviewClient,
            ICustomerStorageClient customerStorageClient,
            IPaymentOverviewClient paymentOverviewClient,
            IPaymentStorageClient paymentStorageClient,
            ISubscriptionOverviewClient subscriptionOverviewClient,
            ISubscriptionStorageClient subscriptionStorageClient,
            IMapper mapper,
            ISubscriptionPlanService subscriptionPlanService,
            ISubscriptionPlanDetailService subscriptionPlanDetailService,
            IUserSubscriptionService userSubscriptionService,
            ISubscriptionTypeService subscriptionTypeService,
            IUserService userService,
            IMollieCustomerService mollieCustomerService,
            IMollieSubscriptionService mollieSubscriptionService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailProviderService emailProviderService,
            IEmailConfigService emailConfigService,
            ITenantService tenantService,
            IRoleService roleService,
            IHubContext<BroadcastHub, IHubClient> hubContext
        )
        {
            // _customerClient = customerClient;
            _customerOverviewClient = customerOverviewClient;
            _customerStorageClient = customerStorageClient;
            _paymentOverviewClient = paymentOverviewClient;
            _paymentStorageClient = paymentStorageClient;
            _subscriptionOverviewClient = subscriptionOverviewClient;
            _subscriptionStorageClient = subscriptionStorageClient;
            _subscriptionPlanService = subscriptionPlanService;
            _subscriptionPlanDetailService = subscriptionPlanDetailService;
            _userSubscriptionService = userSubscriptionService;
            _subscriptionTypeService = subscriptionTypeService;
            _userService = userService;
            _mollieCustomerService = mollieCustomerService;
            _mollieSubscriptionService = mollieSubscriptionService;
            _mapper = mapper;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProviderService = emailProviderService;
            _emailConfigService = emailConfigService;
            _tenantService = tenantService;
            _roleService = roleService;
            _hubContext = hubContext;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
            jwtManager = new JwtManager(userService, tenantService, roleService);
        }


        [HttpPost]
        public async Task<SubscriptionResponse> CreateCustomer(CreateCustomerModel model)
        {
            CustomerResponse customerResponseObj = new CustomerResponse();
            SubscriptionResponse subscriptionResponseObj = new SubscriptionResponse();
            if (string.IsNullOrEmpty(model.CustomerId) && !string.IsNullOrEmpty(model.Email) && !string.IsNullOrEmpty(model.Name))
            {
                customerResponseObj = await this._customerStorageClient.Create(model);
                model.CustomerId = customerResponseObj.Id;
            }
            else
            {

                CreatePaymentModel createPaymentModelObj = new CreatePaymentModel()
                {
                    Amount = Convert.ToDecimal(0.01),
                    Currency = PaymentCurrency.EUR,
                    CustomerId = model.CustomerId,
                    SequenceType = "first",
                    Description = "first payment from api"
                };
                await this._paymentStorageClient.Create(createPaymentModelObj);
                CreateSubscriptionModel createSubscriptionModelObj = new CreateSubscriptionModel();
                createSubscriptionModelObj.CustomerId = model.CustomerId;
                createSubscriptionModelObj.Amount.value = Convert.ToDecimal(100.00);
                createSubscriptionModelObj.Amount.Currency = PaymentCurrency.EUR;
                createSubscriptionModelObj.IntervalAmount = 1;

                createSubscriptionModelObj.Times = 10;
                createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Days;
                createSubscriptionModelObj.Description = "Test 1 subscription from api";

                subscriptionResponseObj = await this._subscriptionStorageClient.Create(createSubscriptionModelObj);
            }
            return subscriptionResponseObj;
        }

        [HttpPost]
        public async Task<PaymentResponse> CreatePayment(CreatePaymentModel model)
        {
            return await this._paymentStorageClient.Create(model);
            // return this.RedirectToAction(nameof(this.Index));
        }

        [HttpPost]
        public async Task<OperationResult<PaymentResponse>> AddUpdatePayment(UserSubscriptionDto model)
        {
            PaymentResponse paymentResponseObj = new PaymentResponse();
            if (!string.IsNullOrEmpty(model.WebhookUrl))
            {
                SubscriptionResponse subscriptionResponseObj = new SubscriptionResponse();

                MollieCustomer? mollieCustomerObj = null;
                UserSubscription? userSubscriptionObj = null;

                ClaimsPrincipal user = this.User as ClaimsPrincipal;
                UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

                var userObj = _userService.GetUserById(UserId);
                if (UserId != null && model.SubScriptionPlanId != null && model.SubscriptionTypeId != null)
                {

                    UserSubscriptionDto userSubscriptionDto = new UserSubscriptionDto();
                    userSubscriptionDto.SubscriptionTypeId = model.SubscriptionTypeId;
                    userSubscriptionDto.SubScriptionPlanId = model.SubScriptionPlanId;
                    userSubscriptionDto.UserId = UserId;
                    userSubscriptionDto.IsSubscribed = true;
                    userSubscriptionObj = await _userSubscriptionService.CheckInsertOrUpdate(userSubscriptionDto);

                    CustomerResponse customerResponseObj = new CustomerResponse();
                    CreateCustomerModel createCustomerModelObj = new CreateCustomerModel();
                    mollieCustomerObj = _mollieCustomerService.GetByUser(UserId);
                    if (mollieCustomerObj != null)
                    {
                        createCustomerModelObj.CustomerId = mollieCustomerObj.CustomerId;
                        createCustomerModelObj.Email = mollieCustomerObj.User.Email;
                        createCustomerModelObj.Name = mollieCustomerObj.User.FirstName;
                    }
                    else
                    {
                        if (userObj != null)
                        {
                            createCustomerModelObj.Email = userObj.Email;
                            createCustomerModelObj.Name = userObj.FirstName;
                            customerResponseObj = await this._customerStorageClient.Create(createCustomerModelObj);
                            MollieCustomerDto mollieCustomerDto = new MollieCustomerDto();
                            mollieCustomerDto.UserId = UserId;
                            mollieCustomerDto.CustomerId = customerResponseObj.Id;
                            mollieCustomerObj = await _mollieCustomerService.CheckInsertOrUpdate(mollieCustomerDto);
                        }
                    }
                    if (model.SubscriptionTypeId != null)
                    {
                        var subscriptionTypeObj = _subscriptionTypeService.GetById(model.SubscriptionTypeId.Value);
                        if (model.SubScriptionPlanId != null)
                        {
                            var subscriptionPlanObj = _subscriptionPlanService.GetById(model.SubScriptionPlanId.Value);
                            if (subscriptionTypeObj.Name == "Yearly")
                            {
                                model.Price = (subscriptionPlanObj.YearlyPrice * 12);
                            }

                            decimal DEBITAMT = Convert.ToDecimal(string.Format("{0:F2}", model.Price));
                            CreatePaymentModel createPaymentModelObj = new CreatePaymentModel()
                            {
                                Amount = DEBITAMT,
                                Currency = PaymentCurrency.EUR,
                                CustomerId = mollieCustomerObj.CustomerId,
                                SequenceType = "first",
                                Description = "first payment from api",
                                WebhookUrl = model.WebhookUrl
                            };
                            paymentResponseObj = await this._paymentStorageClient.Create(createPaymentModelObj);

                            // CreateSubscriptionModel createSubscriptionModelObj = new CreateSubscriptionModel();
                            // createSubscriptionModelObj.CustomerId = mollieCustomerObj.CustomerId;
                            // createSubscriptionModelObj.Amount.value = DEBITAMT;
                            // createSubscriptionModelObj.Amount.Currency = PaymentCurrency.EUR;
                            // // subscriptionModel.IntervalAmount = 1;

                            // // subscriptionModel.Times = 12;
                            // if (subscriptionTypeObj.Name == "Monthly")
                            // {
                            //     createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Months;
                            //     createSubscriptionModelObj.IntervalAmount = 1;
                            //     // subscriptionModel.Times = 12;
                            // }
                            // else
                            // {
                            //     createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Months;
                            //     createSubscriptionModelObj.IntervalAmount = 12;
                            //     // subscriptionModel.Times = 1;
                            // }

                            // Random generator = new Random();
                            // String r = generator.Next(0, 1000000).ToString("D6");
                            // // subscriptionModel.IntervalPeriod = IntervalPeriod.Days;
                            // createSubscriptionModelObj.Description = "Test 1 subscription from api " + r;
                            // var bbb = DateTime.Now.ToString("YYYY-MM-DD");
                            // createSubscriptionModelObj.startDate = DateTime.Now.ToString("yyyy-MM-dd");

                            // subscriptionResponseObj = await this._subscriptionStorageClient.Create(createSubscriptionModelObj);
                        }
                    }
                    // MollieSubscriptionDto mollieSubscriptionDto = new MollieSubscriptionDto();
                    // mollieSubscriptionDto.SubscriptionId = subscriptionResponseObj.Id;
                    // mollieSubscriptionDto.PaymentId = paymentResponseObj.Id;
                    // mollieSubscriptionDto.UserId = UserId;
                    // var mollieSubscriptionObj = await _mollieSubscriptionService.CheckInsertOrUpdate(mollieSubscriptionDto);

                    // if (userObj != null)
                    // {
                    //     var userName = userObj.FirstName + " " + userObj.LastName;
                    //     await sendEmail.SendEmailForUserSubscription(userObj.Email, userName, null);
                    // }


                }
                return new OperationResult<PaymentResponse>(true, System.Net.HttpStatusCode.OK, "", paymentResponseObj);
            }
            return new OperationResult<PaymentResponse>(false, System.Net.HttpStatusCode.OK, "Please provide webhook Url", paymentResponseObj);
        }

        [HttpGet]
        [Authorize]
        public async Task<OperationResult<PaymentResponse>> Payment()
        {
            PaymentResponse paymentResponseObj = new PaymentResponse();
            MollieCustomer? mollieCustomer = null;
            UserSubscription? userSubscription = null;

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            MollieSubscription mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(UserId);

            var userObj = _userService.GetUserById(UserId);
            if (UserId != null)
            {
                paymentResponseObj = await this._paymentStorageClient.GetPayment(mollieSubscriptionObj.PaymentId);

                if (paymentResponseObj.Status == "paid")
                {
                    var userSubscriptionObj = _userSubscriptionService.GetByUser(UserId);
                    var userSubscriptionDto = _mapper.Map<UserSubscriptionDto>(userSubscriptionObj);
                    userSubscriptionDto.IsSubscribed = true;
                    var data = await _userSubscriptionService.CheckInsertOrUpdate(userSubscriptionDto);
                    return new OperationResult<PaymentResponse>(true, System.Net.HttpStatusCode.OK, "", paymentResponseObj);
                }

            }

            return new OperationResult<PaymentResponse>(false, System.Net.HttpStatusCode.OK, "", paymentResponseObj);
        }

        [HttpDelete]
        public async Task<OperationResult<UserSubscriptionDto>> CancelSubscription()
        {
            UserSubscriptionDto? userSubscriptionDto = null;

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var userSubscriptionObj = _userSubscriptionService.GetByUser(UserId);
            var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(UserId);
            var mollieCustomerObj = _mollieCustomerService.GetByUser(UserId);
            if (mollieCustomerObj != null && mollieSubscriptionObj != null)
            {
                await this._subscriptionStorageClient.Cancel(mollieCustomerObj.CustomerId, mollieSubscriptionObj.SubscriptionId);
                var mollieSubscriptionDelete = _mollieSubscriptionService.DeleteMollieSubscription(mollieSubscriptionObj.Id);
                var userSubscriptionDelete = _userSubscriptionService.DeleteUserSubscription(userSubscriptionObj.Id);
                userSubscriptionDto = _mapper.Map<UserSubscriptionDto>(userSubscriptionObj);
                return new OperationResult<UserSubscriptionDto>(true, System.Net.HttpStatusCode.OK, "", userSubscriptionDto);
            }

            return new OperationResult<UserSubscriptionDto>(false, System.Net.HttpStatusCode.OK, "", userSubscriptionDto);
        }


        [HttpPost]
        public async Task<OperationResult<UserSubscriptionDto>> ContinueSubscription()
        {
            UserSubscriptionDto? userSubscriptionDto = null;

            ClaimsPrincipal user = this.User as ClaimsPrincipal;
            UserId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);

            var userSubscriptionObj = _userSubscriptionService.GetByUser(UserId);
            userSubscriptionObj.IsSubscribed = true;
            if (userSubscriptionObj.SubscribedOn != null)
            {
                userSubscriptionObj.SubscribedOn = userSubscriptionObj.SubscribedOn.Value.AddDays(30);
            }
            else
            {
                userSubscriptionObj.SubscribedOn = DateTime.UtcNow;
            }

            userSubscriptionDto = _mapper.Map<UserSubscriptionDto>(userSubscriptionObj);
            var AddUpdate = await _userSubscriptionService.CheckInsertOrUpdate(userSubscriptionDto);
            return new OperationResult<UserSubscriptionDto>(false, System.Net.HttpStatusCode.OK, "", userSubscriptionDto);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<string> NotifyPaymentStatusChange([FromForm] string id)
        {
            decimal DEBITAMT = Convert.ToDecimal(string.Format("{0:F2}", 100));
            // Use the received id value to continue the payment verification process ...
            PaymentResponse paymentResponseObj = await this._paymentStorageClient.GetPayment(id);
            //PaymentResponse paymentResponseObj = new PaymentResponse();
            //Get Status from PaymentId;
            // using (HttpClient httpClient = new HttpClient())
            // {
            //     httpClient.BaseAddress = new Uri("https://api.mollie.com/v2/payments/" + id);
            //     httpClient.DefaultRequestHeaders.Accept.Clear();
            //     httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //     if (!string.IsNullOrEmpty(OneClappContext.MollieApiKey))
            //         httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", OneClappContext.MollieApiKey);



            //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://api.mollie.com/v2/payments/" + id);



            //    HttpResponseMessage httpResponseMessage = httpClient.Send(request);
            //    Console.WriteLine("payment call1");
            //     if (httpResponseMessage.IsSuccessStatusCode)
            //     {
            //         Console.WriteLine("payment call2");
            //         var responseData = httpResponseMessage.Content.ReadAsStringAsync().Result;
            //         paymentResponseObj = JsonConvert.DeserializeObject<PaymentResponse>(responseData);
            //     }
            // }

            if (paymentResponseObj != null && paymentResponseObj.Status == OneClappContext.PaidPaymentStatus)
            {
                Console.WriteLine("payment call3");
                var customerId = paymentResponseObj.CustomerId;
                MollieCustomer mollieCustomerObj = _mollieCustomerService.GetByCustomerId(customerId);
                if (mollieCustomerObj != null)
                {
                    var UserId = mollieCustomerObj.UserId;
                    var user = _userService.GetUserById(UserId.Value);
                    if (user != null)
                    {
                        var userName = user.FirstName + " " + user.LastName;
                        await sendEmail.SendEmailForUserSubscription(user.Email, userName, null);
                    }
                    CreateSubscriptionModel createSubscriptionModelObj = new CreateSubscriptionModel();
                    createSubscriptionModelObj.CustomerId = mollieCustomerObj.CustomerId;
                    createSubscriptionModelObj.Amount.value = DEBITAMT;
                    createSubscriptionModelObj.Amount.Currency = PaymentCurrency.EUR;
                    // subscriptionModel.IntervalAmount = 1;

                    // subscriptionModel.Times = 12;
                    // if (subscriptionTypeObj.Name == "Monthly")
                    // {
                    createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Months;
                    createSubscriptionModelObj.IntervalAmount = 1;
                    // subscriptionModel.Times = 12;
                    // }
                    // else
                    // {
                    //     createSubscriptionModelObj.IntervalPeriod = IntervalPeriod.Months;
                    //     createSubscriptionModelObj.IntervalAmount = 12;
                    //     // subscriptionModel.Times = 1;
                    // }

                    Random generator = new Random();
                    String r = generator.Next(0, 1000000).ToString("D6");
                    // subscriptionModel.IntervalPeriod = IntervalPeriod.Days;
                    createSubscriptionModelObj.Description = "Test 1 subscription from api " + r;
                    var bbb = DateTime.Now.ToString("YYYY-MM-DD");
                    createSubscriptionModelObj.startDate = DateTime.Now.ToString("yyyy-MM-dd");


                    var subscriptionResponseObj = await this._subscriptionStorageClient.Create(createSubscriptionModelObj);
                    var TenantName = "";
                    var RoleName = "";
                    if (subscriptionResponseObj != null)
                    {

                        MollieSubscriptionDto mollieSubscriptionDto = new MollieSubscriptionDto();
                        mollieSubscriptionDto.SubscriptionId = subscriptionResponseObj.Id;
                        mollieSubscriptionDto.PaymentId = id;
                        mollieSubscriptionDto.UserId = UserId;
                        var mollieSubscriptionObj = await _mollieSubscriptionService.CheckInsertOrUpdate(mollieSubscriptionDto);

                        if (user != null && user.TenantId != null)
                        {
                            var tenantObj = _tenantService.GetTenantById(user.TenantId.Value);
                            if (tenantObj != null)
                            {
                                TenantName = tenantObj.TenantName;
                            }
                        }
                        if (user != null && user.RoleId != null)
                        {
                            var roleObj = _roleService.GetRoleById(user.RoleId.Value);
                            if (roleObj != null)
                            {
                                RoleName = roleObj.RoleName;
                            }
                        }
                        //                 var token = new JwtTokenBuilder()
                        //   .AddClaims(GetClaim(user, TenantName, RoleName, true))
                        //   .Build();
                        Tokens JwtTokenData = await jwtManager.GenerateJWTTokens(user.Email, true);
                        var token = "";
                        if(JwtTokenData != null){
                            token = JwtTokenData.Access_Token;
                        }
                        await _hubContext.Clients.All.OnUserSubscriptionEventEmit(UserId, token);
                    }
                }

            }
            Console.WriteLine("Call webhookUrl from mollie");
            Console.WriteLine(id);
            return id;
        }

        private Dictionary<string, string> GetClaim(User userObj, string Tenant, string RoleName, bool IsSubscribed = false)
        {
            var claim = new Dictionary<string, string>();
            claim.Add("Id", userObj.Id.ToString());
            claim.Add("Sid", userObj.Id.ToString());
            if (Tenant != "" || Tenant != null)
            {
                claim.Add("Tenant", Tenant.ToString());
            }
            if (userObj.TenantId != null)
            {
                claim.Add("TenantId", userObj.TenantId.ToString());
            }
            if (userObj.WeClappUserId != null)
            {
                claim.Add("WeClappUserId", userObj.WeClappUserId.ToString());
            }
            if (userObj.WeClappToken != null)
            {
                claim.Add("WeClappToken", userObj.WeClappToken);
            }
            if (RoleName != "")
            {
                claim.Add(ClaimTypes.Role, RoleName);
            }
            if (RoleName != "")
            {
                claim.Add("RoleName", RoleName);
            }
            if (userObj != null && !string.IsNullOrEmpty(userObj.Email))
            {
                claim.Add("Email", userObj.Email);
            }

            claim.Add("IsSubscribed", IsSubscribed.ToString());

            return claim;
        }

    }
}