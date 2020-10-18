using System;
using System.Linq;
using BlogSystem.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
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
    /// This controller handles the interaction between the database and the view layer regarding blog entries.
    /// All rendered pages with actions through this controller require authorization via the AuthServer (see middleware).
    /// </summary>
    public class EntryController : Controller
    {
        private readonly ILogger<EntryController> _logger;
        private readonly IConfiguration _config;
        private readonly BlogSystemContext _db;

        /// <summary>
        /// Inject dependencies.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        /// <param name="config"></param>
        public EntryController(BlogSystemContext db, ILogger<EntryController> logger, IConfiguration config)
        {
            _logger = logger;
            _db = db;
            _config = config;
        }

        /// <summary>
        /// Render the page to display an overview of the entries in which the admin can edit.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            // Catch feedback posted from another view:
            ViewBag.Success = TempData["Success"];

            // IQueryable must be used in DB scenarios in order for the Ajax grid to work properly:
            IQueryable<BsEntry> entries = _db.BsEntries.OrderByDescending(e => e.Date).Select(e => e).AsQueryable();

            foreach (var item in entries)
            {
                item.User = _db.BsUsers.Select(u => u).First();
                item.BsComments = _db.BsComments.Where(e => e.EntryId == item.Id).Select(c => c).ToList();
            }

            // Call partial grid view if it's an Ajax request:
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_AjaxIndex", entries);

            return View();
        }

        /// <summary>
        /// Render the page to create a new blog entry.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            try
            {
                // Prep wanted data to return to view:
                ViewBag.Categories = _db.BsCategories.OrderBy(c => c.Category);
            }
            catch (Exception e)
            {
                _logger.LogInformation(e.Message);
            }

            return View();
        }

        /// <summary>
        /// Handle the post request to create a new blog entry.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="publishStatus"></param>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BsEntry input, bool publishStatus, int[] selectedCategories)
        {
            if (!selectedCategories.Any())
                ViewBag.Error = "Please select a category.";
            else if (ModelState.IsValid)
            {
                try
                {
                    input.Date = DateTime.Now;
                    input.IsPublished = publishStatus;
                    // Select first since this app only has one admin:
                    input.UserId = _db.BsUsers.Select(u => u.Id).First();

                    _db.BsEntries.Add(input);
                    _db.SaveChanges();

                    // After the entry DB save, go on and store the selected entry categories:
                    foreach (var categoryId in selectedCategories)
                    {
                        BsEntryCategory ec = new BsEntryCategory()
                        {
                            EntryId = input.Id,
                            CategoryId = categoryId
                        };

                        input.BsEntryCategories.Add(ec);
                        _db.SaveChanges();
                    }

                    ViewBag.Success = "The entry was successfully saved.";
                }
                catch (Exception e)
                {
                    string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                    _logger.LogInformation(exceptionMessage);
                }
            }

            // Finally, re-populate categories back to view after the post request:
            if (_db.BsCategories.Any())
                ViewBag.Categories = _db.BsCategories.OrderBy(c => c.Category);

            ModelState.Clear();
            return View();
        }

        /// <summary>
        /// Render the page to update a blog entry.
        /// </summary>
        /// <param name="id">Entry ID.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null || !_db.BsEntries.Where(e => e.Id == id).Any())
                return RedirectToAction(nameof(Index));

                BsEntry item = _db.BsEntries.Find(id);

                List<BsCategory> currentCategories = _db.BsEntryCategories
                    .Where(ec => ec.EntryId == item.Id)
                    .Join(_db.BsCategories,
                    ec => ec.CategoryId, c => c.Id,
                    (ec, c) => new BsCategory()
                    {
                        Id = Convert.ToInt32(ec.CategoryId),
                        Category = c.Category
                    }).ToList();

                // Create variables to reach in view:
                ViewBag.Categories = _db.BsCategories;
                ViewBag.CurrentCategories = currentCategories;

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
        /// Handle the post request to update a blog entry.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="publishStatus"></param>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BsEntry input, bool publishStatus, int[] selectedCategories)
        {
            if (!selectedCategories.Any())
                ViewBag.CategoryError = "Please select a category.";
            if (input.Date > DateTime.Now)
            {
                ViewBag.DateError = "The date cannot be later than the current date.";
                input.IsPublished = publishStatus; // Keep last status.
            }
            else if (ModelState.IsValid)
            {
                try
                {
                    input.Date = input.Date.Add(DateTime.Now.TimeOfDay);
                    input.IsPublished = publishStatus;
                    _db.BsEntries.Update(input);
                    _db.SaveChanges();

                    List<BsEntryCategory> storedEntryCategories = _db.BsEntryCategories
                        .Where(e => e.EntryId == input.Id).ToList();

                    // Replace stored entry categories with the recently selected:
                    _db.BsEntryCategories.RemoveRange(storedEntryCategories);
                    foreach (int selectedId in selectedCategories)
                    {
                        BsEntryCategory item = new BsEntryCategory()
                        {
                            EntryId = input.Id,
                            CategoryId = selectedId
                        };

                        _db.BsEntryCategories.Add(item);
                        _db.SaveChanges();
                    }

                    ViewBag.Success = "The entry was successfully updated.";
                }
                catch (Exception e)
                {
                    string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                    _logger.LogInformation(exceptionMessage);
                }
            }

            // Finally, re-populate categories back to view after the post request:
            if (_db.BsCategories.Any())
            {
                ViewBag.Categories = _db.BsCategories;

                List<BsCategory> currentCategories = _db.BsEntryCategories
                    .Where(ec => ec.EntryId == input.Id)
                    .Join(_db.BsCategories,
                    ec => ec.CategoryId, c => c.Id,
                    (ec, c) => new BsCategory()
                    {
                        Id = Convert.ToInt32(ec.CategoryId),
                        Category = c.Category
                    }).ToList();

                ViewBag.CurrentCategories = currentCategories;
            }

            return View(input);
        }

        /// <summary>
        /// Handle the post request to delete a blog entry from the entry list page (see Index action).
        /// </summary>
        /// <param name="id"></param>
        /// <param name="selectedEntries"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(int? id, int[] selectedEntries)
        {
            try
            {
                if (selectedEntries.Any())
                {
                    foreach (var entryId in selectedEntries)
                    {
                        _db.BsEntries.RemoveRange(_db.BsEntries.Where(e => e.Id == entryId));
                        _db.SaveChanges();
                    }

                    TempData["Success"] = "The selected entries were successfully deleted.";
                    return RedirectToAction(nameof(Index));
                }

                BsEntry item = _db.BsEntries.Find(id);
                _db.BsEntries.Remove(item);
                _db.SaveChanges();

                TempData["Success"] = "The entry was successfully deleted.";
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
