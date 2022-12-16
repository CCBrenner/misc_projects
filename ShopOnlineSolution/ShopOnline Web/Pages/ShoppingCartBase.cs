﻿using Microsoft.AspNetCore.Components;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ShoppingCartBase : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }
        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }
        [Inject]
        public IProductService ProductService { get; set; }
        public IEnumerable<CartItemDto> ShoppingCartItems { get; set; }
        public string ErrorMessage { get; set; }
        public string TotalPrice { get; set; }
        public int TotalQuantity { get; set; }
        protected async override Task OnInitializedAsync()
        {
            try
            {
                ShoppingCartItems = await ShoppingCartService.GetItems(HardCoded.UserId);
                CalculateCartSumaryTotals();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
        protected void UpdateQty_Input(int id)
        {
            throw new NotImplementedException();
        }
        protected void UpdateQtyCartItem_Click(int id, int qty)
        {
            try
            {
                if (qty > 0)
                {
                    var updateItemDto = new CartItemQtyUpdateDto
                    {
                        CartItemId = id,
                        Qty = qty,
                    };
                    if (this.ShoppingCartService.UpdateQty(updateItemDto) != null)
                    {
                        UpdateItemTotalPrice(id);
                        CalculateCartSumaryTotals();
                    }
                }
                else
                {
                    var item = this.ShoppingCartItems.FirstOrDefault(i => i.Id == id);
                    if (item != null)
                    {
                        item.Qty = 1;
                        item.TotalPrice = item.Price;
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        protected void DeleteCartItem_Click(int id)
        {
            if (ShoppingCartService.DeleteItem(id) != null)
                RemoveCartItem(id);
            CalculateCartSumaryTotals();
        }
        private void RemoveCartItem(int id)
        {
            var cartItemDto = GetCartItem(id);  // Return CartItemDto having given Id property
            List<CartItemDto> cartItemsList = ShoppingCartItems.ToList();
            cartItemsList.Remove(cartItemDto);
            ShoppingCartItems = cartItemsList;
        }
        private CartItemDto GetCartItem(int id) =>
            ShoppingCartItems.FirstOrDefault(x => x.Id == id);
        private void CalculateCartSumaryTotals()
        {
            SetTotalPrice();
            SetTotalQuantity();
        }
        private void SetTotalPrice() => 
            TotalPrice = this.ShoppingCartItems.Sum(p => p.TotalPrice).ToString("C");
        private void SetTotalQuantity() => 
            TotalQuantity = this.ShoppingCartItems.Sum(p => p.Qty);
        private void UpdateItemTotalPrice(int id)
        {
            var item = GetCartItem(id);

            if (item != null)
                item.TotalPrice = item.Price * item.Qty;
        }
    }
}
