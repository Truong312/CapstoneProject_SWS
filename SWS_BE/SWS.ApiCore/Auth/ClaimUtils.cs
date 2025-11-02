using System.Security.Claims;

namespace SWS.ApiCore.Auth
{
    public static class ClaimUtils
    {
        public static int GetUserIdOrThrow(ClaimsPrincipal user)
        {
            var uid = user.FindFirst("uid")?.Value
                   ?? user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? user.FindFirst("sub")?.Value;

            if (string.IsNullOrWhiteSpace(uid) || !int.TryParse(uid, out var id))
                throw new UnauthorizedAccessException("User id is missing in token.");

            return id;
        }
    }
}
