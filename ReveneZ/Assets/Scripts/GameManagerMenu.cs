using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerMenu : MonoBehaviour
{
    [Header("Background Music")]
    public AudioSource audioSource; // La source audio pour jouer la musique
    public AudioClip backgroundMusic; // La musique de fond

    private void Start()
    {
        // Démarrage de la musique de fond
        PlayBackgroundMusic();
    }

    private void PlayBackgroundMusic()
    {
        if (audioSource != null && backgroundMusic != null)
        {
            audioSource.clip = backgroundMusic;
            audioSource.loop = true; 
            audioSource.Play();
        }
    }
}