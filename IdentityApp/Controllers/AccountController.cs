using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityApp.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly AppDbContext context;

        public AccountController(UserManager<User> userManager, SignInManager<User> signInManager, AppDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Regions"] = new SelectList(context.Regions.ToList(), "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([Bind("Id, Email, Region")]RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new User()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    RegionId = model.Region
                };
                var result = await userManager.CreateAsync(user, model.GenerateNewPassword());
                if (result.Succeeded)
                {
                    //await signInManager.SignInAsync(user, isPersistent: false);
                    TempData["Message"] = model.Password;
                    return RedirectToAction(nameof(PassWord));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            ViewData["Regions"] = new SelectList(context.Regions.ToList(), "Id", "Name");
            return View(model);
        }

        [HttpGet]
        public IActionResult SignIn(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn (LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    if (returnUrl != null) return LocalRedirect(returnUrl);
                    else return RedirectToAction(nameof(Index), "Home");
                }
                else if (result.IsLockedOut) 
                {
                    return RedirectToAction(nameof(SignOut));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        public IActionResult PassWord()
        {        
            return View();
        }
    }
}
