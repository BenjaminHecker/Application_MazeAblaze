using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AssetCatalogue;

public class SoundManager : MonoBehaviour
{
    public Sound mainTheme;
    public WorldTheme[] worldThemes;
    public Sound[] miscSounds;

    public static SoundManager instance;
    private static bool mainScreen;
    private static World world;

    [System.Serializable]
    public struct WorldTheme
    {
        public World world;
        public Sound sound;
        public float initialDelay;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);

        SetupSound(mainTheme);

        foreach (WorldTheme theme in worldThemes)
            SetupSound(theme.sound);

        foreach (Sound s in miscSounds)
            SetupSound(s);
    }

    private void Start()
    {
        mainTheme.source.Play();
        mainScreen = true;
    }

    private void SetupSound(Sound s)
    {
        s.source = gameObject.AddComponent<AudioSource>();
        s.source.clip = s.clip;
        s.source.volume = s.volume;
        s.source.pitch = s.pitch;
        s.source.loop = s.loop;
    }

    public static void UpdatePitch()
    {
        foreach (WorldTheme theme in instance.worldThemes)
            theme.sound.source.pitch = Time.timeScale;
        foreach (Sound s in instance.miscSounds)
            s.source.pitch = Time.timeScale;
    }

    public static void PlayMainTheme()
    {
        if (!mainScreen)
        {
            StopThemes();

            instance.mainTheme.source.Play();
        }

        mainScreen = true;
    }

    public static float PlayTheme(World _world)
    {
        float initialDelay = 0f;
        
        StopThemes();

        foreach (WorldTheme theme in instance.worldThemes)
        {
            if (theme.world == _world)
            {
                theme.sound.source.Play();
                initialDelay = theme.initialDelay;
            }
        }

        mainScreen = false;
        world = _world;

        return initialDelay;
    }

    public static void StopThemes()
    {
        if (instance != null)
        {
            instance.mainTheme.source.Stop();

            foreach (WorldTheme theme in instance.worldThemes)
                theme.sound.source.Stop();
        }
    }

    public static void PlayMisc(string name)
    {
        foreach (Sound s in instance.miscSounds)
        {
            if (s.name == name)
            {
                if (s.stack)
                    s.source.PlayOneShot(s.clip);
                else
                    s.source.Play();

                return;
            }
        }
        Debug.LogWarning("Sound " + name + " not found");
    }

    private static void StopMisc(string name)
    {
        foreach (Sound s in instance.miscSounds)
        {
            if (s.name == name)
            {
                s.source.Stop();
                return;
            }
        }
        Debug.LogWarning("Sound " + name + " not found");
    }
}
