using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebAppAuctionSystem.Controllers
{
    public class UserController : Controller
    {
        UserServiceReference.UserServiceClient userServiceClient;
        ZipServiceReference.ZipServiceClient zipServiceClient;
        AuthController authController;

        public UserController()
        {
            zipServiceClient = new ZipServiceReference.ZipServiceClient();
            userServiceClient = new UserServiceReference.UserServiceClient();
            authController = new AuthController();
        }
        [HttpGet]
        public ActionResult Edit(int userId)
        {
            if (authController.IsUserLoggedIn(Request, Response))
            {
                ViewBag.user = new UserServiceReference.UserServiceClient().GetUserByCookie(Request.Cookies["auth"].Value);
                ViewBag.zips = zipServiceClient.GetAllZips();
                try
                {
                    if (userId == userServiceClient.GetUserByCookie(Request.Cookies["auth"].Value).Id)
                    {
                        var user = userServiceClient.GetUserById(userId);
                        ViewBag.userId = userId;
                        ViewBag.selectedZipId = user.ZipId.ToString();
                        ViewBag.username = user.Username;
                        ViewBag.name = user.Name;
                        ViewBag.birthday = user.DateOfBirth.ToString();
                        ViewBag.phone = user.Phone;
                        ViewBag.email = user.Email;
                        ViewBag.address = user.Address;
                        ViewBag.gender = user.Gender == UserServiceReference.Gender.Female ? 1.ToString() : 0.ToString();
                    }
                    else
                    {
                        ViewBag.MassError = "You have no access to this user details!";
                    }
                }
                catch (Exception e)
                {
                    ViewBag.massError = "Internal server error has ocurred please try again after 5 minutes";
                    ViewBag.MassError = e.Message;
                }
                return View("Edit");
            }
            else
            {
                return Redirect("~/Auth/Login");
            }
        }

        [HttpPost]
        public ActionResult Edit(FormCollection collection, int userId)
        {
            if(authController.IsUserLoggedIn(Request, Response))
            {
                return Redirect("~/auth/login");
            }
            ViewBag.user = new UserServiceReference.UserServiceClient().GetUserByCookie(Request.Cookies["auth"].Value);
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

            if (!string.IsNullOrWhiteSpace(password))
            {
                if (password.Length < passwordMinLenth)
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
            }
            else
            {
                var user = userServiceClient.GetUserByCookie(Request.Cookies["auth"].Value);
                password = user.Password;
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
                var user = new UserServiceReference.UserDto
                {
                    Id = userId,
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
                    userServiceClient.UpdateUser(user);
                    return Redirect("~/");
                }
                catch (Exception e)
                {
                    //ViewBag.massError = "Internal server error, please wait 5 minutes";
                    ViewBag.massError = e;
                    return View("Edit");
                }
            }
            else
            {
                ViewBag.zips = zipServiceClient.GetAllZips();
                ViewBag.selectedZipId = zipId;
                ViewBag.username = username;
                ViewBag.name = name;
                ViewBag.email = email;
                ViewBag.phone = phone;
                ViewBag.birthday = birth;
                ViewBag.zip = zip;
                ViewBag.address = address;
                ViewBag.gender = gender;
                return View("Edit");
            }
        }
    }
}