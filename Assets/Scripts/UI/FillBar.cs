using UnityEngine;
using UnityEngine.UI;


public class FillBar : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private bool inverse;
    

    public void Set(float value)
    {
        if (inverse) value = 1 - value;
        image.fillAmount = value;
    }
    
}