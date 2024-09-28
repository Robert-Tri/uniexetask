using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using uniexetask.api.Models.Request;

namespace uniexetask.api.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task Invoke(HttpContext context)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null || !endpoint.Metadata.Any(meta => meta is AuthorizeAttribute))
            {
                await _next(context);
                return;
            }

            var at = context.Request.Cookies["AccessToken"];
            var rt = context.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(at))
            {
                at = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            }

            if (at != null)
            {
                var userPrincipal = ValidateToken(at);
                if (userPrincipal != null)
                {
                    context.User = userPrincipal;
                }
                else
                {
                    var refreshTokenResult = await RefreshAccessToken(rt);
                    if (refreshTokenResult != null)
                    {
                        context.Response.Cookies.Append("AccessToken", refreshTokenResult.AccessToken);
                        context.User = ValidateToken(refreshTokenResult.AccessToken);
                    }
                }
            }

            await _next(context);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key must be configured"));

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _configuration["Jwt:Issuer"],
                    ValidAudience = _configuration["Jwt:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        private async Task<TokenModel> RefreshAccessToken(string refreshToken)
        {
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsJsonAsync("api/auth/refresh-token", new { RefreshToken = refreshToken });

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<TokenModel>();
                }
                return null;
            }
        }
    }
}
