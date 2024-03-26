using UnityEngine;
using Cysharp.Threading.Tasks;

public struct BuyGoldCommand
{
    public UniTask<bool> ExecuteAsync(string productId, RectTransform errorPopupDisplayTransform)
    {
        //call show ads here
        return UniTask.FromResult(true);
    }
}

public struct BuyGoldResult
{
    public bool IsSuccess;
    public string Error;
}
