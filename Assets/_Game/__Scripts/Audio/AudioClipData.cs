using UnityEngine;

[CreateAssetMenu(fileName = "AudioClipData", menuName = "Audio/AudioClipData")]
public class AudioClipData : ScriptableObject
{
    [field: SerializeField]
    public AudioClip AudioClip { get; private set; }

    [field: SerializeField]
    public float Volume { get; private set; }
}