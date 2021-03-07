using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TweetBook.Data;

namespace TweetBook
{
    public class Program
    {
        private const string PosterEmail = "poster@gmail.com";
        private const string AdminEmail = "admin@gmail.com";
        private const string UserEmail = "user@gmail.com";
        
        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();
                await dbContext.Database.MigrateAsync();

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var adminRole = new IdentityRole("Admin");
                    await roleManager.CreateAsync(adminRole);
                }
                
                if (!await roleManager.RoleExistsAsync("Poster"))
                {
                    var posterRole = new IdentityRole("Poster");
                    await roleManager.CreateAsync(posterRole);
                }

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
                CreateDefaultUserIfMissing(userManager, AdminEmail, "Admin");
                CreateDefaultUserIfMissing(userManager, PosterEmail, "Poster");
                CreateDefaultUserIfMissing(userManager, UserEmail);
                
            }

            await host.RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static async void CreateDefaultUserIfMissing(UserManager<IdentityUser> userManager, string email, string role = "")
        {
            if (await userManager.FindByEmailAsync(email) != null) return;
            
            var user = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = email,
                Email = email
            };

            var createdUser = await userManager.CreateAsync(user, "Pass1234!");
            if (!createdUser.Succeeded) throw new Exception($"Inserting default user {email} failed");
            
            await userManager.AddClaimAsync(user, new Claim("tags.view", "true"));

            if (string.IsNullOrEmpty(role)) return;
            
            var roleAdded = await userManager.AddToRoleAsync(user, role);
            if (!roleAdded.Succeeded) throw new Exception($"Inserting default role {role} to user {email} failed");
        }
    }
}