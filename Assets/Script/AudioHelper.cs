
using System.Collections;
using UnityEngine;

public class AudioHelper : MonoBehaviour
{
    [SerializeField] private AudioSource m_source;

    public void Play3D(AudioClip clip, float distance, bool loop, Vector2 position)
    {
        transform.position = position;
        m_source.clip = clip;
        m_source.maxDistance = distance;
        m_source.loop = loop;
        m_source.Play();
        if(!loop)
            StartCoroutine(WaitComplete());
    }

    public void Play(AudioClip clip, bool loop)
    {
        m_source.spatialBlend = 0;
        m_source.clip = clip;
        m_source.loop = loop;
        m_source.Play();
        if(!loop)
            StartCoroutine(WaitComplete());
    }

    IEnumerator WaitComplete()
    {
        yield return new WaitUntil(()=> m_source.isPlaying == false);
        Destroy(gameObject);
    }
}
