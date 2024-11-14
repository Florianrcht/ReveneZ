using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float health = 50f;
    public float speed = 5f;
    public float damage = 10f;
    public float baseDamage = 10f; // Dégâts infligés à la base par le zombie

    public static int fear = -10;

    public NavMeshAgent agent;
    public Transform player;
    public BaseHealth baseHealth; // Référence au script BaseHealth

    private float originalSpeed;

    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float updatePathInterval = 0.5f; // Temps entre les mises à jour des chemins
    private float pathUpdateTimer = 0f; // Minuteur pour les mises à jour


    private void Awake()
    {
        gameObject.tag = "Zombie"; // Ajout d'un tag
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        Debug.Log("Zombie fear" + fear);
    }

    private void Update()
    {
        pathUpdateTimer += Time.deltaTime;

        float distance = Vector3.Distance(transform.position, player.position);
        playerInSightRange = distance <= sightRange;
        playerInAttackRange = distance <= attackRange;

        if (pathUpdateTimer >= updatePathInterval)
        {
            if (!playerInSightRange && !playerInAttackRange)
            {
                AttackBase();
            }
            else if (playerInSightRange && !playerInAttackRange)
            {
                if (fear > 50)
                {
                    FleePlayer();
                }
                else
                {
                    ChasePlayer();
                }
            }
            else if (playerInSightRange && playerInAttackRange)
            {
                AttackPlayer();
            }

            pathUpdateTimer = 0f; // Réinitialiser le minuteur
        }
    }

    private void AttackBase()
    {
        // Définit la destination vers la base
        Vector3 basePosition = new Vector3(421f, 0f, 406.5f); // Position de la base
        
        // Vérifiez si la position de la base est atteignable
        NavMeshPath path = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, basePosition, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
        {
            agent.SetDestination(basePosition);
        }
        else
        {
            Debug.Log("La destination de la base est bloquée ou inaccessible.");
        }

        // Vérifie si le zombie est proche de la base
        if (Vector3.Distance(transform.position, basePosition) <= agent.stoppingDistance)
        {
            if (!alreadyAttacked) // Empêche les dégâts trop fréquents
            {
                alreadyAttacked = true;
                baseHealth.TakeDamage(baseDamage); // Inflige des dégâts à la base
                Invoke(nameof(ResetAttack), timeBetweenAttacks); // Réinitialise l'attaque après un délai
            }
        }
    }



    private void ChasePlayer()
    {
        if (Vector3.Distance(agent.destination, player.position) > 1f) // Met à jour seulement si nécessaire
        {
            agent.SetDestination(player.position);
        }
        agent.speed = originalSpeed;
    }


    private void FleePlayer()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 fleeDestination = transform.position + directionAwayFromPlayer;

        if (Vector3.Distance(agent.destination, fleeDestination) > 1f) // Met à jour seulement si nécessaire
        {
            agent.SetDestination(fleeDestination);
        }
        agent.speed = originalSpeed * 1.5f;
    }


    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            agent.SetDestination(transform.position); // Arrête le mouvement
            alreadyAttacked = true;

            // Infliger des dégâts au joueur
            player.GetComponent<PlayerHealth>().TakeDamage(damage);

            // Réinitialiser l'attaque après un délai
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
