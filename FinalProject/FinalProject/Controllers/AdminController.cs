using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace FinalProject.Controllers
{
	public class AdminController : Controller
	{

		private AppDbContext _context;
		private readonly IWebHostEnvironment webHostEnvironment;
		public AdminController(AppDbContext context, IWebHostEnvironment environment)
		{
			_context = context;
			webHostEnvironment = environment;

		}


		public IActionResult Index()
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("Index", "Admin");
				return RedirectToAction("Index", "Login");
			}
			else
			{
				var user = JsonConvert.DeserializeObject<User>(userJson);
				if (user.Role == Role.Admin)
				{


					ViewBag.Customers = _context.Users.Where(u => u.Role == Role.User).Count();
					ViewBag.Oreders = _context.Orders.Count();
					ViewBag.Total = _context.Orders.Sum(order => order.Total);
					ViewBag.TotalReveneu = 0;

					if (_context.Orders.Count() > 0)
					{
						//ViewBag.TotalReveneu = _context.Products.Sum(prodcut => prodcut.Cost * prodcut.percentageOfDiscount) - _context.Orders.Sum(order => order.Total);
						ViewBag.TotalReveneu = _context.Orders.Sum(o => o.Total) - _context.Orders.Sum(o => o.Cost);
					}
					var orders = _context.Orders.ToList();
					return View(orders);
				}
				else
				{
					return RedirectToAction("Index", "Customer");
				}
			}
		}







		public IActionResult CategoryContainer()
		{
			List<CategoryContainer>? categoryContainers = _context.categoryContainers.ToList();

			return View(categoryContainers);
		}



		public IActionResult CreateCategoryContainer()
		{
			return View();
		}

		[HttpPost]
		public IActionResult CreateCategoryContainer(CategoryContainer categoryContainer)
		{

			if (categoryContainer != null)
			{
				if (categoryContainer.ImageFile != null)
				{
					string wwwRootPath = webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + "" +
					categoryContainer.ImageFile.FileName;
					string path = Path.Combine(wwwRootPath + "/Images/", fileName);
					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						categoryContainer.ImageFile.CopyTo(fileStream);
					}
					categoryContainer.Image = "/Images/" + fileName;

				}

				_context.categoryContainers.Add(categoryContainer);
				_context.SaveChanges();
				return RedirectToAction("CategoryContainer");
			}
			return View();
		}


		public IActionResult DeleteCategoryContainer(int? id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("DeleteCategories", "Admin");
				return RedirectToAction("Index", "Login");
			}

			CategoryContainer categoryContainer = _context.categoryContainers.FirstOrDefault(c => c.Id == id);
			_context.categoryContainers.Remove(categoryContainer);
			_context.SaveChanges();
			return RedirectToAction("CategoryContainer");

		}

		public IActionResult UpdateCategoryContainer(int? id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("UpdateCategoryContainer", "Admin");
				return RedirectToAction("Index", "Login");
			}
			CategoryContainer categoryContainer = _context.categoryContainers.FirstOrDefault(c => c.Id == id);

			return View(categoryContainer);




		}

		[HttpPost]
		public IActionResult UpdateCategoryContainer(CategoryContainer categoryContainer)
		{
			if (categoryContainer != null)
			{
				if (categoryContainer.ImageFile != null)
				{
					string wwwRootPath = webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + "" +
					categoryContainer.ImageFile.FileName;
					string path = Path.Combine(wwwRootPath + "/Images/", fileName);
					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						categoryContainer.ImageFile.CopyTo(fileStream);
					}
					categoryContainer.Image = "/Images/" + fileName;

				}

				_context.categoryContainers.Update(categoryContainer);
				_context.SaveChanges();
				return RedirectToAction("CategoryContainer");
			}
			return View();

		}


		// ------------------------------------------------------ Categories CRUD


		public IActionResult Categories()
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("Categories", "Admin");
				return RedirectToAction("Index", "Login");
			}
			else
			{
				List<Category>? categories = _context.Categories
						  .Include(p => p.CategoryContainer)
						  .ToList();
				return View(categories);
			}
		}



		[HttpPost]
		public IActionResult Categories(string SearchItem)
		{

			if (!string.IsNullOrEmpty(SearchItem))
			{
				List<Category> Categories = _context.Categories
				 .Include(s => s.CategoryContainer)  // Include the related Product entity
				   .Where(u => u.CategoryContainer.Name.Contains(SearchItem))
					  .ToList();
				return View(Categories);
			}

			List<Category> allCategories = _context.Categories.Include(p => p.CategoryContainer)
				.ToList();
			return View(allCategories);


		}


		// ------- Create
		public IActionResult CreatCategories()
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("CreatCategories", "Admin");
				return RedirectToAction("Index", "Login");
			}

			IEnumerable<SelectListItem> categoryContainers = _context.categoryContainers.ToList().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.Id.ToString()
			});


			ViewBag.categoryContainersList = categoryContainers;

			return View();
		}

		[HttpPost]
		public IActionResult CreatCategories(Category category)
		{


			if (category != null)
			{
				if (category.ImageFile != null)
				{
					string wwwRootPath = webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + "" +
					category.ImageFile.FileName;
					string path = Path.Combine(wwwRootPath + "/Images/", fileName);
					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						category.ImageFile.CopyTo(fileStream);
					}
					category.Image = "/Images/" + fileName;

				}



				_context.Categories.Add(category);
				_context.SaveChanges();
				return RedirectToAction("Categories");
			}
			return View();
		}
		// ------- End Create




		// ------- Delete

		public IActionResult DeleteCategories(int? id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("DeleteCategories", "Admin");
				return RedirectToAction("Index", "Login");
			}

			Category category = _context.Categories.FirstOrDefault(c => c.ID == id);
			_context.Categories.Remove(category);
			_context.SaveChanges();
			return RedirectToAction("Categories");

		}

		// ------- End Delete


		// ------- Update

		public IActionResult UpdateCategories(int? id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("UpdateCategories", "Admin");
				return RedirectToAction("Index", "Login");
			}
			Category category = _context.Categories.FirstOrDefault(c => c.ID == id);
			ViewBag.categoryContainersList = _context.categoryContainers.ToList();

			return View(category);




		}

		[HttpPost]
		public IActionResult UpdateCategories(Category category)
		{
			if (category != null)
			{
				if (category.ImageFile != null)
				{
					string wwwRootPath = webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + "" +
					category.ImageFile.FileName;
					string path = Path.Combine(wwwRootPath + "/Images/", fileName);
					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						category.ImageFile.CopyTo(fileStream);
					}
					category.Image = "/Images/" + fileName;

				}

				_context.Categories.Update(category);
				_context.SaveChanges();
				return RedirectToAction("Categories");
			}
			return View();

		}

		// ------- End Update



		// ------------------------------------------------------ END Categories CRUD






		// ------------------------------------------------------ Products CRUD



		public IActionResult Products()
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("Products", "Admin");
				return RedirectToAction("Index", "Login");
			}
			List<Product>? products = _context.Products
				.Include(p => p.Category)
				.ToList();
			return View(products);
		}




		[HttpPost]
		public IActionResult Products(string SearchItem)
		{

			if (!string.IsNullOrEmpty(SearchItem))
			{
				List<Product> products = _context.Products
				 .Include(s => s.Category)  // Include the related Product entity
				   .Where(u => u.Category.Name.Contains(SearchItem))
					  .ToList();
				return View(products);
			}

			List<Product> allProducts = _context.Products.Include(p => p.Category)
				.ToList();
			return View(allProducts);


		}






		// ------- Create
		public IActionResult CreatProduct()
		{

			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("CreatProduct", "Admin");
				return RedirectToAction("Index", "Login");
			}
			IEnumerable<SelectListItem> categories = _context.Categories.ToList().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.ID.ToString()
			});


			ViewBag.categoriesList = categories;

			return View();
		}

		[HttpPost]
		public IActionResult CreatProduct(Product product)
		{
			if (product != null)
			{
				if (product.ImageFile != null)
				{
					string wwwRootPath = webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + "" +
					product.ImageFile.FileName;
					string path = Path.Combine(wwwRootPath + "/Images/", fileName);
					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						product.ImageFile.CopyTo(fileStream);
					}
					product.UrlImage = "/Images/" + fileName;

				}



				_context.Products.Add(product);
				_context.SaveChanges();
				return RedirectToAction("Products");

			}
			return View();
		}
		// ------- End Create



		// ------- Delete

		public IActionResult DeleteProduct(int? id)
		{

			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("DeleteProduct", "Admin");
				return RedirectToAction("Index", "Login");
			}
			Product product = _context.Products.FirstOrDefault(c => c.ID == id);
			_context.Products.Remove(product);
			_context.SaveChanges();
			return RedirectToAction("Products");

		}

		// ------- End Delete

		// ------- Update

		public IActionResult UpdateProduct(int? id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("UpdateProduct", "Admin");
				return RedirectToAction("Index", "Login");
			}
			Product product = _context.Products.FirstOrDefault(c => c.ID == id);


			//IEnumerable<SelectListItem> categories = _context.Categories.ToList().Select(u => new SelectListItem
			//{
			//    Text = u.Name,
			//    Value = product.categoryID.ToString()
			//});

			ViewBag.categoriesList = _context.Categories.ToList();
			return View(product);
		}

		[HttpPost]
		public IActionResult UpdateProduct(Product product)
		{

			if (product != null)
			{
				if (product.ImageFile != null)
				{
					string wwwRootPath = webHostEnvironment.WebRootPath;
					string fileName = Guid.NewGuid().ToString() + "" +
					product.ImageFile.FileName;
					string path = Path.Combine(wwwRootPath + "/Images/", fileName);
					using (var fileStream = new FileStream(path, FileMode.Create))
					{
						product.ImageFile.CopyTo(fileStream);
					}
					product.UrlImage = "/Images/" + fileName;

				}



				_context.Products.Update(product);
				_context.SaveChanges();
				return RedirectToAction("Products");

			}
			return View();

		}



		// ------------------------------------------------------ END Products CRUD




		public IActionResult Codes()
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("Codes", "Admin");
				return RedirectToAction("Index", "Login");
			}
			List<SecretCodeForProduct>? secretCodeForProducts = _context.SecretCodeForProducts
				.Include(p => p.Product)
				.ToList();
			return View(secretCodeForProducts);
		}



		[HttpPost]
		public IActionResult Codes(string SearchItem)
		{

			if (!string.IsNullOrEmpty(SearchItem))
			{
				List<SecretCodeForProduct> secretCodeForProduct = _context.SecretCodeForProducts
				 .Include(s => s.Product)  // Include the related Product entity
				   .Where(u => u.Product.Name.Contains(SearchItem))
					  .ToList();
				return View(secretCodeForProduct);
			}

			List<SecretCodeForProduct> allSecretCodeForProduct = _context.SecretCodeForProducts.Include(p => p.Product)
				.ToList();
			return View(allSecretCodeForProduct);


		}


		public IActionResult CreatCode()
		{

			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("CreatCode", "Admin");
				return RedirectToAction("Index", "Login");
			}
			IEnumerable<SelectListItem> products = _context.Products.ToList().Select(u => new SelectListItem
			{
				Text = u.Name,
				Value = u.ID.ToString()
			});


			ViewBag.ProductsList = products;

			return View();
		}




		[HttpPost]
		public IActionResult CreatCode(SecretCodeForProduct secretCodeForProduct)
		{
			if (secretCodeForProduct != null)
			{
				_context.SecretCodeForProducts.Add(secretCodeForProduct);
				_context.SaveChanges();
				return RedirectToAction("Codes");

			}
			return View();
		}






		public IActionResult DeleteCode(int? id)
		{

			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("DeleteCode", "Admin");
				return RedirectToAction("Index", "Login");
			}
			SecretCodeForProduct secretCodeForProduct = _context.SecretCodeForProducts.FirstOrDefault(c => c.Id == id);
			_context.SecretCodeForProducts.Remove(secretCodeForProduct);
			_context.SaveChanges();
			return RedirectToAction("Codes");

		}





		public IActionResult UpdateCode(int? id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("UpdateCode", "Admin");
				return RedirectToAction("Index", "Login");
			}
			SecretCodeForProduct secretCodeForProduct = _context.SecretCodeForProducts.FirstOrDefault(c => c.Id == id);


			//IEnumerable<SelectListItem> categories = _context.Categories.ToList().Select(u => new SelectListItem
			//{
			//    Text = u.Name,
			//    Value = product.categoryID.ToString()
			//});

			ViewBag.ProductsList = _context.Products.ToList();
			return View(secretCodeForProduct);
		}


		//IEnumerable<SelectListItem> categories = _context.Categories.ToList().Select(u => new SelectListItem
		//{
		//    Text = u.Name,
		//    Value = product.categoryID.ToString()
		//});







		[HttpPost]
		public IActionResult UpdateCode(SecretCodeForProduct secretCodeForProduct)
		{

			if (secretCodeForProduct != null)
			{


				_context.SecretCodeForProducts.Update(secretCodeForProduct);
				_context.SaveChanges();
				return RedirectToAction("Codes");

			}
			return View();

		}











		// -------------------------------------------------------------------users
		public IActionResult Users()
		{

			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("Users", "Admin");
				return RedirectToAction("Index", "Login");
			}
			List<User> Users = _context.Users.ToList();
			return View(Users);
		}

		[HttpPost]
		public IActionResult Users(string SearchItem)
		{

			if (!string.IsNullOrEmpty(SearchItem))
			{
				List<User> searchUsers = _context.Users.Where(u => u.Name.Contains(SearchItem)).ToList();
				return View(searchUsers);
			}

			List<User> allUsers = _context.Users.ToList();
			return View(allUsers);


		}

		//--------------------------------------------------------------- Testimonials


		public IActionResult Testimonials()
		{

			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("Testimonials", "Admin");
				return RedirectToAction("Index", "Login");
			}
			List<FeedbackForWeb> FeedbackForWeb = _context.FeedbackForWebs
				.Include(p => p.User).Where(p => p.Status == false)
				.ToList();
			return View(FeedbackForWeb);
		}

		public IActionResult DeleteTestimonials(int id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("DeleteTestimonials", "Admin");
				return RedirectToAction("Index", "Login");
			}
			var feedback = _context.FeedbackForWebs.Find(id);
			_context.FeedbackForWebs.Remove(feedback);
			_context.SaveChanges();
			return RedirectToAction("Testimonials");

		}

		public IActionResult ApproveTestimonials(int id)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("ApproveTestimonials", "Admin");
				return RedirectToAction("Index", "Login");
			}
			var feedback = _context.FeedbackForWebs.Find(id);
			feedback.Status = true;
			_context.FeedbackForWebs.Update(feedback);
			_context.SaveChanges();
			return RedirectToAction("Testimonials");

		}
	}
}
