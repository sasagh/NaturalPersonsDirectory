using System;
using System.Collections.Generic;

namespace NaturalPersonsDirectory.Common
{
    public static class StatusMessages
    {
        public static string GetMessageByStatusCode(StatusCode statusCode) => Messages[statusCode];
        
        private static readonly Dictionary<StatusCode, string> Messages = new Dictionary<StatusCode, string>()
        {
            { StatusCode.Success, "Operation completed successfully."},
            { StatusCode.Delete, "Item deleted successfully."},
            { StatusCode.Update, "Item updated successfully."},
            { StatusCode.NotFound, "Not found."},
            { StatusCode.Error, "There was an error while processing request."},
            { StatusCode.ImageAdded, "Image added successfully."},
            { StatusCode.ImageUpdated, "Image updated successfully."},
            { StatusCode.ImageDeleted, "Image deleted successfully."},
            { StatusCode.PassportNumberExists, "Passport number already exists."},
            { StatusCode.IdNotExists, "Item with this id does not exist."},
            { StatusCode.NoImage, "Natural person has no image."},
            { StatusCode.ChangingPassportNumberNotAllowed, "Changing passport number is not allowed"},
            { StatusCode.IncorrectIds, "Incorrect id(s)."},
            { StatusCode.UnsupportedFileFormat, "Unsupported file format."},
            { StatusCode.AlreadyHaveImage, "Natural person already has an image."},
            { StatusCode.RelationNotExists, "Relation between given ids does not exist. "}
        };
    }
}
