using System;
using System.Globalization;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Context;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Utility;
using static matcrm.data.Helpers.DataUtility;

namespace matcrm.service.BusinessLogic
{
    public class SignupUser
    {
        #region Property Initialization
        private readonly IVerificationCodeService _verificationCodeService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProvider;
        private SendEmail sendEmail;
        // private IErrorLogService _errorlogservice;
        private readonly IUserService _userService;
        private IMapper _mapper;

        #endregion

        #region Constructor
        public SignupUser(
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IVerificationCodeService verificationCodeService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProvider,
            // IErrorLogService errorlogservice,
            IMapper mapper,
            IUserService userService,
            OneClappContext context)
        {
            _userService = userService;
            _verificationCodeService = verificationCodeService;
            _emailLogService = emailLogService;
            _emailTemplateService = emailTemplateService;
            // _errorlogservice = errorlogservice;
            _mapper = mapper;
            sendEmail = new SendEmail(emailTemplateService, emailLogService, emailConfigService, emailProvider, mapper);
        }
        #endregion

        //  #region 'Set Function'
        public async Task<UserDto> ProcessUserSignup(UserDto model)
        {
            var user = new User();
            IFormatProvider culture = new CultureInfo("en-US", true);
            var userObj = _mapper.Map<User>(model);
            // Check if email already registered in the system
            var ExistsUser = _userService.GetUserByEmailForVerify(model.Email);
            if (ExistsUser != null)
            {
                model.IsUserAlreadyExist = true;
                model.ErrorMessage = CommonMessage.EmailExist;
                model.IsSuccessSignUp = false;
                return model;
            }
            else
            {
                // Check email is valid or not
                if (!UtilityClass.IsValidEmail(model.Email))
                {
                    model.IsEmailValid = false;
                    model.IsSuccessSignUp = false;
                    model.ErrorMessage = CommonMessage.InValidEmail;
                    return model;
                }
                if (model.CreatedBy != null)
                {
                    user.CreatedBy = model.CreatedBy;
                    model.Password = "Test@123";
                }

                // Set and insert new user
                user.UserName = model.UserName;
                user.PhoneNo = model.PhoneNo;
                user.TenantId = model.TenantId;
                user.Email = model.Email.Trim();
                // user.Password = ShaHash.GetHash (model.Password);
                user.CreatedOn = UtilityClass.GetCurrentDateTime();

                user.FirstName = string.IsNullOrEmpty(model.FirstName) ? null : UtilityClass.ToTitleCase(model.FirstName);
                user.LastName = string.IsNullOrEmpty(model.LastName) ? null : UtilityClass.ToTitleCase(model.LastName);
                // user.RoleId = model.RoleId;
                string guid = Convert.ToString(Guid.NewGuid());
                user.TempGuid = guid;
                model.TempGuid = guid;
                user.RoleId = model.RoleId;
                user = await _userService.AddUser(user, model.Password);
                // user = _userService.Add (user);
                // _userService.SaveAsync ();

                model.IsSuccessSignUp = true;

                // Set verification Code
                var verificationCode = new VerificationCode();
                var code = UtilityClass.VerificationCode();
                verificationCode.UserId = user.Id;
                verificationCode.Email = user.Email;
                verificationCode.Code = code;
                verificationCode.IsUsed = false;
                verificationCode.CreatedOn = DataUtility.GetCurrentDateTime();
                verificationCode.ExpiredOn = UtilityClass.VerificationCodeExpiredOn();
                verificationCode.VerificationFor = DataUtility.VerificationCodeType.SignupEmail.ToString();
                verificationCode = await _verificationCodeService.CheckInsertOrUpdate(verificationCode);

                model.Id = user.Id;
                model.EmailOTP = code;
                // try
                // {
                //     // Send sign up confirmation email with credentials
                //     // await sendEmail.SendSignUpEmail (model.Email, DataUtility.ShaHash.GetHash (user.Email), user.FirstName + ' ' + user.LastName, code);
                //     await sendEmail.SendSignUpEmail(model.Email, guid, user.FirstName + ' ' + user.LastName, code, user.TenantId);
                // }
                // catch (Exception ex)
                // {
                //     // errorlogservice.LogException (ex);
                //     throw;
                // }

            }

            return model;
        }

        public async Task<UserDto> ProcessInviteUser(UserDto model)
        {
            var user = new User();
            IFormatProvider culture = new CultureInfo("en-US", true);
            var userObj = _mapper.Map<User>(model);

            // Check email is valid or not
            if (!UtilityClass.IsValidEmail(model.Email))
            {
                model.IsEmailValid = false;
                model.IsSuccessSignUp = false;
                model.ErrorMessage = CommonMessage.InValidEmail;
                return model;
            }
            if (model.CreatedBy != null)
            {
                user.CreatedBy = model.CreatedBy;
                model.Password = "Test@123";
            }

            // Set and insert new user
            user.UserName = model.UserName;
            user.PhoneNo = model.PhoneNo;
            user.TenantId = model.TenantId;
            user.Email = model.Email.Trim();
            // user.Password = ShaHash.GetHash (model.Password);
            user.CreatedOn = UtilityClass.GetCurrentDateTime();

            user.FirstName = string.IsNullOrEmpty(model.FirstName) ? null : UtilityClass.ToTitleCase(model.FirstName);
            user.LastName = string.IsNullOrEmpty(model.LastName) ? null : UtilityClass.ToTitleCase(model.LastName);
            // user.RoleId = model.RoleId;
            string guid = Convert.ToString(Guid.NewGuid());
            user.TempGuid = guid;
            model.TempGuid = guid;
            user.RoleId = model.RoleId;
            user = await _userService.AddUser(user, model.Password);
            // user = _userService.Add (user);
            // _userService.SaveAsync ();

            model.IsSuccessSignUp = true;

            // // Set verification Code
            // var verificationCode = new VerificationCode ();
            // var code = UtilityClass.VerificationCode ();
            // verificationCode.UserId = user.Id;
            // verificationCode.Email = user.Email;
            // verificationCode.Code = code;
            // verificationCode.IsUsed = false;
            // verificationCode.CreatedOn = DataUtility.GetCurrentDateTime ();
            // verificationCode.ExpiredOn = UtilityClass.VerificationCodeExpiredOn ();
            // verificationCode.VerificationFor = DataUtility.VerificationCodeType.SignupEmail.ToString ();
            // verificationCode = await _verificationCodeService.CheckInsertOrUpdate (verificationCode);

            // model.Id = user.Id;
            // try {
            //     // Send sign up confirmation email with credentials
            //     // await sendEmail.SendSignUpEmail (model.Email, DataUtility.ShaHash.GetHash (user.Email), user.FirstName + ' ' + user.LastName, code);
            //     await sendEmail.SendSignUpEmail (model.Email, guid, user.FirstName + ' ' + user.LastName, code, user.TenantId);
            // } catch (Exception ex) {
            //     // errorlogservice.LogException (ex);
            //     throw;
            // }

            model.TempGuid = user.TempGuid;
            model.IsEmailValid = true;
            // Send verification code email
            try
            {
                await sendEmail.SendSetPasswordEmail(user.Email, user.Id.ToString(), user.TempGuid.ToString(), user.FirstName + ' ' + user.LastName, user.TenantId, "Invite user");
            }
            catch (Exception ex)
            {
                // errorlogservice.LogException (ex);
                throw;
            }



            return model;
        }

        public async Task<ResetPasswordDto> ResetPassword(ResetPasswordDto model)
        {
            var user = _userService.GetUserByEmail(model.Email);
            if (user != null)
            {

                model.TempGuid = user.TempGuid;
                // Set verification Code
                var verificationCode = new VerificationCode();
                var code = UtilityClass.VerificationCode();
                verificationCode.UserId = user.Id;
                verificationCode.Email = user.Email;
                verificationCode.Code = code;
                verificationCode.IsUsed = false;
                verificationCode.CreatedOn = DataUtility.GetCurrentDateTime();
                verificationCode.ExpiredOn = UtilityClass.VerificationCodeExpiredOn();
                verificationCode.VerificationFor = DataUtility.VerificationCodeType.PasswordReset.ToString();
                verificationCode = await _verificationCodeService.CheckInsertOrUpdate(verificationCode);
                model.IsEmailValid = true;
                // Send verification code email
                try
                {
                    await sendEmail.SendPasswordResetEmail(user.Email, user.Id.ToString(), user.TempGuid.ToString(), user.FirstName + ' ' + user.LastName, code, user.TenantId);
                }
                catch (Exception ex)
                {
                    // errorlogservice.LogException (ex);
                    throw;
                }
            }
            else
            {
                model.IsEmailValid = false;
                model.EmailErrorMessage = CommonMessage.EmailNotRegisterd;
            }

            return model;
        }

        public async Task<User> ChangePassword(User user, string password)
        {
            var user1 =  await _userService.UpdatePassword(user, password);
            if (user1 != null)
            {

                // Send verification code email
                try
                {
                    await sendEmail.ChangePasswordEmail(user.Email, user.Id.ToString(), user.TempGuid.ToString(), user.FirstName + ' ' + user.LastName, null);
                }
                catch (Exception ex)
                {
                    // errorlogservice.LogException (ex);
                    throw;
                }
            }
            else
            {
                return null;
            }

            return user;
        }

        public async Task<User> SetPassword(User user, string password)
        {
            var user1 = await _userService.UpdatePassword(user, password);
            if (user1 != null)
            {

                // Send verification code email
                var verificationCode = new VerificationCode();
                var code = UtilityClass.VerificationCode();
                verificationCode.UserId = user1.Id;
                verificationCode.Email = user1.Email;
                verificationCode.Code = code;
                verificationCode.IsUsed = false;
                verificationCode.CreatedOn = DataUtility.GetCurrentDateTime();
                verificationCode.ExpiredOn = UtilityClass.VerificationCodeExpiredOn();
                verificationCode.VerificationFor = DataUtility.VerificationCodeType.EmailVerify.ToString();
                verificationCode = await _verificationCodeService.CheckInsertOrUpdate(verificationCode);
                try
                {
                    await sendEmail.InviteUserVerifyEmail(user1.Email, user1.TempGuid, user1.FirstName + ' ' + user1.LastName, code, user1.TenantId.Value);
                }
                catch (Exception ex)
                {
                    // errorlogservice.LogException (ex);
                    throw;
                }
            }
            else
            {
                return null;
            }

            return user;
        }

        public async Task<UserDto> VerifyUser(UserDto model)
        {
            var user = _userService.GetUserByEmailForVerify(model.Email);
            if (user != null)
            {
                var verificationCode = _verificationCodeService.GetVerificationCodeDetailByUserId(model.EmailOTP, user.Id);
                if (verificationCode != null)
                {
                    if (verificationCode.Code == model.EmailOTP)
                    {
                        user.IsEmailVerified = true;
                        user.VerifiedOn = DateTime.UtcNow;
                        // signupUser
                        user = await _userService.UpdateUser(user, false);
                        verificationCode.IsUsed = true;
                        verificationCode.UpdatedOn = DateTime.UtcNow;
                        _verificationCodeService.UpdateVerificationCode(verificationCode);

                        // Send verification code email
                        try
                        {
                            if (model.IsSignUp == true)
                            {
                                await sendEmail.SendEmailVerifiedNotification(user.Email, user.FirstName + ' ' + user.LastName, user.TenantId.Value);
                            }
                            if (model.CreatedBy != null && model.IsSignUp == false)
                            {
                                var verificationCode1 = new VerificationCode();
                                var code = UtilityClass.VerificationCode();
                                verificationCode1.UserId = user.Id;
                                verificationCode1.Email = user.Email;
                                verificationCode1.Code = code;
                                verificationCode1.IsUsed = false;
                                verificationCode1.CreatedOn = DataUtility.GetCurrentDateTime();
                                verificationCode1.ExpiredOn = UtilityClass.VerificationCodeExpiredOn();
                                verificationCode1.VerificationFor = DataUtility.VerificationCodeType.PasswordReset.ToString();
                                verificationCode1 = await _verificationCodeService.CheckInsertOrUpdate(verificationCode);

                                // Send verification code email

                                await sendEmail.ResetPassWordForNewUser(user.Email, user.Id.ToString(), user.TempGuid.ToString(), user.FirstName + ' ' + user.LastName, code, user.TenantId.Value);

                            }
                            model.IsEmailVerified = true;
                            model = _mapper.Map<UserDto>(user);
                            return model;
                        }
                        catch (Exception ex)
                        {
                            // errorlogservice.LogException (ex);
                            throw;
                        }
                    }
                }
            }
            // else {
            model.IsEmailValid = false;
            model.IsEmailVerified = false;
            model.ErrorMessage = CommonMessage.DefaultErrorMessage;
            // }

            return model;
        }

        public async Task<EmailLogDto> ResendEmailToUser(EmailLogDto model)
        {
            EmailLogDto emailLogDto = new EmailLogDto();
            if (model.Id != null)
            {
                emailLogDto = await sendEmail.ResendEmail(model.Id.Value);
                return emailLogDto;
            }
            else
            {
                return null;
            }
        }
    }
}