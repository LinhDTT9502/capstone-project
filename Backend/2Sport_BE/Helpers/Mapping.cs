using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.ViewModels;
using AutoMapper;

namespace _2Sport_BE.Helpers
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            #region RentalOrder
            CreateMap<RentalOrder, RentalOrderVM>();
            #endregion
            #region User
            CreateMap<User, UserVM>();
            CreateMap<UserCM, User>();
            CreateMap<UserUM, User>().
                ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProfileUM, User>().
               ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Manager
            CreateMap<Manager, ManagerVM>();
            CreateMap<ManagerCM, Manager>();
            CreateMap<ManagerUM, Manager>();
            #endregion
            #region Staff
            CreateMap<Staff, StaffVM>();
            CreateMap<StaffCM, Staff>();
            CreateMap<StaffUM, Staff>();
            #endregion
            #region Blog
            CreateMap<Blog, BlogVM>()
                .ForMember(dest => dest.BlogId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedByStaffId, opt => opt.MapFrom(src => src.CreatedStaffId))
                .ForMember(dest => dest.EditedByStaffId, opt => opt.MapFrom(src => src.EditedByStaffId))
                .ForMember(dest => dest.CreatedByStaffName, opt => opt.MapFrom(src => src.CreatedByStaff.User.UserName))
                .ForMember(dest => dest.CreatedByStaffFullName, opt => opt.MapFrom(src => src.CreatedByStaff.User.FullName))
                .ForMember(dest => dest.EditedByStaffName, opt => opt.MapFrom(src => src.EditedByStaff.User.UserName))
                .ForMember(dest => dest.EditedByStaffFullName, opt => opt.MapFrom(src => src.EditedByStaff.User.FullName))
                .ReverseMap();
            CreateMap<BlogCM, Blog>();
            CreateMap<BlogUM, Blog>();
            #endregion
            #region Customer
            CreateMap<Customer, CustomerVM>();
            CreateMap<CustomerCM, Customer>();
            CreateMap<CustomerUM, Customer>();
            #endregion
            #region Comment
            CreateMap<Comment, CommentVM>()
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.UserName))
                .ReverseMap();
            CreateMap<Comment, CommentCM>().ReverseMap();
            CreateMap<Comment, CommentUM>().ReverseMap();
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
            #region SaleOrder
            CreateMap<SaleOrderCM, SaleOrder>();
            CreateMap<SaleOrderUM, SaleOrder>();
            CreateMap<SaleOrder, SaleOrderVM>();
            #endregion
            #region OrderDetail
            CreateMap<OrderDetail, SaleOrderDetailVM>();
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
            #region Import
            CreateMap<ImportHistory, ImportVM>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
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
