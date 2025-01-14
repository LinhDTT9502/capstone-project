﻿using _2Sport_BE.DataContent;
using _2Sport_BE.Extensions;
using _2Sport_BE.Helpers;
using _2Sport_BE.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.OpenApi.Models;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Infrastructure.Hubs;
using Hangfire;
using HangfireBasicAuthenticationFilter;
using Microsoft.AspNetCore.Http.Connections;

var builder = WebApplication.CreateBuilder(args);
//Setting Mail
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("AppSettings:MailSettings"));
//Setting PayOs
builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection("PayOSSettings"));


builder.Services.AddHttpContextAccessor();
builder.Services.Register();
//Register Hangfire
builder.Services.AddHangfire((sp, config) =>
{
    var connectionString = sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection");
    config.UseSqlServerStorage(connectionString);
});
builder.Services.AddHangfireServer();
// Add services to the container.
builder.Services.AddControllers().AddJsonOptions(x =>
   x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
//JWT services
var appsettingSection = builder.Configuration.GetSection("ServiceConfiguration");
builder.Services.Configure<ServiceConfiguration>(appsettingSection);
var serviceConfiguration = appsettingSection.Get<ServiceConfiguration>();
var JwtSecretkey = Encoding.ASCII.GetBytes(serviceConfiguration.JwtSettings.Secret);
var tokenValidationParameters = new TokenValidationParameters
{
    IssuerSigningKey = new SymmetricSecurityKey(JwtSecretkey),
    ValidateIssuer = false,
    ValidateAudience = false,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    RequireExpirationTime = false,
};

builder.Services.AddSingleton(tokenValidationParameters);
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        
    })
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = tokenValidationParameters;
        options.Events = new JwtBearerEvents
        {
             OnMessageReceived = context =>
             {
                 var accessToken = context.Request.Query["access_token"];
                 var path = context.HttpContext.Request.Path;
                 if (!string.IsNullOrEmpty(accessToken) &&
                     (path.StartsWithSegments("/notificationHub")))
                 {
                     context.Token = accessToken;
                 }
                 return Task.CompletedTask;
             }
        };
    })
   .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
   .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
   {
       options.ClientId = builder.Configuration["Auth0:ClientId"];
       options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
       options.CallbackPath = "/signin-google";
       options.Scope.Add(builder.Configuration["Auth0:ProfileAccess"]);
       options.Scope.Add(builder.Configuration["Auth0:EmailAccess"]);
       options.Scope.Add(builder.Configuration["Auth0:BirthDayAccess"]);
       options.Scope.Add(builder.Configuration["Auth0:PhoneAccess"]);

   })
   .AddFacebook(facebookOptions =>
   {
       facebookOptions.AppId = builder.Configuration["Facebook:AppId"];
       facebookOptions.AppSecret = builder.Configuration["Facebook:AppSecret"];
       facebookOptions.CallbackPath = "/signin-facebook";
       // Yêu cầu quyền truy cập email
       facebookOptions.Scope.Add("email");

       // Lấy thông tin từ Facebook
       facebookOptions.Fields.Add("email"); // Email

   });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập 'Bearer' [space] và sau đó là token của bạn. \n\nVí dụ: \"Bearer abcdefgh12345\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    builder.WithOrigins("https://twosport.vercel.app", "https://twosportshop.vercel.app",
                        "https://2sport-employee.vercel.app",
                        "http://localhost:5173", "http://localhost:5174",
                        "http://demo-api.ap-southeast-2.elasticbeanstalk.com")
           .AllowAnyMethod()
           .AllowAnyHeader()
           .AllowCredentials()
    );
});

//Register SignalR
builder.Services.AddSignalR(options =>
{
    options.EnableDetailedErrors = true; // Optional: bật chi tiết lỗi
});

//builder.Services.AddHttpsRedirection(options =>
//{
//    options.HttpsPort = 443;
//});

//Mapping services
var mappingConfig = new MapperConfiguration(mc =>
{
    mc.AddProfile(new Mapping());
});
IMapper mapper = mappingConfig.CreateMapper();
builder.Services.AddSingleton(mapper);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub");
    endpoints.MapHub<ChatHub>("/chatHub");
});

app.UseWebSockets();


app.MapGet("/", () => Results.Redirect("/swagger"));
app.UseHangfireServer();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    DashboardTitle = "Hangfire for 2Sport",
    DarkModeEnabled = false,
    DisplayStorageConnectionString = false,
    Authorization = new[]
    {
        new HangfireCustomBasicAuthenticationFilter
        {
            User = "admin123",
            Pass = "admin123"
        }
    }
});
using (var scope = app.Services.CreateScope())
{
    var rentalOrderService = scope.ServiceProvider.GetRequiredService<IRentalOrderService>();
    var saleOrderService = scope.ServiceProvider.GetRequiredService<ISaleOrderService>();
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();

        recurringJobs.AddOrUpdate(
        "CheckRentalExpiration",
        () => rentalOrderService.CheckRentalOrdersForExpiration(),
        Cron.Daily
        );

        recurringJobs.AddOrUpdate(
           "CheckPendingRentalOrder",
           () => rentalOrderService.CheckPendingOrderForget(),
           Cron.Hourly
       );
        recurringJobs.AddOrUpdate(
           "CheckPendingSaleOrder",
           () => saleOrderService.CheckPendingOrderForget(),
           Cron.Hourly
       );
        recurringJobs.AddOrUpdate(
           "CompletedSaleOrder",
           () => saleOrderService.CompletedSaleOrder(),
           Cron.Hourly
       );
        recurringJobs.AddOrUpdate(
          "RefundSaleOrder",
          () => saleOrderService.RefundSaleOrder(),
          Cron.Hourly()
      );
        recurringJobs.AddOrUpdate(
          "RefundRentalOrder",
          () => rentalOrderService.RefundRentalOrder(),
          Cron.Hourly()
      );
    });
}

app.Run();
