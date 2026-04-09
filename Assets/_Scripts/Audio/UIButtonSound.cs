using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UIButtonSound : MonoBehaviour, IPointerEnterHandler, ISubmitHandler, IPointerClickHandler
{
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playClickSound = true;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playHoverSound || AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.PlayButtonHover();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        if (!playClickSound || AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.PlayButtonClick();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playClickSound || AudioManager.Instance == null)
        {
            return;
        }

        AudioManager.Instance.PlayButtonClick();
    }
}
