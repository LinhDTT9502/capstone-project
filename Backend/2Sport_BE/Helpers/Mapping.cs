using _2Sport_BE.Repository.Models;
using _2Sport_BE.ViewModels;
using AutoMapper;

namespace _2Sport_BE.Helpers
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            #region User
            CreateMap<User, UserVM>()
                .ForMember(_dest => _dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
            CreateMap<UserCM, User>();
            CreateMap<UserUM, User>();
            #endregion

            #region Sport
            CreateMap<Sport, SportVM>();
            CreateMap<Sport, SportCM>();
            CreateMap<Sport, SportUM>();
            CreateMap<SportUM, Sport>();
            #endregion

            #region Brand
            CreateMap<Brand, BrandVM>().ReverseMap();
            CreateMap<Brand, BrandCM>().ReverseMap();
            CreateMap<Brand, BrandUM>().ReverseMap();
            #endregion
            #region ShipmentDetail
            CreateMap<ShipmentDetail, ShipmentDetailVM>();
            CreateMap<ShipmentDetail, ShipmentDetailCM>();
            CreateMap<ShipmentDetail, ShipmentDetailUM>();
            CreateMap<ShipmentDetailUM, ShipmentDetail>();
            #endregion
            #region PaymentMethod
            CreateMap<PaymentMethod, PaymentMethodCM>();
            CreateMap<PaymentMethod, PaymentMethodVM>();
            CreateMap<PaymentMethod, PaymentMethodUM>();
            CreateMap<PaymentMethodUM, PaymentMethod>();
            #endregion
            #region Order
            CreateMap<Order, OrderCM>();
            CreateMap<Order, OrderVM>();
            CreateMap<Order, OrderUM>();
            CreateMap<OrderUM, Order>();
            #endregion
            #region OrderDetail
            CreateMap<OrderDetail, OrderDetailVM>();
            #endregion
            #region Category
            CreateMap<Category, CategoryVM>();
            CreateMap<Category, CategoryCM>();
            CreateMap<Category, CategoryUM>();
            CreateMap<CategoryUM, Category>();
            #endregion

            #region Product
            CreateMap<Product, ProductVM>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
				.ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.Name))
				.ForMember(dest => dest.ClassificationName, opt => opt.MapFrom(src => src.Classification.ClassificationName))
				.ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Count));
            CreateMap<Product, ProductCM>();
            CreateMap<Product, ProductUM>();
            CreateMap<ProductUM, Product>();
            CreateMap<ProductCM, Product>();
            #endregion


            #region CartItem
            CreateMap<CartItem, CartItemVM>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.MainImageName, opt => opt.MapFrom(src => src.Product.MainImageName))
                .ForMember(dest => dest.MainImagePath, opt => opt.MapFrom(src => src.Product.MainImagePath));
            CreateMap<CartItem, CartItemCM>().ReverseMap();
            CreateMap<CartItem, CartItemUM>().ReverseMap();

            #endregion

            #region Supplier
            CreateMap<Supplier, SupplierVM>().ReverseMap();
            CreateMap<SupplierCM, Supplier>().ReverseMap();
            CreateMap<SupplierUM, Supplier>().ReverseMap();
            #endregion


            #region Import
            CreateMap<ImportHistory, ImportVM>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.SupplierName, opt => opt.MapFrom(src => src.Supplier.SupplierName))
                .ReverseMap();
            CreateMap<ImportCM, ImportHistory>().ReverseMap();
            CreateMap<ImportUM, ImportHistory>().ReverseMap();
            #endregion

            #region Warehouse
            CreateMap<Warehouse, WarehouseVM>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ReverseMap();
            CreateMap<WarehouseCM, Warehouse>().ReverseMap();
            CreateMap<WarehouseUM, Warehouse>().ReverseMap();
            #endregion

        }


    }
}
