using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ErrorPopup : MonoBehaviour
{
    public event Action ButtonClicked = delegate { };

    [SerializeField]
    private Text _titleTxt;
    [SerializeField]
    private InputField _messageTxt;
    [SerializeField]
    private Text _buttonTxt;
    [SerializeField]
    private Button _button;

    private TaskCompletionSource<bool> _startTsc = new TaskCompletionSource<bool>();
    private TaskCompletionSource<bool> _lifeTimeTsc = new TaskCompletionSource<bool>();
    public Task LifeTimeTask => _lifeTimeTsc.Task;

    public static ErrorPopup Show(RectTransform targetTransform, string message, string closeButtonText)
    {
        var prefab = Resources.Load<GameObject>(UIPrefabsConfig.ErrorPopupPrefabPath);
        var errorPopupGo = Instantiate(prefab, targetTransform);
        var errorPopup = errorPopupGo.GetComponent<ErrorPopup>();
        var errorText = LocalizationProvider.Instance.Get(LocalizationGroupId.ErrorPopup, "error");
        errorPopup.SetTexts(errorText, message, closeButtonText);

        return errorPopup;
    }

    public async void SetTexts(string title, string message, string buttonText)
    {
        await _startTsc.Task;

        _titleTxt.text = title;
        _messageTxt.text = message;
        _buttonTxt.text = buttonText;
    }

    private void Start()
    {
        _startTsc.TrySetResult(true);

        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        ButtonClicked();

        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _lifeTimeTsc.TrySetResult(true);

        _button.onClick.RemoveAllListeners();
    }
}
