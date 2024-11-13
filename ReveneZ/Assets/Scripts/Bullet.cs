using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float damage;
    private float range;
    private Vector3 startPosition;
    private Vector3 direction;

    public float speed = 20f;

    public void Initialize(float dmg, float rng, Vector3 dir)
    {
        damage = dmg;
        range = rng;
        direction = dir;
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime, Space.World);

        if (Vector3.Distance(startPosition, transform.position) >= range)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerHealth target = other.GetComponent<PlayerHealth>();
        if (target != null)
        {
            target.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
