using AS400PizzaEnterprise.Application.DTOs;
using AS400PizzaEnterprise.Domain.Entities;
using AutoMapper;

namespace AS400PizzaEnterprise.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Amount))
            .ForMember(dest => dest.DeliveryAddress, opt => opt.MapFrom(src => src.DeliveryAddress.ToString()))
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FullName : string.Empty))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.UnitPrice.Amount))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.Subtotal.Amount));

        CreateMap<Pizza, PizzaDto>()
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.BasePrice.Amount))
            .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.Size.ToString()));

        CreateMap<Customer, CustomerDto>()
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.DefaultAddress != null ? src.DefaultAddress.ToString() : null));
    }
}
