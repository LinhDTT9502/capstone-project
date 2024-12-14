using _2Sport_BE.Repository.Models;
using _2Sport_BE.Infrastructure.DTOs;
using _2Sport_BE.ViewModels;
using AutoMapper;
using _2Sport_BE.Service.DTOs;

namespace _2Sport_BE.Helpers
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            #region RefundRequest
            CreateMap<Feedback, FeedbackCM>().ReverseMap();
            CreateMap<Feedback, FeedbackVM>().ReverseMap();
            CreateMap<Feedback, FeedbackUM>().ReverseMap();
            #endregion
            #region RefundRequest
            CreateMap<RefundRequest, RefundRequestCM>().ReverseMap();
            CreateMap<RefundRequest, RefundRequestVM>().ReverseMap();
            CreateMap<RefundRequest, RefundRequestUM>().ReverseMap();
            CreateMap<RefundRequestUM, RefundRequest>().ReverseMap();
            #endregion
            #region SaleOrder
            CreateMap<SaleOrderCM, SaleOrder>().ReverseMap();
            CreateMap<SaleOrderUM, SaleOrder>().ReverseMap();
            CreateMap<SaleOrder, SaleOrderVM>().ReverseMap();
            #endregion
            #region RentalOrder
            CreateMap<RentalOrder, RentalOrderVM>().ReverseMap();
            #endregion
            #region User
            CreateMap<User, UserVM>().ReverseMap();
            CreateMap<UserCM, User>().ReverseMap();
            CreateMap<UserUM, User>().
                ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<ProfileUM, User>().
               ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            #endregion
            #region Manager
            CreateMap<Manager, ManagerVM>().ReverseMap();
            CreateMap<ManagerCM, Manager>().ReverseMap();
            CreateMap<ManagerUM, Manager>().ReverseMap();
            #endregion
            #region Staff
            CreateMap<Staff, StaffVM>().ReverseMap();
            CreateMap<StaffCM, Staff>().ReverseMap();
            CreateMap<StaffUM, Staff>().ReverseMap();
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
            #region OrderDetail
            CreateMap<OrderDetail, SaleOrderDetailVM>();
            CreateMap<SaleOrderDetailUM, OrderDetail>();
            CreateMap<OrderDetailVM, OrderDetailVM>();

            #endregion
            #region Category
            CreateMap<Category, CategoryVM>()
                .ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.Name))
                .ReverseMap();
            CreateMap<Category, CategoryCM>().ReverseMap();
            CreateMap<Category, CategoryUM>().ReverseMap();
            #endregion
            #region CartItem
            CreateMap<CartItem, CartItemVM>()
                .ForMember(dest => dest.ImgAvatarPath, opt => opt.MapFrom(src => src.Product.ImgAvatarPath))
                .ReverseMap();
            CreateMap<CartItem, CartItemCM>().ReverseMap();
            CreateMap<CartItem, CartItemUM>().ReverseMap();
            #endregion
            #region Product
            CreateMap<Product, ProductVM>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand.BrandName))
				.ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.CategoryName))
				.ForMember(dest => dest.SportName, opt => opt.MapFrom(src => src.Sport.Name))
				.ForMember(dest => dest.ListImages, opt => opt.MapFrom(src => src.ImagesVideos.Select(_ => _.ImageUrl)))
                .ForMember(dest => dest.Likes, opt => opt.MapFrom(src => src.Likes.Count)).ReverseMap();
            CreateMap<Product, ProductCM>().ReverseMap();
            CreateMap<Product, ProductUM>().ReverseMap();
            #endregion
            #region Import
            CreateMap<ImportHistory, ImportVM>()
                .ForMember(dest => dest.ManagerName, opt => opt.MapFrom(src => src.Manager.User.FullName))
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
                .ForMember(dest => dest.BranchName, opt => opt.MapFrom(src => src.Branch.BranchName))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                .ReverseMap();
            CreateMap<WarehouseCM, Warehouse>().ReverseMap();
            CreateMap<WarehouseUM, Warehouse>().ReverseMap();
            #endregion
            #region Role
            CreateMap<Role, RoleCM>().ReverseMap();
            CreateMap<Role, RoleUM>().ReverseMap();
            CreateMap<Role, RoleVM>().ReverseMap();
            #endregion

        }


    }
}
