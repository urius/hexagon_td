﻿using UnityEngine;
using Cysharp.Threading.Tasks;

public struct BuyGoldCommand
{
    public async UniTask<bool> ExecuteAsync(string productId, RectTransform errorPopupDisplayTransform)
    {
        var buyResult = await IAPManager.Instance.BuyProductAsync(productId);
        if (buyResult.IsSuccess)
        {
            var addedAmount = int.Parse(productId.Split('_')[1]);

            var saveResult = await NetworkManager.SaveUserGoldAsync(PlayerSessionModel.Model.Id, PlayerSessionModel.Model.Gold + addedAmount);
            if (saveResult.IsSuccess)
            {
                PlayerSessionModel.Model.AddGold(addedAmount);
                return true;
            }
            else
            {
                var closeText = LocalizationProvider.Instance.Get(LocalizationGroupId.ErrorPopup, "close");
                var errorText = LocalizationProvider.Instance.Get(LocalizationGroupId.ErrorPopup, "error");
                ErrorPopup.Show(errorPopupDisplayTransform, errorText + ": Save data error", closeText);
            }
        }
        else
        {
            var closeText = LocalizationProvider.Instance.Get(LocalizationGroupId.ErrorPopup, "close");
            var errorText = LocalizationProvider.Instance.Get(LocalizationGroupId.ErrorPopup, "error");
            ErrorPopup.Show(errorPopupDisplayTransform, errorText + $": {buyResult.FailureReason}", closeText);
        }

        return false;
    }
}

public struct BuyGoldResult
{
    public bool IsSuccess;
    public string Error;
}