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
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using _2Sport_BE.Services.Caching;
using _2Sport_BE.Infrastructure.Hubs;
using static _2Sport_BE.Infrastructure.Hubs.NotificationHub;

namespace _2Sport_BE.Extensions
{
    public static class ServiceCollection
    {
        public static void Register(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddDbContext<TwoSportCapstoneDbContext>(options =>
            {
                options.UseSqlServer(GetConnectionStrings(), b => b.MigrationsAssembly("2Sport_BE"));
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Transient);

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = GetConnectionStringsRedis();
                options.InstanceName = "CartItems_";
            });

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
            services.AddTransient<IBlogService, BlogService>();
            services.AddTransient<IBranchService, BranchService>();
            services.AddTransient<IBookmarkService, BookmarkService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddScoped<ISportService, SportService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IShipmentDetailService, ShipmentDetailService>();
            services.AddScoped<IPaymentMethodService, PaymentMethodService>();
            services.AddScoped<IPhoneNumberService, PhoneNumberService>();
            services.AddScoped<ILikeService, LikeService>();
			services.AddScoped<IReviewService, ReviewService>();
			services.AddScoped<IRedisCacheService, RedisCacheService>();
			services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IImportHistoryService, ImportHistoryService>();
			services.AddScoped<IWarehouseService, WarehouseService>();
			services.AddScoped<IImageService, ImageService>();
			services.AddScoped<IImageVideosService, ImageVideosService>();

            
            #region Order_Services
            //SaleOrder
            services.AddScoped<ISaleOrderService, SaleOrderService>();
            services.AddScoped<SaleOrderService>();
            services.AddScoped<IOrderDetailService, OrderDetailService>();
            //RentalOrder
            services.AddScoped<IRentalOrderService, RentalOrderService>();
            services.AddScoped<RentalOrderService>();
            #endregion
            #region Helper_Services
            services.AddTransient<IMailService, MailService>();
            services.AddScoped<IMethodHelper, MethodHelper>();
            services.AddScoped<IPaymentService, PayOsPaymentService>();
            services.AddScoped<IPayOsService, PayOsPaymentService>();
            services.AddScoped<PayOsPaymentService>();
            services.AddScoped<IVnPayService, VnPayPaymentService>();
            services.AddScoped<VnPayPaymentService>();

            services.AddScoped<PaymentFactory>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<IDeliveryMethodService, DeliveryMethodService>();
            services.AddScoped<IRefundRequestService, RefundRequestService>();

            #endregion
            #region Notification
            services.AddScoped<INotificationHub, NotificationHubService>();
            services.AddScoped<INotificationService, NotificationService>();
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

        private static string GetConnectionStringsRedis()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();

            var strConn = config["ConnectionStrings:Redis"];
            return strConn;
        }
    }
}
