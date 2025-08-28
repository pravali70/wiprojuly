using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ResumeApi.Models;
using ResumeApi.Services;
using Microsoft.OpenApi.Models;

namespace ResumeApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            ////  Identity (use ApplicationUser instead of IdentityUser)
            //builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            //    .AddEntityFrameworkStores<AppDbContext>()
            //    .AddDefaultTokenProviders();
            builder.Services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddSignInManager<SignInManager<ApplicationUser>>();


            // Services
            builder.Services.AddScoped<JwtService>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Swagger with JWT Auth
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Resume Builder API",
                    Version = "v1"
                });

                var jwtSecurityScheme = new OpenApiSecurityScheme
                {
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Description = "Enter 'Bearer' followed by a space and then your token.\nExample: Bearer eyJhbGciOiJIUzI1NiIs...",
                    Reference = new OpenApiReference
                    {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                    }
                };

                c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { jwtSecurityScheme, Array.Empty<string>() }
                });
            });

            // CORS for React dev (Vite default is 5173, CRA is 3000)
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("Frontend",
                    policy => policy
                        .WithOrigins("http://localhost:5173", "http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                );
            });

            // JWT Auth
            var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!);
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    opts.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key)
                    };
                });

            var app = builder.Build();

            // ? Role seeding on startup
            //using (var scope = app.Services.CreateScope())
            //{
            //    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            //    string[] roles = { "Admin", "RegisteredUser", "Guest" };

            //    foreach (var role in roles)
            //    {
            //        if (!await roleManager.RoleExistsAsync(role))
            //        {
            //            await roleManager.CreateAsync(new IdentityRole(role));
            //        }
            //    }
            //}

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors("Frontend");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            await app.RunAsync(); // async entry point
        }
    }
}