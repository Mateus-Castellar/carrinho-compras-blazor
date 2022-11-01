using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ShopOnline.Models.Dtos;
using ShopOnline.Web.Services.Contracts;

namespace ShopOnline.Web.Pages
{
    public class ShoppingCartBase : ComponentBase
    {
        [Inject]
        public IJSRuntime Js { get; set; }

        [Inject]
        public IShoppingCartService ShoppingCartService { get; set; }

        public List<CartItemDto> ShoppingCartItems { get; set; }

        protected string TotalPrice { get; set; }

        protected int TotalQty { get; set; }

        public string ErrorMessage { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                ShoppingCartItems = await ShoppingCartService.GetItems(HardCoded.UserId);
                CalculateCartSummaryTotal();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        protected async Task DeleteCartItem_Clicked(int id)
        {
            var carrinhoItemDto = await ShoppingCartService.DeleteItem(id);
            RemoveCartItem(id);
            CalculateCartSummaryTotal();
        }

        protected async Task UpdateQtyCartItem_Click(int id, int qty)
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

                    var returnedUpdateItemDto = await ShoppingCartService.UpdateQty(updateItemDto);

                    UpdateTotalItemPrice(returnedUpdateItemDto);
                    CalculateCartSummaryTotal();
                    await MakeUpdateQtyButtonVisible(id, false);
                }
                else
                {
                    var item = ShoppingCartItems.FirstOrDefault(lbda => lbda.Id == id);

                    if (item is not null)
                    {
                        item.Qty = 1;
                        item.TotalPrice = item.Price;
                    };
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        protected async Task UpdateQty_Input(int id)
        {
            await MakeUpdateQtyButtonVisible(id, true);
        }

        private async Task MakeUpdateQtyButtonVisible(int id, bool visible)
        {
            await Js.InvokeVoidAsync("MakeUpdateQtyButtonVisible", id, visible);
        }

        private void UpdateTotalItemPrice(CartItemDto cartItemDto)
        {
            var item = GetCartItem(cartItemDto.Id);

            if (item is not null)
            {
                item.TotalPrice = cartItemDto.Price * cartItemDto.Qty;
            }
        }

        private void CalculateCartSummaryTotal()
        {
            SetTotalPrice();
            SetTotalQty();
        }

        private void SetTotalPrice()
        {
            TotalPrice = ShoppingCartItems.Sum(lbda => lbda.TotalPrice).ToString("C");
        }

        private void SetTotalQty()
        {
            TotalQty = ShoppingCartItems.Sum(lbda => lbda.Qty);
        }


        private CartItemDto GetCartItem(int id)
        {
            return ShoppingCartItems.FirstOrDefault(x => x.Id == id);
        }

        private void RemoveCartItem(int id)
        {
            var cartItemDto = GetCartItem(id);
            ShoppingCartItems.Remove(cartItemDto);
        }
    }
}
