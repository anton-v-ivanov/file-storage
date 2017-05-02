using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StatsdClient;

namespace FileStorage.Authentication
{
    public class AuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IAuthValidator _authValidator;

        public AuthenticationMiddleware(RequestDelegate next, IAuthValidator authValidator)
        {
            _next = next;
            _authValidator = authValidator;
        }

        public async Task Invoke(HttpContext context)
        {
            using (Metrics.StartTimer("file-storage.authentication"))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic"))
                {
                    //Extract credentials
                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    var seperatorIndex = usernamePassword.IndexOf(':');

                    var username = usernamePassword.Substring(0, seperatorIndex);
                    var password = usernamePassword.Substring(seperatorIndex + 1);

                    if (_authValidator.Validate(username, password, context.Request.Method, context.Request.Path))
                    {
                        Metrics.Counter("file-storage.authentication.ok");
                        await _next.Invoke(context);
                    }
                    else
                    {
                        Metrics.Counter("file-storage.authentication.unauthorized.wrong-credentials");
                        context.Response.StatusCode = 401; //Unauthorized
                    }
                }
                else
                {
                    Metrics.Counter("file-storage.authentication.unauthorized.no-header");
                    context.Response.StatusCode = 401; //Unauthorized
                }
            }
        }
    }
}