using Microsoft.AspNetCore.Mvc;
using RetailCoreWeb.Data;
using RetailCoreWeb.Models;

namespace RetailCoreWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDBContext _dbContext = null;
        public CategoryController(ApplicationDBContext dBContext)
        {
            _dbContext = dBContext;
        }

        public IActionResult Index()
        {
            List<Category> lstCategoryList = _dbContext.Categories.ToList();
            return View(lstCategoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            }

            if (ModelState.IsValid)
            {
                _dbContext.Categories.Add(category);
                _dbContext.SaveChanges();
                TempData["Success"] = "Category created successfully";
                return RedirectToAction("Index");
            }
            return View("Create", category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _dbContext.Categories.FirstOrDefault(lobjCat => lobjCat.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("name", "The Display Order cannot exactly match the Name.");
            }
            if (ModelState.IsValid)
            {
                _dbContext.Categories.Update(category);
                _dbContext.SaveChanges();
                TempData["Success"] = "Category updated successfully";
                return RedirectToAction("Index");
            }
            return View("Edit", category);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _dbContext.Categories.FirstOrDefault(lobjCat => lobjCat.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category category = _dbContext.Categories.FirstOrDefault(lobjCat => lobjCat.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            _dbContext.Categories.Remove(category);
            _dbContext.SaveChanges();
            TempData["Success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
