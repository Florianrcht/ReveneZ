using UnityEngine;

public class Zombie : MonoBehaviour
{
    public float health = 50f;
    public int fear = 0;
    public float speed = 5f;
    public float gravity = -9.8f;
    private bool sprinting = false;

    // Vitesse pour marcher et sprinter
    private const float walkSpeed = 5f;
    private const float sprintSpeed = 8f;

    public void TakeDamage (float amount)
    {
        health -= amount;
        if(health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

    
}
