using ABC_MVC.Models;
using Azure.Storage.Files.Shares;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ABC_MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly string _fileShareConnectionString = "DefaultEndpointsProtocol=https;AccountName=st10275496;AccountKey=CpfBmfw/u2CiDAGJGrNOYWedlAYXqrYgH2D+9lPjyacwFuTX+ZR7gv3DugtodgImsQQ2MbypK40f+AStDs84jQ==;EndpointSuffix=core.windows.net";
        private readonly string _fileShareName = "logreport";

        public LoginController(TableStorageService tableStorageService)
        {
            _tableStorageService = tableStorageService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var profile = await _tableStorageService.GetProfileAsync("Customer", model.Email);

                if (profile != null && BCrypt.Net.BCrypt.Verify(model.Password, profile.PasswordHash))
                {
                    // Log the login event
                    await LogUserLoginAsync(model.Email);

                    // Login successful
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    // Login failed
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }
            return View(model);
        }

        private async Task LogUserLoginAsync(string email)
        {
            var timeStamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"User: {email} logged in at {timeStamp}\n";

            var shareClient = new ShareClient(_fileShareConnectionString, _fileShareName);
            await shareClient.CreateIfNotExistsAsync();

            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient("login_log.txt");

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(logEntry)))
            {
                if (await fileClient.ExistsAsync())
                {
                    // Append to the existing file
                    var fileProperties = await fileClient.GetPropertiesAsync();
                    await fileClient.UploadRangeAsync(
                        new Azure.HttpRange(fileProperties.Value.ContentLength, memoryStream.Length),
                        memoryStream);
                }
                else
                {
                    // Create a new file
                    await fileClient.CreateAsync(memoryStream.Length);
                    await fileClient.UploadRangeAsync(
                        new Azure.HttpRange(0, memoryStream.Length),
                        memoryStream);
                }
            }
        }
    }
}
