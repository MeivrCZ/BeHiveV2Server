using BeHiveV2Server.Areas.AdminArea.Models;
using BeHiveV2Server.Services.Database;
using BeHiveV2Server.Services.Database.Models;
using BeHiveV2Server.Services.Other;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace BeHiveV2Server.Areas.AdminArea.Controllers
{
    [Authorize(Roles = "admin")]
    public class UserAdminManagementController : Controller
    {
        private readonly ILogger<UserAdminManagementController> _logger;
        private readonly ServerDBContext _dbContext;
        UserManager<UserIdentity> _userManager;
        public UserAdminManagementController(ILogger<UserAdminManagementController> logger, ServerDBContext context, UserManager<UserIdentity> userManager)
        {
            _logger = logger;
            _dbContext = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Users(string searchString, string currentSearch, string searchOption, int? pageNumber)
        {
            ViewData["CurrentSearchOption"] = searchOption ?? "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentSearch;
            }

            ViewData["CurrentSearchString"] = searchString;

            var userlist = from s in _dbContext.Users select s;
            if (!string.IsNullOrEmpty(searchString))
            {
                switch (searchOption)
                {
                    case "id":
                        userlist = userlist.Where(x => x.Id.Contains(searchString));
                        break;
                    case "name":
                        userlist = userlist.Where(x => x.UserName.Contains(searchString));
                        break;
                    case "email":
                        userlist = userlist.Where(x => x.Email.Contains(searchString));
                        break;
                }
            }

            userlist = userlist.OrderBy(u => u.UserName);

            int pageSize = 10;
            return View(await PaginatedList<UserIdentity>.CreateAsync(userlist.AsNoTracking(), pageNumber ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View(new CreateUserModel());
        }
        [HttpPost]
        public IActionResult CreateUser(CreateUserModel createUserModel)
        {
            if (ModelState.IsValid)
            {
                if (createUserModel.password != createUserModel.passwordConformation)
                {
                    return View(createUserModel);
                }

                UserIdentity user = new UserIdentity() { UserName = createUserModel.userName, Email = createUserModel.email };

                var result = _userManager.CreateAsync(user, createUserModel.password);
                if (result.Result.Succeeded)
                {
                    return Redirect("/admin/users");
                }
                foreach (var error in result.Result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                ModelState.AddModelError(string.Empty, "Invalid Login Attempt");
            }
            return View(createUserModel);
        }

        public IActionResult manage(string id)
        {
            var userlist = from u in _dbContext.Users where u.Id == id select u;
            UserIdentity user = userlist.FirstOrDefault();

            return View(user);
        }

        public IActionResult changeName(string id, string newName)
        {
            var userlist = from u in _dbContext.Users where u.Id == id select u;
            UserIdentity user = userlist.FirstOrDefault();

            user.UserName = newName;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            return Redirect("/admin/users/manage/" + id);
        }

        public IActionResult changeEmail(string id, string email)
        {
            var userlist = from u in _dbContext.Users where u.Id == id select u;
            UserIdentity user = userlist.FirstOrDefault();

            user.Email = email;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            return Redirect("/admin/users/manage/" + id);
        }

        public IActionResult changePassword(string id, string password)
        {
            var userlist = from u in _dbContext.Users where u.Id == id select u;
            UserIdentity user = userlist.FirstOrDefault();

            _userManager.PasswordHasher.HashPassword(user, password);

            user.PasswordHash = password;
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();

            return Redirect("/admin/users/manage/" + id);
        }
    }
}
