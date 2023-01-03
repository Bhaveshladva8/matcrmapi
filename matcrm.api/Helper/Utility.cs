using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace matcrm.api.Helper {
    public static class Utility {

        public static int AdminRoleId;

         /// <summary>
        ///  This function is used to resize the image
        /// </summary>
        /// <param name="Hsize">Height</param>
        /// <param name="Wsize">Width</param>
        /// <param name="filePath">File Path</param>
        /// <param name="saveFilePath">Save Path</param>
        public static void ResizeImage(int Hsize, int Wsize, string filePath, string saveFilePath)
        {
            //variables for image dimension/scale
            double newHeight = 0;
            double newWidth = 0;
            double scale = 0;

            //create new image object
            Bitmap curImage = new Bitmap(filePath);

            //Determine image scaling
            //if (curImage.Height > curImage.Width)
            //{
            //    scale = Convert.ToSingle(Hsize) / curImage.Height;
            //}
            //else
            //{
            //    scale = Convert.ToSingle(Wsize) / curImage.Width;
            //}
            //if (scale < 0 || scale > 1) { scale = 1; }

            if (curImage.Width > curImage.Height)
            {
                newWidth = Wsize;
                newHeight = Convert.ToInt32(curImage.Height * Wsize / (double)curImage.Width);
            }
            else
            {
                newWidth = Convert.ToInt32(curImage.Width * Wsize / (double)curImage.Height);
                newHeight = Wsize;
            }

            //New image dimension
            //newHeight = Math.Floor(Convert.ToSingle(curImage.Height) * scale);
            //newWidth = Math.Floor(Convert.ToSingle(curImage.Width) * scale);

            //Create new object image
            Bitmap newImage = new Bitmap(curImage, Convert.ToInt32(newWidth), Convert.ToInt32(newHeight));
            newImage = new Bitmap(Convert.ToInt32(newWidth), Convert.ToInt32(newHeight), PixelFormat.Format32bppArgb);
            Graphics imgDest = Graphics.FromImage(newImage);
            imgDest.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            imgDest.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            imgDest.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            imgDest.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            imgDest.Clear(Color.Black);
            ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
            EncoderParameters param = new EncoderParameters(1);
            param.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);

            //Draw the object image
            imgDest.DrawImage(curImage, 0, 0, newImage.Width, newImage.Height);

            //Save image file
            newImage.Save(saveFilePath, info[1], param);

            //Dispose the image objects
            curImage.Dispose();
            newImage.Dispose();
            imgDest.Dispose();
        }

        //ClaimsPrincipal user = this.User as ClaimsPrincipal;
        //AdminRoleId = Convert.ToInt32(user.FindFirst(JwtRegisteredClaimNames.Sid).Value);
    }

}