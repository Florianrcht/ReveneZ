using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    public static HUDController instance; 

    private void Awake()
    {
        instance = this;
    }

    [SerializeField] TMP_Text interactionText;
    [SerializeField] TMP_Text progressionText;
    [SerializeField] Slider progressionBar;

    private int upgradesDone = 0;

    public void EnableInteractiontext(string text, int price)
    {
        interactionText.text = text +" "+ price + "$ (F)"; 
        interactionText.gameObject.SetActive(true);
    } 
    public void DisableInteractiontext()
    {
        interactionText.gameObject.SetActive(false);
    } 

    public void UpdateUpgradesRemaining()
    {
        Debug.Log("ici");
        upgradesDone++;
        progressionBar.value = upgradesDone;
        progressionText.text = upgradesDone + "/5";
    }
}
