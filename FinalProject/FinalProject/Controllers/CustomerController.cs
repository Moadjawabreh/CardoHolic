using FinalProject.Data;
using FinalProject.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace FinalProject.Controllers
{
	public class CustomerController : Controller
	{
		private readonly AppDbContext _db;
		private readonly IWebHostEnvironment webHostEnvironment;
		public CustomerController(AppDbContext db, IWebHostEnvironment environment
)
		{
			_db = db;
			webHostEnvironment = environment;

		}

		public IActionResult Index()
		{
			//var query = _db.categoryContainers
			//    .SelectMany(container => container.Categories.SelectMany(category => category.Products
			//        .Where(product => product.percentageOfDiscount > 0)
			//        .Select(product => new
			//        {
			//            ContainerName = container.Name,
			//            CategoryName = category.Name,
			//            ProductName = product.Name,
			//            DiscountPercentage = product.percentageOfDiscount
			//        })
			//    ))
			//    .ToList();

			//         var categoryContainer = _db.categoryContainers
			//.Include(cc => cc.Categories)  // Include the Categories navigation property
			//.ThenInclude(cat => cat.Products.Where(product => product.percentageOfDiscount < 1))  // Include filtered Products
			//.ThenInclude(prod => prod.secretCodeForProducts)  // Include the SecretCodeForProducts for each filtered Product
			//.ToList();

			var categoryContainer = _db.categoryContainers
			 .Include(cc => cc.Categories)
			   .ThenInclude(cat => cat.Products
				   .Where(product => product.percentageOfDiscount < 1 && product.secretCodeForProducts.Any(sc => sc.status == 0)))  // Filtered Products with SecretCodeForProducts having Flag=false
					.ThenInclude(prod => prod.secretCodeForProducts)  // Include the SecretCodeForProducts for each filtered Product
					  .ToList();

			return View(categoryContainer);
		}

		public IActionResult Products(int id)
		{
			var categoryContainer = _db.categoryContainers
	  .Include(cc => cc.Categories)
		  .ThenInclude(cat => cat.Products
			  .Where(product => product.secretCodeForProducts.Any(sc => sc.status == 0)))  // Filtered Products with SecretCodeForProducts having Flag=false
		  .FirstOrDefault(cc => cc.Id == id);

			if (categoryContainer != null)
			{
				// categoryContainer now contains the specified CategoryContainer along with its Categories and Products
				// ...

				return View(categoryContainer);  // Pass categoryContainer to the view
			}

			// Handle the case where the specified CategoryContainer is not found
			return RedirectToAction("Index", "Home");  // Replace with your desired action and controller

		}

		[HttpPost]
		public IActionResult Products(int IdForCategory, int IdForContainerCategory)
		{

			var categoryContainer = _db.categoryContainers
	  .Include(cc => cc.Categories)
		  .ThenInclude(cat => cat.Products
			  .Where(product => product.secretCodeForProducts.Any(sc => sc.status==0)))  // Filtered Products with SecretCodeForProducts having Flag=false
		  .FirstOrDefault(cc => cc.Id == IdForContainerCategory);

			if (categoryContainer != null)
			{
				// categoryContainer now contains the specified CategoryContainer along with its Categories and Products
				// ...

				return View(categoryContainer);  // Pass categoryContainer to the view
			}

			// Handle the case where the specified CategoryContainer is not found
			return RedirectToAction("Index", "Home");  // Replace with your desired action and controller
		}




		public IActionResult Cart()
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");
			var userSession = JsonConvert.DeserializeObject<User>(userJson);

			var carts = _db.Cart
			.Include(c => c.product)
			.Where(c => c.UserId == userSession.ID && c.Flag == true && c.product.secretCodeForProducts.Any(sc => sc.status == 1))
			.ToList();
			return View(carts);
		}



		public IActionResult AddToCart(int id)
		{



			string? userJson = HttpContext.Session.GetString("LiveUser");



			if (!string.IsNullOrEmpty(userJson))
			{
				var userSession = JsonConvert.DeserializeObject<User>(userJson);
				var existCarts = _db.Cart
	.Where(cart => cart.UserId == userSession.ID && cart.productId == id && cart.Flag == true)
	.ToList();
				var singleProductUrl = TempData["SingleProductUrl"] as string;
				var product = _db.Products
	 .Include(p => p.secretCodeForProducts)
	 .FirstOrDefault(p => p.ID == id && p.secretCodeForProducts.Any(sc => sc.status==0));

				int codeNums = product.secretCodeForProducts.Count(sc => sc.status == 0);

				foreach (var item in existCarts)
				{
					if (item.productId == id)
					{
						if (codeNums >= 1)
						{
							var firstSecretCode = product.secretCodeForProducts.FirstOrDefault(sc => sc.status == 0);
							var firstSecretCodes = product.secretCodeForProducts
													.Where(sc => sc.status == 0)
													.Take(1);

							foreach (var secretCode in firstSecretCodes)
							{
								secretCode.status = 1;
							}
							item.Quantity = item.Quantity + 1;
							item.Flag = true;
							_db.Cart.Update(item);
							_db.SaveChanges();
							TempData["success"] = "Great news! You've successfully added the item to your cart. Happy shopping!";
							return Redirect(string.IsNullOrEmpty(singleProductUrl) ? "/Customer/Index" : singleProductUrl);
						}
						else
						{
							TempData["error"] = "Bad news! This item is sold out, don't worry we will check it again!";
							return Redirect(string.IsNullOrEmpty(singleProductUrl) ? "/Customer/Index" : singleProductUrl);

						}


					}

				}


				var product2 = _db.Products
	 .Include(p => p.secretCodeForProducts)
	 .FirstOrDefault(p => p.ID == id && p.secretCodeForProducts.Any(sc => sc.status == 0));

				int availableCodes = product2.secretCodeForProducts.Count(sc => sc.status == 0);



				Cart cart = new Cart();
				cart.productId = id;
				cart.UserId = userSession.ID;

				if (1 <= availableCodes)
				{
					var firstSecretCodes = product2.secretCodeForProducts
						.Where(sc => sc.status == 0)
						.Take(1);

					foreach (var secretCode in firstSecretCodes)
					{
						secretCode.status = 1;
					}
					cart.Quantity = 1;
					cart.Flag = true;
					_db.Add(cart);
					_db.SaveChanges();

					TempData["success"] = "Great news! You've successfully added the item to your cart. Happy shopping!";

				}
				var singleProductUrll = TempData["SingleProductUrl"] as string;
				return Redirect(string.IsNullOrEmpty(singleProductUrll) ? "/Customer/Index" : singleProductUrll);
			}


			return RedirectToAction("Login", "LoginAndRegister");







		}

		[HttpPost]
		public IActionResult AddToCart(int productId, int quantity)
		{

			string? userJson = HttpContext.Session.GetString("LiveUser");



			if (!string.IsNullOrEmpty(userJson))
			{
				var userSession = JsonConvert.DeserializeObject<User>(userJson);
                var existCarts = _db.Cart
				.Where(cart => cart.UserId == userSession.ID && cart.productId == productId && cart.Flag == true)
				 .ToList();
                var singleProductUrl = TempData["SingleProductUrl"] as string;
				var product = _db.Products
				 .Include(p => p.secretCodeForProducts)
				 .FirstOrDefault(p => p.ID == productId && p.secretCodeForProducts.Any(sc => sc.status==0));

				int codeNums = product.secretCodeForProducts.Count(sc => sc.status == 0);

				foreach (var item in existCarts)
				{
					if (item.productId == productId)
					{
						if (codeNums >= quantity)
						{
							var firstSecretCode = product.secretCodeForProducts.FirstOrDefault(sc => sc.status == 0);
							var firstSecretCodes = product.secretCodeForProducts
													.Where(sc => sc.status == 0 )
													.Take(quantity);

							foreach (var secretCode in firstSecretCodes)
							{
								secretCode.status = 1;
							}
							item.Quantity = item.Quantity + quantity;
							item.Flag = true;
							_db.Cart.Update(item);
							_db.SaveChanges();
							TempData["success"] = "Great news! You've successfully added the item to your cart. Happy shopping!";
							return Redirect(string.IsNullOrEmpty(singleProductUrl) ? "/Customer/Index" : singleProductUrl);
						}
						else
						{
							TempData["error"] = "Bad news! This item is sold out, don't worry we will check it again!";
							return Redirect(string.IsNullOrEmpty(singleProductUrl) ? "/Customer/Index" : singleProductUrl);

						}


					}

				}


				var product2 = _db.Products
			.Include(p => p.secretCodeForProducts)  // Include SecretCodeForProducts
			.FirstOrDefault(p => p.ID == productId);

				int availableCodes = product2.secretCodeForProducts.Count(sc => sc.status == 0);



				Cart cart = new Cart();
				cart.productId = productId;
				cart.UserId = userSession.ID;

				if (quantity <= availableCodes)
				{
					var firstSecretCodes = product2.secretCodeForProducts
						.Where(sc => sc.status == 0)
						.Take(quantity);

					foreach (var secretCode in firstSecretCodes)
					{
						secretCode.status = 1;
					}
					cart.Quantity = quantity;
					cart.Flag = true;
					_db.Add(cart);
					_db.SaveChanges();
					TempData["success"] = "Great news! You've successfully added the item to your cart. Happy shopping!";

				}
				var singleProductUrll = TempData["SingleProductUrl"] as string;
				return Redirect(string.IsNullOrEmpty(singleProductUrll) ? "/Customer/Index" : singleProductUrll);
			}


			return RedirectToAction("Login", "LoginAndRegister");

		}


		public IActionResult DeleteCart(int id)
		{
			var cart = _db.Cart.Find(id);

			if (cart != null)
			{
				var productId = cart.productId;

                // Load the product with secretCodeForProducts
                //var product = _db.Products
                //	.Include(p => p.secretCodeForProducts)
                //	.FirstOrDefault(p => p.ID == productId);
                var product = _db.Products
          .Include(p => p.secretCodeForProducts.Where(s => s.status == 1))
          .FirstOrDefault(p => p.ID == productId);

                if (product != null && product.secretCodeForProducts != null)
				{
					foreach (var secretCode in product.secretCodeForProducts)
					{
						secretCode.status = 0;

					}


				}

			}
			cart.Flag = true;
			cart.Quantity = 0;
			_db.Update(cart);
			_db.SaveChanges();
			return RedirectToAction("Cart");
		}



		public IActionResult Checkout(string password, string cardNumber)
		{
			var CardPayments = _db.Payments.ToList();

			bool credentialsExist = CardPayments.Any(payment =>
	   payment.Password == password && payment.cardNo == cardNumber);

			// If the credentials exist, you can proceed with your logic
			if (credentialsExist)
			{
				string? userJson = HttpContext.Session.GetString("LiveUser");

				if (!string.IsNullOrEmpty(userJson))
				{
					var userSession = JsonConvert.DeserializeObject<User>(userJson);
					var userCarts = _db.Cart
	.Where(c => c.UserId == userSession.ID && c.Flag == true)  // Filter based on user ID and Flag
	.Include(c => c.product)  // Include the products for each cart
	.ToList();

					Order order = new Order();

					double totalPrice = 0;
					double cost = 0;
					foreach (var cart in userCarts)
					{
						if (cart.product.percentageOfDiscount < 1)
						{
							totalPrice += cart.Quantity * cart.product.Price * (1 - cart.product.percentageOfDiscount);
						}
						else
						{
							totalPrice += cart.Quantity * cart.product.Price * cart.product.percentageOfDiscount;

						}
						cost += cart.product.Cost * cart.Quantity;
					}
					//ViewBag.orders = _db.Users.Select(
					//	x=> new
					//	{
					//		orders=_db.Orders.Where(u=>u.userId==userSession.ID),
					//                       carts = _db.Cart.Where(u => u.UserId == userSession.ID),

					//                   }

					//                   );


					//var CartsForOrder = _db.Cart
					//.Where(c => c.UserId == userSession.ID && c.OrderId == null)
					//	.Include(c => c.product)
					//		.ThenInclude(product => product.secretCodeForProducts)
					//			.ToList();

					//				var CartsForOrder = _db.Cart
					//.Where(c => c.UserId == userSession.ID && c.OrderId == null && c.Flag == true)
					//.Include(c => c.product)
					//	.ThenInclude(product => product.secretCodeForProducts)
					//.ToList();

					//				var CartsForOrder = _db.Cart
					//.Where(c => c.UserId == userSession.ID && c.OrderId == null && c.Flag == true)
					//.Include(c => c.product)
					//	.ThenInclude(product => product.secretCodeForProducts.Where(sc => sc.Flag == true))
					//.ToList();

					var CartsForOrder = _db.Cart
	.Where(c => c.UserId == userSession.ID && c.OrderId == null && c.Flag == true)
	.Include(c => c.product)
		.ThenInclude(product => product.secretCodeForProducts
			.Where(sc => sc.status == 1))
	.ToList();
					order.Name = userSession.Name;
					order.Email = userSession.Email;
					order.password = userSession.Password;
					order.Phone = userSession.phoneNumber;
					order.userId = userSession.ID;
					order.Cost = cost;
					order.Date = DateTime.Now;
					order.Total = totalPrice;
					order.Carts = CartsForOrder;
					order.card = cardNumber;
					order.Location = "sadas";
					order.Phone = "0789";
					_db.Orders.Add(order);
					_db.SaveChanges();

					foreach (var cart in order.Carts)
					{
						foreach(var sc in cart.product.secretCodeForProducts)
						{
							sc.status = 2;
						}
						cart.OrderId = order.ID;
						cart.Flag = false;
						_db.Update(cart);
						_db.SaveChanges();
					}

					TempData["success"] = "Congratulations! Your order has been successfully placed. Thank you for choosing us. Happy shopping!";
					return RedirectToAction("Cart", "Customer");
				}
			}
			else
			{
				TempData["error"] = "Invalid password or card number. Please try again.";
				return RedirectToAction("Cart", "Customer");
			}


			return View();
		}


		public IActionResult Orders()
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");

			if (!string.IsNullOrEmpty(userJson))
			{
				var userSession = JsonConvert.DeserializeObject<User>(userJson);

				//1
				//	var orders = _db.Orders
				//.Include(o => o.user)
				//	.ThenInclude(u => u.carts)
				//		.ThenInclude(cart => cart.product)  // Include the products for each cart
				//			.ThenInclude(product => product.secretCodeForProducts)  // Include the secretCodeForProducts for each product
				//.Where(u => u.userId == userSession.ID)
				//.ToList();


				//2
				//			var orders = _db.Orders
				//.Include(o => o.user)
				//	.ThenInclude(u => u.carts)
				//		.ThenInclude(cart => cart.product)  // Include the products for each cart
				//			.ThenInclude(product => product.secretCodeForProducts)  // Include the secretCodeForProducts for each product
				//.Where(o => o.userId == userSession.ID && o.Carts.Any(c => c.product.secretCodeForProducts.Any(sc => sc.Flag == true)))
				//.ToList();


				//3 it have the true only but it will add them together even if i did different orders

				//			var orders = _db.Orders
				//.Include(o => o.user)
				//			//	.ThenInclude(u => u.carts)
				//		.ThenInclude(cart => cart.product)  // Include the products for each cart
				//			.ThenInclude(product => product.secretCodeForProducts.Where(sc => sc.Flag == true))  // Filter secretCodeForProducts
				//.Where(o => o.userId == userSession.ID && o.Carts.Any(c => c.product.secretCodeForProducts.Any(sc => sc.Flag == true)))
				//.ToList();

				//4  it's separate the orders but each order have the all secret numbers if it true and false


				//			var orders = _db.Orders
				//.Include(o => o.user)
				//.ThenInclude(u => u.carts)
				//.ThenInclude(cart => cart.product)
				//	.ThenInclude(product => product.secretCodeForProducts.Where(sc => sc.Flag == true))  // Filter secretCodeForProducts
				//.Where(o => o.userId == userSession.ID)
				//.ToList();

				//		var orders = _db.Orders
				//.Include(o => o.Carts)
				//	.ThenInclude(cart => cart.product)
				//		.ThenInclude(product => product.secretCodeForProducts.Where(sc => sc.Flag == true))
				//.Where(o => o.userId == userSession.ID)
				//.ToList();

				var orders = _db.Orders.Where(o => o.userId == userSession.ID).ToList();


				//foreach (var order in orders)
				//{
				//	var carts = new HashSet<Cart>();
				//	foreach (var cart in order.user.carts)
				//	{
				//		if (cart.Flag == false)
				//		{

				//			carts.Add(cart);
				//		}

				//	}
				//	order.user.carts = carts;
				//}

				return View(orders);
			}
			else
			{

			}
			return View();
		}
		public IActionResult OrderDetails(int orderId)
		{
			//var order = _db.Orders
			// .Include(a => a.user)
			//  .ThenInclude(u => u.carts)
			//   .ThenInclude(cart => cart.product)  // Include the products for each cart
			//	   .ThenInclude(product => product.secretCodeForProducts)  // Include the secretCodeForProducts for each product
			// .Include(o => o.Carts) // Include the carts directly for the specified order
			// .SingleOrDefault(o => o.ID == orderId);

			//var order = _db.Orders
			// .Include(a => a.user)
			//  .ThenInclude(u => u.carts)
			//   .ThenInclude(cart => cart.product)  // Include the products for each cart
			//	   .ThenInclude(product => product.secretCodeForProducts)  // Include the secretCodeForProducts for each product
			// .Include(o => o.Carts) // Include the carts directly for the specified order
			//  .ThenInclude(cart => cart.product)  // Include the products for each cart in the specified order
			//   .ThenInclude(product => product.secretCodeForProducts)  // Include the secretCodeForProducts for each product in the specified order
			// .SingleOrDefault(o => o.ID == orderId);


			var order = _db.Orders
	   .Include(a => a.user)
		   .ThenInclude(u => u.carts)
			   .ThenInclude(cart => cart.product)  // Include the products for each cart
				   .ThenInclude(product => product.secretCodeForProducts.Where(sc => sc.status==2))  // Filter secretCodeForProducts
	   .Include(o => o.Carts) // Include the carts directly for the specified order
		   .ThenInclude(cart => cart.product)  // Include the products for each cart in the specified order
			   .ThenInclude(product => product.secretCodeForProducts.Where(sc => sc.status == 2))  // Filter secretCodeForProducts in the specified order
	   .SingleOrDefault(o => o.ID == orderId);


			return View(order);
		}

		public IActionResult About()
		{
			var feedBacks = _db.FeedbackForWebs.Include(c => c.User).Where(u => u.Status == true).ToList();

			return View(feedBacks);
		}


		public IActionResult AddFeedBackForWeb()
		{
			return View();
		}
		[HttpPost]
		public IActionResult AddFeedBackForWeb(string msg)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");

			if (!string.IsNullOrEmpty(userJson))
			{
				// Now you can use 'userSession' object

				var userSession = JsonConvert.DeserializeObject<User>(userJson);
				var FeedBack = new FeedbackForWeb();
				FeedBack.Text = msg;
				FeedBack.Status = false;
				FeedBack.userID = userSession.ID;
				_db.FeedbackForWebs.Add(FeedBack);
				_db.SaveChanges();
				return RedirectToAction("About");
			}
			return RedirectToAction("Login", "LoginAndRegister");

		}
		public IActionResult Contact()
		{
			return View();
		}




		public IActionResult SingleProduct(int id)
		{
			TempData.Remove("SingleProductUrl");
			TempData.Remove("GoBackToSingleProductUrl");
			//	var product = _db.Products
			//.Include(p => p.FeedbackForProducts)
			//	.ThenInclude(f => f.User)
			//.Include(p => p.Category) // Include the Category for the product
			//	.ThenInclude(c => c.CategoryContainer) // Include the CategoryContainer for the Category
			//.FirstOrDefault(p => p.ID == id);
			string? userJson = HttpContext.Session.GetString("LiveUser");



			var product = _db.Products
		.Include(p => p.FeedbackForProducts)
		.ThenInclude(f => f.User)
		.Include(p => p.Category) // Include the Category for the product
		.ThenInclude(c => c.CategoryContainer) // Include the CategoryContainer for the Category
		.Include(p => p.secretCodeForProducts)  // Include SecretCodeForProducts
		.FirstOrDefault(p => p.ID == id && p.secretCodeForProducts.Any(sc => sc.status == 0));



			if (product != null)
			{
				ViewBag.RelatedProducts = _db.Products
				.Where(p => p.categoryID == product.categoryID &&
					!p.secretCodeForProducts.Any(s => s.status==2))
						.ToList();

				return View(product);
			}


			return RedirectToAction("Products");
		}



		public IActionResult AddReview()
		{
			return RedirectToAction("SingleProduct");
		}

		[HttpPost]
		public IActionResult AddReviewForProduct(int id, string reviewInput)
		{
			string? userJson = HttpContext.Session.GetString("LiveUser");

			if (!string.IsNullOrEmpty(userJson))
			{
				// Now you can use 'userSession' object

				var userSession = JsonConvert.DeserializeObject<User>(userJson);

				int productId = id;
				FeedbackForProduct feedback = new FeedbackForProduct();
				feedback.productID = productId;
				feedback.Text = reviewInput;
				feedback.userID = userSession.ID;
				_db.Add(feedback);
				_db.SaveChanges();
				return RedirectToAction("SingleProduct", new { id = productId });

			}
			else
			{
				return RedirectToAction("Login", "LoginAndRegister");
				// Handle the case where the JSON string is null or empty
			}

		}



		public IActionResult Profile()
		{
			TempData.Remove("ReturnUrl");
			string? userJson = HttpContext.Session.GetString("LiveUser");
			if (userJson == null)
			{
				TempData["ReturnUrl"] = Url.Action("Profile", "Customer");
				return RedirectToAction("Index", "Login");
			}
			else
			{
				var userSession = JsonConvert.DeserializeObject<User>(userJson);
				var user = _db.Users.Find(userSession.ID);
				return View(user);
			}
		}

		[HttpPost]
		public IActionResult Profile(User user)
		{
			if (user.ImageFile != null)
			{
				string wwwRootPath = webHostEnvironment.WebRootPath;
				string fileName = Guid.NewGuid().ToString() + "" +
				user.ImageFile.FileName;
				string path = Path.Combine(wwwRootPath + "/Images/", fileName);
				using (var fileStream = new FileStream(path, FileMode.Create))
				{
					user.ImageFile.CopyTo(fileStream);
				}
				user.ImageUrl = "/Images/" + fileName;
			}
			string? userJson = HttpContext.Session.GetString("LiveUser");
			var userSession = JsonConvert.DeserializeObject<User>(userJson);
			if (userSession.Role == Role.Admin)
			{
				user.Role = Role.Admin;
			}
			else
			{
				user.Role = Role.User;
			}
			_db.Update(user);
			_db.SaveChanges();
			TempData["success"] = "Great news! You've successfully Edited Your Profile!";
			return View(user);
		}
	}
}
