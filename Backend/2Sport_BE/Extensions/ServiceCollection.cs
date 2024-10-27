using _2Sport_BE.Repository.Interfaces;
using _2Sport_BE.Repository.Implements;
using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore;
using _2Sport_BE.Infrastructure.Services;
using _2Sport_BE.Service.Services;
using _2Sport_BE.Services;
using System.Configuration;
using _2Sport_BE.Repository.Data;
using _2Sport_BE.Infrastructure.Helpers;

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
            #region User_Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IManagerService, ManagerService>();
            services.AddScoped<IStaffService, StaffService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            #endregion

            services.AddTransient<IBrandService, BrandService>();
            services.AddTransient<IBranchService, BranchService>();
            services.AddScoped<ISportService, SportService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<IShipmentDetailService, ShipmentDetailService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
			services.AddScoped<ILikeService, LikeService>();
			services.AddScoped<IReviewService, ReviewService>();
			services.AddScoped<ISupplierService, SupplierService>();
			services.AddScoped<IImportHistoryService, ImportHistoryService>();
			services.AddScoped<IWarehouseService, WarehouseService>();
			services.AddScoped<IImageService, ImageService>();
			services.AddScoped<IImageVideosService, ImageVideosService>();

            
            #region Order_Services
            //SaleOrder
            services.AddScoped<ISaleOrderService, SaleOrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            //RentalOrder
            services.AddScoped<IRentalOrderService, RentalOrderService>();
            #endregion
            #region Helper_Services
            services.AddTransient<IMailService, MailService>();
            services.AddScoped<IMethodHelper, MethodHelper>();
            services.AddScoped<IPaymentService, PaymentService>();
            #endregion
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
