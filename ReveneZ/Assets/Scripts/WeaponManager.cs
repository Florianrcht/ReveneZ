using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder;
    public GameObject equippedWeapon;

    private bool isShooting = false;

    public void EquipWeapon(GameObject newWeapon)
    {
        if (newWeapon == null)
        {
            Debug.LogError("Tried to equip a null weapon.");
            return;
        }

        if (equippedWeapon != null)
        {
            Destroy(equippedWeapon);
        }

        equippedWeapon = Instantiate(newWeapon, weaponHolder);
    }

    public void StartShooting()
    {
        if (equippedWeapon == null)
        {
            Debug.LogWarning("No weapon equipped, cannot shoot.");
            return;
        }

        if (!isShooting)
        {
            isShooting = true;
            StartCoroutine(ShootContinuously());
        }
    }

    public void StopShooting()
    {
        isShooting = false;
    }

    private IEnumerator ShootContinuously()
    {
        while (isShooting)
        {
            Weapon weapon = equippedWeapon.GetComponent<Weapon>();
            if (weapon != null)
            {
                float timeBetweenShots = 1f / weapon.fireRate; // Calcule l'intervalle entre les tirs
                weapon.Fire();
                yield return new WaitForSeconds(timeBetweenShots); // Attente avant le prochain tir
            }
            else
            {
                Debug.LogError("Equipped weapon does not have a Weapon component.");
                break;
            }
        }
    }
}
