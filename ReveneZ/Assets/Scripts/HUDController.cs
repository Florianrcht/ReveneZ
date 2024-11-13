using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDController : MonoBehaviour
{
    public static HUDController instance; 

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] TMP_Text interactionText;

    public void EnableInteractiontext(string text)
    {
        interactionText.text = text + " (F)"; 
        interactionText.gameObject.SetActive(true);
    } 
    public void DisableInteractiontext()
    {
        interactionText.gameObject.SetActive(false);
    } 
}
