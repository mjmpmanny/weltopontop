using System;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    private AudioSource source;

    public AudioClip shoveSFX;
    
    public enum PlayerSoundEffect
    {
        Shove
    }
    
    private void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void PlaySoundEffect(PlayerSoundEffect sfx)
    {
        AudioClip clipToPlay = null;
        
        switch(sfx)
        {
            case PlayerSoundEffect.Shove:
                clipToPlay = shoveSFX;
                break;
        }
        
        source.PlayOneShot(clipToPlay);
    }
}
