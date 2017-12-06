using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppAuctionSystem.Controllers
{
    public class ProductController : Controller
    {
        AuthController authController;
        ProductServiceReference.ProductServiceClient productServiceClient;
        public ProductController()
        {
            authController = new AuthController();

            productServiceClient = new ProductServiceReference.ProductServiceClient();
        }

        [HttpGet]
        public ActionResult Index()
        {
            if (authController.IsUserLoggedIn(Request, Response))
            {
                ViewBag.userId = authController.GetUserIdByCookie(Request.Cookies["auth"]);
                var products = productServiceClient.GetAllProducts();
                var availableProducts = new List<ProductServiceReference.ProductDto>();
                foreach (var product in products)
                {
                    if (product.IsAvailable)
                        availableProducts.Add(product);
                }

                ViewBag.products = availableProducts;

                return View("Catalog");
            }
            else
            {
                return Redirect("~/Auth/Login");
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            if (authController.IsUserLoggedIn(Request, Response))
            {
                if (authController.IsUserAdmin(Request))
                {
                    var userId = authController.GetUserIdByCookie(Request.Cookies["auth"]);
                    @ViewBag.userId = userId;
                    return View("Create");
                }
            }
            return Redirect("~/Auth/Login");
        }

        [HttpGet]
        public ActionResult Show(int productId)
        {
            if (authController.IsUserLoggedIn(Request, Response))
            {
                var userId = authController.GetUserIdByCookie(Request.Cookies["auth"]);
                @ViewBag.userId = userId;
                try
                {
                    var product = productServiceClient.GetProductById(productId);
                    if (product != null)
                    {
                        ViewBag.description = product.Description;
                        ViewBag.price = product.Price;
                        ViewBag.name = product.Name;
                        ViewBag.available = product.IsAvailable;
                        ViewBag.startDate = product.StartDate;
                        ViewBag.endDate = product.EndDate;
                    }
                    else
                    {
                        ViewBag.massError = "Product is not available in the system anymore";
                    }
                    return View("Show");
                }
                catch (Exception e)
                {
                    ViewBag.massError = e.Message;
                    return View("Show");
                }
            }
            return Redirect("~/Auth/Login");
        }

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            var productName = collection["name"];
            var productDescription = collection["description"];
            var productStartDate = collection["startDate"];
            var productEndDate = collection["endDate"];
            var productPrice = collection["price"];
            var valid = true;

            #region validation
            var productNameMinLength = 2;
            var productNameMaxLength = 50;

            var productDescriptionMinLength = 2;
            var productDescriptionMaxLength = 255;

            if (string.IsNullOrWhiteSpace(productName))
            {
                @ViewBag.nameError = "Product name is a required field";
                valid = false;
            }
            else if (productName.Length < productNameMinLength)
            {
                ViewBag.nameError = $"Product name must contain at least {productNameMinLength} characters";
                valid = false;
            }
            else if (productName.Length > productNameMaxLength)
            {
                ViewBag.nameError = $"Product name can not contain more than {productNameMaxLength} characters";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(productDescription))
            {
                @ViewBag.descriptionError = "Product description is a required field";
                valid = false;
            }
            else if (productDescription.Length < productDescriptionMinLength)
            {
                ViewBag.descriptionError = $"Product description must contain at least {productDescriptionMinLength} characters";
                valid = false;
            }
            else if (productDescription.Length > productDescriptionMaxLength)
            {
                ViewBag.descriptionError = $"Product description can not contain more than {productDescriptionMaxLength} characters";
                valid = false;
            }


            if (string.IsNullOrWhiteSpace(productStartDate))
            {
                ViewBag.startDateError = "Start date date is a required field";
                valid = false;
            }
            else if ((DateTime.Now - DateTime.Parse(productStartDate)).Days == 0)
            {
                productStartDate = DateTime.Now.ToString();
            }
            else if (DateTime.Now > DateTime.Parse(productStartDate))
            {
                ViewBag.startDateError = "Start date has to be either at the current moment or in future one";
                valid = false;
            }
            else if (DateTime.Parse(productStartDate) > DateTime.Parse(productEndDate))
            {
                ViewBag.startDateError = "The end date cannot have earlier date than the current moment";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(productEndDate))
            {
                ViewBag.endDateError = "Start date date is a required field";
                valid = false;
            }
            else if (DateTime.Now > DateTime.Parse(productEndDate))
            {
                ViewBag.endDateError = "The end date cannot have earlier date than the current moment";
                valid = false;
            }
            else if (DateTime.Parse(productStartDate) > DateTime.Parse(productEndDate))
            {
                ViewBag.endDateError = "The end date cannot have earlier date than the current moment";
                valid = false;
            }
            if (string.IsNullOrWhiteSpace(productPrice))
            {
                ViewBag.priceError = "Price is required field";
                valid = false;
            }
            else if (int.Parse(productPrice) <= 0)
            {
                ViewBag.priceError = "Price cannot be negative";
                valid = false;
            }
            #endregion

            if (valid)
            {
                var product = new ProductServiceReference.Product
                {
                    Name = productName,
                    Description = productDescription,
                    StartDate = DateTime.Parse(productStartDate),
                    EndDate = DateTime.Parse(productEndDate),
                    Price = int.Parse(productPrice)
                };
                try
                {
                    productServiceClient.CreateProduct(product);
                }
                catch (Exception)
                {
                    ViewBag.massError = "Internal server error please try again after 5 minutes";
                    return View("Create");
                }
                return Redirect("~/");//TODO redirect to view page
            }
            ViewBag.description = productDescription;
            ViewBag.price = productPrice;
            ViewBag.name = productName;
            ViewBag.startDate = productStartDate;
            ViewBag.endDate = productEndDate;
            return View("Create");
        }
    }
}