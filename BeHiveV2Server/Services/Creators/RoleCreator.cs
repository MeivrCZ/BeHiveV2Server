using BeHiveV2Server.Services.Database.Models;
using Microsoft.AspNetCore.Identity;

namespace BeHiveV2Server.Services.Creators
{
    public class RoleCreator
    {
        private readonly ILogger<RoleCreator> _logger;
        private readonly RoleManager<RoleIdentity> _roleManager;

        public RoleCreator(ILogger<RoleCreator> logger, RoleManager<RoleIdentity> roleManager)
        {
            _logger = logger;
            _roleManager = roleManager;
        }

        public async Task CreateRoles()
        {
            if (!_roleManager.Roles.Where(r => r.Name == "Developer").Any())
            {
                await _roleManager.CreateAsync(new RoleIdentity { Name = "admin" });
            }
        }
    }
}
