using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.service.Services;
using matcrm.service.Services.Mollie.Payment;
using matcrm.service.Services.Mollie.Subscription;

namespace matcrm.service.BusinessLogic
{
    public class PaymentNotificationLogic
    {
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly ISubscriptionTypeService _subscriptionTypeService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IMollieSubscriptionService _mollieSubscriptionService;
        private readonly IMollieCustomerService _mollieCustomerService;
        private readonly ISubscriptionStorageClient _subscriptionStorageClient;
        private readonly IClientService _clientService;
        private readonly IClientInvoiceService _clientInvoiceService;
        private readonly IInvoiceMollieSubscriptionService _invoiceMollieSubscriptionService;
        private readonly IPaymentStorageClient _paymentStorageClient;
        private readonly IContractService _contractService;
        // private readonly IEmployeeTaskService _employeeTaskService;
        private IMapper _mapper;
        private SendEmail sendEmail;

        public PaymentNotificationLogic(
            IUserSubscriptionService userSubscriptionService,
            ISubscriptionTypeService subscriptionTypeService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailProviderService emailProviderService,
            IEmailConfigService emailConfigService,
            IMollieSubscriptionService mollieSubscriptionService,
            IMollieCustomerService mollieCustomerService,
            ISubscriptionStorageClient subscriptionStorageClient,
            IInvoiceMollieSubscriptionService invoiceMollieSubscriptionService,
            IPaymentStorageClient paymentStorageClient,
            IClientInvoiceService clientInvoiceService,
            IClientService clientService,
            IContractService contractService,
            // IEmployeeTaskService employeeTaskService,
            IMapper mapper
            )
        {
            _userSubscriptionService = userSubscriptionService;
            _subscriptionTypeService = subscriptionTypeService;
            _mapper = mapper;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProviderService = emailProviderService;
            _emailConfigService = emailConfigService;
            _mollieSubscriptionService = mollieSubscriptionService;
            _mollieCustomerService = mollieCustomerService;
            _subscriptionStorageClient = subscriptionStorageClient;
            _invoiceMollieSubscriptionService = invoiceMollieSubscriptionService;
            _paymentStorageClient = paymentStorageClient;
            _clientInvoiceService = clientInvoiceService;
            _clientService = clientService;
            _contractService = contractService;
            // _employeeTaskService = employeeTaskService;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
        }

        public async Task<List<UserSubscriptionDto>> PaymentNotification()
        {
            var subscriptionTypes = _subscriptionTypeService.GetAll();
            var userSubscriptionList = _userSubscriptionService.GetAll();

            var subscriptionMonthlyTypeObj = _subscriptionTypeService.GetByName("Monthly");
            var subscriptionYearlyTypeObj = _subscriptionTypeService.GetByName("Yearly");
            if (subscriptionMonthlyTypeObj != null && subscriptionYearlyTypeObj != null)
            {
                foreach (var userSubscriptionObj in userSubscriptionList)
                {
                    if (userSubscriptionObj.SubscriptionTypeId == subscriptionMonthlyTypeObj.Id)
                    {
                        if (userSubscriptionObj.SubscribedOn != null)
                        {
                            var subscribedOn = userSubscriptionObj.SubscribedOn.Value;
                            var Today = DateTime.Today;
                            TimeSpan diff = Today.Date - subscribedOn.Date;

                            if (diff.Days % 25 == 0)
                            {
                                var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                                userSubscriptionObj.IsSubscribed = false;
                                var userSubscriptionDto = _mapper.Map<UserSubscriptionDto>(userSubscriptionObj);
                                var AddUpdate = await _userSubscriptionService.CheckInsertOrUpdate(userSubscriptionDto);
                                // var AddUpdate = await _userSubscriptionService.UpdateIsSubscribed(userSubscriptionObj, userSubscriptionObj.Id);
                                await sendEmail.SendEmailForExpireNotification(userSubscriptionObj.User.Email, userName, null);
                            }
                            else if (diff.Days == 29)
                            {
                                var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                                await sendEmail.SendEmailForExpireNotification(userSubscriptionObj.User.Email, userName, null);
                            }

                            // Add logic for cancel subscription after 30 days
                            else if (diff.Days == 30)
                            {

                                UserSubscriptionDto? userSubscriptionDto = null;
                                var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(userSubscriptionObj.UserId.Value);
                                var mollieCustomerObj = _mollieCustomerService.GetByUser(userSubscriptionObj.UserId.Value);
                                if (mollieCustomerObj != null && mollieSubscriptionObj != null)
                                {
                                    await this._subscriptionStorageClient.Cancel(mollieCustomerObj.CustomerId, mollieSubscriptionObj.SubscriptionId);
                                    var mollieSubscriptionDelete = _mollieSubscriptionService.DeleteMollieSubscription(mollieSubscriptionObj.Id);
                                    var userSubscriptionDelete = _userSubscriptionService.DeleteUserSubscription(userSubscriptionObj.Id);
                                }

                                var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                                await sendEmail.SendEmailForRemoveUserSubscription(userSubscriptionObj.User.Email, userName, null);
                            }
                        }
                        // else
                        // {
                        //     var UpdatedOn = userSubscriptionObj.UpdatedOn.Value;
                        //     var Today = DateTime.Today;
                        //     TimeSpan diff = Today.Date - UpdatedOn.Date;

                        //     if (diff.Days % 25 == 0)
                        //     {
                        //         var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                        //         userSubscriptionObj.IsSubscribed = false;
                        //         var userSubscriptionDto = _mapper.Map<UserSubscriptionDto>(userSubscriptionObj);
                        //         var AddUpdate = await _userSubscriptionService.UpdateIsSubscribed(userSubscriptionObj, userSubscriptionObj.Id);
                        //         await sendEmail.SendEmailForExpireNotification(userSubscriptionObj.User.Email, userName, null);
                        //     }
                        //     else if (diff.Days == 29)
                        //     {
                        //         var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                        //         await sendEmail.SendEmailForExpireNotification(userSubscriptionObj.User.Email, userName, null);
                        //     }

                        //     // Add logic for cancel subscription after 30 days
                        //     else if (diff.Days == 30)
                        //     {

                        //         UserSubscriptionDto? userSubscriptionDto = null;

                        //         var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(userSubscriptionObj.UserId.Value);
                        //         var mollieCustomerObj = _mollieCustomerService.GetByUser(userSubscriptionObj.UserId.Value);
                        //         if (mollieCustomerObj != null && mollieSubscriptionObj != null)
                        //         {
                        //             await this._subscriptionStorageClient.Cancel(mollieCustomerObj.CustomerId, mollieSubscriptionObj.SubscriptionId);
                        //             var mollieSubscriptionDelete = _mollieSubscriptionService.DeleteMollieSubscription(mollieSubscriptionObj.Id);
                        //             var userSubscriptionDelete = _userSubscriptionService.DeleteUserSubscription(userSubscriptionObj.Id);
                        //         }

                        //         var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                        //         await sendEmail.SendEmailForRemoveUserSubscription(userSubscriptionObj.User.Email, userName, null);
                        //     }
                        // }
                    }
                    else if (userSubscriptionObj.SubscriptionTypeId == subscriptionYearlyTypeObj.Id)
                    {
                        if (userSubscriptionObj.SubscribedOn != null)
                        {
                            var subscribedOn = userSubscriptionObj.SubscribedOn.Value;
                            var Today = DateTime.Today;
                            TimeSpan diff = Today.Date - subscribedOn.Date;

                            if (diff.Days == 360)
                            {
                                var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                                userSubscriptionObj.IsSubscribed = false;
                                var userSubscriptionDtoObj = _mapper.Map<UserSubscriptionDto>(userSubscriptionObj);
                                var AddUpdate = await _userSubscriptionService.CheckInsertOrUpdate(userSubscriptionDtoObj);
                                await sendEmail.SendEmailForExpireNotification(userSubscriptionObj.User.Email, userName, null);
                            }

                            // Add logic for cancel subscription after 1 year
                            else if (diff.Days == 365)
                            {

                                UserSubscriptionDto? userSubscriptionDto = null;

                                var mollieSubscriptionObj = _mollieSubscriptionService.GetByUser(userSubscriptionObj.UserId.Value);
                                var mollieCustomerObj = _mollieCustomerService.GetByUser(userSubscriptionObj.UserId.Value);
                                if (mollieCustomerObj != null && mollieSubscriptionObj != null)
                                {
                                    await this._subscriptionStorageClient.Cancel(mollieCustomerObj.CustomerId, mollieSubscriptionObj.SubscriptionId);
                                    var mollieSubscriptionDelete = _mollieSubscriptionService.DeleteMollieSubscription(mollieSubscriptionObj.Id);
                                    var userSubscriptionDelete = _userSubscriptionService.DeleteUserSubscription(userSubscriptionObj.Id);
                                }

                                var userName = userSubscriptionObj.User.FirstName + " " + userSubscriptionObj.User.LastName;
                                await sendEmail.SendEmailForRemoveUserSubscription(userSubscriptionObj.User.Email, userName, null);
                            }
                        }
                    }
                }
            }

            List<UserSubscriptionDto> userSubscritionDtoList = new List<UserSubscriptionDto>();

            userSubscritionDtoList = _mapper.Map<List<UserSubscriptionDto>>(userSubscriptionList);

            return new List<UserSubscriptionDto>(userSubscritionDtoList);

        }

        // Create logic for check invoice mollie payment status and update in to database
        public async Task<List<InvoiceMollieSubscription>> InvoicePaymentStatusCheck()
        {
            var invoicePayments = _invoiceMollieSubscriptionService.GetAllStatusNotPaid();
            foreach (var invoicePaymentObj in invoicePayments)
            {
                if (!string.IsNullOrEmpty(invoicePaymentObj.PaymentId))
                {
                    var MolliePaymentObj = await _paymentStorageClient.GetPayment(invoicePaymentObj.PaymentId);
                    if (MolliePaymentObj != null && invoicePaymentObj.Status != MolliePaymentObj.Status)
                    {
                        invoicePaymentObj.Status = MolliePaymentObj.Status;
                        await _invoiceMollieSubscriptionService.CheckInsertOrUpdate(invoicePaymentObj);
                    }
                }
            }
            return new List<InvoiceMollieSubscription>(invoicePayments);
        }

        // public async Task<List<ClientInvoice>> GenerateMonthlyInvoice()
        // {
        //     List<ClientInvoice> clientInvoices = new List<ClientInvoice>();
        //     List<Client> ClientList = _clientService.GetAll();
        //     foreach (var clientObj in ClientList)
        //     {
        //         // var clientInvoiceList = _clientInvoiceService.GetAllByClient(clientObj.Id);
        //         var clientInvoiceList = clientObj.Invoices;
        //         var clientContracts = clientObj.Contracts;
        //         // var TaskList = _employeetas
        //         foreach (var item in clientContracts)
        //         {
                    
        //         }
        //     }
        //     return new List<ClientInvoice>(clientInvoices);
        // }

    }
}