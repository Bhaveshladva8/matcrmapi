using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Newtonsoft.Json;
using matcrm.data;
using matcrm.data.Models.Dto;
using matcrm.data.Models.Tables;
using matcrm.data.Models.ViewModels;
using matcrm.service.Utility;

namespace matcrm.service.Services
{
    public partial class UserService : Service<User>, IUserService
    {
        private IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork, IMapper mapper) : base(unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<User> AddUser(User user, string Password)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Email == user.Email && t.IsDeleted == false).Result.FirstOrDefault();
            if (alreadyUser == null)
            {
                user.CreatedOn = DateTime.UtcNow;
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(Password, out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                }

                var newItem = await _unitOfWork.UserRepository.AddAsync(user);
                await _unitOfWork.CommitAsync();
                return newItem;
            }
            else
            {
                var updatedItem = await UpdateUser(user, true);
                return updatedItem;
            }

        }

        public async Task<User> UpdatePassword(User user, string Password)
        {
            if (user != null)
            {
                if (!string.IsNullOrWhiteSpace(Password))
                {
                    byte[] passwordHash, passwordSalt;
                    CreatePasswordHash(Password, out passwordHash, out passwordSalt);

                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    var updatedItem = await UpdateUser(user, false);
                    return updatedItem;
                }
            }
            return null;
        }

        public async Task<User> UpdateRefreshToken(User user, string RefreshToken)
        {
            if (user != null)
            {
                if (!string.IsNullOrEmpty(RefreshToken))
                {
                    user.RefreshToken = RefreshToken;
                    await _unitOfWork.UserRepository.UpdateAsync(user, user.Id);
                    await _unitOfWork.CommitAsync();
                    return user;
                }
            }
            return null;
        }

        public async Task<User> ExternalUser(UserDto userDto)
        {
            if (userDto != null)
            {
                var userObj = _mapper.Map<User>(userDto);
                var existingItem = new User();
                existingItem = _unitOfWork.UserRepository.GetMany(t => t.Email == userObj.Email && t.IsDeleted == false).Result.FirstOrDefault();

                if (existingItem == null)
                {
                    userObj.CreatedOn = DateTime.UtcNow;
                    userObj.LastLoggedIn = DateTime.UtcNow;
                    userObj.PasswordHash = Array.Empty<byte>();
                    userObj.PasswordSalt = Array.Empty<byte>();
                    var newItem = await _unitOfWork.UserRepository.AddAsync(userObj);
                    await _unitOfWork.CommitAsync();
                    return newItem;
                }
                else
                {

                    existingItem.UpdatedBy = userObj.Id;
                    existingItem.LastLoggedIn = DateTime.UtcNow;
                    // existingItem.TenantId = obj.TenantId;

                    await _unitOfWork.UserRepository.UpdateAsync(existingItem, existingItem.Id);
                    await _unitOfWork.CommitAsync();
                    return existingItem;
                }
            }
            return null;
        }
        public User Block(int userId)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Id == userId && t.IsDeleted == false).Result.FirstOrDefault();

            if (alreadyUser == null)
                return null;

            alreadyUser.IsBlocked = true;
            alreadyUser.BlockedOn = DateTime.UtcNow;
            _unitOfWork.CommitAsync();
            return alreadyUser;
        }

        public User Unblock(int userId)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Id == userId && t.IsDeleted == false).Result.FirstOrDefault();

            if (alreadyUser == null)
                return null;

            alreadyUser.IsBlocked = false;
            alreadyUser.BlockedOn = null;
            _unitOfWork.CommitAsync();
            return alreadyUser;
        }

        public List<User> RevokeAllBlocked()
        {
            var userList = _unitOfWork.UserRepository.GetMany(t => t.IsBlocked == true).Result.ToList();
            if (userList != null && userList.Count() > 0)
            {
                foreach (var user in userList)
                {
                    user.IsBlocked = false;
                    user.BlockedOn = null;
                }
                _unitOfWork.CommitAsync();
            }
            return userList;
        }

        public List<User> GetAll(int? tenantId)
        {

            if (tenantId == null)
                return _unitOfWork.UserRepository.GetMany(t => t.IsBlocked == false && t.IsDeleted == false).Result.ToList();
            return _unitOfWork.UserRepository.GetMany(u => u.TenantId == tenantId.Value && u.IsBlocked == false && u.IsDeleted == false).Result.ToList();
        }

        public List<User> GetAllTeamMate(int userId)
        {
            return _unitOfWork.UserRepository.GetMany(u => (u.CreatedBy == userId || u.Id == userId) && u.IsBlocked == false && u.IsDeleted == false).Result.ToList();
        }

        public User UpdateLastLogin(int userId)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Id == userId && t.IsDeleted == false).Result.FirstOrDefault();

            if (alreadyUser == null)
                return null;

            alreadyUser.LastLoggedIn = DateTime.UtcNow;
            _unitOfWork.UserRepository.UpdateAsync(alreadyUser, alreadyUser.Id);
            _unitOfWork.CommitAsync();
            return alreadyUser;
        }

        public async Task<User> UpdateUser(User user, bool IsUpdateLastLogin)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Id == user.Id && t.IsDeleted == false).Result.FirstOrDefault();

            if (alreadyUser == null)
                return null;
            if (IsUpdateLastLogin)
            {
                alreadyUser.LastLoggedIn = DateTime.UtcNow;
                user.LastLoggedIn = DateTime.UtcNow;

            }
            else
            {
                alreadyUser.FirstName = user.FirstName;
                alreadyUser.LastName = user.LastName;
                alreadyUser.UserName = user.UserName;
                alreadyUser.PhoneNo = user.PhoneNo;
                alreadyUser.DialCode = user.DialCode;
                alreadyUser.RoleId = user.RoleId;
                alreadyUser.Address = user.Address;
                alreadyUser.TenantId = user.TenantId;
            }
            alreadyUser.CreatedBy = alreadyUser.CreatedBy;
            await _unitOfWork.UserRepository.UpdateAsync(alreadyUser, alreadyUser.Id);
            await _unitOfWork.CommitAsync();
            return user;
        }

        public bool IsBlocked(int userId)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(u => u.Id == userId && u.IsDeleted == false).Result.FirstOrDefault();
            if (alreadyUser == null)
                return true;

            if (alreadyUser.IsBlocked)
                return true;
            else
                return false;
        }

        public User DeleteUser(int userId)
        {
            var userObj = _unitOfWork.UserRepository.GetMany(u => u.Id == userId && u.IsDeleted == false).Result.FirstOrDefault();
            if (userObj == null)
            {
                return null;
            }
            else
            {
                userObj.IsDeleted = true;
                userObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.UserRepository.UpdateAsync(userObj, userObj.Id);
                _unitOfWork.CommitAsync();
                return userObj;
            }
        }
        public User DeleteUserModel(User user)
        {
            var userObj = _unitOfWork.UserRepository.GetMany(u => u.Id == user.Id && u.IsDeleted == false).Result.FirstOrDefault();
            if (userObj == null)
            {
                return null;
            }
            else
            {
                userObj.IsDeleted = true;
                userObj.DeletedBy = user.DeletedBy;
                userObj.DeletedOn = DateTime.UtcNow;
                _unitOfWork.UserRepository.UpdateAsync(userObj, userObj.Id);
                _unitOfWork.CommitAsync();
                return userObj;
            }
        }

        public async Task<WeClappUserVM> GetWeClappUser(string apiKey, string tenant)
        {
            try
            {
                var user = await ApiManager<WeClappUserVM>.GetAsync("user/currentUser", tenant, null, apiKey);
                if (!string.IsNullOrEmpty(user))
                {
                    WeClappUserResult response = JsonConvert.DeserializeObject<WeClappUserResult>(user);

                    if (response != null)
                        return response.result;
                }
                return null;
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");

            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private static bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
        {
            if (password == null) throw new ArgumentNullException("password");
            if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Value cannot be empty or whitespace only string.", "password");
            if (storedHash.Length != 64) throw new ArgumentException("Invalid length of password hash (64 bytes expected).", "passwordHash");
            if (storedSalt.Length != 128) throw new ArgumentException("Invalid length of password salt (128 bytes expected).", "passwordHash");

            using (var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != storedHash[i]) return false;
                }
            }
            return true;
        }

        public User Authenticate(string email, string password, int? tenantId)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                return null;

            var user = _unitOfWork.UserRepository.GetMany(x => x.Email == email && x.IsDeleted == false && x.IsBlocked == false).Result.FirstOrDefault();
            // var user = _unitOfWork.UserRepository.GetMany (x => x.Email == email && x.IsEmailVerified == true && x.IsDeleted == false && x.IsBlocked == false).Result.FirstOrDefault ();
            // var user = _unitOfWork.UserRepository.GetMany (x => x.Email == email && x.TenantId == tenantId && x.IsEmailVerified == true && x.IsDeleted == false && x.IsBlocked == false).Result.FirstOrDefault ();

            // check if username exists
            if (user == null)
                return null;

            // check if password is correct
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
                return null;

            // authentication successful
            return user;
        }

        public User GetUserById(int userId)
        {
            var user = _unitOfWork.UserRepository.GetMany(t => t.Id == userId && t.IsBlocked == false && t.IsDeleted == false).Result.FirstOrDefault();
            return user;
        }

        public User GetUserByWeclappUserId(int weclappUserId)
        {
            var user = _unitOfWork.UserRepository.GetMany(t => t.WeClappUserId == weclappUserId && t.IsBlocked == false && t.IsEmailVerified == true && t.IsDeleted == false).Result.FirstOrDefault();
            return user;
        }

        public User GetUser(User user)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Email.ToLower() == user.Email.ToLower() && t.IsBlocked == false && t.TenantId == user.TenantId && t.Id == user.Id && t.IsEmailVerified == true && t.IsDeleted == false).Result.FirstOrDefault();
            return alreadyUser;
        }

        public User GetUserByEmail(string email)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Email.ToLower() == email.ToLower() && t.IsBlocked == false && t.IsDeleted == false).Result.FirstOrDefault();
            return alreadyUser;
        }

        public User GetUserByEmailHash(string email)
        {
            return _unitOfWork.UserRepository.GetMany(t => ShaHashData.GetHash(t.Email).ToLower() == email.ToLower() && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public User GetUserByEmailForVerify(string email)
        {
            var alreadyUser = _unitOfWork.UserRepository.GetMany(t => t.Email.ToLower() == email.ToLower() && t.IsBlocked == false && t.IsDeleted == false).Result.FirstOrDefault();
            return alreadyUser;
        }

        public User GetUserByTempGuid(string guid)
        {
            return _unitOfWork.UserRepository.GetMany(t => t.TempGuid == guid && t.IsDeleted == false && t.IsBlocked == false).Result.FirstOrDefault();
        }

        public List<User> GetAll()
        {
            return _unitOfWork.UserRepository.GetMany(t => t.IsDeleted == false).Result.ToList();
        }

        public List<User> GetAllUsersByTenantAdmin(int tenantId)
        {
            return _unitOfWork.UserRepository.GetMany(t => t.TenantId == tenantId && t.IsBlocked == false && t.IsDeleted == false).Result.ToList();
        }

        public User VerifyUser(int userId)
        {
            var existingItem = _unitOfWork.UserRepository.GetMany(t => t.Id == userId && t.IsDeleted == false).Result.FirstOrDefault();
            if (existingItem == null)
            {
                return null;
            }
            else
            {
                existingItem.IsEmailVerified = true;
                existingItem.VerifiedOn = DateTime.UtcNow;
                var item = _unitOfWork.UserRepository.UpdateAsync(existingItem, existingItem.Id);
                _unitOfWork.CommitAsync();
                return existingItem;
            }
        }

        public User GetUserByUserIdAndTempGuid(int UserId, string TempGuid)
        {
            return _unitOfWork.UserRepository.GetMany(t => t.Id == UserId && t.TempGuid == TempGuid && t.IsDeleted == false && t.IsEmailVerified == true).Result.FirstOrDefault();
        }

        public User GetUserByTempGuid(int UserId, string TempGuid)
        {
            return _unitOfWork.UserRepository.GetMany(t => t.Id == UserId && t.TempGuid == TempGuid && t.IsDeleted == false).Result.FirstOrDefault();
        }

        public User GetAdminUser(int adminRoleId)
        {
            return _unitOfWork.UserRepository.GetMany(t => t.RoleId == adminRoleId && t.IsDeleted == false && t.IsBlocked == false).Result.FirstOrDefault();
        }

    }

    public partial interface IUserService : IService<User>
    {
        Task<User> AddUser(User user, string Password);
        User Block(int userId);
        User Unblock(int userId);
        List<User> RevokeAllBlocked();
        List<User> GetAll(int? tenantId);
        User UpdateLastLogin(int userId);
        Task<User> UpdateUser(User user, bool IsUpdateLastLogin);
        bool IsBlocked(int userId);
        User DeleteUser(int userId);
        User DeleteUserModel(User user);
        Task<WeClappUserVM> GetWeClappUser(string apiKey, string tenant);
        User Authenticate(string email, string password, int? tenantId);
        User GetUserById(int userId);
        User GetUserByWeclappUserId(int weclappUserId);
        User GetUser(User user);
        User GetUserByEmail(string email);
        User GetUserByEmailForVerify(string email);
        User GetUserByEmailHash(string email);
        List<User> GetAll();
        User VerifyUser(int userId);
        User GetUserByTempGuid(string guid);
        User GetUserByUserIdAndTempGuid(int UserId, string TempGuid);
        Task<User> UpdatePassword(User user, string Password);
        List<User> GetAllUsersByTenantAdmin(int tenantId);
        User GetAdminUser(int adminRoleId);
        User GetUserByTempGuid(int UserId, string TempGuid);

        Task<User> ExternalUser(UserDto userDto);

        List<User> GetAllTeamMate(int userId);
        Task<User> UpdateRefreshToken(User user, string RefreshToken);
    }
}