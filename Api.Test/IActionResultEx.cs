using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Api.Test
{
    public static class IActionResultEx
    {
        public static HttpStatusCode? TryGetStatusCode(this IActionResult actionResult)
            => (actionResult is ObjectResult objResult && objResult.StatusCode.HasValue)
                ? (HttpStatusCode)objResult.StatusCode.Value
                : null as HttpStatusCode?;
    }
}