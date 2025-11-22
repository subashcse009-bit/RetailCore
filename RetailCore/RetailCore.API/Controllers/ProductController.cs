using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailCore.DataAccess.Repository;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using RetailCore.Model.ViewModel;
using RetailCore.Utilities;

namespace RetailCore.Web.Areas.Admin.Controllers
{
    [ApiController]
    [Authorize(Roles = RetailCoreConstants.Roles.Role_Admin)]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Ok(products);
        }

        public ActionResult Upsert(int? id)
        {
            //ViewBag.CategoryList = categoryList;
            //ViewData["CategoryList"] = categoryList;

            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category.GetAll().
                Select(cat => new SelectListItem
                {
                    Text = cat.Name,
                    Value = cat.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                //Create
                return Ok(productVM);
            }
            else
            {
                //Update
                productVM.Product = _unitOfWork.Product.Get(prod => prod.Id == id);
                return Ok(productVM);
            }
        }

        [HttpPost]
        public ActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;

                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine(wwwRootPath, @"images/product");

                    //Delete old file
                    if (!string.IsNullOrEmpty(productVM.Product.ImageUrl))
                    {
                        string oldImagePath = Path.Combine(wwwRootPath, productVM.Product.ImageUrl.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(productPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"/images/product/" + fileName;
                }
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }
                _unitOfWork.Save();
                // TempData is used to pass a one - time success message from the controller to the next request.
                // It stores data temporarily in session, survives a single redirect, and is cleared automatically after being read.

                return RedirectToAction("Index");
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().
                Select(cat => new SelectListItem
                {
                    Text = cat.Name,
                    Value = cat.Id.ToString()
                });

                return Ok(productVM);
            }
        }

        #region Comment Delete 
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product product = _unitOfWork.Product.Get(lobjCat => lobjCat.Id == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(product);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Product product = _unitOfWork.Product.Get(lobjCat => lobjCat.Id == id);

        //    if (product == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Product.Remove(product);
        //    _unitOfWork.Save();
        //    TempData["Success"] = "Product deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #endregion

        #region API CALLS
        public IActionResult GetAll()
        {
            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();

            return Ok();
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                return Ok();
            }
            string oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Ok();
        }
        #endregion
    }
}
