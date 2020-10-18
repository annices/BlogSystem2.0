using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using BlogSystem.Models;
using Microsoft.Extensions.Logging;

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
    /// This controller handles the interaction between the database and the view layer regarding entry comments.
    /// All rendered pages with actions through this controller require authorization via the AuthServer (see middleware).
    /// </summary>
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly BlogSystemContext _db;

        /// <summary>
        /// Inject dependencies.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        public CommentController(BlogSystemContext db, ILogger<CommentController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Render the page to display an overview of the entry comments in which the admin can edit.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            // Catch feedback posted from another view:
            ViewBag.Success = TempData["Success"];

            // IQueryable must be used in DB scenarios in order for the Ajax grid to work properly:
            IQueryable<BsComment> comments = _db.BsComments.OrderByDescending(c => c.Date).Select(c => c).AsQueryable();

            // Call partial grid view if it's an Ajax request:
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_AjaxIndex", comments);

            return View();
        }

        /// <summary>
        /// Render the page to update an entry comment.
        /// </summary>
        /// <param name="id">Comment ID.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null || !_db.BsComments.Where(c => c.Id == id).Any())
                return RedirectToAction(nameof(Index));

                BsComment item = _db.BsComments.Find(id);
                return View(item);
            }
            catch (Exception e)
            {
                string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                _logger.LogInformation(exceptionMessage);
            }

            return View();
        }

        /// <summary>
        /// Handle the post request to update an entry comment.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BsComment input)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _db.BsComments.Update(input);
                    _db.SaveChanges();

                    ViewBag.Success = "The comment was successfully updated.";
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
        /// Handle the post request to delete entry comments.
        /// </summary>
        /// <param name="id">Comment ID.</param>
        /// <param name="selectedComments"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(int? id, int[] selectedComments)
        {
            try
            {
                if (selectedComments.Any())
                {
                    foreach (var commentId in selectedComments)
                    {
                        _db.BsComments.RemoveRange(_db.BsComments.Where(c => c.Id == commentId));
                        _db.SaveChanges();
                    }

                    TempData["Success"] = "The selected comments were successfully deleted.";
                    return RedirectToAction(nameof(Index));
                }

                BsComment item = _db.BsComments.Find(id);
                _db.BsComments.Remove(item);
                _db.SaveChanges();

                TempData["Success"] = "The comment was successfully deleted.";
            }
            catch (Exception e)
            {
                string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                _logger.LogInformation(exceptionMessage);
            }

            return RedirectToAction(nameof(Index));
        }

    } // End controller.
} // End namespace.