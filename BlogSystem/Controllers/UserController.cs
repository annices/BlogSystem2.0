using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlogSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity;

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
namespace BlogSystem.Controllers
{
    /// <summary>
    /// This controller handles the interaction between the database and the view layer regarding the admin settings.
    /// All rendered pages with actions through this controller require authorization via the AuthServer (see middleware).
    /// </summary>
    public class UserController : Controller
    {
        private readonly PasswordHasher<BsUser> _hasher = new PasswordHasher<BsUser>();
        private readonly ILogger<UserController> _logger;
        private readonly IConfiguration _config;
        private readonly BlogSystemContext _db;

        public UserController(BlogSystemContext db, ILogger<UserController> logger, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _config = config;
        }

        /// <summary>
        /// Render the admin edit page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit()
        {
            // Select first since this app only has one admin:
            BsUser item = _db.BsUsers.Select(u => u).First();

            if (item == null)
                return NotFound();

            return View(item);
        }

        /// <summary>
        /// Handle the post request to update the admin details.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="inputPassword"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BsUser input, string inputPassword)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Instantiate a new user item to keep the existing password hashed:
                    BsUser user = _db.BsUsers.Find(input.Id);
                    // Uppercase first letter in username for prettier appearance:
                    user.Username = input.Username.Substring(0, 1).ToUpper() + input.Username.Substring(1);
                    user.Password = !string.IsNullOrEmpty(inputPassword) ? _hasher.HashPassword(user, inputPassword) : user.Password;
                    user.Firstname = input.Firstname;
                    user.Lastname = input.Lastname;
                    user.Email = input.Email;

                    _db.BsUsers.Update(user);
                    _db.SaveChanges();

                    ViewBag.Success = "The user details are now updated.";
                }
                catch (Exception e)
                {
                    string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                    _logger.LogInformation(exceptionMessage);
                }
            }
            return View(input);
        }

        /// <summary>
        /// Handle the logout request by clearing all set sessions.
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData.Remove("Admin");
            return RedirectToAction("Index", "Home");
        }

    } // End class.
} // End namespace.