using UnityEngine;

public class PlayerEconomy : MonoBehaviour
{
    [Header("Economy Settings")]
    public int currentMoney = 0; // Argent actuel du joueur

    /// <summary>
    /// Ajoute de l'argent au joueur.
    /// </summary>
    /// <param name="amount">Montant à ajouter.</param>
    public void AddMoney(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Le montant à ajouter doit être positif !");
            return;
        }

        currentMoney += amount;

        Debug.Log($"Argent ajouté : {amount}. Argent total : {currentMoney}");
    }

    /// <summary>
    /// Retire de l'argent au joueur.
    /// </summary>
    /// <param name="amount">Montant à retirer.</param>
    /// <returns>Retourne vrai si la transaction a réussi, faux sinon.</returns>
    public bool SpendMoney(int amount)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Le montant à retirer doit être positif !");
            return false;
        }

        if (currentMoney < amount)
        {
            Debug.Log("Pas assez d'argent pour cette transaction.");
            return false;
        }

        currentMoney -= amount;
        Debug.Log($"Argent dépensé : {amount}. Argent restant : {currentMoney}");
        return true;
    }

    /// <summary>
    /// Obtient l'argent actuel du joueur.
    /// </summary>
    /// <returns>Le montant d'argent actuel.</returns>
    public int GetMoney()
    {
        return currentMoney;
    }

    /// <summary>
    /// Réinitialise l'argent du joueur à zéro.
    /// </summary>
    public void ResetMoney()
    {
        currentMoney = 0;
    }
}
