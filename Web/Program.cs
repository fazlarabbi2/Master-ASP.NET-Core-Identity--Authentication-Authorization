using Microsoft.AspNetCore.Authorization;
using Web.Authorization;

namespace Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddAuthentication("MyCookieAuth").AddCookie("MyCookieAuth", options =>
            {
                options.Cookie.Name = "MyCookieAuth";
                options.ExpireTimeSpan = TimeSpan.FromSeconds(200);
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => { policy.RequireClaim("Admin"); });

                options.AddPolicy("HRManagerPolicy", policy => policy
                .RequireClaim("Department", "HR")
                .RequireClaim("Manager")
                .Requirements.Add(new HRManagerProbationRequirement(3)));

                options.AddPolicy("MustBelongToHRDepartment",
                    policy => policy.RequireClaim("Department", "HR"));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, HRManagerProbationRequirementHandler>();

            builder.Services.AddHttpClient("OurWebAPI", client =>
            {
                client.BaseAddress = new Uri("https://localhost:7073");
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
