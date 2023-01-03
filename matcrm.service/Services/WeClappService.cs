using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using matcrm.data.Models.ViewModels;
using matcrm.service.Utility;

namespace matcrm.service.Services
{
    public class WeClappService : IWeClappService
    {
        public async Task<List<CustomerVM>> GetCustomers(string apiKey, string tenant)
        {
            try
            {
                int pageNumber = 1;
                long pageSize = 100;
                var customerList = new List<CustomerVM>();

                bool hasCustomers = true;

                while (hasCustomers)
                {
                    var records = await ApiManager<CustomerVM>.GetAsync("customer?page=" + pageNumber + "&pageSize=" + pageSize, tenant, null, apiKey);
                    pageNumber++;
                    var data = JsonConvert.DeserializeObject<CustomerResult>(records).Result;

                    customerList.AddRange(data);
                    if (data.Count < pageSize) hasCustomers = false;
                }

                if (customerList.Count > 0)
                {
                    return customerList;
                }
                return null;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return null;
            }
        }

        public async Task<long> GetCustomerCount(string apiKey, string tenant)
        {
            try
            {
                var records = await ApiManager<CustomerCountResult>.GetAsync("customer/count", tenant, null, apiKey);

                var data = JsonConvert.DeserializeObject<CustomerCountResult>(records).Result;

                return data;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return 0;
            }
        }

        public async Task<List<Ticket>> GetTickets(string apiKey, string tenant)
        {

            int pageNumber = 1;
            long pageSize = 100;
            var ticketList = new List<Ticket>();

            bool hasTickets = true;

            while (hasTickets)
            {
                var records = await ApiManager<Ticket>.GetAsync("ticket?page=" + pageNumber + "&pageSize=" + pageSize, tenant, null, apiKey);
                pageNumber++;
                var data = JsonConvert.DeserializeObject<TicketResult>(records).Result;

                ticketList.AddRange(data);
                if (data.Count < pageSize) hasTickets = false;
            }

            if (ticketList.Count > 0)
            {
                return ticketList;
            }
            return null;
        }

        public async Task<List<TicketCategory>> GetTicketCategories(string apiKey, string tenant)
        {
            int pageNumber = 1;
            long pageSize = 100;
            var ticketCategoryList = await ApiManager<TicketCategory>.GetAsync("ticketCategory", tenant, null, apiKey);
            if (!string.IsNullOrEmpty(ticketCategoryList))
            {
                TicketCategoryResult result = JsonConvert.DeserializeObject<TicketCategoryResult>(ticketCategoryList);

                if (result == null || result.Result == null) { return null; }

                return result.Result;
            }
            return null;
        }

        public async Task<List<TicketType>> GetTicketTypes(string apiKey, string tenant)
        {
            var ticketTypeList = await ApiManager<Ticket>.GetAsync("ticketType", tenant, null, apiKey);
            if (!string.IsNullOrEmpty(ticketTypeList))
            {
                TicketTypeResult result = JsonConvert.DeserializeObject<TicketTypeResult>(ticketTypeList);

                if (result == null || result.Result == null) { return null; }

                return result.Result;
            }
            return null;
        }

        public async Task<List<TicketStatus>> GetTicketStatus(string apiKey, string tenant)
        {
            var ticketStatusList = await ApiManager<Ticket>.GetAsync("ticketStatus", tenant, null, apiKey);
            if (!string.IsNullOrEmpty(ticketStatusList))
            {
                TicketStatusResult result = JsonConvert.DeserializeObject<TicketStatusResult>(ticketStatusList);

                if (result == null || result.Result == null) { return null; }

                return result.Result;
            }
            return null;
        }

        public async Task<List<TimeRecord>> GetTimeRecords(string apiKey, string tenant)
        {
            var timeRecordList = await ApiManager<TimeRecord>.GetAsync("timeRecord?page=1&pageSize=5000", tenant, null, apiKey);
            if (!string.IsNullOrEmpty(timeRecordList))
            {
                TimeRecordResult result = JsonConvert.DeserializeObject<TimeRecordResult>(timeRecordList);

                if (result == null || result.Result == null) { return null; }

                return result.Result;
            }
            return null;
        }

        public async Task<Ticket> SaveTicket(string apiKey, string tenant)
        {
            var ticket = await ApiManager<Ticket>.PostAsync("ticket", tenant, null, apiKey);
            if (!string.IsNullOrEmpty(ticket))
            {
                Ticket result = JsonConvert.DeserializeObject<Ticket>(ticket);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

         public async Task<SalesInvoiceVM> SaveSalesInvoice(string apiKey, string tenant, SalesInvoiceVM model)
        {
            var ticket = await ApiManager<SalesInvoiceVM>.PostAsync("salesInvoice", tenant, model, apiKey);
            if (!string.IsNullOrEmpty(ticket))
            {
                SalesInvoiceVM result = JsonConvert.DeserializeObject<SalesInvoiceVM>(ticket);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<PostTimeRecord> AddJob(string apiKey, string tenant, PostTimeRecord record)
        {
            var response = await ApiManager<PostTimeRecord>.PostAsync("timeRecord", tenant, record, apiKey);
            if (!string.IsNullOrEmpty(response))
            {
                PostTimeRecord result = JsonConvert.DeserializeObject<PostTimeRecord>(response);
                // return true;
                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<bool> PostDocument(string apiKey, string tenant, string fileName, int id, byte[] doc)
        {
            try
            {
                var postUrl = "document/upload?entityName=ticket&entityId=" + id + "&name=" + fileName;
                var result = await ApiManager<string>.PostFileAsync(postUrl, tenant, null, apiKey, doc, "application/octet-stream");

                if (!string.IsNullOrEmpty(result)) return true;
                return false;
            }
            catch (Exception ex)
            {
                var error = ex.Message;
                return false;
            }
        }

        public async Task<Ticket> UpdateTicketStatus(string apiKey, string tenant, TicketVM ticket)
        {

            var response = await ApiManager<TicketVM>.PutAsync("ticket/id/" + ticket.id, tenant, ticket, apiKey);
            if (!string.IsNullOrEmpty(response))
            {
                Ticket result = JsonConvert.DeserializeObject<Ticket>(response);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<TimeRecord> UpdateTimeRecord(string apiKey, string tenant, TimeRecord timeRecord)
        {

            var response = await ApiManager<TimeRecord>.PutAsync("timeRecord/id/" + timeRecord.id, tenant, timeRecord, apiKey);
            if (!string.IsNullOrEmpty(response))
            {
                TimeRecord result = JsonConvert.DeserializeObject<TimeRecord>(response);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public async Task<dynamic> SaveCustomer(string apiKey, string tenant, dynamic model)
        {
            var customer = await ApiManager<dynamic>.PostAsync("customer", tenant, model, apiKey);
            if (!string.IsNullOrEmpty(customer))
            {
                dynamic result = JsonConvert.DeserializeObject<CustomerVM>(customer);

                if (result == null || result == null) { return null; }

                return result;
            }
            return null;
        }

        public static T Deserialize<T>(string json)
        {
            T obj = Activator.CreateInstance<T>();
            MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(json));
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            obj = (T)serializer.ReadObject(ms);
            ms.Close();
            return obj;
        }
    }

    public interface IWeClappService
    {
        Task<List<CustomerVM>> GetCustomers(string apiKey, string tenant);
        Task<long> GetCustomerCount(string apiKey, string tenant);
        Task<List<Ticket>> GetTickets(string apiKey, string tenant);
        Task<List<TicketCategory>> GetTicketCategories(string apiKey, string tenant);
        Task<List<TicketType>> GetTicketTypes(string apiKey, string tenant);
        Task<List<TicketStatus>> GetTicketStatus(string apiKey, string tenant);
        Task<List<TimeRecord>> GetTimeRecords(string apiKey, string tenant);
        Task<dynamic> SaveCustomer(string apiKey, string tenant, dynamic model);
        Task<Ticket> UpdateTicketStatus(string apiKey, string tenant, TicketVM ticket);
        Task<PostTimeRecord> AddJob(string apiKey, string tenant, PostTimeRecord record);
        Task<bool> PostDocument(string apiKey, string tenant, string fileName, int id, byte[] doc);
        Task<TimeRecord> UpdateTimeRecord(string apiKey, string tenant, TimeRecord timeRecord);
        Task<SalesInvoiceVM> SaveSalesInvoice(string apiKey, string tenant, SalesInvoiceVM model);
    }
}