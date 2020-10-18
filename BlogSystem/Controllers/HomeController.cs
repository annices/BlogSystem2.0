using System;
using System.Web;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlogSystem.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ReflectionIT.Mvc.Paging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http.Extensions;
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
namespace BlogSystem.Controllers
{
    /// <summary>
    /// This controller handles the interaction between the database and view layer regarding the public pages of this application.
    /// </summary>
    public class HomeController : Controller
    {
        private readonly PasswordHasher<BsUser> _hasher = new PasswordHasher<BsUser>();
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;
        private readonly BlogSystemContext _db;

        /// <summary>
        /// Inject dependencies.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public HomeController(BlogSystemContext db, ILogger<HomeController> logger, IConfiguration config)
        {
            _db = db;
            _logger = logger;
            _config = config;
        }

        public IActionResult Index()
        {
            ViewBag.Text = "Hello world!";

            return View();
        }

        /// <summary>
        /// Render the public start page of the application.
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            try
            {
                var blogPosts = _db.BsEntries.Where(e => e.IsPublished == true).OrderByDescending(e => e.Date);

                // Populate necessary property values to each post:
                foreach (var entry in blogPosts)
                {
                    entry.User = _db.BsUsers.Where(u => u.Id == entry.UserId).Select(u => u).First();

                    entry.BsComments = _db.BsComments
                        .Where(c => c.EntryId == entry.Id)
                        .Select(c => c).ToList();
                }

                ViewBag.Categories = _db.BsEntryCategories
                    .Select(ec => new EntryCategory
                    {
                        EntryId = Convert.ToInt32(ec.EntryId),
                        CategoryName = ec.Category.Category
                    }).ToList();

                var pagingList = PagingList.Create(blogPosts, 20, page);

                ViewBag.NoEntries = !pagingList.Any() ? "No entries found." : "";

                return View(pagingList);
            }
            catch (Exception e)
            {
                string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                _logger.LogInformation(exceptionMessage);
            }

            return View();
        }

        /// <summary>
        /// Render the public entry details page with related entry comments.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EntryComments(int? id)
        {
            // Catch posted feedback:
            ViewBag.Success = TempData["Success"];

            if (id == null || !_db.BsEntries.Where(e => e.Id == id).Any())
                return RedirectToAction(nameof(Index));

            BsEntry entry = _db.BsEntries.Find(id);

            try
            {
                // Select first since this app only has one admin:
                entry.User = _db.BsUsers.Select(u => u).First();

                entry.BsComments = _db.BsComments
                    .Where(c => c.EntryId == id).Select(c => c)
                    .OrderByDescending(c => c.Date).ToList();

                ViewBag.EntryCategories = _db.BsEntryCategories
                    .Where(ec => ec.EntryId == id)
                    .Join(_db.BsCategories,
                    ec => ec.CategoryId, c => c.Id,
                    (ec, c) => new
                    {
                        entryCategory = ec,
                        category = c
                    }).Select(c => c.category.Category)
                    .ToList();
            }
            catch (Exception e)
            {
                string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                _logger.LogInformation(exceptionMessage);
            }

            return View(entry);
        }

        /// <summary>
        /// Handle the post request of a new comment from the public entry details page.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EntryComments(BsEntry input, IFormCollection comment)
        {
            try
            {
                BsComment item = new BsComment()
                {
                    Date = DateTime.Now,
                    Name = comment["name"],
                    Email = comment["email"],
                    Website = comment["website"],
                    Comment = comment["comment"],
                    EntryId = input.Id
                };

                _db.BsComments.Add(item);
                _db.SaveChanges();

                TempData["Success"] = "The comment was successfully added.";

                // Re-populate some necessary data back to view after post request:
                input = _db.BsEntries.Find(input.Id);

                input.User = _db.BsUsers.Select(u => u).First();

                input.BsComments = _db.BsComments
                    .Where(c => c.EntryId == input.Id).Select(c => c)
                    .OrderByDescending(c => c.Date)
                    .ToList();

                ViewBag.EntryCategories = _db.BsEntryCategories
                    .Where(ec => ec.EntryId == input.Id)
                    .Join(_db.BsCategories,
                    ec => ec.CategoryId, c => c.Id,
                    (ec, c) => new
                    {
                        entryCategory = ec,
                        category = c
                    }).Select(c => c.category.Category)
                    .ToList();
            }
            catch (Exception e)
            {
                string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                _logger.LogInformation(exceptionMessage);
            }

            return Redirect(input.Id + "#comment");
        }

        /// <summary>
        /// Render the user login page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login() => View();

        /// <summary>
        /// Handle the post request when the user has requested to login.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Login(IFormCollection input)
        {
            string errorFeedback = "Invalid login!";

            try
            {
                BsUser existingUser = _db.BsUsers.Where(a => a.Email.Equals(input["email"])).Select(a => a).First();

                if (ValidLogin(existingUser, input["password"]))
                {
                    HttpContext.Session.SetString("UserID", existingUser.Id.ToString());
                    // Also, use temp data session to handle the admin GUI such as layout menu appearance etc:
                    TempData["Admin"] = existingUser.Id.ToString();

                    return RedirectToAction(nameof(Index));
                }
                else
                    ViewBag.Error = errorFeedback;
            }
            catch (Exception e)
            {
                ViewBag.Error = errorFeedback;
                _logger.LogInformation(e.Message);
            }
            return View(input);
        }

        /// <summary>
        /// Render the password reset page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult RequestPassword() => View();

        /// <summary>
        /// Post the password recovery link with an attached JWT to the admin email.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sender"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RequestPassword(IFormCollection input, [FromServices] IEmailSender sender, [FromServices] ITokenHandler generator)
        {
            BsUser user = new BsUser();

            if (_db.BsUsers.Where(u => u.Email.Equals(input["email"])).Any())
            {
                user = _db.BsUsers.Where(u => u.Email.Equals(input["email"])).Select(u => u).First();
                var token = generator.CreateJWT(user);
                string ResetPassUrlWithToken = Url.Action("ResetPassword", "Home", new { token }, protocol: HttpContext.Request.Scheme);

                sender.SendEmail
                    (
                        user.Email,
                        _config["EmailSettings:From"],
                        "Password reset link.",
                        "Navigate to the following link to reset your user password. The link is valid for 30 minutes:\n\r" +
                        ResetPassUrlWithToken +
                        "\n\r(If you did not request this link, you can ignore this email.)"
                    );

                ViewBag.Success = "A password reset link has now been sent to your user email.";
            }
            else
                ViewBag.Error = "The user was not found.";

            return View();
        }

        /// <summary>
        /// Render the reset password page with user ID fetched from the decoded token.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult ResetPassword()
        {
            string url = HttpContext.Request.GetDisplayUrl();

            // Get token if it's attached to the URL:
            if (!url.EndsWith("ResetPassword"))
            {
                string subUrl = url.Substring(url.IndexOf("?"));
                var urlParams = HttpUtility.ParseQueryString(subUrl);
                string token = urlParams["token"];
                TempData["token"] = token;
            }

            return View();
        }

        /// <summary>
        /// Handle the post request to reset the user password based on a valid token.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(IFormCollection input, [FromServices] ITokenHandler handler)
        {
            TempData["token"] = input["token"];
            TempData.Keep("token");

            if (!input["password"].Equals(input["password2"]))
            {
                ViewBag.Error = "The passwords did not match.";
                return View();
            }
            else if (handler.ValidateJWT(TempData["token"].ToString()))
            {
                string userID = handler.DecodeJWT(TempData["token"].ToString());
                BsUser item = _db.BsUsers.Find(Convert.ToInt32(userID));

                if (item.Email.Equals(input["email"]))
                {
                    item.Password = _hasher.HashPassword(item, input["password"]);
                    _db.BsUsers.Update(item);
                    _db.SaveChanges();
                    // Once input is correct, we can clear the temp data session:
                    TempData.Remove("token");
                    ViewBag.Success = "The password is now updated.";
                }
                else
                    ViewBag.Error = "The user was not found.";

                return View();
            }

            ViewBag.Error = "Invalid token.";
            return View();
        }

        /// <summary>
        /// Render the access denied page displayed on pages where a user don't have permissions,
        /// i.e. is not logged in.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult AccessDenied() => View();

        /// <summary>
        /// Utilize the ASP.NET Core Identity function to verify a user password.
        /// </summary>
        /// <param name="item">VsAdmins object.</param>
        /// <param name="passwordInput">User password input in plain text.</param>
        /// <returns>Valid or invalid login.</returns>
        public bool ValidLogin(BsUser item, string passwordInput)
        {
            var result = _hasher.VerifyHashedPassword(item, item.Password, passwordInput);

            if (result == PasswordVerificationResult.Success)
                return true;

            return false;
        }

    } // End controller.
} // End namespace.