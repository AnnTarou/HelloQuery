using HelloQuery.Data;

using Microsoft.EntityFrameworkCore;
namespace HelloQuery
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<HelloQueryContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("HelloQueryContext") ?? throw new InvalidOperationException("Connection string 'HelloQueryContext' not found.")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // セッションの追加
            builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromHours(1));

            // IHttpContextAccessorをサービスとして登録
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // シードデータを初期化
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                SeedData.Initialize(services);
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            // セッションの追加
            app.UseSession();

            app.Run();
        }
    }
}
