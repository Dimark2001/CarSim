using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SAM : MonoBehaviour
{
    [SerializeField]
    private AudioSource _bgMusicSource;

    [SerializeField]
    private GameObject _sfxSourcePoolObject;

    private List<AudioSource> _sfxSources;
    
    public void PlayBgMusic(AudioClipData data)
    {
        _bgMusicSource.clip = data.AudioClip;
        _bgMusicSource.volume = data.Volume;
        _bgMusicSource.loop = true;
        _bgMusicSource.Play();
    }

    public void PlaySfx(AudioClipData data)
    {
        var source = GetFreeSfxSource();
        source.clip = data.AudioClip;
        source.volume = data.Volume;
        source.Play();
    }

    private void Awake()
    {
        InitializeSfxPool();
    }

    private void InitializeSfxPool()
    {
        _sfxSources = new List<AudioSource>();

        for (var i = 0; i < 5; i++)
        {
            CreateSfxSource();
        }
    }

    private AudioSource CreateSfxSource()
    {
        var source = _sfxSourcePoolObject.AddComponent<AudioSource>();
        source.playOnAwake = false;
        _sfxSources.Add(source);
        return source;
    }

    private AudioSource GetFreeSfxSource()
    {
        foreach (var source in _sfxSources.Where(source => !source.isPlaying))
        {
            return source;
        }

        return CreateSfxSource();
    }
}