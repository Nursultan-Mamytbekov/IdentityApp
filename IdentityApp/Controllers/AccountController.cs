using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using IdentityApp.Models;
using IdentityApp.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        private readonly AppDbContext context;
        private readonly IEmailSender sender;

        public AccountController(
            UserManager<User> userManager, 
            SignInManager<User> signInManager, 
            AppDbContext context, 
            IEmailSender sender)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.context = context;
            this.sender = sender;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["Regions"] = new SelectList(context.Regions.ToList(), "Id", "Name");
            return View();
        }
        [AllowAnonymous]
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
                    await sender.SendEmailAsync(model.Email, "Ваш пароль", model.Password);
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
        [AllowAnonymous]
        [HttpGet]
        public IActionResult SignIn(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        [AllowAnonymous]
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
        
        [HttpGet]
        public async Task<IActionResult> ChangePassword()
        {
            var username = User.Identity.Name;
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return NotFound();
            var model = new ChangePasswordViewModel() { Id = user.Id, Email = user.Email }; 
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                    if(result.Succeeded)
                    {
                        return RedirectToAction(nameof(Index), "Home");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }


        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }

        [AllowAnonymous]
        public IActionResult PassWord() => View();
        
    }
}
