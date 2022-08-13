using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    [HideInInspector]
    public string name;
    // = clip.name;
    [HideInInspector]
    public AudioSource source;
    [Range(0f, 1f)][SerializeField]
    public float volume = 1.0f;
    [HideInInspector]
    public float pitch = 1.0f;
    [SerializeField]
    private bool loop = false;

    public float get_volume()
    {
        return volume;
    }
    public void set_volume(float v)
    {
        volume = v;
    }
    
    public bool get_loop()
    {
        return loop;
    }
}