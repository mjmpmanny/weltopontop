using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class Audio : MonoBehaviour
{
    //Set this number to how many sounds of that type exist.
    //Sounds MUST follow the naming convention: "shove1.wav, shove2.wav, etc." (Use 1, even if you only have 1)
    private int jumpSoundsCount = 1;
    private int landSoundsCount = 1;
    private int gruntSoundsCount = 1;
    private int shoveSoundsCount = 1;

    //Can't use Resources.Load because these files are too big and will lag the game.
    public AudioClip parkourRound;
    public AudioClip buildRound;
    
    #region Enum Definitions
    public enum MusicType
    {
        Countdown,
        ParkourRound,
        BuildRound
    }
    
    public enum PlayerSoundEffect
    {
        Jump,
        Land,
        Grunt,
        Shove
    }

    public enum PlaceableSoundEffect
    {
        Place,
        Invalid
    }

    public enum ItemSoundEffect
    {
        Pickup,
        Drop
    }
    #endregion
    
    public AudioSource currentSource;
    public AudioSource incomingSource;    
    private float fadeDurationInSeconds = 0.35f;
    private float maximumVolume = 0.8f;

    public void PlayPlayerSoundEffect(PlayerController player, PlayerSoundEffect soundEffectType)
    {
        AudioSource source = player.GetComponent<AudioSource>();
        
        source.PlayOneShot(RandomPlayerClip(soundEffectType));
    }

    public AudioClip RandomPlayerClip(PlayerSoundEffect soundEffectType)
    {
        string retrievePath = "";
        int clipVariant = 0;
        
        switch (soundEffectType)
        {
            case PlayerSoundEffect.Grunt:
                retrievePath = "Audio/SFX/Player/grunt";
                clipVariant = GetRandomInt(gruntSoundsCount);
                break;
            case PlayerSoundEffect.Jump:
                retrievePath = "Audio/SFX/Player/jump";
                clipVariant = GetRandomInt(jumpSoundsCount);
                break;
            case PlayerSoundEffect.Land:
                retrievePath = "Audio/SFX/Player/land";
                clipVariant = GetRandomInt(landSoundsCount);
                break;
            case PlayerSoundEffect.Shove:
                retrievePath = "Audio/SFX/Player/shove";
                clipVariant = GetRandomInt(shoveSoundsCount);
                break;
        }
        
        return Resources.Load<AudioClip>($"{retrievePath}{clipVariant}");
    }

    public void PlayMusic(MusicType musicType)
    {
        switch (musicType)
        {
            case MusicType.ParkourRound:
                ParkourMusic();
                break;
            case MusicType.BuildRound:
                BuildMusic();
                break;
        }
    }
    
    private void ParkourMusic()
    {
        if (currentSource.clip == null)
        {
            currentSource.clip = parkourRound;
            currentSource.Play();
            return;
        }

        if (currentSource.clip == parkourRound)
            return;

        StartCoroutine(CrossFade(parkourRound));
    }

    private void BuildMusic()
    {
        if (currentSource.clip == null)
        {
            currentSource.clip = buildRound;
            currentSource.Play();
            return;
        }

        if (currentSource.clip == buildRound)
            return;

        StartCoroutine(CrossFade(buildRound));
    }
    
    private int GetRandomInt(int maxCount)
    {
        return Random.Range(1, maxCount + 1);
    }
    
    public IEnumerator CrossFade(AudioClip newClip)
    {
        incomingSource.clip = newClip;
        incomingSource.volume = 0f;
        incomingSource.Play();

        float elapsedTimeInSeconds = 0f;
        while (elapsedTimeInSeconds < fadeDurationInSeconds)
        {
            elapsedTimeInSeconds += Time.deltaTime;
            float percentTime = elapsedTimeInSeconds / fadeDurationInSeconds;
            
            currentSource.volume = maximumVolume * (1 - percentTime);
            incomingSource.volume = maximumVolume * percentTime;

            yield return null;
        }
        
        currentSource.Stop();
        currentSource.volume = maximumVolume;
        (currentSource, incomingSource) = (incomingSource, currentSource);
    }
}
