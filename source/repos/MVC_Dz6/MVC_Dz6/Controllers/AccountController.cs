using Microsoft.AspNetCore.Mvc;
using MVC_Dz6.Data;
using MVC_Dz6.Models;

namespace MVC_Dz6.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!string.IsNullOrEmpty(model.Username) &&
                _context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError(nameof(model.Username), "This username is already taken.");
                return View(model);
            }

            var user = new User
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Username = model.Username,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Password = model.Password,
                Age = model.Age,
                CreditCardNumber = model.CreditCardNumber,
                Website = model.Website,
                TermsOfService = model.TermsOfService ?? false
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            TempData["SuccessMessage"] =
                $"User \"{user.Username ?? user.Email}\" was registered successfully!";
            return RedirectToAction(nameof(RegisterSuccess));
        }

        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        [AcceptVerbs("GET", "POST")]
        public IActionResult IsUserNameAvailable(string username)
        {
            bool taken = !string.IsNullOrEmpty(username) &&
                         _context.Users.Any(u => u.Username == username);

            if (taken)
            {
                return Json($"The username \"{username}\" is already taken.");
            }

            return Json(true);
        }
    }
}
