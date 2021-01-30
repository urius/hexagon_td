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
        _button.onClick.RemoveAllListeners();
    }
}
