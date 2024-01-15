using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace FinalProject.Controllers
{
	public class LoginAndRegisterController : Controller
	{
		private readonly AppDbContext _db;

		public LoginAndRegisterController(AppDbContext db)
		{
			_db = db;
		}
		public IActionResult Register()
		{
			return View();
		}

		[HttpPost]
		public IActionResult Register(User user)
		{
			var existingUser = _db.Users.FirstOrDefault(u => u.Email == user.Email);

			if (existingUser == null)
			{
				// User does not exist, proceed with registration
				user.Role = Role.User;
				_db.Add(user);
				_db.SaveChanges();
				var liveUser = JsonSerializer.Serialize(user);
				HttpContext.Session.SetString("LiveUser", liveUser);
				TempData["success"] = "Great news! You've successfully Registered to your website !";
				return RedirectToAction("Index", "Customer"); // Redirect to a success page or another action
			}
			else
			{
				// User already exists, handle accordingly (e.g., show an error message)
				TempData["error"] = "This Email is already used, please login";
				ModelState.AddModelError("Email", "This email is already registered.");
				return View(); // Return to the registration view with an error message
			}

		}

		public IActionResult Login()
		{
			return View();
		}


		[HttpPost]
		public IActionResult Login(string Email, string Password)
		{
			var user = _db.Users.FirstOrDefault(u => u.Email == Email);

			if (user != null && Password == user.Password)
			{
				if (user.Role == Role.User)
				{
					var singleProductUrl = TempData["GoBackToSingleProductUrl"] as string;

					if (singleProductUrl != null)
					{
						var liveUser = JsonSerializer.Serialize(user);
						HttpContext.Session.SetString("LiveUser", liveUser);
						TempData["success"] = "Great news! You've successfully sign in to your website !";
						return Redirect(string.IsNullOrEmpty(singleProductUrl) ? "/Customer/Index" : singleProductUrl);
					}
					else
					{
						var liveUser = JsonSerializer.Serialize(user);
						HttpContext.Session.SetString("LiveUser", liveUser);
						TempData["success"] = "Great news! You've successfully sign in to your website !";
						return RedirectToAction("Index", "Customer");
					}

				}
				else
				{
					var singleProductUrl = TempData["GoBackToSingleProductUrl"] as string;
					if (singleProductUrl != null)
					{
						var liveUser = JsonSerializer.Serialize(user);
						HttpContext.Session.SetString("LiveUser", liveUser);
						TempData["success"] = "Great news! You've successfully sign in to your website !";
						return Redirect(string.IsNullOrEmpty(singleProductUrl) ? "/Customer/Index" : singleProductUrl);
					}
					else
					{
						var liveUser = JsonSerializer.Serialize(user);
						HttpContext.Session.SetString("LiveUser", liveUser);
						TempData["success"] = "Great news! You've successfully sign in to your website !";
						return RedirectToAction("Index", "Admin");
					}

				}

			}
			else
			{


				ModelState.AddModelError("Email", "Email or Password are invalid!");
				TempData["error"] = "Invalid Email or Please Try Again";

				return View();
			}

		}





		public IActionResult Logout()
		{
			HttpContext.Session.Remove("LiveUser");
			TempData["success"] = "You've successfully logged out. Come back soon!";
			return RedirectToAction("Index", "Customer");
		}






	}
}
