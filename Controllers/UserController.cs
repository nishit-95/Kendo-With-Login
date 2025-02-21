using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Kendo.Models;
using Kendo.Repositories.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Kendo.Controllers
{
    public class UserController : Controller
    {

        private readonly IUserInterface _user;

        public UserController(IUserInterface user)
        {
            _user = user;
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        [HttpGet]
        public ActionResult KendoLogin()
        {
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> KendoLogin([FromBody] Login user)
        {

            User UserData = await _user.Login(user);

            if (UserData != null && UserData.c_userId != 0)
            {
                HttpContext.Session.SetInt32("UserId", UserData.c_userId);
                HttpContext.Session.SetString("UserName", UserData.c_userName);
                HttpContext.Session.SetString("UserProfilePicture", UserData.c_Image ?? "user.png");

                return Json(new { success = true, message = "User logged in successfully!", redirectUrl = Url.Action("Index", "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid Username or Password" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Login(Login login)
        {
            // if (!ModelState.IsValid)
            // {
            //     return Json(new { success = false, message = "Invalid input. Please check your details." });
            // }

            User UserData = await _user.Login(login);

            if (UserData != null && UserData.c_userId != 0)
            {
                HttpContext.Session.SetInt32("UserId", UserData.c_userId);
                HttpContext.Session.SetString("UserName", UserData.c_userName);
                HttpContext.Session.SetString("UserProfilePicture", UserData.c_Image ?? "user.png");

                return Json(new { success = true, message = "User logged in successfully!", redirectUrl = Url.Action("Index", "Home") });
            }
            else
            {
                return Json(new { success = false, message = "Invalid Username or Password" });
            }
        }



        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        public ActionResult LogOut()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("KendoLogin", "User");
        }
        [HttpPost]
        public async Task<IActionResult> Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid input. Please check your details." });
            }


            if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
            {
                var fileName = user.c_Email + Path.GetExtension(user.ProfilePicture.FileName);
                var filePath = Path.Combine("./wwwroot/profile_images/", fileName);
                Directory.CreateDirectory(Path.Combine("./wwwroot/profile_images"));
                user.c_Image = fileName;
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    user.ProfilePicture.CopyTo(stream);
                }
            }


            // contact.c_userId = (int)HttpContext.Session.GetInt32("UserId");
            // Console.WriteLine("user.c_fname: " + user.c_userName);
            var status = await _user.Register(user);
            if (status == 1)
            {
                TempData["message"] = "User Registred";
                // return RedirectToAction("Login");
                return Json(new { success = true, message = "User Registerd in successfully!", redirectUrl = Url.Action("Login", "User") });
            }
            else if (status == 0)
            {
                return Json(new { success = false, message = "User Exist Already!!" });
            }
            else
            {
                TempData["message"] = "There was some error while Registration";
                return Json(new { success = false, message = TempData["message"] });

            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        [HttpGet]
        public IActionResult KendoRegister()
        {
            return View();
        }


        [HttpPost]
        public async Task<JsonResult> KendoRegister(User user)
        {
            try
            {
                if (user.ProfilePicture != null && user.ProfilePicture.Length > 0)
                {
                    var fileName = user.c_Email + Path.GetExtension(user.ProfilePicture.FileName);
                    var filePath = Path.Combine("wwwroot/profile_images", fileName);

                    Directory.CreateDirectory(Path.Combine("wwwroot/profile_images"));
                    user.c_Image = fileName;

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await user.ProfilePicture.CopyToAsync(stream);
                    }
                }

                // _user.Add(user);
                // return Json(new { success = true, message = "Registration Successful!!", redirectUrl = Url.Action("KendoLogin", "User") });
                int result = await _user.Register(user);

                if (result == 0)
                {
                    return Json(new { success = false, message = "Email already exists. Please use a different email." });
                }
                else if (result == 1)
                {
                    return Json(new { success = true, message = "Registration Successful!!", redirectUrl = Url.Action("KendoLogin", "User") });
                }
                else
                {
                    return Json(new { success = false, message = "Registration failed due to an unexpected error." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error: " + ex.Message });
            }
        }


    }
}