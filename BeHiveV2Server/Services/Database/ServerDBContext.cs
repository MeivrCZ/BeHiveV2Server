using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BeHiveV2Server.Services.Database.Models;
using BeHiveV2Server.Models;
using BeHiveV2Server.Models.Hives.SHP1Device;
using BeHiveV2Server.Models.Hives.SHB1Device;
using BeHiveV2Server.Models.Hives.SHB1Device.Data;

namespace BeHiveV2Server.Services.Database
{
    public class ServerDBContext : IdentityDbContext<UserIdentity, RoleIdentity, string>
    {
        public ServerDBContext(DbContextOptions<ServerDBContext> options) : base(options)
        {

        }

        public DbSet<Device> Devices { get; set; }
        public DbSet<UserDevice> userDevices { get; set; }
        public DbSet<SHB1Device> SHB1Devices { get; set; }
        public DbSet<SHB1Data> SHB1Datas { get; set; }
        public DbSet<SHP1Device> SHP1Devices { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); //creates database base (AspNetUsers, AspNetRoles, ...)

            //user <-> device
            modelBuilder.Entity<UserDevice>()
                .HasKey(ud => new { ud.DeviceID, ud.UserID });
            modelBuilder.Entity<UserDevice>()
                .HasOne(ud => ud.device)
                .WithMany(d => d.users)
                .HasForeignKey(ud => ud.DeviceID);
            modelBuilder.Entity<UserDevice>()
                .HasOne(ud => ud.user)
                .WithMany(u => u.MemberDevices)
                .HasForeignKey(ud => ud.UserID);

            //user -> devices
            modelBuilder.Entity<UserIdentity>()
                .HasMany(u => u.OwnedDevices)
                .WithOne(d => d.owner);

            //shb1device => shb1data
            modelBuilder.Entity<SHB1Device>()
                .HasMany(dev => dev.data)
                .WithOne(dat => dat.device);

            //shb1device => device
            modelBuilder.Entity<SHB1Device>()
                .HasOne(d => d.device);
        }
    }
}
