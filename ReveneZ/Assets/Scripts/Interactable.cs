using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class Interactable : MonoBehaviour
{
    private Outline outline;
    public string message;
    public int price = 0;  

    public UnityEvent onInteraction;

    private PlayerEconomy playerEconomy;  

    void Start()
    {
        outline = GetComponent<Outline>();
        DisableOutline();   
        
        playerEconomy = FindObjectOfType<PlayerEconomy>();
        if (playerEconomy == null)
        {
            Debug.LogError("PlayerEconomy non trouvé dans la scène.");
        }
    }

    public void Interact()
    {
        if (playerEconomy != null && playerEconomy.SpendMoney(price))
        {
            onInteraction.Invoke();
            if(price == 1500)
            {
                LoadVictoryScene();
            }
            Debug.Log("Interaction réussie avec " + gameObject.name);
            if (HUDController.instance != null)
            {
                HUDController.instance.UpdateUpgradesRemaining();
            }
        }
        else
        {
            Debug.Log("Pas assez d'argent pour interagir avec " + gameObject.name);
        }
    }
    public void LoadVictoryScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 3);
    }
    
    public void DisableOutline()
    {
        outline.enabled = false;
    }
    public void EnableOutline()
    {
        outline.enabled = true;
    }
}
