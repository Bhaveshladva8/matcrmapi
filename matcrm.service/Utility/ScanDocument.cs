using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using matcrm.service.Common;
using Microsoft.AspNetCore.Http;
using nClam;
using matcrm.data.Context;

namespace matcrm.service.Utility
{
    public class ScanDocument
    {
        public async Task<bool> ScanDocumentWithClam(IFormFile file)
        {            
            bool status = false;
            var ms = new MemoryStream();
            file.OpenReadStream().CopyTo(ms);
            byte[] fileBytes = ms.ToArray();
            try
            {
                // var clam = new ClamClient(this._configuration[OneClappContext.ClamAVServerURL],
                //                         Convert.ToInt32(this._configuration[OneClappContext.ClamAVServerPort]));

                var clam = new ClamClient(System.Net.IPAddress.Parse(OneClappContext.ClamAVServerURL), Convert.ToInt32(OneClappContext.ClamAVServerPort));
                var scanResult = await clam.SendAndScanFileAsync(fileBytes);
                switch (scanResult.Result)
                {
                    case ClamScanResults.Clean:
                        //statusMsg = "The file is clean! ScanResult:{1}" + scanResult.RawResult;
                        status = false;
                        break;
                    case ClamScanResults.VirusDetected:
                        //statusMsg = "Virus Found! Virus name: {1}" + scanResult.InfectedFiles.FirstOrDefault().VirusName;
                        status = true;
                        break;
                    case ClamScanResults.Error:
                        //statusMsg = "An error occured while scaning the file! ScanResult: {1}" + scanResult.RawResult;
                        status = false;
                        break;
                    case ClamScanResults.Unknown:
                        //statusMsg = "Unknown scan result while scaning the file! ScanResult: {0}" + scanResult.RawResult;
                        status = false;
                        break;
                }
            }
            catch (Exception ex)
            {                
                //statusMsg = "ClamAV Scan Exception: {0}" + ex.ToString();
                ex.ToString();
            }            
            //statusMsg = new KeyValuePair<string, string>("ClamAV scan completed for file {0", file.FileName);                      
            return status;
        }
    }
}