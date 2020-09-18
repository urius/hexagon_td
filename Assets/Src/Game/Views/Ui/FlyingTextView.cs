using UnityEngine;
using UnityEngine.UI;

public class FlyingTextView : MonoBehaviour
{
    [SerializeField] private Text _textField;
    private RectTransform _rectTransform;

    public void SetText(string text)
    {
        _textField.text = text;
    }

    public void SetColor(Color color)
    {
        _textField.color = color;
    }

    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();

        var color = _textField.color;
        color.a = 2f;
        _textField.color = color;
    }

    void Update()
    {
        _rectTransform.anchoredPosition += new Vector2(0, 2);
        var color = _textField.color;
        color.a -= 0.03f;
        _textField.color = color;

        if (color.a <= 0)
        {
            Destroy(gameObject);
        }
    }
}
