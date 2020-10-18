using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using BlogSystem.Services;

/*
Copyright (c) 2020 Annice Strömberg – Annice.se

This script is MIT (Massachusetts Institute of Technology) licensed, which means that
permission is granted, free of charge, to any person obtaining a copy of this software
and associated documentation files to deal in the software without restriction. This
includes, without limitation, the rights to use, copy, modify, merge, publish, distribute,
sublicense, and/or sell copies of the software, and to permit persons to whom the software
is furnished to do so subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or
substantial portions of the software.
*/
namespace BlogSystem.Middleware
{
    /// <summary>
    /// This middleware checks a valid login on every HTTP request to allow or deny
    /// user access to password protected pages.
    /// </summary>
    public class AuthServer
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        /// <summary>
        /// Inject necessary dependencies.
        /// </summary>
        /// <param name="next"></param>
        /// <param name="config"></param>
        public AuthServer(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        /// <summary>
        /// Check valid admin login to reach protected pages.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context, [FromServices] IAuthHandler handler)
        {
            string[] paths =
            {
                "/Entry", "/Comment", "/Category", "/User"
            };

            foreach(string path in paths)
            {
                if (!handler.IsLoggedIn() && context.Request.Path.ToString().StartsWith(path))
                {
                    context.Response.StatusCode = 401; // Unauthorized.
                    context.Response.Redirect(context.Request.Scheme + "://"
                        + context.Request.Host + "/Home/AccessDenied");
                    return;
                }
            }

            await _next.Invoke(context);
        }

    } // End class.
} // End namespace.