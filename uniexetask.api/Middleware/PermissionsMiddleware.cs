using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using uniexetask.services.Interfaces;

namespace uniexetask.api.Middleware
{
    public class PermissionsMiddleware
    {
        private readonly RequestDelegate _next;

        public PermissionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRolePermissionService rolePermissionService)
        {
            var endpoint = context.GetEndpoint();
            if (endpoint == null || !endpoint.Metadata.Any(meta => meta is AuthorizeAttribute))
            {
                await _next(context);
                return;
            }
            if (context.User.Identity.IsAuthenticated)
            {
                var roleName = context.User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleName != null)
                {
                    var permissions = await rolePermissionService.GetRolePermissionsByRole(roleName);

                    var claims = permissions.Select(p => new Claim("permissions", p.Name)).ToList();
                    var identity = new ClaimsIdentity(claims);
                    context.User.AddIdentity(identity);
                }
            }

            await _next(context);
        }
    }
}
