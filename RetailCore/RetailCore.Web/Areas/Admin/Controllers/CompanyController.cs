using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RetailCore.DataAccess.Repository;
using RetailCore.DataAccess.Repository.IRepository;
using RetailCore.Model;
using RetailCore.Model.ViewModel;
using RetailCore.Utilities;
using RetailCore.Web.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace RetailCore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = RetailCoreConstants.Roles.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Company> companies = _unitOfWork.Company.GetAll().ToList();
            return View(companies);
        }

        public ActionResult Upsert(int? id)
        {
            if (id == null || id == 0)
            {
                //Create
                return View(new Company());
            }
            else
            {
                //Update
                Company company = _unitOfWork.Company.Get(prod => prod.Id == id);
                return View(company);
            }
        }

        [HttpPost]
        public ActionResult Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if (company.Id == 0)
                {
                    TempData["Success"] = "Company created successfully";
                    _unitOfWork.Company.Add(company);
                }
                else
                {
                    TempData["Success"] = "Company updated successfully";
                    _unitOfWork.Company.Update(company);
                }
                _unitOfWork.Save();
                // TempData is used to pass a one - time success message from the controller to the next request.
                // It stores data temporarily in session, survives a single redirect, and is cleared automatically after being read.

                return RedirectToAction("Index");
            }
            else
            {
                return View(new Company());
            }
        }

        #region Comment Delete 
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Company Company = _unitOfWork.Company.Get(lobjCat => lobjCat.Id == id);
        //    if (Company == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(Company);
        //}

        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePost(int? id)
        //{
        //    Company Company = _unitOfWork.Company.Get(lobjCat => lobjCat.Id == id);

        //    if (Company == null)
        //    {
        //        return NotFound();
        //    }
        //    _unitOfWork.Company.Remove(Company);
        //    _unitOfWork.Save();
        //    TempData["Success"] = "Company deleted successfully";
        //    return RedirectToAction("Index");
        //}

        #endregion

        #region API CALLS
        public IActionResult GetAll()
        {
            List<Company> Companies = _unitOfWork.Company.GetAll().ToList();

            return Json(new { data = Companies });
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var CompanyToBeDeleted = _unitOfWork.Company.Get(u => u.Id == id);
            if (CompanyToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }
            _unitOfWork.Company.Remove(CompanyToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }
        #endregion
    }
}
