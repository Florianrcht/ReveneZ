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
    [SerializeField] TMP_Text progressionBar;
    public int upgradesRemaining = 5;

    public void EnableInteractiontext(string text, int price)
    {
        interactionText.text = text +" "+ price + "$ (F)"; 
        interactionText.gameObject.SetActive(true);
    } 
    public void DisableInteractiontext()
    {
        interactionText.gameObject.SetActive(false);
    } 

    public void UpdateUpgradesRemaining(int newCount)
    {
        upgradesRemaining = newCount;
        progressionBar.text = "Améliorations restantes : " + upgradesRemaining;
    }
}
