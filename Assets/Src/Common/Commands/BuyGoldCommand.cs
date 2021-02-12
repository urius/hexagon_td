using System.Threading.Tasks;

public struct BuyGoldCommand
{
    public async Task<bool> Execute(string productId)
    {
        var buyResult = await IAPManager.Instance.BuyProductAsync(productId);
        if (buyResult.IsSuccess)
        {
            var amount = int.Parse(productId.Split('_')[1]);
            PlayerGlobalModelHolder.Model.AddGold(amount);
        }

        return buyResult.IsSuccess;
    }
}
