using BeHiveV2Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace BeHiveV2Server.Services.Creators
{
    public class DefaultAdminCreator
    {
        private readonly ILogger<DefaultAdminCreator> _logger;
        private readonly UserManager<UserIdentity> _userManager;

        public DefaultAdminCreator(ILogger<DefaultAdminCreator> logger, UserManager<UserIdentity> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        public async Task CreateAdmin()
        {
            if (!_userManager.Users.Where(u => u.UserName == "admin1234").Any())
            {
                UserIdentity user = new UserIdentity() { UserName = "admin1234" };
                await _userManager.CreateAsync(user, "admin");
                await _userManager.AddToRoleAsync(user, "admin");
            }

            //test
            UserIdentity Testuser = new UserIdentity() { UserName = "test1" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test2" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test3" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test4" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test5" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test6" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test7" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test8" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test9" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test10" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test11" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test12" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test13" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test14" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test15" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test16" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test17" };
            await _userManager.CreateAsync(Testuser, "test");
            Testuser = new UserIdentity() { UserName = "test18" };
            await _userManager.CreateAsync(Testuser, "test");
        }
    }
}
