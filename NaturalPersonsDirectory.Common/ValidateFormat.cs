using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NaturalPersonsDirectory.Common
{
    public static class ValidateFormat
    {
        public static bool ValidDate(string value)
        {
            return DateTime.TryParse(value, out DateTime dateTime);
        }

        public static bool ValidImage(IFormFile file)
        {
            if(file != null)
            {
                var fileExtension = file.FileName.Split('.').Last();

                foreach (var format in AllowedFileFormats)
                {
                    if (fileExtension == format)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public static bool ValidContactInformations(string contactInformations)
        {
            if(contactInformations != null)
            {
                string phoneNumberPattern = @"^(\+995)[\s-]?[5]\d{1,3}[\s-]?\d{1,3}[\s-]?\d{1,3}[\s-]?\d{1,3}$";
                string emailPattern = @"^([\w.-]+)@([\w-]+)((.(\w){2,3})+)$";

                var informations = contactInformations.Split(',');

                foreach (var item in informations)
                {
                    var trimmedItem = item.Trim();

                    if (!Regex.Match(trimmedItem, phoneNumberPattern, RegexOptions.IgnoreCase).Success &&
                       !Regex.Match(trimmedItem, emailPattern, RegexOptions.IgnoreCase).Success)
                    {
                        return false;
                    }
                }

                return true;
            }
            return false;
        }

        public static bool ValidOrder(string value)
        {
            foreach (var order in AllowedOrders)
            {
                if (value == order)
                {
                    return true;
                }
            }

            return false;
        }

        private static List<string> AllowedFileFormats = new List<string>()
        {
            "jpg",
            "jpeg",
            "png",
            "tiff",
            "gif"
        };
        private static List<string> AllowedOrders = new List<string>()
        {
            "NaturalPersonId",
            "FirstNameEn",
            "LastNameEn",
            "PassportNumber",
            "Birthday"
        };
    }
}
