using blaLink.Data;
using Microsoft.EntityFrameworkCore;

namespace blaLink
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Add DbContext with SQL Server provider
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Add the ShortenerService to the dependency injection container
            builder.Services.AddScoped<Services.ShortenerService>();

            var app = builder.Build();

            // ====================================================================
            // --- ĐOẠN CODE THÊM VÀO ĐỂ TỰ ĐỘNG TẠO DATABASE TRONG DOCKER ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    // Kiểm tra và tự động tạo Database 'blaLink' nếu chưa tồn tại
                    context.Database.EnsureCreated();
                    Console.WriteLine("--- Database 'blaLink' đã sẵn sàng! ---");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("--- Lỗi khởi tạo DB: " + ex.Message);
                }
            }
            // ====================================================================

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}