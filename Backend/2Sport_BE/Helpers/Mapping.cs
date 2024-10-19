using _2Sport_BE.Repository.Models;
using _2Sport_BE.Service.DTOs;
using _2Sport_BE.ViewModels;
using AutoMapper;

namespace _2Sport_BE.Helpers
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            #region User
            CreateMap<User, UserVM>();
            CreateMap<UserCM, User>();
            CreateMap<UserUM, User>().
                ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProfileUM, User>().
               ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Sport
            CreateMap<Sport, SportVM>().ReverseMap();
            CreateMap<Sport, SportCM>().ReverseMap();
            CreateMap<Sport, SportUM>().ReverseMap();
            #endregion
            #region Brand
            CreateMap<Brand, BrandVM>().ReverseMap();
            CreateMap<Brand, BrandCM>().ReverseMap();
            CreateMap<Brand, BrandUM>().ReverseMap();
            #endregion
            #region Branch
            CreateMap<Branch, BranchVM>().ReverseMap();
            CreateMap<Branch, BranchCM>().ReverseMap();
            CreateMap<Branch, BranchUM>().ReverseMap();
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
            CreateMap<OrderDetail, Service.DTOs.OrderDetailVM>();
            #endregion
            #region Category
            CreateMap<Category, CategoryVM>()
                .ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.Name))
                .ReverseMap();
            CreateMap<Category, CategoryCM>().ReverseMap();
            CreateMap<Category, CategoryUM>().ReverseMap();
            #endregion
            #region Product
            CreateMap<Product, ProductVM>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
				.ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.Name))
				.ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Count)).ReverseMap();
            CreateMap<Product, ProductCM>().ReverseMap();
            CreateMap<Product, ProductUM>().ReverseMap();
            #endregion
            #region CartItem
            CreateMap<CartItem, CartItemVM>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Warehouse.Product.ProductName))
                .ForMember(dest => dest.MainImagePath, opt => opt.MapFrom(src => src.Warehouse.Product.ImgAvatarPath))
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Warehouse.Branch.BranchName));
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

            #region ImageVideo
            CreateMap<ImagesVideo, ImagesVideoVM>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ReverseMap();

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
