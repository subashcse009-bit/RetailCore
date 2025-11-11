using Microsoft.AspNetCore.Mvc;
using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;

namespace RetailCore.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategory _category = null;
        public CategoryController(ICategory category)
        {
            _category = category;
        }

        public IActionResult Index()
        {
            List<Category> lstCategoryList = _category.GetAll().ToList();
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
                _category.Add(category);
                _category.Save();
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
            Category category = _category.Get(lobjCat => lobjCat.Id == id);
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
                _category.Update(category);
                _category.Save();
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
            Category category = _category.Get(lobjCat => lobjCat.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category category = _category.Get(lobjCat => lobjCat.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            _category.Remove(category);
            _category.Save();
            TempData["Success"] = "Category deleted successfully";
            return RedirectToAction("Index");
        }
    }
}
