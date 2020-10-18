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
    /// This controller handles the interaction between the database and the view layer regarding the entry categories.
    /// All rendered pages with actions through this controller require authorization via the AuthServer (see middleware).
    /// </summary>
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly BlogSystemContext _db;

        public CategoryController(BlogSystemContext db, ILogger<CategoryController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Render the page to display an overview of the entry categories in which the admin can edit.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Index()
        {
            // Catch feedback posted from another view:
            ViewBag.Success = TempData["Success"];
            ViewBag.Error = TempData["Error"];

            // IQueryable must be used in DB scenarios in order for the Ajax grid to work properly:
            IQueryable<BsCategory> categories = _db.BsCategories.OrderBy(c => c.Category).Select(c => c).AsQueryable();

            // Call partial grid view if it's an Ajax request:
            if (HttpContext.Request.Headers["X-Requested-With"] == "XMLHttpRequest")
                return PartialView("_AjaxIndex", categories);

            return View();
        }

        /// <summary>
        /// Render the page to create a new entry category.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create() => View();

        /// <summary>
        /// Handle the post request to create a new category.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BsCategory input)
        {
            if (_db.BsCategories.Where(c => c.Category.Equals(input.Category)).Any())
                ViewBag.Error = "The category name already exists.";
            else if (ModelState.IsValid)
            {
                try
                {
                    _db.BsCategories.Add(input);
                    _db.SaveChanges();

                    TempData["Success"] = "The category '" + input.Category + "' was successfully created.";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception e)
                {
                    string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                    _logger.LogInformation(exceptionMessage);
                }
            }

            return View();
        }

        /// <summary>
        /// Render the page to update an entry category.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            try
            {
                if (id == null || !_db.BsCategories.Where(c => c.Id == id).Any())
                    return RedirectToAction(nameof(Index));

                BsCategory item = _db.BsCategories.Find(id);

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
        /// Handle the post request to update a category.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(BsCategory input)
        {
            try
            {
                if (_db.BsCategories.Where(c => c.Category.Equals(input.Category) && c.Id != input.Id).Any())
                    ViewBag.Error = "The category name already exists.";
                else if (ModelState.IsValid)
                {
                    _db.BsCategories.Update(input);
                    _db.SaveChanges();

                    ViewBag.Success = "The category was successfully updated.";
                }
            }
            catch (Exception e)
            {
                string exceptionMessage = (e.InnerException != null) ? e.InnerException.Message : e.Message;
                _logger.LogInformation(exceptionMessage);
            }

            return View(input);
        }

        /// <summary>
        /// Handle the post request to delete a category.
        /// </summary>
        /// <param name="id">Category ID.</param>
        /// <param name="selectedCategories"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Delete(int? id, int[] selectedCategories)
        {
            try
            {
                if (selectedCategories.Any())
                {
                    foreach (var categoryId in selectedCategories)
                    {
                        if (_db.BsEntryCategories.Where(e => e.CategoryId == categoryId).Any())
                        {
                            TempData["Error"] = "One or many of the selected categories are linked to one or many blog entries. " +
                                "In order to delete categories, you must either delete the related entries first, alternatively " +
                                "uncheck these categories from the related entries!";
                            break;
                        }
                        else
                        {
                            _db.BsCategories.RemoveRange(_db.BsCategories.Where(c => c.Id == categoryId));
                            _db.SaveChanges();

                            TempData["Success"] = "The selected categories were successfully deleted.";
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }

                if (_db.BsEntryCategories.Where(e => e.CategoryId == id).Any())
                {
                    TempData["Error"] = "The category is linked to one or many blog entries. In order to delete this category, you " +
                        "must either delete the related entries first, alternatively uncheck the category from the related entries!";
                }
                else
                {
                    BsCategory item = _db.BsCategories.Find(id);
                    _db.BsCategories.Remove(item);
                    _db.SaveChanges();

                    TempData["Success"] = "The category was successfully deleted.";
                }
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