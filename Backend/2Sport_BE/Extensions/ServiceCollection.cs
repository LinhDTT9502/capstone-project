using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Implements;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.API.Services;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using System.Configuration;

namespace _2Sport_BE.Extensions
{
    public static class ServiceCollection
    {
        public static void Register (this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<TwoSportDBContext>(options => options
            .UseSqlServer(GetConnectionStrings()));
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IBrandService, BrandService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<ISendMailService, MailService>();
            services.AddScoped<ISportService, SportService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<IShipmentDetailService, ShipmentDetailService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<ILikeService, LikeService>();
			services.AddScoped<IReviewService, ReviewService>();
			services.AddScoped<ISupplierService, SupplierService>();
			services.AddScoped<IImportHistoryService, ImportHistoryService>();
			services.AddScoped<IWarehouseService, WarehouseService>();
        }

        private static string GetConnectionStrings()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var strConn = config["ConnectionStrings:DefaultConnection"];
            return strConn; 
        }
    }
}
