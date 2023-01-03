using System;
using matcrm.data.Helpers;
using matcrm.data.Models.Dto;
using Microsoft.AspNetCore.Hosting;
using matcrm.service.Services;
using System.Net;

namespace matcrm.service.BusinessLogic
{
    public class FileUpload
    {

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly ICustomerAttachmentService _customerAttachmentService;

        public FileUpload(IHostingEnvironment hostingEnvironment,
        ICustomerAttachmentService customerAttachmentService)
        {
            _hostingEnvironment = hostingEnvironment;
            _customerAttachmentService = customerAttachmentService;
        }
        public Byte[] DownloadFile(long fileId, bool isAzureFile, string container, ref FileDto model)
        {
            Byte[] imageByte = new Byte[0];
            var dPath = _hostingEnvironment.WebRootPath + "\\Uploads\\resized";
            var fPath = dPath + "\\" + "file-not-found.jpg";
            var file = _customerAttachmentService.GetCustomerAttachmentById(fileId);
            var fileDto = new FileDto();
            if (file == null)
            {
                // file = new FileDto();
                fileDto.FileType = "image/jpeg";
                fileDto.Extention = "jpg";
                fileDto.FileName = "File-Not-Found";
                return System.IO.File.ReadAllBytes(fPath);
            }

            // If azure file enabled then get all the file other that image from azure. We are still fetching resized images from our server
            var rootPath = (isAzureFile && !DataUtility.ImageExtention.Contains(fileDto.Extention) ? DataUtility.AzureFileUrl + container : _hostingEnvironment.WebRootPath) + "/FileStorage/" + file.CreatedBy;
            // var rootPath = (isAzureFile ? SystemSetting["AZFU"] + container : _hostingEnvironment.WebRootPath) + "/FileStorage/" + file.CreatedBy;

            var pathName = "";
            switch (fileDto.InfoCode)
            {
                case DataUtility.ListContact:
                    pathName = "contact";
                    break;

                case DataUtility.ListProperty:
                    pathName = "property";
                    break;

                case DataUtility.MarketingPieceTemplate:
                    pathName = "marketing";
                    break;

                case DataUtility.MarketingPieceFlyer:
                    pathName = "marketing";
                    break;

                case DataUtility.MarketingPiecePostCard:
                    pathName = "marketing";
                    break;

                case DataUtility.MarketingPieceDigitalAdd:
                    pathName = "marketing";
                    break;

                case DataUtility.ListWebPage:
                    pathName = "webpages";
                    break;

                default:
                    pathName = "default";
                    break;
            }

            var dirPath = rootPath + "/" + pathName;
            model = fileDto;
            var filePath = dirPath + "\\" + fileDto.FilePath;

            if (!System.IO.File.Exists(filePath) && !isAzureFile)
            {
                imageByte = System.IO.File.ReadAllBytes(fPath);
                return imageByte;
            }

            if (isAzureFile) using (var webClient = new WebClient()) imageByte = webClient.DownloadData(filePath);
            else imageByte = System.IO.File.ReadAllBytes(filePath);

            return imageByte;
        }

    }
}