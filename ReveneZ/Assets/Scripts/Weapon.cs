using System.Collections;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Weapon Stats")]
    public float damage = 10f;
    public float range = 100f;
    public float impactForce = 30f;
    public float fireRate = 15f;

    [Header("Ammo Stats")]
    public int magazineSize = 30; // Nombre de balles dans un chargeur
    public float reloadTime = 2f; // Temps nécessaire pour recharger
    public bool isReloading = false; // Indique si l'arme est en train de recharger
    public bool isMelee = false;

    private int currentAmmo; // Nombre de balles restantes dans le chargeur
    private float nextTimeToFire = 0f; // Temps avant le prochain tir

    public Camera cam;
    public ParticleSystem muzzleFlash;
    public GameObject impactEffect;

    [Header("Weapon Rotation During Reload")]
    public float reloadRotationSpeed = 50f; // Vitesse de rotation de l'arme pendant le rechargement
    private Vector3 initialRotation; 

    private void Start()
    {
        currentAmmo = magazineSize; // Chargeur plein au début
    }

    private void Update()
    {
        // Déclencher le rechargement si le joueur appuie sur la touche R
        if (Input.GetKeyDown(KeyCode.R) && !isReloading && currentAmmo < magazineSize)
        {
            StartCoroutine(Reload());
        }
    }

    public void Fire()
    {
        if (isReloading)
        {
            Debug.Log("Cannot fire while reloading.");
            return;
        }

        if (currentAmmo <= 0 && !isMelee)
        {
            Debug.Log("Out of ammo! Reloading...");
            StartCoroutine(Reload());
            return;
        }

        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;

            if(!isMelee) currentAmmo--; // Consomme une balle
            Debug.Log("Ammo remaining: " + currentAmmo);

            if (muzzleFlash != null) muzzleFlash.Play();

            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, range))
            {
                Zombie target = hit.transform.GetComponent<Zombie>();
                if (target != null)
                {
                    target.TakeDamage(damage);
                }

                if (hit.rigidbody != null)
                {
                    hit.rigidbody.AddForce(-hit.normal * impactForce);
                }

                GameObject impactGO = Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impactGO, 2f);
            }
        }
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");

        // Enregistrer la rotation initiale de l'arme
        initialRotation = transform.rotation.eulerAngles;

        // Effectuer une rotation de l'arme pendant le rechargement
        float startTime = Time.time;
        while (Time.time < startTime + reloadTime)
        {
            transform.Rotate(0f, 0f, -reloadRotationSpeed * Time.deltaTime / reloadTime);
            yield return null;
        }

        // Remet l'arme à sa position initiale après le rechargement
        transform.rotation = Quaternion.Euler(initialRotation); // Retour à la position initiale
        currentAmmo = magazineSize; // Remet à jour le chargeur
        isReloading = false;
        Debug.Log("Reload complete. Ammo refilled: " + currentAmmo);
    }
}
