using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Sound
{
    public string name;
    public AudioClip clip;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(0.1f, 3f)]
    public float pitch = 1f;
    public bool loop = false;
}

public class AudioManager : Singleton<AudioManager>
{
    [Header("Звуковые эффекты")]
    public Sound[] sounds;
    
    [Header("Музыка")]
    public Sound[] music;
    
    private Dictionary<string, AudioSource> soundSources = new Dictionary<string, AudioSource>();
    private AudioSource currentMusicSource;
    
    [Header("Настройки громкости")]
    [Range(0f, 1f)]
    public float masterVolume = 1f;
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    [Range(0f, 1f)]
    public float musicVolume = 1f;
    
    protected override void Awake()
    {
        base.Awake();
        InitializeAudio();
    }
    
    void InitializeAudio()
    {
        // Создаем AudioSource для каждого звука
        foreach (Sound s in sounds)
        {
            GameObject soundObject = new GameObject("Sound_" + s.name);
            soundObject.transform.SetParent(transform);
            
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.clip = s.clip;
            audioSource.volume = s.volume;
            audioSource.pitch = s.pitch;
            audioSource.loop = s.loop;
            audioSource.playOnAwake = false;
            
            soundSources.Add(s.name, audioSource);
        }
    }
    
    // Воспроизведение звукового эффекта
    public void PlaySound(string soundName)
    {
        if (soundSources.TryGetValue(soundName, out var source))
        {
            source.volume = source.volume * soundVolume * masterVolume;
            source.Play();
        }
        else
        {
            Debug.LogWarning($"Звук '{soundName}' не найден!");
        }
    }
    
    // Остановка звука
    public void StopSound(string soundName)
    {
        if (soundSources.ContainsKey(soundName))
        {
            soundSources[soundName].Stop();
        }
    }
    
    // Воспроизведение музыки
    public void PlayMusic(string musicName)
    {
        Sound musicToPlay = System.Array.Find(music, m => m.name == musicName);
        
        if (musicToPlay == null)
        {
            Debug.LogWarning($"Музыка '{musicName}' не найдена!");
            return;
        }
        
        // Останавливаем текущую музыку
        if (currentMusicSource != null)
        {
            currentMusicSource.Stop();
            Destroy(currentMusicSource.gameObject);
        }
        
        // Создаем новый источник для музыки
        GameObject musicObject = new GameObject("Music_" + musicToPlay.name);
        musicObject.transform.SetParent(transform);
        
        currentMusicSource = musicObject.AddComponent<AudioSource>();
        currentMusicSource.clip = musicToPlay.clip;
        currentMusicSource.volume = musicToPlay.volume * musicVolume * masterVolume;
        currentMusicSource.pitch = musicToPlay.pitch;
        currentMusicSource.loop = musicToPlay.loop;
        currentMusicSource.Play();
    }
    
    // Остановка музыки
    public void StopMusic()
    {
        if (currentMusicSource != null)
        {
            currentMusicSource.Stop();
        }
    }
    
    // Пауза музыки
    public void PauseMusic()
    {
        if (currentMusicSource != null)
        {
            currentMusicSource.Pause();
        }
    }
    
    // Продолжение музыки
    public void UnpauseMusic()
    {
        if (currentMusicSource != null)
        {
            currentMusicSource.UnPause();
        }
    }
    
    // Установка громкости
    public void SetMasterVolume(float volume)
    {
        masterVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }
    
    public void SetSoundVolume(float volume)
    {
        soundVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
    }
    
    public void SetMusicVolume(float volume)
    {
        musicVolume = Mathf.Clamp01(volume);
        if (currentMusicSource != null)
        {
            currentMusicSource.volume = musicVolume * masterVolume;
        }
    }
    
    void UpdateAllVolumes()
    {
        foreach (var source in soundSources.Values)
        {
            source.volume = source.volume * soundVolume * masterVolume;
        }
        
        if (currentMusicSource != null)
        {
            currentMusicSource.volume = musicVolume * masterVolume;
        }
    }
}