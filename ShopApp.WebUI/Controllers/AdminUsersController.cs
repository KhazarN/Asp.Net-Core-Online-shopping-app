﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopApp.WebUI.Identity;
using ShopApp.WebUI.ViewModels;

namespace ShopApp.WebUI.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminUsersController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminUsersController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }
        public IActionResult ListUsers()
        {
            return View(userManager.Users);
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                var selectedRoles = await userManager.GetRolesAsync(user);
                var roles = roleManager.Roles.Select(r => r.Name);
                ViewBag.Roles = roles;
                return View(new UserDetailsViewModel()
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    SelectedRoles = selectedRoles
                });
            }
            return RedirectToAction("ListUsers");
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(UserDetailsViewModel model, string[] selecedRoles)
        {
            if (ModelState.IsValid)
            {
                var user = await userManager.FindByIdAsync(model.UserId);
                if (user != null)
                {
                    var userRoles = await userManager.GetRolesAsync(user);
                    selecedRoles = selecedRoles ?? new string[] { };
                    await userManager.AddToRolesAsync(user, selecedRoles.Except(userRoles).ToArray<string>());
                    await userManager.RemoveFromRolesAsync(user, userRoles.Except(selecedRoles).ToArray<string>());
                    return RedirectToAction("ListUsers");
                }
                return RedirectToAction("ListUsers");
            }
            return View(model);
        }
    }
}
