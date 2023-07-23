using Duende.IdentityServer.EntityFramework.DbContexts;
using Duende.IdentityServer.EntityFramework.Mappers;
using Microsoft.EntityFrameworkCore;
using PersistenceService.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;
using PersistenceService.Data.ApplicationDb;
using PersistenceService.Stores;
using PersistenceService.Utils;

namespace IdentityService;

internal static class HostingExtensions
{
    public static WebApplication ConfigureServices(
        this WebApplicationBuilder builder
    )
    {
        DotNetEnv.Env.Load();
        var migrationsAssembly = typeof(Program).Assembly.GetName().Name;
        string connectionString = Environment.GetEnvironmentVariable(
            "DB_CONNECTION_STRING"
        );

        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<ApplicationDbContext>(
            options => options.UseNpgsql(connectionString)
        );

        builder.Services.AddScoped<
            IPasswordHasher<User>,
            BcryptPasswordHasher
        >();

        builder.Services.AddScoped<SignInManager<User>, BCryptSigninManager>();

        builder.Services
            .AddIdentity<User, IdentityRole<Guid>>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = true;
                options.Password.RequiredUniqueChars = 6;
                options.User.RequireUniqueEmail = true;
            })
            .AddUserManager<UserStore>()
            .AddSignInManager<BCryptSigninManager>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        builder.Services
            .AddIdentityServer()
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(
                        connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly)
                    );
            })
            .AddOperationalStore(options =>
            {
                options.ConfigureDbContext = b =>
                    b.UseNpgsql(
                        connectionString,
                        sql => sql.MigrationsAssembly(migrationsAssembly)
                    );
            })
            .AddInMemoryIdentityResources(Config.IdentityResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddInMemoryClients(Config.Clients)
            .AddAspNetIdentity<User>()
            .AddProfileService<CustomProfileService>();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        InitializeDatabase(app);

        app.UseStaticFiles();
        app.UseRouting();
        app.UseIdentityServer();
        app.UseAuthorization();

        app.MapRazorPages().RequireAuthorization();

        return app;
    }

    private static void InitializeDatabase(IApplicationBuilder app)
    {
        using (
            var serviceScope = app.ApplicationServices
                .GetService<IServiceScopeFactory>()
                .CreateScope()
        )
        {
            serviceScope.ServiceProvider
                .GetRequiredService<PersistedGrantDbContext>()
                .Database.Migrate();

            var context =
                serviceScope.ServiceProvider.GetRequiredService<ConfigurationDbContext>();
            context.Database.Migrate();
            if (!context.Clients.Any())
            {
                foreach (var client in Config.Clients)
                {
                    context.Clients.Add(client.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.IdentityResources.Any())
            {
                foreach (var resource in Config.IdentityResources)
                {
                    context.IdentityResources.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }

            if (!context.ApiScopes.Any())
            {
                foreach (var resource in Config.ApiScopes)
                {
                    context.ApiScopes.Add(resource.ToEntity());
                }
                context.SaveChanges();
            }
        }
    }
}
