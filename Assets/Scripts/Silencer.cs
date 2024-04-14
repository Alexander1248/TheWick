using NPC;
using UnityEngine;

public class Silencer : MonoBehaviour
{
    [SerializeField] private AudioSource source;
    [SerializeField] private float volume;

    private float _buff;
    public void SilenceStart(string tag)
    {
        if (tag != "silence") return;
        _buff = source.volume;
        source.volume = volume;
    }
    public void SilenceEnd(string tag)
    {
        if (tag != "silence") return;
        source.volume = _buff;
    }
}