using System.Threading.Tasks;
using UnityEngine;

public struct BuyGoldCommand
{
    public async Task<BuyGoldResult> Execute(string productId)
    {
        var buyResult = await IAPManager.Instance.BuyProductAsync(productId);
        if (buyResult.IsSuccess)
        {
            var addedAmount = int.Parse(productId.Split('_')[1]);
            PlayerGlobalModelHolder.Model.AddGold(addedAmount);

            var saveResult = await NetworkManager.SaveUserGoldAsync(PlayerGlobalModelHolder.Model);
            if(!saveResult.IsSuccess)
            {
                return new BuyGoldResult
                {
                    IsSuccess = false,
                    Error = "Save data error",
                };
            }
        } else
        {
            return new BuyGoldResult
            {
                IsSuccess = false,
                Error = buyResult.FailureReason.ToString(),
            };
        }

        return new BuyGoldResult
        {
            IsSuccess = true,
        };
    }
}

public struct BuyGoldResult
{
    public bool IsSuccess;
    public string Error;
}
