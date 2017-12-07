using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppAuctionSystem.Controllers
{
    public class AuthController : Controller
    {
        LoginServiceReference.LoginServiceClient wcfClient;
        UserServiceReference.UserServiceClient wcfUserService;
        ZipServiceReference.ZipServiceClient wcfZipService;

        public AuthController()
        {
            wcfClient = new LoginServiceReference.LoginServiceClient();
            wcfUserService = new UserServiceReference.UserServiceClient();
            wcfZipService = new ZipServiceReference.ZipServiceClient();
        }
        // GET: Login
        public ActionResult Index()
        {
            return Redirect("~/Auth/Login");
        }

        [HttpGet]
        public ActionResult Register()
        {
            if (IsUserLoggedIn(Request, Response))
            {
                return Redirect("~/");
            }
            try
            {
                var zips = wcfZipService.GetAllZips();
                ViewBag.zips = zips;
            }
            catch (Exception)
            {
                ViewBag.massError = "Internal server error please try again after 5 minutes";
            }
            return View("Register");
        }

        [HttpPost]
        public ActionResult Register(FormCollection collection)
        {
            if(IsUserLoggedIn(Request, Response))
            {
                return Redirect("~/");
            }
            var username = collection["username"];
            var name = collection["name"];
            var birth = collection["birthday"];
            var phone = collection["phone"];
            var email = collection["email"];
            var password = collection["password"];
            var zip = collection["zip"];
            var address = collection["address"];
            var gender = collection["gender"];
            var zipId = collection["zip"];

            #region validation
            var valid = true;
            var usernameMinLength = 3;
            var usernameMaxLength = 30;

            var passwordMinLenth = 5;
            var passwordMaxLength = 100;

            var nameMinLength = 2;
            var nameMaxLenth = 30;

            var phoneMinLength = 5;
            var phoneMaxLength = 40;

            var addressMinLength = 3;
            var addressMaxLength = 100;
            if (string.IsNullOrWhiteSpace(username))
            {
                ViewBag.usernameError = "Username is a required field";
                valid = false;
            }
            else if (username.Length < usernameMinLength)
            {
                ViewBag.usernameError = $"Username must contain {usernameMinLength} or more characters";
                valid = false;
            }
            else if (username.Length > usernameMaxLength)
            {
                ViewBag.usernameError = $"Username can not contain more than {usernameMaxLength} characters";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                ViewBag.passwordError = "Password is a required field";
                valid = false;
            }
            else if (password.Length < passwordMinLenth)
            {
                ViewBag.passwordError = $"Password must contain {passwordMinLenth} or more characters";
                valid = false;
            }
            else if (password.Length > passwordMaxLength)
            {
                ViewBag.passwordError = $"Password can not contain more than {passwordMaxLength} characters";
                valid = false;
            }
            else if (!password.Any(c => char.IsLower(c)))
            {
                ViewBag.passwordError = "Password must contain at least one lowercased letter";
                valid = false;
            }
            else if (!password.Any(c => char.IsUpper(c)))
            {
                ViewBag.passwordError = "Password must contain at least one capital letter";
                valid = false;
            }
            else if (!password.Any(c => char.IsDigit(c)))
            {
                ViewBag.passwordError = "Password must contain at least one digit";
                valid = false;
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                ViewBag.nameError = "Name is a required field";
                valid = false;
            }
            else if (name.Length < nameMinLength)
            {
                ViewBag.nameError = $"Name must contain {nameMinLength} or more characters";
                valid = false;
            }
            else if (name.Length > nameMaxLenth)
            {
                ViewBag.nameError = $"Name can not contain more than {nameMaxLenth} characters";
                valid = false;
            }

            if (!string.IsNullOrWhiteSpace(birth))
            {
                try
                {
                    if (DateTime.Now.Year - DateTime.Parse(birth).Year < 18)
                    {
                        ViewBag.birthError = "Users with age less than 18 can not register";
                        valid = false;
                    }
                }
                catch (Exception)
                {
                    ViewBag.birthError = "Birthday does not match the format. Ex: 5-8-2017 (5 August 2017)";
                    valid = false;
                }
            }
            else
            {
                ViewBag.birthError = "Birth date is a required field";
                valid = false;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                if (phone.Length < phoneMinLength)
                {
                    ViewBag.phoneError = $"Mobile phone must contain {phoneMinLength} or more characters";
                    valid = false;
                }
                else if (phone.Length > phoneMaxLength)
                {
                    ViewBag.phoneError = $"Mobile phone can not contain more than {phoneMaxLength} characters";
                    valid = false;
                }
            }

            if (string.IsNullOrWhiteSpace(address))
            {
                ViewBag.addressError = $"Address is a required field";
                valid = false;
            }
            else if (address.Length < addressMinLength)
            {
                ViewBag.addressError = $"Address must contain {addressMinLength} or more characters";
                valid = false;
            }
            else if (address.Length > addressMaxLength)
            {
                ViewBag.addressError = $"Address can not contain more than {addressMaxLength} characters";
                valid = false;
            }
            #endregion

            if (valid)
            {
                var user = new UserServiceReference.User
                {
                    Username = username,
                    Password = password,
                    Name = name,
                    Address = address,
                    Email = email,
                    Phone = phone,
                    DateOfBirth = DateTime.Parse(birth),
                    Gender = gender == "0" ? UserServiceReference.Gender.Male : UserServiceReference.Gender.Female,
                    ZipId = int.Parse(zip),
                    Coins = 0
                };
                try
                {
                    wcfUserService.CreateUser(user);
                    return Redirect("/");
                }
                catch (Exception e)
                {
                    ViewBag.massError = e;
                    return View("Register");
                }
            }
            else
            {
                ViewBag.zips = wcfZipService.GetAllZips();
                ViewBag.selectedZipId = zipId;
                ViewBag.username = username;
                ViewBag.password = password;
                ViewBag.name = name;
                ViewBag.email = email;
                ViewBag.phone = phone;
                ViewBag.birthday = birth;
                ViewBag.zip = zip;
                ViewBag.address = address;
                ViewBag.gender = gender;
                return View("Register");
            }

        }

        [HttpGet]
        public ActionResult Login()
        {
            if (IsUserLoggedIn(Request, Response))
            {
                return Redirect("~/");
            }
            return View("Login");
        }

        [HttpPost]
        public ActionResult Login(FormCollection collection)
        {
            if (IsUserLoggedIn(Request, Response))
            {
                return Redirect("~/");
            }
            var isAuthenticated = false;
            var username = collection["username"];
            var password = collection["password"];

            #region validation
            bool valid = true;
            if (string.IsNullOrWhiteSpace(username))
            {
                valid = false;
                ViewBag.usernameEmpty = "Username is a required field";
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                valid = false;
                ViewBag.passwordEmpty = "Password is a required field";
            }
            #endregion

            //checking if user exists in db with the credentials provided
            if (valid)
            {
                try
                {
                    isAuthenticated = wcfClient.ValidateLogin(username, password);
                }
                catch (Exception)
                {
                    ViewBag.massError = "Internal error has occurred please try again after 5 minutes";
                    return View("Login");
                };

                if (isAuthenticated)
                {
                    HttpCookie authCookie = new HttpCookie("auth");
                    var user = wcfUserService.GetUserByUsername(username);
                    authCookie.Value = wcfUserService.AddCookie(user.Id);
                    Response.Cookies.Add(authCookie);
                    Response.Cookies["auth"].Expires.AddDays(3);
                    return Redirect("~/");//TODO user should be redirected to index page of logged in users
                }
                else
                {
                    ViewBag.username = username;
                    ViewBag.errorNotExisting = "The given credentials do not match";
                    return View("Login");
                }
            }
            else
            {
                ViewBag.username = username;
                return View("Login");
            }

        }

        [HttpPost]
        public void Logout()
        {
            var authCookie = Request.Cookies["auth"];
            if (authCookie != null)
            {
                authCookie.Expires = DateTime.Now.AddDays(-1);
                Response.Cookies.Add(authCookie);
                Response.Redirect("~/");
            }
        }

        /// <summary>
        ///     Returns true if user is logged in and updates cookies 
        ///     Returns false when user is not logged in and removed cookie
        /// </summary>
        /// <param name="cookie">Request.Cookies["auth"]</param>
        public bool IsUserLoggedIn(HttpRequestBase request, HttpResponseBase response)
        {
            var cookie = request.Cookies["auth"];
            if (cookie != null && !string.IsNullOrWhiteSpace(cookie.Value))
            {
                if (wcfUserService.IsCookieValid(cookie.Value))
                {
                    cookie.Expires = DateTime.Now.AddDays(3);
                    response.Cookies.Add(cookie);
                    return true;
                }
                else
                {
                    cookie.Expires = DateTime.Now.AddDays(-1);
                    response.Cookies.Add(cookie);
                    response.Redirect(request.RawUrl);
                }
            }
            return false;
        }

        public int GetUserIdByCookie(HttpCookie cookie)
        {

            if (cookie != null && wcfUserService.IsCookieValid(cookie.Value))
            {
                var user = wcfUserService.GetUserByCookie(cookie.Value);
                if (user != null)
                {
                    var userId = user.Id;
                    return userId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        public bool IsUserAdmin(HttpRequestBase request)
        {
            if (request.Cookies["auth"] != null)
            {
                if (wcfUserService.IsCookieValid(request.Cookies["auth"].Value))
                {
                    var user = wcfUserService.GetUserByCookie(request.Cookies["auth"].Value);
                    return user.IsAdmin;
                }
            }
            return false;
        }

        public UserServiceReference.UserDto GetUserLoggedUser(HttpRequestBase request)
        {
            return new UserServiceReference.UserServiceClient().GetUserByCookie(request.Cookies["auth"].Value);
        }
    }
}