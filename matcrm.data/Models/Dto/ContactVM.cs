using System;
using System.Collections.Generic;
using System.Text;

namespace matcrm.data.Models.Dto
{
    // public class ContactVM
    // {
    //     public bool IsValid { get; set; } = true;

    //     public string ErrorMessage { get; set; }

    //     public Contact Contact { get; set; }

    //     public ContactCompany Company { get; set; }

    //     public bool IsEdit { get; set; }

    //     public UserContactResult UserContactResult { get; set; }

    //     public string CountryName { get; set; }

    //     public string StateName { get; set; }

    //     public string CityName { get; set; }

    //     public string NewSelectOption { get; set; }

    //     public string SelectOptionInfoCode { get; set; }

    //     public string TypeInfoCode { get; set; }

    //     public string AssignedUser { get; set; }

    //     public Nullable<long> CommunityShareId { get; set; }

    //     public Notification Notification { get; set; }

    //     public List<NotificationVM> NotificationVM { get; set; }

    //     public List<SocialInformation> socialInformation { get; set; }

    //     public bool IsAllowDuplicate { get; set; }

    //     public bool IsIncomingLead { get; set; }

    //     public bool IsAutomationNotification { get; set; } = false;

    //     public bool IsNotCheckSubscriptionFeature { get; set; }

    //     public string NoteFieldName { get; set; }
    // }

    // public class ContactSelectOptionVM
    // {
    //     public List<SelectOption> ContactType { get; set; }

    //     public List<SelectOption> ContactStage { get; set; }

    //     public List<SelectOption> Rating { get; set; }

    //     public List<SelectOption> ContactSource { get; set; }

    //     public List<SelectOption> LoanType { get; set; }

    //     public List<SelectOption> SellReason { get; set; }

    //     public List<SelectOption> CompanyType { get; set; }

    //     public List<SelectOption> MaritalStatus { get; set; }

    //     public List<SelectOption> DownPmtType { get; set; }

    //     public List<SelectOption> ActivityStatus { get; set; }

    //     public List<SelectOption> ContactCategory { get; set; }
    // }

    // public class ContactListVM
    // {
    //     public List<Contact> Contacts;
    // }

    public class UserContactResult
    {
        public long ContactId { get; set; }

        public long OwnerUserId { get; set; }

        public string Owner { get; set; }

        public string Thumbnail { get; set; }

        public Nullable<long> DotLoopContactId { get; set; }

        public Nullable<bool> IsDotLoopContact { get; set; }

        public Nullable<bool> IsSyncWithDotLoop { get; set; }

        public string FullName { get; set; }

        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Notes { get; set; }

        public string Address { get; set; }

        public string StreetNumber { get; set; }

        public string StreetName { get; set; }

        public string UnitNumber { get; set; }

        public int CityId { get; set; }

        public int StateId { get; set; }

        public string Zip { get; set; }

        public int CountryId { get; set; }

        public Nullable<DateTime> Appointment { get; set; }

        public Nullable<long> Assigned { get; set; }

        public Nullable<int> TypeId { get; set; }

        public string Type { get; set; }

        public Nullable<int> StageId { get; set; }

        public string Stage { get; set; }

        public Nullable<int> RatingId { get; set; }

        public string Rating { get; set; }

        public string Tags { get; set; }

        public Nullable<int> SourceId { get; set; }

        public string Source { get; set; }

        public Nullable<DateTime> CaptureDate { get; set; }

        public Nullable<bool> IsPreApproved { get; set; }

        public string Lender { get; set; }

        public Nullable<decimal> Amount { get; set; }

        public int CurrencyId { get; set; }

        public Nullable<int> LoanTypeId { get; set; }

        public string LoanType { get; set; }

        public Nullable<int> DownPmtTypeId { get; set; }

        public string DownPmtType { get; set; }

        public Nullable<decimal> DownPmt { get; set; }

        public Nullable<DateTime> BuyerTimeframe { get; set; }

        public bool IsReferral { get; set; } = false;

        public Nullable<decimal> AskingPrice { get; set; }

        public Nullable<DateTime> SellerTimeframe { get; set; }

        public Nullable<int> SellReasonId { get; set; }

        public string SellReason { get; set; }

        public string ReferredBy { get; set; }

        public string ReferralSplit { get; set; }

        public bool IsRentNow { get; set; } = false;

        public Nullable<decimal> CreditScore { get; set; }

        public Nullable<decimal> MaxPrice { get; set; }

        public Nullable<DateTime> RenterTimeframe { get; set; }

        public Nullable<DateTime> LeaseExpiry { get; set; }

        public string InterestedIn { get; set; }

        public Nullable<long> UserContactCompanyId { get; set; }

        public string ContactCompany { get; set; }

        public Nullable<DateTime> BirthDay { get; set; }

        public Nullable<int> MaritalStatusId { get; set; }

        public string MaritalStatus { get; set; }

        public string Spouse { get; set; }

        public Nullable<DateTime> Anniversary { get; set; }

        public string Children { get; set; }

        public string Interests { get; set; }

        public string Income { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; }

        public Nullable<DateTime> UpdatedOn { get; set; }

        public bool IsDeleted { get; set; }

        public Nullable<DateTime> DeletedOn { get; set; }

        public long ContactCompanyId { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CompanyPhone { get; set; }

        public string CompanyFax { get; set; }

        public string CompanyWebsite { get; set; }

        public int CompanyTypeId { get; set; }

        public string CompanyType { get; set; }

        public long TotalRecords { get; set; }

        public string ContactCreatedOn { get; set; }

        public string ContactUpdatedOn { get; set; }

        public string ContactPictureUrl { get; set; }

        public int random { get; set; } = 0;

        public string CountryName { get; set; }

        public string StateName { get; set; }

        public string CityName { get; set; }

        public string AssignedUser { get; set; }

        public bool IsAssignedContact { get; set; }

        public Nullable<int> CategoryId { get; set; }

        public string CategoryName { get; set; }

        public Boolean IsContactLinkedWithMember { get; set; }

        public string MemberName { get; set; }

        public long MembersId { get; set; }

        public string ContactSourceId { get; set; }

        public string socialSettings { get; set; }

        public bool IsRead { get; set; }

        public Nullable<DateTime> ReadOn { get; set; }

        public bool Archived { get; set; }

        public bool Junk { get; set; }

        public string SourceInfoCode { get; set; }

        public long SourceInfoId { get; set; }

        public Nullable<int> CountryCode { get; set; }
    }

    public class ContactToExportIntoCsvResult
    {
        public Nullable<DateTime> CreatedOn { get; set; }

        //public long ContactId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string State { get; set; }

        public string Zip { get; set; }

        //public Nullable<int> TypeId { get; set; }

        public string Type { get; set; }

        //public Nullable<int> StageId { get; set; }

        public string Stage { get; set; }

        //public Nullable<int> SourceId { get; set; }

        public string Source { get; set; }

        //public Nullable<int> RatingId { get; set; }

        public string Rating { get; set; }

        public Nullable<long> Assigned { get; set; }

        public Nullable<DateTime> LastUpdate { get; set; }

    }

    public class ContactFilter
    {
        public long UserId { get; set; }

        public string UserIds { get; set; }

        public string ContactEmail { get; set; }

        public string Query { get; set; }

        public int PageSize { get; set; }

        public int PageNum { get; set; }

        public bool IsAlphabetFilter { get; set; }

        public int SelectOptionId { get; set; }

        public string InfoCode { get; set; }

        public string FilterInfoCode { get; set; }

        public string FilterInfoCodeQuery { get; set; }

        public bool IsSort { get; set; }

        public string SortOrder { get; set; }

        public List<IFilter> Filter { get; set; }

        public List<long> ExportContactId { get; set; }

        public string OrderColumnName { get; set; }

        public long ContactId { get; set; }

        public string SourceId { get; set; }

        public string SourceInfoCode { get; set; }

        public long CommunityId { get; set; } = 0;

        public long CommunityTypeId { get; set; } = 0;

        public bool IsDefault { get; set; } = false;
        public Nullable<DateTime> CustomFilterFromDate { get; set; }
        public Nullable<DateTime> CustomFilterToDate { get; set; }
    }

    public class IFilter
    {
        public string FilterInfoCode { get; set; }

        public string FilterInfoCodeQuery { get; set; }

        public string[] Selection { get; set; }
    }

    public class ContactViewFilterResult
    {
        public int SelectOptionId { get; set; }

        public string Name { get; set; }

        public string InfoCode { get; set; }

        public int Count { get; set; }

        public string CustomValue { get; set; }
    }

    // public class UserContactList : CommonResponse
    public class UserContactList
    {
        public long ListId { get; set; }

        public string Name { get; set; }

        public string InfoCode { get; set; }

        public Nullable<int> SortOrder { get; set; }

        public long ContactId { get; set; }
    }

    // public class UserContactListItem : CommonResponse
    // {
    //     public long ListId { get; set; }

    //     public List<Contact> Contact { get; set; }
    // }

    // public class UpdateContactOption : CommonResponse
    // {
    //     public List<IUpdateContactOption> Item { get; set; }
    // }

    public class IUpdateContactOption
    {
        public long ContactId { get; set; }

        public int SelectOptionId { get; set; }
    }

    // public class UpdateAssignedUser : CommonResponse
    // {
    //     public long TableRecordId { get; set; }

    //     public Nullable<long> TeamId { get; set; }

    //     public Nullable<long> TeamMemberId { get; set; }

    //     public Nullable<long> UserId { get; set; }

    //     public string AssignUser { get; set; }

    //     public Nullable<int> action { get; set; }

    //     public bool IsTransfer { get; set; }

    //     public string Description { get; set; }

    //     public NotificationVM NotificationVM { get; set; }
    // }

    public class UserShareMemberResult
    {
        public long UserId { get; set; }

        public string Thumbnail { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string InfoCode { get; set; }

    }

    public class UserShareLookupResult
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public long OwnerId { get; set; }

        public string LogoPath { get; set; }

        public string InfoCode { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }
    }

    // public class ContactSummaryResult : CommonResponse
    // {
    //     public int TotalContactCreated { get; set; }

    //     public int NewContacts { get; set; }

    //     public int LeadContacts { get; set; }

    //     public int ClientContacts { get; set; }

    //     public int ContactsClosed { get; set; }

    //     public int ContactsLost { get; set; }

    // }

    // public class LeadCaptureForm : CommonResponse
    // {
    //     public long UserId { get; set; }

    //     public long ContactId { get; set; }

    //     public string SourceInfoCode { get; set; }

    //     public string FirstName { get; set; }

    //     public string LastName { get; set; }

    //     public string Phone { get; set; }

    //     public string Email { get; set; }

    //     public string Note { get; set; }

    //     public bool IsCapchaVerified { get; set; }

    //     public long MarketingPieceTemplateId { get; set; }

    //     public List<NotificationVM> NotificationVM { get; set; }
    // }

    // public class ListResult
    // {
    //     public long ListId { get; set; }

    //     public long TableRecordId { get; set; }

    //     public string InfoCode { get; set; }

    //     public string Name { get; set; }

    //     public long ListItemId { get; set; }
    // }

    // public class SocialInformation : CommonResponse
    // {
    //     public string Name { get; set; }

    //     public string Value { get; set; }

    //     public string DisplayIcon { get; set; }
    // }

    // public class UserSavedFilterVM : CommonResponse
    // {
    //     public long UserSavedFilterId { get; set; }

    //     public long UserId { get; set; }

    //     public string InfoCode { get; set; }

    //     public string Filter { get; set; }

    //     public bool IsPrimary { get; set; }

    //     public Nullable<DateTime> DeletedOn { get; set; }

    //     public bool IsDeleted { get; set; } = false;
    // }

    public class VerifyCall
    {
        public long ContactId { get; set; }
    }

}