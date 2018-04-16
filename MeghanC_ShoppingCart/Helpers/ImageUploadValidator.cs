using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace MeghanC_ShoppingCart.Helpers
{
    public class ImageUploadValidator
    {
        public static bool IsWebFriendlyImage(HttpPostedFileBase file)
        {
            if (file == null)
                return false;

            if (file.ContentLength > 2 * 1024 * 1024 || file.ContentLength < 1024)
                return false;

            string fileExt = Path.GetExtension(file.FileName).ToLower();
            if (fileExt == ".jpg" || fileExt == ".png" || fileExt == ".gif" || fileExt == ".bmp")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}