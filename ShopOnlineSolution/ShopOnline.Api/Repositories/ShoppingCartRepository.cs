﻿using Microsoft.EntityFrameworkCore;
using ShopOnline.Api.Data;
using ShopOnline.Api.Entities;
using ShopOnline.Api.Repositories.Contracts;
using ShopOnline.Models.Dtos;

namespace ShopOnline.Api.Repositories
{
    public class ShoppingCartRepository : IShoppingCartRepository
    {
        public ShoppingCartRepository(ShopOnlineDbContext shopOnlineDbContext) =>
            this.shopOnlineDbContext = shopOnlineDbContext;
        private ShopOnlineDbContext shopOnlineDbContext;
        private async Task<bool> CartItemExists(int cartId, int productId) =>
            await this.shopOnlineDbContext.CartItems.AnyAsync(c => c.CartId == cartId &&
                                                                   c.ProductId == productId);
        public async Task<CartItem> AddItem(CartItemToAddDto cartItemToAddDto)
        {
            if(await CartItemExists(cartItemToAddDto.CartId, cartItemToAddDto.ProductId) == false)
            {
                var item = await (from product in this.shopOnlineDbContext.Products
                                  where product.Id == cartItemToAddDto.ProductId
                                  select new CartItem
                                  {
                                      CartId = cartItemToAddDto.CartId,
                                      ProductId = product.Id,
                                      Qty = cartItemToAddDto.Qty,
                                  }).SingleOrDefaultAsync();
                if (item != null)
                {
                    var result = await this.shopOnlineDbContext.CartItems.AddAsync(item);
                    await this.shopOnlineDbContext.SaveChangesAsync();
                    return result.Entity;
                }
            }
            return null;
        }
        
        public async Task<CartItem> GetItem(int id) => 
            await (from cart in this.shopOnlineDbContext.Carts
                   join cartItem in this.shopOnlineDbContext.CartItems
                   on cart.Id equals cartItem.Id
                   where cartItem.Id == id
                   select new CartItem
                       {
                           Id = cartItem.Id,
                           ProductId = cartItem.ProductId,
                           Qty = cartItem.Qty,
                           CartId = cartItem.CartId,
                       }).SingleOrDefaultAsync();

        public async Task<IEnumerable<CartItem>> GetItems(int userId) =>
            await (from cart in this.shopOnlineDbContext.Carts
                          join cartItem in this.shopOnlineDbContext.CartItems
                          on cart.Id equals cartItem.CartId
                          where cart.UserId == userId
                          select new CartItem
                          {
                              Id = cartItem.Id,
                              ProductId = cartItem.ProductId,
                              Qty = cartItem.Qty,
                              CartId = cartItem.CartId
                          }).ToListAsync();

        public async Task<CartItem> UpdateQty(int id, CartItemQtyUpdateDto cartItemQtyUpdateDto)
        {
            var item = await this.shopOnlineDbContext.CartItems.FindAsync(id);
             if (item != null)
            {
                item.Qty = cartItemQtyUpdateDto.Qty;
                await this.shopOnlineDbContext.SaveChangesAsync();
                return item;
            }
            return null;
        }

        public async Task<CartItem> DeleteItem(int id)
        {
            var item = await this.shopOnlineDbContext.CartItems.FindAsync(id);
            if (item != null)
            {
                this.shopOnlineDbContext.CartItems.Remove(item);
                await shopOnlineDbContext.SaveChangesAsync();
            }
            return item;
        }
    }
}
