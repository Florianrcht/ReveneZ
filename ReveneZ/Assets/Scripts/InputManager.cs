using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerLook look;
    private WeaponManager weaponManager;

    private PlayerInput.OnFootActions onFoot;

    private PlayerMotor motor;
    [SerializeField] private bool lockCursor = true;

    // Start is called before the first frame update
    void Awake()
    {
        if(lockCursor)
        {
            Cursor.lockState = CursorLockMode.Locked; // Correction de la faute de frappe
            Cursor.visible = false; // Masquer le curseur en mode verrouillé
        }

        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;

        motor = GetComponent<PlayerMotor>();
        look = GetComponent<PlayerLook>();
        weaponManager = GetComponent<WeaponManager>();

        if (weaponManager == null)
        {
            Debug.LogError("WeaponManager is not assigned on " + gameObject.name);
        }

        onFoot.Jump.performed += ctx => motor.Jump();
        onFoot.Crouch.performed += ctx => motor.Crouch();
        onFoot.Sprint.performed += ctx => motor.Sprint();
        
        onFoot.Shoot.started += ctx => weaponManager.StartShooting();
        onFoot.Shoot.canceled += ctx => weaponManager.StopShooting();

        onFoot.SwitchWeapons.performed += ctx => weaponManager.Switch();

    }


    // Update is called once per frame
    void FixedUpdate()
    {
        motor.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    }

    void LateUpdate()
    {
        look.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }
}