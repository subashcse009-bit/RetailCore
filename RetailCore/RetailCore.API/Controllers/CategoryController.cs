using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RetailCore.DataAccess.Data;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using RetailCore.Utilities;

namespace RetailCore.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Authorize(Roles =RetailCoreConstants.Roles.Role_Admin)]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork = null;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Category> lstCategoryList = _unitOfWork.Category.GetAll().ToList();
            return Ok(lstCategoryList);
        }

        public IActionResult Create()
        {
            return Ok();
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
                _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return Ok(category);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _unitOfWork.Category.Get(lobjCat => lobjCat.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
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
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return Ok(category);
        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Category category = _unitOfWork.Category.Get(lobjCat => lobjCat.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return Ok(category);
        }

        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            Category category = _unitOfWork.Category.Get(lobjCat => lobjCat.Id == id);

            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
