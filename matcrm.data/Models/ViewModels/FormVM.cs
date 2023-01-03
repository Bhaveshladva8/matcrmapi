using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.ViewModels
{
    public class FormVM
    {
        public FormVM()
        {
            //EmailTemplate = new EmailTemplateVM();
            // IdentifierLinked = new List<SelectListItem>();
            FormFields = new List<FormFieldVM>();
            FormPreview = new FormPreview();
            FormDashboard = new FormDashBoardVM();
            FormInstruction = new List<string>();
        }


        public long FormId { get; set; }
        public string FormName { get; set; }
        public bool IsActive { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public Nullable<System.DateTime> LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public Nullable<System.DateTime> DeletedOn { get; set; }

        public string HeadLine { get; set; }
        public string Description { get; set; }
        public string ButtonText { get; set; }
        public string ButtonTextSubmit { get; set; }
        public string ButtonColor { get; set; }
        public string ImageName { get; set; }
        public string ImagePath { get; set; }
        //public int ImagePlacement { get; set; }
        public string EmbededUrl { get; set; }
        public string EmbededCode { get; set; }
        public string FormHtml { get; set; }
        public bool HasCaptcha { get; set; }
        public string CapchaSitekey { get; set; }
        public string CapchaSecret { get; set; }
        public bool IsShowOnEveryPage { get; set; }
        public bool IsShowOnSpecificPage { get; set; }
        public int Orientation { get; set; }
        public string HeadLineColorCode { get; set; }
        public string TabColorCode { get; set; }
        public string LandingBackground { get; set; }
        public double FontSize { get; set; }
        public double Opacity { get; set; }
        public int FormLabelPlaceId { get; set; }
        public string LandingPageMessage { get; set; }
        public string RedirectUrl { get; set; }
        public bool RedirectChecked { get; set; }
        public string ImagePlaceClass { get; set; }
        public long? Action { get; set; }

        public int EmailTemplateId { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }

        public List<string> FormInstruction { get; set; }
        // public List<SelectListItem> IdentifierType { get; set; }
        // public List<SelectListItem> FormLabelPlaces { get; set; }
        //public EmailTemplateVM EmailTemplate { get; set; }
        public FormPreview FormPreview { get; set; }
        public string FormSpecificPages { get; set; }
        public string FormExcludedPages { get; set; }
        //public string SpecificPageList { get; set; }
        //public string ExcludedPageList { get; set; }
        public FormDashBoardVM FormDashboard { get; set; }
        // public List<SelectListItem> IdentifierLinked { get; set; }
        public List<FormFieldVM> FormFields { get; set; }
        // public List<SelectListItem> Actions { get; set; }
        //public List<SelectListItem> Placements { get; set; }
    }

    public class FormSubmissionVM
    {
        public long FormId { get; set; }
        public long FormSubmissionId { get; set; }
        public Nullable<System.DateTime> FormSubmitedOn { get; set; }
        public int IdentifierId { get; set; }
        public string Value { get; set; }
    }

    public class SubmitFormVM
    {
        //public long FormId { get; set; }
        //public string field_value_Email { get; set; }
        //public string field_value_First { get; set; }
        //public string field_value_Last { get; set; }
        public List<LandingField> LandingFields { get; set; }
    }

    public class LandingField
    {
        public string KeyId { get; set; }
        public string KeyName { get; set; }
        public string Value { get; set; }
    }

    public class FormSubmissionFieldsDetailVM
    {
        public int IdentifierId { get; set; }
        public string Value { get; set; }
    }

    public class EmailTemplateVM
    {
        public int EmailTemplateId { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }

    public class PostSignUpVM
    {
        public int EmailTemplateId { get; set; }
        public long FormId { get; set; }
        public string FromName { get; set; }
        public string FromEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string LandingPageMessage { get; set; }
        public string RedirectUrl { get; set; }
        public string SpecificPageList { get; set; }
        public string ExcludedPageList { get; set; }
        public string TabColorCode { get; set; }
    }

    public class FormExcludedPagesVM
    {
        public Nullable<long> FormId { get; set; }
        public string PageUrl { get; set; }
    }

    public class FormSpecificPagesVM
    {
        public Nullable<long> FormId { get; set; }
        public string PageUrl { get; set; }
    }

    public class FormActionVM
    {
        public long ActionId { get; set; }
        public string ActionName { get; set; }
    }

    public class ActivateFormVM
    {
        public long FormId { get; set; }
        public string CapchaSiteKey { get; set; }
        public string CapchaSecret { get; set; }
        public string EmbededCode { get; set; }
    }

    public class FormFieldVM
    {
        public long FormId { get; set; }
        public long FormFieldLinkId { get; set; }
        public string Label { get; set; }
        public int Identifier { get; set; }
        public string IdentifierName { get; set; }
        public int IdentifierType { get; set; }
        public bool IsRequired { get; set; }
    }

    public class ImagePlacementVM
    {
        public int PlacementId { get; set; }
        public string Placement { get; set; }
    }

    public class FormPreview
    {
        public long FormId { get; set; }
        public string FormName { get; set; }
        public string HeadLine { get; set; }
        public string Description { get; set; }
        public string HeadLineColorCode { get; set; }
        public string TabColorCode { get; set; }
        public int FormLabelPlaceId { get; set; }
        public string ButtonText { get; set; }
        public string ButtonTextSubmit { get; set; }
        public string ButtonColor { get; set; }
        //public string EmailAddress { get; set; }
        public double FontSize { get; set; }
        public double Opacity { get; set; }
        public int Orientation { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public int ImagePlacement { get; set; }
        public string HeaderStyle { get; set; }
        public string WidgetStyle { get; set; }
        public string ImagePlaceClass { get; set; }
        public string SideTabClass { get; set; }
        public string LandingBackground { get; set; }
        public List<FormFieldVM> FormFields { get; set; }
    }

    public class FormModalPopUP
    {
        public FormModalPopUP()
        {
            CustomFields = new List<OneClappFormFieldDto>();
            FormFields = new List<FormFieldVM>();
        }
        public long FormId { get; set; }
        public string FormName { get; set; }
        public string HeadLine { get; set; }
        public string Description { get; set; }
        public string HeadLineColorCode { get; set; }
        public string TabColorCode { get; set; }
        public int FormLabelPlaceId { get; set; }
        public string ButtonText { get; set; }
        public string ButtonTextSubmit { get; set; }
        public string ButtonColor { get; set; }
        //public string EmailAddress { get; set; }
        public double FontSize { get; set; }
        [Column(TypeName = "jsonb")]
        public object? FormStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? LayoutStyle { get; set; }

        [Column(TypeName = "jsonb")]
        public object? HeaderStyle { get; set; }
        public double Opacity { get; set; }
        public int Orientation { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public string SubmitUrl { get; set; }
        public int ImagePlacement { get; set; }
        public string ImagePlaceClass { get; set; }
        // public string HeaderStyle { get; set; }
        public string WidgetStyle { get; set; }
        public string SideTabClass { get; set; }
        public List<FormFieldVM> FormFields { get; set; }
        public List<OneClappFormFieldDto> CustomFields { get; set; }
    }

    public class LandingPageVM
    {
        public long FormId { get; set; }
        public string FormName { get; set; }
        public string HeadLine { get; set; }
        //public string WebSiteUrl { get; set; }
        //public string PageList { get; set; }
        public string LandingPageMessage { get; set; }
        //public string RedirectUrl { get; set; }
        public string Description { get; set; }
        public string EmailAddress { get; set; }
        public string ImagePath { get; set; }
        public string ImageName { get; set; }
        public int ImagePlacement { get; set; }
        public string SubmitUrl { get; set; }
        public string LandingBackground { get; set; }
        public string ButtonText { get; set; }
        public string ButtonColor { get; set; }
        public int FormLablePlaceId { get; set; }
        public bool HasCaptcha { get; set; }
        public string CaptchaSiteKey { get; set; }
        public string CaptchaSecretKey { get; set; }
        public List<FormFieldVM> FormFields { get; set; }
        //public FormPreview FormPreview { get; set; }
    }

    public class FormDashBoardVM
    {
        public FormDashBoardVM()
        {
            SubmissionList = new List<FormUserSubmission>();
        }
        public long? FormId { get; set; }
        public Nullable<int> submission { get; set; }
        public Nullable<int> open { get; set; }
        public Nullable<int> load { get; set; }
        public string month { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        // public List<SelectListItem> FormListDrp { get;set;}
        public List<FormUserSubmission> SubmissionList { get; set; }
        public List<FormSubmissionContact> FormContactList { get; set; }
    }

    public class FormSubmissionContact
    {
        public long FormId { get; set; }
        public string FormName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Company { get; set; }
        public string Phone { get; set; }
        public string Title { get; set; }
    }

    public class FormUserSubmission
    {
        public string Email { get; set; }
        public int Submission { get; set; }
    }

    public class CreateJSVM
    {
        public long FormId { get; set; }
        public string FormName { get; set; }
        public string JSPath { get; set; }
        public string Contents { get; set; }
        public string SubmitUrl { get; set; }
        public string WidgetUrl { get; set; }
    }

    public class APIModal
    {
        public string MasterListName { get; set; }
        public long CampaignID { get; set; }
        public long CampaignFormID { get; set; }
        public string Company { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Title { get; set; }
        public string Phone { get; set; }
    }

    public class APIResponseModal
    {
        public string AuthCode { get; set; }
        public string AuthMessage { get; set; }
        public string AuthToken { get; set; }
    }

    public class Recapcha
    {
        public string response { get; set; }

        public string secret { get; set; }

        public string remoteip { get; set; }

        public string success { get; set; }

        public DateTime challenge_ts { get; set; }

        public string hostname { get; set; }
    }

}