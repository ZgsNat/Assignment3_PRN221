using Microsoft.Extensions.Configuration;
using _22_NguyenThaiThinh_Assignment3.Models;
using Microsoft.AspNetCore.Mvc;

namespace _22_NguyenThaiThinh_Assignment3.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly Ass3_Prn221_Bl5Context _context;

        public AccountController(IConfiguration configuration)
        {
            _context = new Ass3_Prn221_Bl5Context(); // Replace YourDbContext with your actual DbContext class
            _configuration = configuration;
        }

        // GET: /Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel model)
        {
            if (ModelState.IsValid)
            {
                var adminUsername = _configuration["AdminCredentials:Username"];
                var adminPassword = _configuration["AdminCredentials:Password"];

                if (model.Email == adminUsername && model.Password == adminPassword)
                {
                    // Admin login successful, perform necessary actions
                    // For example, set admin flag in session or cookie
                    // Redirect to admin dashboard or desired page
                    HttpContext.Session.SetString("Type", "1");
                    return RedirectToAction("Index", "Home");
                }
                var user = _context.AppUsers.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    HttpContext.Session.SetString("Type", "0");
                    // Authentication successful, redirect to dashboard or desired page
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid email or password");
                }
            }
            return View(model);
        }
        // GET: /Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {

                // Check if the email is already registered
                if (_context.AppUsers.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("", "Email is already registered");
                    return View(model);
                }

                // Create a new user record
                var newUser = new AppUser
                {
                    Fullname = model.Fullname,
                    Email = model.Email,
                    Password = model.Password,
                    Address = model.Address
                };

                _context.AppUsers.Add(newUser);
                _context.SaveChanges();

                // Redirect to login page or any other desired page
                return RedirectToAction("Login");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            // Xóa thông tin phiên đăng nhập
            HttpContext.Session.Clear();

            // Redirect về trang đăng nhập
            return RedirectToAction("Login");
        }
    }
}
