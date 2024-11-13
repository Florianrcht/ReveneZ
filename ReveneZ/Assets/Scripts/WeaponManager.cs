using System.Collections;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public Transform weaponHolder; // Contient les armes
    private Weapon currentWeapon; // Référence au script Weapon de l'arme équipée
    private bool isShooting = false;

    public void StartShooting()
    {
        if (currentWeapon == null)
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
            if (currentWeapon != null)
            {
                float timeBetweenShots = 1f / currentWeapon.fireRate;
                currentWeapon.Fire();
                yield return new WaitForSeconds(timeBetweenShots);
            }
            else
            {
                Debug.LogError("Current weapon is not assigned or does not have a Weapon component.");
                break;
            }
        }
    }

    public void UpdateCurrentWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;

        if (currentWeapon == null)
        {
            Debug.LogWarning("Current weapon is null after switch.");
        }
    }

    public void Switch()
    {
        WeaponSwitching weaponSwitching = GetComponentInChildren<WeaponSwitching>();
        if (weaponSwitching != null)
        {
            weaponSwitching.SwitchWeapon();
        }
    }
}
