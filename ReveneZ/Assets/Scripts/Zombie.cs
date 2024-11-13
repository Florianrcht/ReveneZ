using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float health = 50f;
    public int fear = 0;  // La peur du zombie (en pourcentage)
    public float speed = 5f;
    public float damage = 10f; // Définir la valeur des dégâts infligés par le zombie

    public NavMeshAgent agent;
    public Transform player;

    // Vitesse originale pour la fuite
    private float originalSpeed;

    // Attaque
    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    // States
    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    private void Awake()
    {
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();

        // Sauvegarder la vitesse originale du zombie
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
            Debug.Log("Zombie attaque le joueur");

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
        Destroy(gameObject);
    }
}
