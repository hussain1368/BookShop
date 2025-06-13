﻿using AutoMapper;
using Shop.Sales.API.Domain;
using Shop.Sales.API.Features.Purchases;

namespace Shop.Sales.API.Mappings
{
    public class PurchaseProfile : Profile
    {
        public PurchaseProfile()
        {
            CreateMap<CreatePurchaseCommand, Purchase>();
        }
    }
}
