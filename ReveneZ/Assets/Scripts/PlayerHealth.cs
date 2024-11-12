using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public float maxHealth = 100f; // Vie maximale du personnage
    private float currentHealth;  // Vie actuelle du personnage
    private bool isDead = false;  // Indique si le personnage est mort

    [Header("Invincibility Settings")]
    public bool useInvincibility = true; // Activer/désactiver l'invincibilité temporaire
    public float invincibilityDuration = 2f; // Durée d'invincibilité après avoir pris des dégâts
    private bool isInvincible = false;  // Indique si le personnage est invincible

    [Header("UI/Feedback")]
    public GameObject deathEffect; // Effet de mort (facultatif)

    void Start()
    {
        // Initialise la vie au maximum au début
        currentHealth = maxHealth;
    }

    /// <summary>
    /// Inflige des dégâts au personnage.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (isDead || (useInvincibility && isInvincible))
            return;

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            Die();
        }
        else if (useInvincibility)
        {
            StartCoroutine(InvincibilityCooldown());
        }

        Debug.Log(currentHealth);
    }

    /// <summary>
    /// Soigne le personnage.
    /// </summary>
    public void Heal(float amount)
    {
        if (isDead)
            return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth); // Ne dépasse pas la vie max
    }

    /// <summary>
    /// Tue le personnage.
    /// </summary>
    private void Die()
    {
        isDead = true;
        Debug.Log($"{gameObject.name} est mort.");

        // Jouer un effet de mort, si disponible
        if (deathEffect != null)
        {
            Instantiate(deathEffect, transform.position, Quaternion.identity);
        }

        // Désactiver le personnage (peut être ajusté en fonction de votre jeu)
        gameObject.SetActive(false);

        // Vous pouvez appeler ici d'autres méthodes pour gérer la mort (recharger le niveau, afficher un écran de défaite, etc.)
    }

    /// <summary>
    /// Coroutine pour gérer l'invincibilité temporaire après avoir pris des dégâts.
    /// </summary>
    private IEnumerator InvincibilityCooldown()
    {
        isInvincible = true;
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false;
    }

    /// <summary>
    /// Réinitialise la santé et l'état du personnage (utile lors d'un respawn).
    /// </summary>
    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDead = false;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Renvoie la santé actuelle.
    /// </summary>
    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    /// <summary>
    /// Renvoie si le personnage est mort.
    /// </summary>
    public bool IsDead()
    {
        return isDead;
    }
}
