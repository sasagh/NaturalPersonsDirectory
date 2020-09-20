using System;
using System.Collections.Generic;

namespace NaturalPersonsDirectory.Common
{
    public static class Statuses
    {
        public static readonly Dictionary<StatusCode, Status> GetStatus = new Dictionary<StatusCode, Status>()
        {
            { StatusCode.Success, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.Success), Message = "Operation completed successfully."} },
            { StatusCode.Delete, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.Delete), Message = "Item deleted successfully."} },
            { StatusCode.Update, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.Update), Message = "Item updated successfully."} },
            { StatusCode.NotFound, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.NotFound), Message = "Not found."} },
            { StatusCode.Error, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.Error), Message = "There was an error while processing request."} },
            { StatusCode.ImageAdded, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.ImageAdded), Message = "Image added successfully."} },
            { StatusCode.ImageUpdated, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.ImageUpdated), Message = "Image updated successfully."} },
            { StatusCode.ImageDeleted, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.ImageDeleted), Message = "Image deleted successfully."} },
            { StatusCode.PassportNumberExists, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.PassportNumberExists), Message = "Passport number already exists."} },
            { StatusCode.IdNotExists, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.IdNotExists), Message = "Item with this id does not exist."} },
            { StatusCode.NoImage, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.NoImage), Message = "Natural person has no image."} },
            { StatusCode.ChangingPassportNumberNotAllowed, new Status()
                { Code = Enum.GetName(typeof(StatusCode), StatusCode.ChangingPassportNumberNotAllowed),
                Message = "Changing passport number is not allowed"} },
            { StatusCode.IncorrectIds, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.IncorrectIds), Message = "Incorrect id(s)."} },
            { StatusCode.UnsupportedFileFormat, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.UnsupportedFileFormat), Message = "Unsupported file format."} },
            { StatusCode.AlreadyHaveImage, new Status(){ Code = Enum.GetName(typeof(StatusCode), StatusCode.AlreadyHaveImage), Message = "Natural person already has an image."} },
        };

    }
}
