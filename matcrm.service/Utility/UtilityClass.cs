using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace matcrm.service.Utility
{
    public class UtilityClass
    {
        #region Initialization
        private const string _key = "3nc7p1D3cr7p1";
        #endregion

        #region Helper Methods
        /// <summary>
        /// Used to encrypt the password
        /// </summary>
        /// <param name="str">Plain password string</param>
        /// <returns></returns>
        public static string Encrypt(string str)
        {
            string EncrptKey = _key;
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(EncrptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(str);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }

        /// <summary>
        /// Used to decrypt the password
        /// </summary>
        /// <param name="str">Encypted password string</param>
        /// <returns></returns>
        public static string Decrypt(string str)
        {
            str = str.Replace(" ", "+");
            string DecryptKey = _key;
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[str.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(DecryptKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(str.Replace(" ", "+"));
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        /// <summary>
        /// Function that creates an object from the given data row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <returns></returns>
        public static T CreateItemFromRow<T>(DataRow row) where T : new()
        {
            // create a new object
            T item = new T();

            // set the item
            SetItemFromRow(item, row);

            // return 
            return item;
        }

        /// <summary>
        /// Function that set item from the given row
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="row"></param>
        public static void SetItemFromRow<T>(T item, DataRow row) where T : new()
        {
            // go through each column
            foreach (DataColumn c in row.Table.Columns)
            {
                // find the property for the column
                PropertyInfo p = item.GetType().GetProperty(c.ColumnName);

                // if exists, set the value
                if (p != null && row[c] != DBNull.Value)
                {
                    p.SetValue(item, row[c], null);
                }
            }
        }

        /// <summary>
        /// function that creates a list of an object from the given data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tbl"></param>
        /// <returns></returns>
        public static List<T> CreateListFromTable<T>(DataTable tbl) where T : new()
        {
            // define return list
            List<T> lst = new List<T>();

            // go through each row
            foreach (DataRow r in tbl.Rows)
            {
                // add to the list
                lst.Add(CreateItemFromRow<T>(r));
            }

            // return the list
            return lst;
        }

        /// <summary>
        /// Used to generate random password
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static string GenerateRandomPassword(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 8,
                RequiredUniqueChars = 4,
                RequireDigit = true,
                RequireLowercase = true,
                RequireNonAlphanumeric = true,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "abcdefghijkmnopqrstuvwxyz",    // lowercase
                "0123456789",                   // digits
                "!@$_-"                        // non-alphanumeric
            };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireLowercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[2][rand.Next(0, randomChars[2].Length)]);

            if (opts.RequireNonAlphanumeric)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[3][rand.Next(0, randomChars[3].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        /// <summary>
        /// This function used to generate 8 digit Entity code.
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static string GenerateEntityCode(string preFix, PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 6,
                RequiredUniqueChars = 6,
                RequireDigit = false,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
            };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return preFix + new string(chars.ToArray());
        }

        /// <summary>
        /// This function used to generate verification code.
        /// Contains only Uppercase and Digits.
        /// </summary>
        /// <param name="opts"></param>
        /// <returns></returns>
        public static string VerificationCode(PasswordOptions opts = null)
        {
            if (opts == null) opts = new PasswordOptions()
            {
                RequiredLength = 6,
                RequiredUniqueChars = 3,
                RequireDigit = true,
                RequireLowercase = false,
                RequireNonAlphanumeric = false,
                RequireUppercase = true
            };

            string[] randomChars = new[] {
                "ABCDEFGHJKLMNOPQRSTUVWXYZ",    // uppercase 
                "0123456789"                    // digits
            };

            Random rand = new Random(Environment.TickCount);
            List<char> chars = new List<char>();

            if (opts.RequireUppercase)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[0][rand.Next(0, randomChars[0].Length)]);

            if (opts.RequireDigit)
                chars.Insert(rand.Next(0, chars.Count),
                    randomChars[1][rand.Next(0, randomChars[1].Length)]);

            for (int i = chars.Count; i < opts.RequiredLength || chars.Distinct().Count() < opts.RequiredUniqueChars; i++)
            {
                string rcs = randomChars[rand.Next(0, randomChars.Length)];
                chars.Insert(rand.Next(0, chars.Count),
                    rcs[rand.Next(0, rcs.Length)]);
            }

            return new string(chars.ToArray());
        }

        /// <summary>
        /// This function copy source object property value to destination object property value
        /// This function copy value for only properties having same name
        /// </summary>
        /// <param name="sourceClassObject">Object of source class</param>
        /// <param name="destinationClassObject">Object of destination class</param>
        public static void CopyObject(object sourceClassObject, object destinationClassObject)
        {
            foreach (PropertyInfo property in sourceClassObject.GetType().GetProperties())
            {
                if (!property.CanRead || (property.GetIndexParameters().Length > 0))
                    continue;

                PropertyInfo other = destinationClassObject.GetType().GetProperty(property.Name);
                if ((other != null) && (other.CanWrite))
                    other.SetValue(destinationClassObject, property.GetValue(sourceClassObject, null), null);
            }
        }

        /// <summary>
        /// This function is used to get subscription expiry date based on time period passed in parameter
        /// </summary>
        /// <param name="lastSubscriptionDate"></param>
        /// <returns></returns>
        public static DateTime GetSubscriptionExpiryDate(DateTime lastSubscriptionDate)
        {   
            var newDt = lastSubscriptionDate.AddMonths(12).ToString("d");
            return Convert.ToDateTime(newDt);
        }

        /// <summary>
        /// This function is used to get verification code expiry date
        /// </summary>
        /// <returns></returns>
        public static DateTime VerificationCodeExpiredOn()
        {
            var dt = UtilityClass.GetCurrentDateTime();
            var newDt = dt.AddHours(24);

            return Convert.ToDateTime(newDt);
        }

        public static string GetCurrentTimeStamp()
        {
            return Convert.ToString(new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds());
        }
        #endregion

        #region Format Methods
        /// <summary>
        /// This function convert name into Title case letter
        /// </summary>
        /// <param name="str">Name</param>
        /// <returns></returns>
        public static string ToTitleCase(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
                return textInfo.ToTitleCase(str.ToLower().Trim());
            }

            return "";
        }

        /// <summary>
        /// This function used to provide current date for the system.
        /// This is mostly used for CreatedOn field.
        /// </summary>
        /// <returns></returns>
        public static DateTime GetCurrentDateTime()
        {
            return DateTime.UtcNow;
        }

        public static string GetDatePartFromDate(DateTime dt)
        {
            var date = dt.ToString("d");
            return Convert.ToString(date);
        }

        public static string GetTimePartFromDate(DateTime dt)
        {
            var time = dt.ToString("h:mm tt");
            return Convert.ToString(time);
        }
        #endregion

        #region Validation Methods
        /// <summary>
        /// This function used to validate email 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            if (String.IsNullOrEmpty(email))
                return false;

            try
            {
                MailAddress m = new MailAddress(email);

                try
                {
                    return Regex.IsMatch(email,
                          @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                          @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                          RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
                }
                catch (RegexMatchTimeoutException)
                {
                    return false;
                }
            }
            catch (FormatException)
            {
                return false;
            }
        }

        /// <summary>
        /// This function is used to validate password.
        /// Length should be between 6 to 15 charactor.
        /// At least one uppercase letter.
        /// At least one lowercase letter.
        /// </summary>
        /// <param name="password">password</param>
        /// <returns></returns>
        public static bool ValidatePassword(string password, string confirmPassword)
        {
            const int MinLength = 6;
            const int MaxLength = 15;

            if (password != confirmPassword) return false;

            bool meetsLengthRequirements = password.Length >= MinLength && password.Length <= MaxLength;
            bool hasUpperCaseLetter = false;
            bool hasLowerCaseLetter = false;
            bool hasDecimalDigit = false;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    else if (char.IsDigit(c)) hasDecimalDigit = true;
                }
            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit
                        ;
            return isValid;
        }
        #endregion
    }

    public enum FlowInterestType
    {
        Hourly = 1,
        Daily = 2,
        Weekly = 3
    }
}
