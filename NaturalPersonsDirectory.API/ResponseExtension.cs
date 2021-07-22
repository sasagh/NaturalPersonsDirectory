using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using NaturalPersonsDirectory.Common;

namespace NaturalPersonsDirectory.API
{
    public static class ResponseExtension
    {
        public static ActionResult<T> MatchActionResult<T>(this Response<T> response)
        {
            return new OkResult();
        }
    }
}