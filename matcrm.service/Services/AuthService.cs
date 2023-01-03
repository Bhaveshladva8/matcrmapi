using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using matcrm.data.Context;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Utility;

namespace matcrm.service.Services {
    public class AuthService : IAuthService {
        private readonly OneClappContext _context;
        public AuthService (OneClappContext context) {
            _context = context;
        }

        public async Task<UserVM> Login (UserVM objUser) {
            try {
                var response = await ApiManager<UserVM>.GetAsync ("user/currentUser", objUser.Tenant, objUser, objUser.ApiKey);

                if (!string.IsNullOrEmpty (response)) {
                    var userVMResult = JsonConvert.DeserializeObject<UserVMResult> (response);

                    if (userVMResult.Result != null && userVMResult.Result.Username == objUser.Username) {
                        userVMResult.Result.IsAdmin = false;
                        return userVMResult.Result;
                    }
                } else {
                    //check for admin login
                    var adminUser = _context.Tenants
                        .Where (t => t.Username == objUser.Username &&
                            t.TenantName == objUser.Tenant &&
                            t.Token == objUser.ApiKey)
                        .FirstOrDefault ();

                    if (adminUser != null)
                        return new UserVM () {
                            // IsAdmin = true,
                            IsAdmin = adminUser.IsAdmin,
                            Tenant = adminUser.TenantName,
                            ApiKey = "",
                            Username = adminUser.Username,
                            Id = adminUser.TenantId
                        };
                }

                return null;
            } catch (Exception ex) {
                var error = ex.Message;
                return null;
            }
        }
    }

    public interface IAuthService {
        Task<UserVM> Login (UserVM user);
    }
}