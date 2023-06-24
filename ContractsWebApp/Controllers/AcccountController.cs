using Microsoft.AspNetCore.Mvc;
using ContractsWebApp.Models;
using ContractsWebApp.ViewModel;
using ContractsWebApp.DBContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Diagnostics;

namespace ContractsWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("User/Register")]
        public IActionResult Register()
        {
            return View("~/Views/User/Register.cshtml");
        }

        [HttpPost("User/Register")]
        public IActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                string userId = Guid.NewGuid().ToString();

                User newUser = new User
                {
                    Id = userId,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    PhoneNumber = model.PhoneNumber,
                    PersonalIdentificationNumber = model.SocialSecurityNumber,
                    Age = model.Age,
                    Role = UserRole.Client
                };

                var passwordHasher = new PasswordHasher<User>();
                newUser.Password = passwordHasher.HashPassword(newUser, model.Password);

                _context.Users.Add(newUser);
                _context.SaveChanges();

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }


        //LOGIN

        [HttpGet("User/Login")]
        public IActionResult Login()
        {
            return View("~/Views/User/Login.cshtml");
        }


        [HttpPost("User/Login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);

                if (user != null)
                {
                    var passwordHasher = new PasswordHasher<User>();
                    var passwordVerificationResult = passwordHasher.VerifyHashedPassword(user, user.Password, model.Password);

                    if (passwordVerificationResult == PasswordVerificationResult.Success)
                    {
                        var userClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                };

                        var userIdentity = new ClaimsIdentity(userClaims, "login");

                        var userPrincipal = new ClaimsPrincipal(userIdentity);

                        await HttpContext.SignInAsync(userPrincipal);


                        return RedirectToAction("Index", "Home");
                    }
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }

            return View("~/Views/User/Login.cshtml", model);
        }


        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }


    }
}