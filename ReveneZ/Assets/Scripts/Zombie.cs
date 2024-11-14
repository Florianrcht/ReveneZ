using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float health = 50f;
    public float speed = 5f;
    public float damage = 10f;

    public static int fear = -10;

    public NavMeshAgent agent;
    public Transform player;

    private float originalSpeed;

    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;

    public float updatePathInterval = 0.5f; // Temps entre les mises à jour des chemins
    private float pathUpdateTimer = 0f; // Minuteur pour les mises à jour

    public BaseHealth baseGO;

    private void Awake()
    {
        gameObject.tag = "Zombie"; // Ajout d'un tag
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        Debug.Log("Zombie fear" + fear);
        baseGO = FindObjectOfType<BaseHealth>();
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
        // Liste des positions "faces" de la base (2D, sans tenir compte de la hauteur)
        Vector3[] basePositions = new Vector3[]
        {
            new Vector3(427.95f, 0f, 416.31f), // Bas 1
            new Vector3(413.63f, 0f, 416.31f), // Bas 2
            new Vector3(413.63f, 0f, 397.58f), // Bas 3
            new Vector3(427.95f, 0f, 397.58f)  // Bas 4
        };

        // Vérification si le zombie est à portée d'attaque du carré formé par les 4 coins
        if (IsWithinAttackRange(basePositions))
        {
            // Si à portée d'attaque, infliger des dégâts à la base
            if (!alreadyAttacked)
            {
                alreadyAttacked = true;
                // Infliger des dégâts à la base via le composant BaseHealth
                if (baseGO != null)
                {
                    baseGO.TakeDamage(damage); // Inflige des dégâts à la base
                }
                Invoke(nameof(ResetAttack), timeBetweenAttacks); // Réinitialise l'attaque après un délai
            }
        }
        else
        {
            // Si pas encore à portée, se diriger vers la position la plus proche du carré
            Vector3 closestBasePosition = GetClosestPositionOnBase(basePositions);
            agent.SetDestination(closestBasePosition);
        }
    }

    // Vérifie si le zombie est à portée de l'une des faces de la base (rectangle 2D)
    private bool IsWithinAttackRange(Vector3[] basePositions)
    {
        // Calcule la distance minimale entre le zombie et les bords du carré (défini par les 4 coins)
        for (int i = 0; i < basePositions.Length; i++)
        {
            Vector3 point1 = basePositions[i];
            Vector3 point2 = basePositions[(i + 1) % basePositions.Length]; // Le prochain point, avec wrap-around

            // Vérifie si la distance entre le zombie et le segment de ligne est inférieure à la portée d'attaque
            if (DistanceToLineSegment(transform.position, point1, point2) <= attackRange)
            {
                return true; // Si à portée de l'un des bords
            }
        }
        return false; // Si aucun des bords n'est dans la portée
    }

    // Calcule la distance entre un point et un segment de ligne (entre deux points)
    private float DistanceToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        // Projette le point sur la ligne
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();
        
        // Trouver la projection du point sur le segment
        float projection = Vector3.Dot(point - lineStart, lineDirection);
        projection = Mathf.Clamp(projection, 0f, lineLength); // Clamping pour être dans la plage du segment

        // Trouver la position projetée sur la ligne
        Vector3 projectedPoint = lineStart + lineDirection * projection;
        
        // Retourner la distance entre le point projeté et le point initial
        return Vector3.Distance(point, projectedPoint);
    }

    // Obtient la position la plus proche parmi les 4 coins de la base (2D)
    private Vector3 GetClosestPositionOnBase(Vector3[] basePositions)
    {
        // Initialiser la distance minimale et la position de la face la plus proche
        float minDistance = Mathf.Infinity;
        Vector3 closestBasePosition = Vector3.zero;

        // Trouver le coin le plus proche
        foreach (var basePos in basePositions)
        {
            float distance = Vector3.Distance(transform.position, basePos);
            if (distance < minDistance)
            {
                minDistance = distance;
                closestBasePosition = basePos;
            }
        }
        return closestBasePosition;
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
