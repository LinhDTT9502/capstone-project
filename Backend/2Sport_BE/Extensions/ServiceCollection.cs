using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Implements;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using System.Configuration;
using _2Sport_BE.Repository.Data;
using _2Sport_BE.Helpers;

namespace _2Sport_BE.Extensions
{
    public static class ServiceCollection
    {
        public static void Register (this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddDbContext<TwoSportCapstoneDbContext>(options => options
            .UseSqlServer(GetConnectionStrings(),
            b => b.MigrationsAssembly("2Sport_BE")));
            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IBrandService, BrandService>();
            services.AddTransient<IBranchService, BranchService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddTransient<IMailService, MailService>();
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
			services.AddScoped<IImageService, ImageService>();
			services.AddScoped<IImageVideosService, ImageVideosService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<AuthService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAttendanceService, AttendanceService>();
            services.AddScoped<IEmployeeService, EmployeeService>();
            services.AddScoped<IEmployeeDetailService, EmployeeDetailService>();
            services.AddScoped<ICustomerDetailService, CustomerDetailService>();
            services.AddScoped<IRentalOrderService, RentalOrderService>();
            services.AddScoped<IMethodHelper, MethodHelper>();

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
