using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthWithIdentity.DLL
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var dbContext = serviceProvider.GetRequiredService<AppDbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            dbContext.Database.EnsureCreated();

            if (!dbContext.Users.Any())
            {
                AppUser user = new AppUser();
                user.UserName = "testusername";
                user.Email = "testuseremail@test.com";
                user.SecurityStamp = Guid.NewGuid().ToString();

                userManager.CreateAsync(user, "Password@123").Wait();
            }
        }
    }
}
