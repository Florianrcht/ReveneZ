using UnityEngine;
using UnityEngine.AI;

public class Zombie : MonoBehaviour
{
    public float health = 50f;
    public float speed = 5f;
    public float damage = 10f;

    public static int fear = 50;

    public NavMeshAgent agent;
    public Transform player;

    private float originalSpeed;

    public float timeBetweenAttacks;
    public bool alreadyAttacked;

    public float sightRange, attackRange;
    public bool playerInSightRange, playerInAttackRange;
    private bool playerHasBeenInSightRange = false;

    public float updatePathInterval = 0.5f; // Temps entre les mises à jour des chemins
    private float pathUpdateTimer = 0f; // Minuteur pour les mises à jour

    public BaseHealth baseGO;

    private Vector3 closestBasePosition; // Nouvelle variable pour stocker la position la plus proche
    private bool pathToBaseHasBeenSet = false;

    private void Awake()
    {
        gameObject.tag = "Zombie"; // Ajout d'un tag
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        originalSpeed = agent.speed;
        Debug.Log("Zombie fear" + fear);
        baseGO = FindObjectOfType<BaseHealth>();
    }

    private void Start()
    {
        // Calculer la position de la base la plus proche dès le début
        Vector3[] basePositions = new Vector3[]
        {
            new Vector3(427.95f, 0f, 416.31f), // Bas 1
            new Vector3(413.63f, 0f, 416.31f), // Bas 2
            new Vector3(413.63f, 0f, 397.58f), // Bas 3
            new Vector3(427.95f, 0f, 397.58f)  // Bas 4
        };

        closestBasePosition = GetClosestPositionOnBase(basePositions);
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

        // Si le joueur quitte la portée de vision, recalculer la trajectoire vers la base
        if (playerInSightRange && !playerHasBeenInSightRange)
        {
            playerHasBeenInSightRange = true;
        }

        if (!playerInSightRange && playerHasBeenInSightRange)
        {
            // Calculer à nouveau la trajectoire vers la base la plus proche
            closestBasePosition = GetClosestPositionOnBase(new Vector3[]
            {
                new Vector3(427.95f, 0f, 416.31f),
                new Vector3(413.63f, 0f, 416.31f),
                new Vector3(413.63f, 0f, 397.58f),
                new Vector3(427.95f, 0f, 397.58f)
            });

            // Réinitialiser le statut
            playerHasBeenInSightRange = false;
            pathToBaseHasBeenSet = false;
        }
    }

    private void AttackBase()
    {
        // Vérification si le zombie est à portée d'attaque du carré formé par les 4 coins
        Vector3[] basePositions = new Vector3[]
        {
            new Vector3(427.95f, 0f, 416.31f), // Bas 1
            new Vector3(413.63f, 0f, 416.31f), // Bas 2
            new Vector3(413.63f, 0f, 397.58f), // Bas 3
            new Vector3(427.95f, 0f, 397.58f)  // Bas 4
        };

        // Vérifier si le zombie est dans la portée d'attaque
        if (IsWithinAttackRange(basePositions))
        {
            if (!alreadyAttacked)
            {
                alreadyAttacked = true;
                if (baseGO != null)
                {
                    baseGO.TakeDamage(damage);
                }
                Invoke(nameof(ResetAttack), timeBetweenAttacks);
            }
        }
        else
        {
            // Vérifier si le zombie est déjà proche de la destination avant de redéfinir la destination
            if (!pathToBaseHasBeenSet)
            {
                agent.SetDestination(closestBasePosition);
                pathToBaseHasBeenSet = true;
            }
        }
    }

    // Vérifie si le zombie est à portée de l'une des faces de la base
    private bool IsWithinAttackRange(Vector3[] basePositions)
    {
        for (int i = 0; i < basePositions.Length; i++)
        {
            Vector3 point1 = basePositions[i];
            Vector3 point2 = basePositions[(i + 1) % basePositions.Length];

            if (DistanceToLineSegment(transform.position, point1, point2) <= attackRange)
            {
                return true;
            }
        }
        return false;
    }

    // Calcule la distance entre un point et un segment de ligne
    private float DistanceToLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        Vector3 lineDirection = lineEnd - lineStart;
        float lineLength = lineDirection.magnitude;
        lineDirection.Normalize();

        float projection = Vector3.Dot(point - lineStart, lineDirection);
        projection = Mathf.Clamp(projection, 0f, lineLength);

        Vector3 projectedPoint = lineStart + lineDirection * projection;
        return Vector3.Distance(point, projectedPoint);
    }

    // Obtient la position la plus proche parmi les 4 coins de la base
    private Vector3 GetClosestPositionOnBase(Vector3[] basePositions)
    {
        float minDistance = Mathf.Infinity;
        Vector3 closestBasePosition = Vector3.zero;

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
        if (Vector3.Distance(agent.destination, player.position) > 1f)
        {
            agent.SetDestination(player.position);
        }
        agent.speed = originalSpeed;
    }

    private void FleePlayer()
    {
        Vector3 directionAwayFromPlayer = transform.position - player.position;
        Vector3 fleeDestination = transform.position + directionAwayFromPlayer;

        if (Vector3.Distance(agent.destination, fleeDestination) > 1f)
        {
            agent.SetDestination(fleeDestination);
        }
        agent.speed = originalSpeed * 1.5f;
    }

    private void AttackPlayer()
    {
        if (!alreadyAttacked)
        {
            agent.SetDestination(transform.position);
            alreadyAttacked = true;

            player.GetComponent<PlayerHealth>().TakeDamage(damage);
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

        FindObjectOfType<GameManager>().OnZombieKilled();
    }

    private void Drop()
    {
        int[] probabilities = { 40, 30, 20, 7, 3 };
        int[] ranges = { 10, 20, 30, 40, 50 };

        int selectedRange = 0;
        int cumulativeProbability = 0;
        int randomValue = Random.Range(0, 100);

        for (int i = 0; i < probabilities.Length; i++)
        {
            cumulativeProbability += probabilities[i];
            if (randomValue < cumulativeProbability)
            {
                selectedRange = ranges[i];
                break;
            }
        }

        // Placeholder pour la logique de drop (à adapter)
        Debug.Log($"Zombie dropped {selectedRange} items");
    }
}
