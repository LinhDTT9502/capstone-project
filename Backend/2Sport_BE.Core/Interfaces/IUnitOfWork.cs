using _2Sport_BE.Repository.Models;
using Microsoft.EntityFrameworkCore.Storage;

namespace _2Sport_BE.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Blog> BlogRepository { get; }
        IGenericRepository<Brand> BrandRepository { get; }
        IGenericRepository<Branch> BranchRepository { get; }
        IGenericRepository<Cart> CartRepository { get; }
        IGenericRepository<CartItem> CartItemRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<Comment> CommentRepository { get; }
        IGenericRepository<Feedback> FeedbackRepository { get; }
        IGenericRepository<ImagesVideo> ImagesVideoRepository { get; }
        IGenericRepository<ImportHistory> ImportHistoryRepository { get; }
        IGenericRepository<Like> LikeRepository { get; }
        IGenericRepository<SaleOrder> SaleOrderRepository { get; }
        IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        IGenericRepository<PaymentMethod> PaymentMethodRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<ShipmentDetail> ShipmentDetailRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Warehouse> WarehouseRepository { get; }
        IGenericRepository<RefreshToken> RefreshTokenRepository { get; }
        IGenericRepository<Sport> SportRepository { get; }
        IGenericRepository<Customer> CustomerDetailRepository { get; }
        IGenericRepository<Manager> ManagerRepository { get; }
        IGenericRepository<Staff> StaffRepository { get; }
        IGenericRepository<TwilioAccount> TwilioRepository { get; }
        IGenericRepository<ErrorLog> ErrorLogRepository { get; }
        IGenericRepository<RentalOrder> RentalOrderRepository { get; }
        IGenericRepository<RefundRequest> RefundRequestRepository { get; }

        Task<IDbContextTransaction> BeginTransactionAsync();
        void Save();
        Task<bool> SaveChanges();
    }
}
