using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

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
namespace BlogSystem.Services
{
    /// <summary>
    /// This is a service class to check a valid user login in interaction with the AuthServer (middleware).
    /// </summary>
    public class AuthHandler : IAuthHandler
    {
        private readonly IServiceScopeFactory _scope;
        public AuthHandler(IServiceScopeFactory scope) => _scope = scope;

        /// <summary>
        /// Check logged in user by set session.
        /// </summary>
        /// <returns></returns>
        public bool IsLoggedIn()
        {
            var scope = _scope.CreateScope();
            var context = scope.ServiceProvider.GetService<IHttpContextAccessor>();

            // If user session is set, it means we have a valid login:
            if(context.HttpContext.Session.GetString("UserID") != null)
                return true;
            else
                return false;
        }

    } // End class.
} // End namespace.
