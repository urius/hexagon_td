using System.Collections.Generic;
using System.Linq;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;

public class AnalyticsManager
{
    public static AnalyticsManager Instance { get; private set; }

    private const string LevelParamName = "Level";
    private const string StarsParamName = "Stars";
    private const string ContinuesAmountParamName = "ContinuesAmount";    
    private const string WaveNumParamName = "WaveNum";
    private const string BoosterIdsParamName = "BoosterIds";
    private const string SceneNameParamName = "SceneName";
    private const string ProductIdParamName = "ProductId";
    private const string ReasonParamName = "Reason";

    static AnalyticsManager()
    {
        Instance = new AnalyticsManager();
    }

    public AnalyticsResult SendLevelCompleted(int levelIndex, int starsAmount, int continuesAmount, IEnumerable<BoosterId> boosterIds)
    {
        var boosterIdsStr = string.Join(",", boosterIds.Select(id => id.ToString()));
        return AnalyticsEvent.Custom("LevelCompleted", new Dictionary<string, object>() {
            { LevelParamName, levelIndex + 1 },
            { StarsParamName, starsAmount },
            { ContinuesAmountParamName, continuesAmount },
            { BoosterIdsParamName, boosterIdsStr },
        });
    }

    public AnalyticsResult SendBuyGoldError(string productId, string reason)
    {
        return AnalyticsEvent.Custom("BuyGoldError", new Dictionary<string, object>() {
            { ProductIdParamName, productId },
            { ReasonParamName, reason },
        });
    }

    public AnalyticsResult SendLevelFailed(int levelIndex, int continuesAmount, int waveIndex, IEnumerable<BoosterId> boosterIds)
    {
        var boosterIdsStr = string.Join(",", boosterIds.Select(id => id.ToString()));
        return AnalyticsEvent.Custom("LevelFailed", new Dictionary<string, object>() {
            { LevelParamName, levelIndex + 1 },
            { ContinuesAmountParamName, continuesAmount },
            { WaveNumParamName, waveIndex + 1},
            { BoosterIdsParamName, boosterIdsStr },
        });
    }

    public AnalyticsResult SendStoreOpened()
    {
        var sceneName = SceneManager.GetActiveScene().name;
        return AnalyticsEvent.Custom("StoreOpened", new Dictionary<string, object>() {
            { SceneNameParamName, sceneName},
        });
    }

    public AnalyticsResult SendHowToPlayOpened()
    {
        return AnalyticsEvent.Custom("HowToPlayOpened");
    }

    public AnalyticsResult SendHowToPlayFinishClicked()
    {
        return AnalyticsEvent.Custom("HowToPlayFinishClicked");
    }
}
