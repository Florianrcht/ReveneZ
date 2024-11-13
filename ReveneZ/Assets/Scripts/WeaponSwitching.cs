using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    private int selectedWeaponIndex = 0; // Arme par défaut
    private WeaponManager weaponManager;

    private void Start()
    {
        weaponManager = GetComponentInParent<WeaponManager>();
        SelectWeapon(); // Sélection initiale
    }

    public void SwitchWeapon()
    {
        if (transform.childCount <= 0) return;

        // Cycler entre les armes
        selectedWeaponIndex = (selectedWeaponIndex + 1) % transform.childCount;
        SelectWeapon();
    }

    private void SelectWeapon()
    {
        int index = 0;

        foreach (Transform weapon in transform)
        {
            bool isActive = index == selectedWeaponIndex;

            // Arrêter le rechargement de l'arme avant de la désactiver
            if (!isActive)
            {
                Weapon weaponComponent = weapon.GetComponent<Weapon>();
                if (weaponComponent != null)
                {
                    weaponComponent.CancelReload();
                }
            }

            weapon.gameObject.SetActive(isActive);

            if (isActive && weaponManager != null)
            {
                // Notifier WeaponManager de l'arme active
                weaponManager.UpdateCurrentWeapon(weapon.GetComponent<Weapon>());
            }

            index++;
        }
    }

}
