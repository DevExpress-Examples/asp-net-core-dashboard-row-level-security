using System.Globalization;
using System.Security.Claims;

namespace WebDashboard.Code {
    public static class HttpContextAccessorExtensions {
        public static int GetCurrentUserId(this IHttpContextAccessor contextAccessor) {
            var sidStr = contextAccessor.HttpContext.User.Claims
                    .FirstOrDefault(x => x.Type == ClaimTypes.Sid);
            return Convert.ToInt32(sidStr.Value, CultureInfo.InvariantCulture);
        }
    }
}
