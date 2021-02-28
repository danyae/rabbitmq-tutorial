using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMq.Tutorial.Store.Clients;
using RabbitMq.Tutorial.Store.Options;
using RabbitMq.Tutorial.Store.Producers;

namespace RabbitMq.Tutorial.Store
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddHttpClient();
            services.AddScoped(typeof(SupplierClient));
            services.Configure<RabbitmqOptions>(Configuration.GetSection(nameof(RabbitmqOptions)));
            services.Configure<SupplierOptions>(Configuration.GetSection(nameof(SupplierOptions)));
            services.Configure<StoreOptions>(Configuration.GetSection(nameof(StoreOptions)));
            services.AddScoped(typeof(OrderProducer));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
