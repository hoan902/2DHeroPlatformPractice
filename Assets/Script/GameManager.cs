using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    AudioClip m_musicTheme;
    // Start is called before the first frame update
    void Start()
    {
        InitMusic();
    }

    void InitMusic()
    {
        SoundManager.PlaySound(m_musicTheme, true, true);
    }
}
