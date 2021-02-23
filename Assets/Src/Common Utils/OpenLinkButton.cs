using UnityEngine;
using UnityEngine.UI;

public class OpenLinkButton : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private string _linkUrl;

    void Start()
    {
        _button.onClick.AddListener(OnOpenLinkClicked);
    }

    private void OnOpenLinkClicked()
    {
        Application.OpenURL(_linkUrl);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveListener(OnOpenLinkClicked);
    }
}
