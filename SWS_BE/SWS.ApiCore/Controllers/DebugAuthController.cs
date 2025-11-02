using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace SWS.ApiCore.Controllers
{
    [ApiController]
    [Route("api/debug")]
    public class DebugAuthController : ControllerBase
    {
        [HttpGet("echo")]
        [AllowAnonymous]
        public IActionResult Echo([FromHeader(Name = "Authorization")] string? authorization)
            => Ok(new { authorization = authorization ?? "<null>", hasBearerPrefix = (authorization ?? "").StartsWith("Bearer ") });

        [HttpPost("token")]
        [AllowAnonymous]
        public IActionResult IssueToken([FromQuery] int uid = 1, [FromQuery] string role = "1",
                                        [FromQuery] int minutes = 120, [FromServices] IConfiguration cfg = null!)
        {
            var key = cfg["Jwt:Key"];
            var iss = cfg["Jwt:Issuer"] ?? "AppBackend";
            var aud = cfg["Jwt:Audience"] ?? "AppBackendUsers";
            if (string.IsNullOrWhiteSpace(key)) return BadRequest("Jwt:Key is missing.");

            var creds = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                SecurityAlgorithms.HmacSha256);

            var claims = new[] { new Claim("uid", uid.ToString()), new Claim(ClaimTypes.Role, role) };

            var jwt = new JwtSecurityToken(issuer: iss, audience: aud, claims: claims,
                                           notBefore: DateTime.UtcNow,
                                           expires: DateTime.UtcNow.AddMinutes(minutes),
                                           signingCredentials: creds);

            return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(jwt), iss, aud, alg = "HS256" });
        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me() => Ok(User.Claims.Select(c => new { c.Type, c.Value }));
    }
}
