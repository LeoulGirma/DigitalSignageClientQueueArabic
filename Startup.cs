using DigitalSignageClient.Data;
using DigitalSignageClient.Data.Hubs;
using DigitalSignageClient.Data.Interface;
using DigitalSignageClient.Data.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace DigitalSignageClient
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
            services.AddCors();
            services.AddSignalR();
            //services.AddDbContext<ClientDSDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("ClientDS")));
            //services.AddDbContext<ClientDSDbContext>(options => options.UseSqlServer("Server=localhost;Database=ClientDS;User Id=sa;Password=Cre@tive2020;"));
            
            services.AddDbContext<ClientDSDbContext>(options => options.UseSqlServer("Server=localhost;Database=ClientDS;Integrated Security=True;" +
            "Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False"));

            services.AddScoped<ICurrencyViewRepository, CurrencyViewRepository>();
            services.AddScoped<IScheduleViewRepository, SchduleViewRepository>();
            services.AddScoped<IVideoViewRepository, VideoViewRepository>();
            services.AddScoped<ICalendarViewRepository, CalendarViewRepository>();
            services.AddScoped<INavStyleViewRepository, NavStyleViewRepository>();
            services.AddScoped<IBodyStyleViewRepository, BodyStyleViewRepository>();
            services.AddScoped<IRSSNewsViewRepository, RSSNewsViewRepository>();
            services.AddScoped<IRSSStyleViewRepository, RSSStyleViewRepository>();
            services.AddScoped<ILicenseStatusRepository, LicenseStatusRepository>();
            services.AddScoped<ICounterRepository, CounterRepository>();
            services.AddScoped<ICounterServiceRepository, CounterServiceRepository>();
            services.AddScoped<IQueueDisplayRepository, QueueDisplayRepository>();
            services.AddScoped<IQueueRepository, QueueRepository>();
            services.AddScoped<IServiceGroupRepository, ServiceGroupRepository>();
            services.AddScoped<IServiceRepository, ServiceRepository>();
            services.AddScoped<ITokenRepository, TokenRepository>();
            services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            services.AddMvc().AddSessionStateTempDataProvider().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());

            app.UseSignalR(config =>
            {
                config.MapHub<QueueHub>("/queues");
                config.MapHub<MissingServiceHub>("/missing");
                config.MapHub<AllServiceHub>("/service");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
