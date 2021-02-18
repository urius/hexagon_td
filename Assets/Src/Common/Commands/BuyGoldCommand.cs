using System.Threading.Tasks;
using UnityEngine;

public struct BuyGoldCommand
{
    public async Task<BuyGoldResult> ExecuteAsync(string productId)
    {
        var buyResult = await IAPManager.Instance.BuyProductAsync(productId);
        if (buyResult.IsSuccess)
        {
            var addedAmount = int.Parse(productId.Split('_')[1]);

            var saveResult = await NetworkManager.SaveUserGoldAsync(PlayerGlobalModelHolder.Model.Id, PlayerGlobalModelHolder.Model.Gold + addedAmount);
            if(!saveResult.IsSuccess)
            {
                PlayerGlobalModelHolder.Model.AddGold(addedAmount);
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
