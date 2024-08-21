﻿using ABC_MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace ABC_MVC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly TableStorageService _tableStorageService;

        public RegisterController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Hash the password before storing (you should use a stronger hash function in a real app)
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var profile = new CustomerProfile
                {
                    PartitionKey = "Customer", // Example partition key
                    RowKey = model.Email, // Use email as RowKey for simplicity
                    Email = model.Email,
                    PasswordHash = passwordHash
                };

                await _tableStorageService.AddOrUpdateProfileAsync(profile);
                return RedirectToAction("Login", "Login");
            }
            return View(model);
        }
    }
}
