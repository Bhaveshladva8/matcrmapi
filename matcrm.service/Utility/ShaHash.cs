using System;
using System.Security.Cryptography;
using System.Text;

namespace matcrm.service.Utility
{
    public class ShaHashData
    {
         public static String GetHash (string text) {
            using (var sha256 = SHA256.Create ()) {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash (Encoding.UTF8.GetBytes (text));
                // Get the hashed string.  
                return BitConverter.ToString (hashedBytes).Replace ("-", "").ToLower ();
            }
        }

          public static String GetStringHash (string text) {
            // SHA512 is disposable by inheritance. 
            string base64Decoded;
            byte[] data = System.Convert.FromBase64String (text);
            base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString (data);
            
            using (var sha256 = SHA256.Create ()) {
                // Send a sample text to hash.  
                var hashedBytes = sha256.ComputeHash (Encoding.UTF8.GetBytes (base64Decoded));
                // Get the hashed string.  
                return BitConverter.ToString (hashedBytes).Replace ("-", "").ToLower ();
            }
        }

        public static String DecodePassWord (string text) {
            // SHA512 is disposable by inheritance. 
            string base64Decoded;
            byte[] data = System.Convert.FromBase64String (text);
            base64Decoded = System.Text.ASCIIEncoding.ASCII.GetString (data);
            
            return base64Decoded;
        }


        // public string Encryptword(string Encryptval)  
        // {  
        //     byte[] SrctArray;  
        //      EnctArray ;
        //     EnctArray = UTF8Encoding.UTF8.GetBytes(Encryptval);  
        //     // SrctArray = UTF8Encoding.UTF8.GetBytes(key);  
        //     TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();  
        //      MD5CryptoServiceProvider objcrpt = new MD5CryptoServiceProvider();  
        //     SrctArray = objt.CreateEncryptor(UTF8Encoding.UTF8.GetBytes(EnctArray));  

        //     objcrpt.Clear();  
        //     objt.Key = SrctArray;  
        //     objt.Mode = CipherMode.ECB;  
        //     objt.Padding = PaddingMode.PKCS7;  
        //     ICryptoTransform crptotrns = objt.CreateEncryptor();  
        //     byte[] resArray = crptotrns.TransformFinalBlock(EnctArray, 0, EnctArray.Length);  
        //     objt.Clear();  
        //     return Convert.ToBase64String(resArray, 0, resArray.Length);  
        // }


        // public string Decryptword(string DecryptText)  
        // {  
        //     byte[] SrctArray;  
        //     byte[] DrctArray = Convert.FromBase64String(DecryptText);  
        //     SrctArray = UTF8Encoding.UTF8.GetBytes(key);  
        //     TripleDESCryptoServiceProvider objt = new TripleDESCryptoServiceProvider();  
        //     MD5CryptoServiceProvider objmdcript = new MD5CryptoServiceProvider();  
        //     SrctArray = objmdcript.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));  
        //     objmdcript.Clear();  
        //     objt.Key = SrctArray;  
        //     objt.Mode = CipherMode.ECB;  
        //     objt.Padding = PaddingMode.PKCS7;  
        //     ICryptoTransform crptotrns = objt.CreateDecryptor();  
        //     byte[] resArray = crptotrns.TransformFinalBlock(DrctArray, 0, DrctArray.Length);  
        //     objt.Clear();  
        //     return UTF8Encoding.UTF8.GetString(resArray);  
        // }    


        public static string EncodePasswordToBase64(string password)
        {
            try
            {
                byte[] encData_byte = new byte[password.Length];
                encData_byte = System.Text.Encoding.UTF8.GetBytes(password);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in base64Encode" + ex.Message);
            }
        }

        public static string DecodeFrom64(string encodedData)
        {
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            System.Text.Decoder utf8Decode = encoder.GetDecoder();
            byte[] todecode_byte = Convert.FromBase64String(encodedData);
            int charCount = utf8Decode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
            char[] decoded_char = new char[charCount];
            utf8Decode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
            string result = new String(decoded_char);
            return result;
        }

        public static String DecodeBase64String(string s)
        {
            var ts = s.Replace("-", "+");
            ts = ts.Replace("_", "/");
            var bc = Convert.FromBase64String(ts);
            var tts = Encoding.UTF8.GetString(bc);

            return tts;
        }
        
    }
}