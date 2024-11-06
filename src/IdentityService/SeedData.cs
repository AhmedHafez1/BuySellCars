using System.Security.Claims;
using IdentityModel;
using IdentityService.Data;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace IdentityService;

public class SeedData
{
    public static void EnsureSeedData(WebApplication app)
    {
        using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        if (userMgr.Users.Any())
            return;

        var Yahya = userMgr.FindByNameAsync("Yahya").Result;
        if (Yahya == null)
        {
            Yahya = new ApplicationUser
            {
                UserName = "Yahya",
                Email = "Yahya@email.com",
                EmailConfirmed = true,
            };
            var result = userMgr.CreateAsync(Yahya, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr.AddClaimsAsync(Yahya, [new Claim(JwtClaimTypes.Name, "Yahya")]).Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("Yahya created");
        }
        else
        {
            Log.Debug("Yahya already exists");
        }

        var Ismail = userMgr.FindByNameAsync("Ismail").Result;
        if (Ismail == null)
        {
            Ismail = new ApplicationUser
            {
                UserName = "Ismail",
                Email = "Ismail@email.com",
                EmailConfirmed = true,
            };
            var result = userMgr.CreateAsync(Ismail, "Pass123$").Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }

            result = userMgr
                .AddClaimsAsync(Ismail, [new Claim(JwtClaimTypes.Name, "Ismail")])
                .Result;
            if (!result.Succeeded)
            {
                throw new Exception(result.Errors.First().Description);
            }
            Log.Debug("Ismail created");
        }
        else
        {
            Log.Debug("Ismail already exists");
        }
    }
}
