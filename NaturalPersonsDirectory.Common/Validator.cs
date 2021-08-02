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

            return AllowedImageExtensions.Any(format => fileExtension == format);
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

                var isPhoneNumber = Regex.Match(trimmedItem, phoneNumberPattern, RegexOptions.IgnoreCase).Success;
                var isEmail = Regex.Match(trimmedItem, emailPattern, RegexOptions.IgnoreCase).Success;
                
                if (!isPhoneNumber && !isEmail)
                {
                    return false;
                }
            }

            return true;
        }

        public static bool IsValidOrder(string value)
        {
            return AllowedOrders
                .Any(order =>
                    string.Equals(order, value, StringComparison.CurrentCultureIgnoreCase));
        }

        private static readonly List<string> AllowedImageExtensions = new List<string>()
        {
            GlobalVariables.Jpg,
            GlobalVariables.Jpeg,
            GlobalVariables.Png,
            GlobalVariables.Tiff,
            GlobalVariables.Gif
        };

        private static readonly List<string> AllowedOrders = new List<string>()
        {
            GlobalVariables.Id,
            GlobalVariables.FirstNameEn,
            GlobalVariables.FirstNameGe,
            GlobalVariables.LastNameEn,
            GlobalVariables.LastNameGe,
            GlobalVariables.Birthday,
            GlobalVariables.PassportNumber
        };
    }
}
