using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    [SerializeField] 
    private float bpm = 90.0f;
    [SerializeField] 
    private float signature = 4;
    
    private float spb; //seconds per beat
    private float bar_duration;
    private float time_since_last_bar = 0f;

    [SerializeField] 
    private Sound[] clips;

    public static MusicManager instance;
    enum phases
    {
        intro, hub, standoff, respite
    }
    [SerializeField]
    private phases current_phase = phases.intro;
    [SerializeField]
    private phases next_phase = phases.intro;

    public void enterHub(bool complete)
    {
        if (complete)
            next_phase = phases.respite;
        else
            next_phase = phases.hub;
    }

    public void enterLevel()
    {
        next_phase = phases.standoff;
    }
    
    public void enterIntro()
    {
        next_phase = phases.intro;
    }

    private void Awake()
    {
        if(instance == null){
            instance = this;
        }
        else{
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
        
        spb = 60f / bpm;
        bar_duration = spb * signature;
        Debug.Log("bar duration " + + bar_duration);
        Debug.Log(clips[0].clip.length / 8f);
        foreach(Sound s in clips)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.name = s.clip.name;
            s.source.clip = s.clip;
            s.source.name = s.name;
            //Debug.Log(s.source.name);

            s.source.volume = s.get_volume();
            s.source.pitch = 1.0f;
                //s.pitch;
            s.source.loop = s.get_loop();
            s.source.volume = 0f;
            s.source.Play();
        }

        getSource(phases.intro).volume = 1f;
    }
    // Update is called once per frame
    void Update()
    {
        if (time_since_last_bar > bar_duration)
        {
            time_since_last_bar -= bar_duration;
            if (next_phase != current_phase)
            {
                StartCoroutine(Crossfade(current_phase, next_phase, bar_duration / 8));
                current_phase = next_phase;
            }
        }

        time_since_last_bar += Time.deltaTime;

    }

    private AudioSource getSource(phases phase)
    {
        switch (phase){
            case phases.intro:
                return clips[0].source;
            case phases.hub:
                return clips[1].source;
            case phases.standoff:
                return clips[2].source;
            case phases.respite:
                return clips[3].source;
            default:
                return null;
        }
    }
    
    private IEnumerator Crossfade(phases current_phase, phases next_phase, float time) {
        AudioSource currentSource = getSource(current_phase);
        AudioSource nextSource = getSource(next_phase);
        
        float increment = 1f / (time * 60f);

        for (float volume = 0f; volume < 1f; volume += increment)
        {
            currentSource.volume = 1f - volume;
            nextSource.volume = volume;
            yield return null;
        }
    }
}
