using UnityEngine;

public class BaseHealth : MonoBehaviour
{
    public float maxHealth = 1000f;
    private float currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    // Fonction pour infliger des dégâts à la base
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            DestroyBase();
        }
    }

    // Fonction appelée lorsque la base est détruite
    private void DestroyBase()
    {
        Debug.Log("La base a été détruite !");
    }


    // Fonction pour restaurer la santé (optionnel)
    public void Heal()
    {
        currentHealth = maxHealth;
        Debug.Log("Base soignée. Santé actuelle : " + currentHealth);
    }
}