using Microsoft.AspNetCore.Mvc;
using Kendo.Models;
using Kendo.Repositories.Implementations;
using Kendo.Repositories.Interfaces;

namespace Kendo.Controllers
{
    public class MyCityController : Controller
    {
        private readonly ICityHelper _cityHelper;

        public MyCityController(ICityHelper cityHelper)
        {
            _cityHelper = cityHelper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult City_Read()
        {
            var cities = _cityHelper.GetAll();
            return Json(cities);
        }

        public IActionResult Details(int id)
        {
            var city = _cityHelper.GetOne(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(tblCity city)
        {
            if (ModelState.IsValid)
            {
                _cityHelper.Insert(city);
                return Json(new { success = true, message = "Data Inserted", newCityId = city.c_cityid });
            }
            return Json(new { success = false, message = "Invalid data." });
        }

        public IActionResult Edit(int id)
        {
            var city = _cityHelper.GetOne(id);
            if (city == null)
            {
                return NotFound();
            }
            return View(city);
        }

        [HttpPost]
        public IActionResult Edit(tblCity city)
        {
            if (ModelState.IsValid)
            {
                _cityHelper.Update(city);
                return Json(new { success = true, message = "Data Successfully Updated" });
            }
            return Json(new { success = false, message = "Invalid data." });
        }

        [HttpPost]
        public IActionResult Delete(int c_cityid)
        {
            try
            {
                var city = _cityHelper.GetOne(c_cityid);
                if (city == null)
                {
                    return Json(new { success = false, message = "City not found." });
                }

                _cityHelper.Delete(city);
                return Json(new { success = true, message = "City Deleted" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred while deleting the city." });
            }
        }
    }
}