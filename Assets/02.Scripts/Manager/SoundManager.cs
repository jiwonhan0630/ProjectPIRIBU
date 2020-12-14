﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : SingleTon<SoundManager>
{

    public AudioMixer audioMixer;

    [Tooltip("BGM = 0, SFX = 1, UI = 2")]
    public AudioMixerGroup[] audioMixerGroups;
    public AudioClip[] audioClips;
    public AudioSource[] audioSources;
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Init()
    {
        base.Init();
        for (int i = 0; i < 3; i++)
        {
            audioSources[i].outputAudioMixerGroup = audioMixerGroups[i];
        }
        audioSources[0].clip = audioClips[0];
        audioSources[1].clip = audioClips[1];

    }


    private void Start()
    {
    }
    public void PlayBGM()
    {
        audioSources[0].Play();
    }
    public void PlayBGM(int _i)
    {
        audioSources[_i].Play();
    }

    public void PlaySFX()
    {

    }

    public void PlayUI()
    {

    }

}