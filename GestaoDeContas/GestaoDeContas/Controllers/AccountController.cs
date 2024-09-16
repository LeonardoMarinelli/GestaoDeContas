using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GestaoDeContas.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoDeContas.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;

    public AccountController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    [HttpGet]
    public async Task<IActionResult> Login()
    {
        var userCount = await _userManager.Users.CountAsync();
        ViewBag.ShowRegisterButton = userCount is 0;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _signInManager
            .PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);

        if (result.Succeeded) return RedirectToAction("Index", "ContasReceber");

        ModelState.AddModelError("", "Tentativa de login inválida.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [Route("Account/Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        
        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);
            
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "ContasReceber");
        }

        foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);
        
        return View(model);
    }
}