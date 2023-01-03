using System;
using System.Collections.Generic;
using matcrm.data.Models.Dto;

namespace matcrm.data.Models.ViewModels
{
    public class FormClass
    {
        public FormClass()
        {
            FormFields = new List<FormFieldVM>();
            FormSubmissions = new List<FormSubmissionVM>();
            EmailTemplate = new EmailTemplateVM();
            FormPreview = new FormPreview();
            CustomFields = new List<OneClappFormFieldDto>();
        }

        public long FormId { get; set; }
        public string FormName { get; set; }
        public bool IsActive { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? LastModifiedOn { get; set; }
        public bool IsDeleted { get; set; }
        public bool RedirectChecked { get; set; }
        public DateTime? DeletedOn { get; set; }
        public string HeadLine { get; set; }
        public string Description { get; set; }
        public string ButtonText { get; set; }
        public string ButtonTextSubmit { get; set; }
        public string ImageName { get; set; }
        public string EmbededUrl { get; set; }
        public string EmbededCode { get; set; }
        public string CapchaSiteKey { get; set; }
        public string CapchaSecret { get; set; }
        public bool IsShowOnEveryPage { get; set; }
        public bool IsShowOnSpecificPage { get; set; }
        public string LandingPageMessage { get; set; }
        public string RedirectUrl { get; set; }
        public Nullable<bool> IsSendEmail { get; set; }
        public string HeadLineColorCode { get; set; }
        public string TabColorCode { get; set; }
        public string LandingBackground { get; set; }
        public long Action { get; set; }
        public Nullable<double> FontSize { get; set; }
        public Nullable<double> Opacity { get; set; }
        public Nullable<int> FormLablePlaceId { get; set; }
        public Nullable<int> OrientationId { get; set; }
        public Nullable<int> EmailTemplateId { get; set; }
        public Nullable<long> FormSubmissionId { get; set; }
        public Nullable<int> OpenRate { get; set; }
        public Nullable<int> SubmissionRate { get; set; }
        public Nullable<int> WidgetOpen { get; set; }
        public Nullable<int> LandingPageLoad { get; set; }
        public string FormHTMLString { get; set; }
        public string LayoutBackground { get; set; }

        public EmailTemplateVM EmailTemplate { get; set; }
        public FormPreview FormPreview { get; set; }
        public Nullable<bool> HasCaptcha { get; set; }
        public List<FormFieldVM> FormFields { get; set; }

        // public List<CustomFieldDto> CustomFields { get; set; }
        public List<OneClappFormFieldDto> CustomFields { get; set; }
        public List<FormSubmissionVM> FormSubmissions { get; set; }
        public string SpecificPages { get; set; }
        public string ExcludedPages { get; set; }
    }
}