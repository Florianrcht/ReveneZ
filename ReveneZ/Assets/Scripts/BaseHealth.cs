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
        Debug.Log("Base a subi " + damage + " points de dégâts. Santé restante : " + currentHealth);

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
        
        // Appel de la fonction dans GameManager pour gérer la fin de partie
        //FindObjectOfType<GameManager>().OnBaseDestroyed();
        
        // Détruire l'objet de la base
        Destroy(gameObject);  // Cette ligne détruit l'objet contenant le script BaseHealth
    }


    // Fonction pour restaurer la santé (optionnel)
    public void Heal()
    {
        currentHealth = maxHealth;
        Debug.Log("Base soignée. Santé actuelle : " + currentHealth);
    }
}
