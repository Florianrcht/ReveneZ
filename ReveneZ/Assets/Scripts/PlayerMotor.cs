using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool isGrounded;
    public float speed = 5f;
    public float gravity = -9.8f;
    public float jumpHeight = 0.5f;

    private bool crouching = false;
    private bool lerpCrouch = false;
    private float crouchTimer = 0f;
    private bool sprinting = false;

    // Hauteurs pour accroupi et normal
    private const float crouchHeight = 1f;
    private const float normalHeight = 2f;

    // Vitesse pour marcher et sprinter
    private const float walkSpeed = 5f;
    private const float sprintSpeed = 8f;
    private const float crouchSpeed = 2.5f;

    void Start()
    {
        // Récupère le CharacterController attaché à l'objet
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Vérifie si le joueur est au sol
        isGrounded = controller.isGrounded;

        // Gère l'animation de transition d'accroupissement
        if (lerpCrouch)
        {
            crouchTimer += Time.deltaTime;
            float progress = crouchTimer / 1f; // Durée totale de 1 seconde pour la transition

            if (crouching)
                controller.height = Mathf.Lerp(controller.height, crouchHeight, progress);
            else
                controller.height = Mathf.Lerp(controller.height, normalHeight, progress);

            if (progress > 1f)
            {
                lerpCrouch = false; // Transition terminée
                crouchTimer = 0f;
            }
        }
    }

    public void Crouch()
    {
        // Alterne entre accroupi et debout
        crouching = !crouching;
        speed = crouching ? crouchSpeed : walkSpeed;
        crouchTimer = 0f;
        lerpCrouch = true; // Commence l'animation d'accroupissement
    }

    public void Sprint()
    {
        // Alterne entre sprint et marche
        sprinting = !sprinting;
        speed = sprinting ? sprintSpeed : walkSpeed;
    }

    public void ProcessMove(Vector2 input)
    {
        // Calcule la direction de déplacement en fonction de l'entrée
        Vector3 moveDirection = Vector3.zero;
        moveDirection.x = input.x;
        moveDirection.z = input.y;

        // Applique le mouvement en fonction de la rotation du joueur
        controller.Move(transform.TransformDirection(moveDirection) * speed * Time.deltaTime);

        // Gère la gravité
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = -2f; // Empêche l'accumulation de la gravité au sol
        }

        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    public void Jump()
    {
        // Effectue un saut si le joueur est au sol
        if (isGrounded)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }
}
