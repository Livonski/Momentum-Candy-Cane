using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSoundEffects : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound; 
    public AudioClip clickSound; 

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            SFXPlayer.Instance.PlaySFX(hoverSound);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            SFXPlayer.Instance.PlaySFX(clickSound);
        }
    }
}