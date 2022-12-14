﻿using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ProductsBase : ComponentBase
    {
        [Inject]
        public IProductService ProductService { get; set; }
        public IEnumerable<ProductDto> Products { get; set; }
        public string ErrorMsg { get; set; }
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Products = await ProductService.GetItems();
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
            }
        }
        protected IOrderedEnumerable<IGrouping<int, ProductDto>> GetGroupedProductsByCategory() =>
            from product in Products
            group product by product.CategoryId into prodByCatGroup
            orderby prodByCatGroup.Key
            select prodByCatGroup;
        protected string GetCategoryName(IGrouping<int, ProductDto> groupedProductDtos) =>
            groupedProductDtos.FirstOrDefault(prodGroup => prodGroup.CategoryId == groupedProductDtos.Key).CategoryName;
    }
}