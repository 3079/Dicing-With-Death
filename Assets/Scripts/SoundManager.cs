using UnityEngine.Audio;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoundManager : MonoBehaviour
{
	[SerializeField]
	public Sound[] clips;

	public static SoundManager instance;

	void Awake()
	{
		if(instance == null){
			instance = this;
		}
		else{
			Destroy(gameObject);
			return;
		}
		DontDestroyOnLoad(gameObject);
		foreach(Sound s in clips)
		{
			s.source = gameObject.AddComponent<AudioSource>();
			s.name = s.clip.name;
			s.source.clip = s.clip;
			s.source.name = s.name;
			//Debug.Log(s.source.name);

			s.source.volume = s.get_volume();
			s.source.pitch = 1.0f;
			s.source.loop = s.get_loop();
		}
	}

	public void Play(string name)
	{
		Sound s = Array.Find(clips, sound => sound.name == name);
		if (s == null){
			Debug.LogWarning("Clip " + name + " not found");
			return;
		}
		s.source.Play();
	}
    

    public void PlayStep(int index)
    {       
        //AudioSource source = getSFXSource("step_heavy", 5);
        AudioSource source = getSFXSource("step_med", 5);
        //pitch according to index
        source.pitch = 1.0f + 0.1f * index;
        source.Play();
    }

    public void PlayBurn()
    {
        //getSFXSource("cig_burn", 3).Play();
        getSFXSource("acid_burn", 5).Play();
    }
    
    public void PlayBark()
    {
	    getSFXSource("bark", 3).Play();
    }

    public void PlaySlide(int index)
    {        
        AudioSource source = getSFXSource("cardSlide", 8);
        //pitch according to index
        source.pitch = 1.0f + 0.1f * index;
        source.Play();
    }

    public void PlayClick()
    {
        getSFXSource("chipsCollide", 4).Play();
    }


    private AudioSource getSFXSource(string name, int n) {
        int index = Random.Range(1, n);

        foreach(Sound s in clips) {
            if(s.name == name + index || s.name == name) {
                return s.source;            
            }

        }
        foreach(Sound s in clips) {
            if(s.name == name + "1") {
                return s.source;            
            }

        }
        Debug.Log("Couldn't find sfx with name " + name + index);
        return null;
    }
    
}
