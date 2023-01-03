using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using matcrm.data.Models.Dto;
using matcrm.data.Models.ViewModels.Calendar;

namespace matcrm.data.Models.ViewModels
{
    public class ClientContactVM : CommonResponse
    {
        public ClientContactVM()
        {
            value = new List<MicrosoftClientContactVM>();
            connections = new List<GoogleClientContactVM>();
        }
        public List<MicrosoftClientContactVM> value { get; set; }
        public List<GoogleClientContactVM> connections { get; set; }
    }

    public class MicrosoftClientContactVM
    {
        public MicrosoftClientContactVM()
        {
            emailAddresses = new List<MicrosoftClientContactEmailAddressesVM>();
            //homePhones = new List<MicrosoftClientContactPhoneVM>();
        }
        public string id { get; set; }
        public string createdDateTime { get; set; }
        public string lastModifiedDateTime { get; set; }
        public string changeKey { get; set; }
        public string parentFolderId { get; set; }
        public string birthday { get; set; }
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string initials { get; set; }
        public string middleName { get; set; }
        public string nickName { get; set; }
        public string surname { get; set; }
        public string title { get; set; }
        public string yomiGivenName { get; set; }
        public string yomiSurname { get; set; }
        public string yomiCompanyName { get; set; }
        public string generation { get; set; }
        //public string imAddresses { get; set; }
        public string jobTitle { get; set; }
        public string companyName { get; set; }
        public string department { get; set; }
        public string officeLocation { get; set; }
        public string profession { get; set; }
        public string businessHomePage { get; set; }
        public string assistantName { get; set; }
        public string manager { get; set; }
        //public List<MicrosoftClientContactPhoneVM> homePhones { get; set; }
        public string mobilePhone { get; set; }
        public string spouseName { get; set; }
        public string personalNotes { get; set; }
        public List<MicrosoftClientContactEmailAddressesVM> emailAddresses { get; set; }

        //public MicrosoftClientContactPersonTypeVM businessAddress { get; set; }
    }
    public class MicrosoftClientContactEmailAddressesVM
    {
        public string address { get; set; }
        //public string relevanceScore { get; set; }
        //public string selectionLikelihood { get; set; }
    }
    public class MicrosoftClientContactPhoneVM
    {
        public string phone { get; set; }
    }

    // public class MicrosoftClientContactPersonTypeVM
    // {
    //     public string subclass { get; set; }
    // }

    public class GoogleClientContactVM
    {
        public GoogleClientContactVM()
        {
            names = new List<GoogleClientContactNamesVM>();
            emailAddresses = new List<GoogleClientContactEmailAddressesVM>();
        }
        public string resourceName { get; set; }
        public string etag { get; set; }
        public List<GoogleClientContactEmailAddressesVM> emailAddresses { get; set; }
        public List<GoogleClientContactNamesVM> names { get; set; }
    }
    public class GoogleClientContactNamesVM
    {
        public string displayName { get; set; }
        public string givenName { get; set; }
        public string displayNameLastFirst { get; set; }
    }
    public class GoogleClientContactEmailAddressesVM
    {
        public GoogleClientContactEmailMetadataVM metadata { get; set; }
        public string value { get; set; }
        public string displayName { get; set; }
    }
    public class GoogleClientContactEmailMetadataVM
    {
        public bool primary { get; set; }
        public GoogleClientContactEmailMetadataSourceVM source { get; set; }
    }
    public class GoogleClientContactEmailMetadataSourceVM
    {
        public string type { get; set; }
        public string id { get; set; }
    }

    public class ClientCreateContactVM : CommonResponse
    {
        public string id { get; set; }
        public string displayName { get; set; }
        public string createdDateTime { get; set; }
        public string error_description { get; set; }
        public string resourceName { get; set; }
    }


}