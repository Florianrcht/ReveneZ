using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float health = 50f;
    public float speed = 5f;
    public float damage = 10f;

    public static int fear = 0;

    public NavMeshAgent agent;
    public Transform player;

    private float originalSpeed;

    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        gameObject.tag = "Zombie"; // Ajout d'un tag
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
    }

    private void Update()
    {
        // Vérification de la distance du joueur
        float distance = Vector3.Distance(transform.position, player.position);

        playerInSightRange = distance <= sightRange;
        playerInAttackRange = distance <= attackRange;

        // Comportement du zombie en fonction de la peur
        if (!playerInSightRange && !playerInAttackRange) 
        {
            AttackBase(); // Retour à la base s'il ne voit pas le joueur
        }

        if (playerInSightRange && !playerInAttackRange) 
        {
            if (fear > 50) 
            {
                FleePlayer(); // Si la peur est grande, le zombie fuit
            }
            else 
            {
                ChasePlayer(); // Sinon, il poursuit le joueur
            }
        }

        if (playerInSightRange && playerInAttackRange) 
        {
            AttackPlayer(); // Attaquer le joueur s'il est dans la portée d'attaque
        }
    }

    private void AttackBase()
    {
        // Le zombie retourne à la base (peut être un point spécifique de la carte)
        // Agent devrait être configuré pour aller à la base
        agent.SetDestination(new Vector3(421f, 0f, 406.5f)); // Remplacer par la position de la base
    }

    private void ChasePlayer()
    {
        // Le zombie poursuit le joueur
        agent.SetDestination(player.position);
        agent.speed = originalSpeed; // Rétablir la vitesse normale en poursuivant
    }

    private void FleePlayer()
    {
        
        // Le zombie fuit dans la direction opposée du joueur
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 fleeDestination = transform.position + directionAwayFromPlayer;

        agent.SetDestination(fleeDestination); // Se déplacer dans la direction opposée du joueur
        agent.speed = originalSpeed * 1.5f; // Augmenter la vitesse de fuite (1.5x la vitesse normale)
    }

    private void AttackPlayer()
    {
        // Attaquer le joueur si dans la portée
        if (!alreadyAttacked)
        {
            alreadyAttacked = true;
            // Implémenter la logique d'attaque (par exemple, infliger des dégâts au joueur)
            player.GetComponent<PlayerHealth>().TakeDamage(damage);

            // Attendre avant de pouvoir attaquer à nouveau
            Invoke(nameof(ResetAttack), timeBetweenAttacks);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Drop();
        Destroy(gameObject);

        // Informer le GameManager qu'un zombie est mort
        FindObjectOfType<GameManager>().OnZombieKilled();
    }

    private void Drop()
    {
        // Définir les probabilités des différentes tranches
        int[] probabilities = { 40, 30, 20, 7, 3 }; // Probabilités pour les tranches
        int[] ranges = { 10, 20, 30, 40, 50 }; // Maximum pour chaque tranche

        int selectedRange = 0;
        int cumulativeProbability = 0;
        int randomValue = Random.Range(0, 100); // Random entre 0 et 99

        // Déterminer la tranche
        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability)
            {
                selectedRange = ranges[i];
                break;
            }
        }

        // Déterminer le montant dans la tranche
        int dropAmount = Random.Range(selectedRange - 9, selectedRange + 1);

        // Ajouter l'argent au joueur
        player.GetComponent<PlayerEconomy>().AddMoney(dropAmount);
    }
    

}
