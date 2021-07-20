using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NaturalPersonsDirectory.Common
{
    public static class Validator
    {
        public static bool IsValidDate(string value)
        {
            return DateTime.TryParse(value, out _);
        }

        public static bool IsValidImage(IFormFile file)
        {
            if (file == null)
            {
                return false;
            }

            var fileExtension = file.FileName.Split('.').Last();

            return AllowedFileFormats.Any(format => fileExtension == format);
        }

        public static bool IsValidContactInformation(string contactInformation)
        {
            if (string.IsNullOrEmpty(contactInformation))
            {
                return false;
            }

            const string phoneNumberPattern = @"^(\+995)[\s-]?[5]\d{1,3}[\s-]?\d{1,3}[\s-]?\d{1,3}[\s-]?\d{1,3}$";
            const string emailPattern = @"^([\w.-]+)@([\w-]+)((.(\w){2,3})+)$";

            var information = contactInformation.Split(',');

            foreach (var item in information)
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

        public static bool IsValidOrder(string value)
        {
            return AllowedOrders.Any(order => value == order);
        }

        private static readonly List<string> AllowedFileFormats = new List<string>()
        {
            "jpg",
            "jpeg",
            "png",
            "tiff",
            "gif"
        };

        private static readonly List<string> AllowedOrders = new List<string>()
        {
            "NaturalPersonId",
            "FirstNameEn",
            "LastNameEn",
            "PassportNumber",
            "Birthday"
        };
    }
}
