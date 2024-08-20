using _2Sport_BE.Repository.Models;

namespace _2Sport_BE.Repository.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<Blog> BlogRepository { get; }
        IGenericRepository<Brand> BrandRepository { get; }
        IGenericRepository<Cart> CartRepository { get; }
        IGenericRepository<CartItem> CartItemRepository { get; }
        IGenericRepository<Category> CategoryRepository { get; }
        IGenericRepository<ImagesVideo> ImagesVideoRepository { get; }
        IGenericRepository<ImportHistory> ImportHistoryRepository { get; }
        IGenericRepository<Like> LikeRepository { get; }
        IGenericRepository<Order> OrderRepository { get; }
        IGenericRepository<OrderDetail> OrderDetailRepository { get; }
        IGenericRepository<PaymentMethod> PaymentMethodRepository { get; }
        IGenericRepository<Product> ProductRepository { get; }
        IGenericRepository<Classification> ClassificationRepository { get; }
        IGenericRepository<Review> ReviewRepository { get; }
        IGenericRepository<Role> RoleRepository { get; }
        IGenericRepository<ShipmentDetail> ShipmentDetailRepository { get; }
        IGenericRepository<Supplier> SupplierRepository { get; }
        IGenericRepository<User> UserRepository { get; }
        IGenericRepository<Warehouse> WarehouseRepository { get; }
        IGenericRepository<RefreshToken> RefreshTokenRepository { get; }
        IGenericRepository<Sport> SportRepository { get; }
        void Save();
        Task<bool> SaveChanges();
    }
}
