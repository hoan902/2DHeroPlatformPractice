
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager s_api;

    [SerializeField] private GameObject m_audioPrefab;
    [SerializeField] private AudioClip[] m_clips;

    private readonly List<AudioSource> m_masterSounds = new();
    
    private AudioSource m_music;
    private List<AudioSource> m_sounds;
    private bool m_musicOn;
    private bool m_soundOn;
    private  Dictionary<string, float> m_durationConfig;
    private Dictionary<string, float> m_duration;
    private static float m_currentMusicVolume;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (s_api == null)
            s_api = this;
        LoadCache();
        //
        m_durationConfig = new Dictionary<string, float>(){
            {"collect-coin", 0.1f}
        };
        m_duration = new Dictionary<string, float>();
        m_currentMusicVolume = s_api.m_musicOn ? 1 : 0;
    }

    public static bool IsMusicOn()
    {
        return s_api.m_musicOn;
    }
    public static bool IsSoundOn()
    {
        return s_api.m_soundOn;
    }

    public static void MuteSound(bool mute, bool save = true)
    {
        if(save)
        {
            s_api.m_soundOn = !mute;
            s_api.SaveCache();
        }
        else
        {
            if(mute)
                 s_api.m_soundOn = false;
            else
                s_api.LoadCache();
        }
        List<AudioSource> newArr = new List<AudioSource>();
        foreach(AudioSource s in s_api.m_sounds)
        {
            if(s == null)
                continue;
            newArr.Add(s);
            s.volume = s_api.m_soundOn ? 1: 0;
        }
        s_api.m_sounds = newArr;
    }

    public static void MuteMusic(bool mute, bool save = true)
    {
        if(save)
        {
            s_api.m_musicOn = !mute;
            s_api.SaveCache();
        }
        else
        {
            if(mute)
                 s_api.m_musicOn = false;
            else
                s_api.LoadCache();
        }
        if(s_api.m_music == null)
            return;
        s_api.m_music.volume = s_api.m_musicOn ? 1 : 0;
    }

    public static void AdjustVolumeMusic(float targetVolume)
    {
        if(!s_api.m_musicOn)
            return;
        m_currentMusicVolume = targetVolume;
        s_api.m_music.volume = m_currentMusicVolume;
    }

    private void SaveCache()
    {
        PlayerPrefs.SetInt(DataKey.MUSIC, m_musicOn ? 1 : 0);
        PlayerPrefs.SetInt(DataKey.SOUND, m_soundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadCache()
    {
        m_musicOn = PlayerPrefs.GetInt(DataKey.MUSIC, 1) == 1;
        m_soundOn = PlayerPrefs.GetInt(DataKey.SOUND, 1) == 1;
    }

    public static GameObject PlaySound(string clipName, bool loop, bool music = false)
    {
        if(music)
        {
            if (s_api.m_music != null)
                Destroy(s_api.m_music.gameObject);
            
            GameObject go = Instantiate(s_api.m_audioPrefab);
            AudioHelper source = go.GetComponent<AudioHelper>();
            source.Play(s_api.GetClip(clipName), loop);
            s_api.m_music = go.GetComponent<AudioSource>();
            s_api.m_music.volume = s_api.m_musicOn ? 1 : 0;
            
            return s_api.m_music.gameObject;
        }else
        {
            if(!loop)
            {
                if(s_api.m_durationConfig.ContainsKey(clipName) && s_api.m_duration.ContainsKey(clipName))
                {
                    float delTime = Time.time - s_api.m_duration[clipName];
                    if(delTime < s_api.m_durationConfig[clipName])
                        return null;
                }
            }
            s_api.m_duration.Remove(clipName);
            s_api.m_duration.Add(clipName, Time.time);
            GameObject go = Instantiate(s_api.m_audioPrefab);
            AudioHelper source = go.GetComponent<AudioHelper>();
            source.Play(s_api.GetClip(clipName), loop);    
            AudioSource audioComp =  go.GetComponent<AudioSource>();
            audioComp.volume = s_api.m_soundOn ? 1 : 0;
            if(s_api.m_sounds == null)
                s_api.m_sounds = new List<AudioSource>();
            s_api.m_sounds.Add(audioComp);
            return go;
        }
    }
    public static GameObject PlaySound(AudioClip clip, bool loop, bool music = false, bool unpause = false)
    {
        if (music)
        {
            if (s_api.m_music != null)
                Destroy(s_api.m_music.gameObject);

            GameObject go = Instantiate(s_api.m_audioPrefab);
            AudioHelper source = go.GetComponent<AudioHelper>();
            source.Play(clip, loop);
            s_api.m_music = go.GetComponent<AudioSource>();
            s_api.m_music.volume = s_api.m_musicOn ? m_currentMusicVolume : 0;
            s_api.m_music.ignoreListenerPause = unpause;

            return s_api.m_music.gameObject;
        }
        else
        {
            if (!loop)
            {
                if (s_api.m_durationConfig.ContainsKey(clip.name) && s_api.m_duration.ContainsKey(clip.name))
                {
                    float delTime = Time.time - s_api.m_duration[clip.name];
                    if (delTime < s_api.m_durationConfig[clip.name])
                        return null;
                }
            }
            s_api.m_duration.Remove(clip.name);
            s_api.m_duration.Add(clip.name, Time.time);
            GameObject go = Instantiate(s_api.m_audioPrefab);
            AudioHelper source = go.GetComponent<AudioHelper>();
            source.Play(clip, loop);
            AudioSource audioComp = go.GetComponent<AudioSource>();
            audioComp.volume = s_api.m_soundOn ? 1 : 0;
            audioComp.ignoreListenerPause = unpause;
            if (s_api.m_sounds == null)
                s_api.m_sounds = new List<AudioSource>();
            s_api.m_sounds.Add(audioComp);
            return go;
        }
    }
    public static GameObject PlaySound3D(string clipName, float distance, bool loop, Vector2 position)
    {
        if(!loop)
        {
            if(s_api.m_durationConfig.ContainsKey(clipName) && s_api.m_duration.ContainsKey(clipName))
            {
                float delTime = Time.time - s_api.m_duration[clipName];
                if(delTime < s_api.m_durationConfig[clipName])
                    return null;
            }
        }
        GameObject go = Instantiate(s_api.m_audioPrefab);
        AudioHelper source = go.GetComponent<AudioHelper>();
        source.Play3D(s_api.GetClip(clipName), distance, loop, position);
        AudioSource audioComp =  go.GetComponent<AudioSource>();
        audioComp.volume = s_api.m_soundOn ? 1 : 0;
        if(s_api.m_sounds == null)
            s_api.m_sounds = new List<AudioSource>();
        s_api.m_sounds.Add(audioComp);
        return go;
    }
    AudioSource Create2DSound(AudioClip clip, bool loop, bool music, bool unpause)
    {
        if (music)
        {
            if (m_music != null)
                Destroy(m_music.gameObject);

            GameObject go = Instantiate(m_audioPrefab);
            AudioHelper source = go.GetComponent<AudioHelper>();
            source.Play(clip, loop);
            m_music = go.GetComponent<AudioSource>();
            m_music.volume = m_musicOn ? m_currentMusicVolume : 0;
            m_music.ignoreListenerPause = unpause;

            return m_music;
        }
        else
        {
            if (!loop)
            {
                if (m_durationConfig.ContainsKey(clip.name) && m_duration.TryGetValue(clip.name, out var value))
                {
                    float delTime = Time.time - value;
                    if (delTime < m_durationConfig[clip.name])
                        return null;
                }
            }
            m_duration.Remove(clip.name);
            m_duration.Add(clip.name, Time.time);
            GameObject go = Instantiate(m_audioPrefab);
            AudioHelper source = go.GetComponent<AudioHelper>();
            source.Play(clip, loop);
            AudioSource audioComp = go.GetComponent<AudioSource>();
            audioComp.volume = m_soundOn ? 1 : 0;
            audioComp.ignoreListenerPause = unpause;
            m_sounds.Add(audioComp);
            return audioComp;
        }
    }
    AudioSource Create3DSound(AudioClip clip, float distance, bool loop, Vector2 position)
    {
        if (!loop)
        {
            if (m_durationConfig.ContainsKey(clip.name) && m_duration.TryGetValue(clip.name, out var value))
            {
                float delTime = Time.time - value;
                if (delTime < m_durationConfig[clip.name])
                    return null;
            }
        }
        GameObject go = Instantiate(m_audioPrefab);
        AudioHelper source = go.GetComponent<AudioHelper>();
        source.Play3D(clip, distance, loop, position);
        AudioSource audioComp = go.GetComponent<AudioSource>();
        audioComp.volume = m_soundOn ? 1 : 0;
        m_sounds.Add(audioComp);
        return audioComp;
    }

    public static GameObject PlaySound3D(AudioClip clip, float distance, bool loop, Vector2 position)
    {
        if (!loop)
        {
            if (s_api.m_durationConfig.ContainsKey(clip.name) && s_api.m_duration.ContainsKey(clip.name))
            {
                float delTime = Time.time - s_api.m_duration[clip.name];
                if (delTime < s_api.m_durationConfig[clip.name])
                    return null;
            }
        }
        GameObject go = Instantiate(s_api.m_audioPrefab);
        AudioHelper source = go.GetComponent<AudioHelper>();
        source.Play3D(clip, distance, loop, position);
        AudioSource audioComp = go.GetComponent<AudioSource>();
        audioComp.volume = s_api.m_soundOn ? 1 : 0;
        if (s_api.m_sounds == null)
            s_api.m_sounds = new List<AudioSource>();
        s_api.m_sounds.Add(audioComp);
        return go;
    }
    public GameObject PlayMasterSound(AudioClip clip, bool loop, bool music = false, bool unpause = false)
    {
        Clean();
        int index = m_masterSounds.FindIndex(x => x.clip == clip);
        if (index > -1)
            return m_masterSounds[index].gameObject;
        var comp = Create2DSound(clip, loop, music, unpause);
        m_masterSounds.Add(comp);
        return comp == null ? null : comp.gameObject;
    }
    public GameObject PlayMasterSound3D(AudioClip clip, float distance, bool loop, Vector2 position)
    {
        Clean();
        int index = m_masterSounds.FindIndex(x => x.clip == clip);
        if (index > -1)
            return m_masterSounds[index].gameObject;
        var comp = Create3DSound(clip, distance, loop, position);
        m_masterSounds.Add(comp);
        return comp == null ? null : comp.gameObject;
    }
    
    void Clean()
    {
        for (int i = 0; i < m_sounds.Count; i++)
        {
            if (m_sounds[i] == null)
            {
                m_sounds.RemoveAt(i);
                i--;
            }
        }

        for (int i = 0; i < m_masterSounds.Count; i++)
        {
            if (m_masterSounds[i] == null)
            {
                m_masterSounds.RemoveAt(i);
                i--;
            }
        }
    }
    private AudioClip GetClip(string clipName)
    {
        return m_clips.FirstOrDefault(t => t.name == clipName);
    }
    void OnApplicationQuit()
    {
        MainModel.SaveAllInfo();
    }
    public static void StopMusic()
    {
        if(s_api.m_music == null)
            return;
        Destroy(s_api.m_music.gameObject);
        s_api.m_music = null;
    }
    public static AudioSource GetSource()
    {
        return s_api.m_music;
    }
    public static float GetCurrentMusicVolume()
    {
        if(s_api.m_music == null)
            return 1;
        return s_api.m_music.volume;
    }
}
