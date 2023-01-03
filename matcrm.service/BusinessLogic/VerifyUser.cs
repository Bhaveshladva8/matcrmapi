using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;
using matcrm.service.Common;
using matcrm.service.Services;
using matcrm.service.Utility;

namespace matcrm.service.BusinessLogic {
    public class VerifyUser {
        #region Property Initialization
        private readonly IUserService _userService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly IEmailLogService _emailLogService;
        private readonly IEmailConfigService _emailConfigService;
        private readonly IEmailProviderService _emailProviderService;
        private readonly IVerificationCodeService _verificationCodeService;
        private SendEmail sendEmail;
        private ErrorLogService errorlogservice;
        private IMapper _mapper;
        #endregion

        #region Constructor
        public VerifyUser (IUserService userService,
            IEmailTemplateService emailTemplateService,
            IEmailLogService emailLogService,
            IEmailConfigService emailConfigService,
            IEmailProviderService emailProviderService,
            IVerificationCodeService verificationCodeService,
            IMapper mapper) {
            _userService = userService;
            _emailTemplateService = emailTemplateService;
            _emailLogService = emailLogService;
            _emailProviderService = emailProviderService;
            _emailConfigService = emailConfigService;
            _verificationCodeService = verificationCodeService;
            _mapper = mapper;
            sendEmail = new SendEmail (emailTemplateService, emailLogService, emailConfigService, emailProviderService, mapper);
        }
        #endregion

        #region Set Function
        public async Task<VerifyUserDto> VerifyUserByVerificationCode (VerifyUserDto model) {
            var taskList = new List<Task> ();
            var verificationCode = (dynamic) null;
            var user = _userService.GetUserByEmailHash (model.Hash);
            model.IsCodeHashVerified = false;
            // model.NotificationVM = new NotificationVM ();
            string verificationFor = "";

            // Set User Details
            if (user != null) {
                // Check is verification code has verified
                // User made password reset request
                if (!string.IsNullOrEmpty (model.VerificationCodeHash)) {
                    verificationCode = _verificationCodeService.GetVerificationCodeByHash (model.VerificationCodeHash, user.Id);
                    if (verificationCode == null || (verificationCode != null && verificationCode.Code != model.VerificationCode)) {
                        model.IsVerified = false;
                        model.IsAlreadyVerified = false;
                        model.IsVerificationCodeValid = false;
                        model.Message = CommonMessage.VerificationCodeInvalid;
                        return model;
                    }

                    // Means user is being verified for password reset request
                    model.IsCodeHashVerified = true;
                    verificationFor = verificationCode.VerificationFor;
                }

                // Check if email has already verificied or not 
                // User has already signup & verified their email and trying to verify email again
                if (user.IsEmailVerified && !model.IsCodeHashVerified) {
                    model.IsVerified = false;
                    model.IsAlreadyVerified = true;
                    model.Message = CommonMessage.SuccessEmailAlreadyVerified;
                    return model;
                }

                // User is signup recently and verifying their email for the first time
                if (!string.IsNullOrEmpty (model.VerificationCode) && !model.IsCodeHashVerified) {
                    // verificationCode = _verificationCodeService.GetVerificationCodeDetailByUserId (model.VerificationCode, user.Id, DataUtility.VerificationCodeType.PasswordReset.ToString ());
                    verificationCode = _verificationCodeService.GetVerificationCodeDetailByUserId (model.VerificationCode, user.Id);
                }

                // Check verification code is valid
                if (verificationCode == null || verificationCode.ExpiredOn < DateTime.UtcNow || verificationCode.IsUsed) {
                    model.IsVerified = false;
                    model.IsVerificationCodeValid = false;
                    model.Message = CommonMessage.VerificationCodeInvalid;
                    return model;
                } else {
                    // Update verification code as used
                    verificationCode.IsUsed = true;
                    verificationCode.UpdatedOn = DateTime.UtcNow;
                   await _verificationCodeService.CheckInsertOrUpdate (verificationCode);
                }

                user.IsEmailVerified = true;
                user.VerifiedOn = DateTime.UtcNow;
                // _userService.UpdateAsync (user, Convert.ToInt32 (user.Id));
                await _userService.UpdateUser (user, false);
                // _userService.SaveAsync ();
                model.IsVerified = true;
                model.Message = CommonMessage.SuccessEmailVerified.Replace ("[email]", user.Email);

                // Send email verified notification email

                if (!model.IsCodeHashVerified) {
                    taskList.Add (sendEmail.SendEmailVerifiedNotification (user.Email, user.FirstName + ' ' + user.LastName, model.TenantId.Value));
                    var code = UtilityClass.VerificationCode ();
                    model.ResetPasswordLink = "verify/account/" + ShaHashData.GetHash (code) + "/" + ShaHashData.GetHash (user.Email);
                    model.IsVerified = true;
                    if(user.CreatedBy != null)
                    {
                        await sendEmail.SendPasswordResetEmail (user.Email, user.Id.ToString (), user.TempGuid.ToString (), user.FirstName + ' ' + user.LastName, code, user.TenantId.Value);
                    }
                }
                await Task.WhenAll (taskList);
                model.UserId = user.Id;
            }
            return model;
        }

        public async Task<VerifyUserDto> IsEmailAlreadyVerified (VerifyUserDto model) {
            var user = _userService.GetUserByEmailHash (model.Hash);
            if (user != null) {
                // Check is email already verificied or not
                if (user.IsEmailVerified) {
                    model.IsVerified = false;
                    model.IsAlreadyVerified = true;
                    model.Message = CommonMessage.SuccessEmailAlreadyVerified;
                    return model;
                }
            }

            return model;
        }
        #endregion
    }
}