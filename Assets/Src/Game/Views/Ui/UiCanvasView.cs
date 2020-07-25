using UnityEngine;
using UnityEngine.EventSystems;

public class UiCanvasView : MonoBehaviour, IPointerUpHandler
{
    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("UiCanvasView: OnPointerUp");
    }
}
