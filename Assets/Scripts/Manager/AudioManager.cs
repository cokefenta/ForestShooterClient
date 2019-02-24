using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 声音枚举
/// </summary>
public enum AudioType { Alert,ArrowShoot,Bg_fast,Bg_moderate,ButtonClicked,Miss,ShootPerson,Timer }

/// <summary>
/// 非组件化的声音控制器
/// </summary>
public class AudioManager : BaseManager {
    private GameObject _audioManagerGo;//空物体，来管理播放声音的两个子物体
    private GameObject _bgmAudioSourceGo;//播放音乐的AudioSource组件所在游戏物体
    private GameObject _soundAudioSourceGo;//播放音效的AudioSource组件所在游戏物体
    private AudioSource _bgmAS;//背景音乐音源组件
    private AudioSource _soundAS;//音效音源组件

    public Dictionary<AudioType, AudioClip> audioDic;//音乐资源字典
    public AudioManager(GameFacade gamefacade):base(gamefacade)
    {
        //创建游戏物体并添加组件
        _audioManagerGo = new GameObject("AudioManager");
        _bgmAudioSourceGo = new GameObject("BGM");
        _soundAudioSourceGo = new GameObject("Sound");
        _bgmAudioSourceGo.transform.SetParent(_audioManagerGo.transform);
        _soundAudioSourceGo.transform.SetParent(_audioManagerGo.transform);
        _bgmAS= _bgmAudioSourceGo.AddComponent<AudioSource>();
        _soundAS= _soundAudioSourceGo.AddComponent<AudioSource>();
    }

    public override void OnInit()
    {
        //初始化字典，并填充字典
        audioDic = new Dictionary<AudioType, AudioClip>();
        audioDic.Add(AudioType.Alert, Resources.Load("Sounds/Alert") as AudioClip);
        audioDic.Add(AudioType.ArrowShoot, Resources.Load("Sounds/ArrowShoot") as AudioClip);
        audioDic.Add(AudioType.Bg_fast, Resources.Load("Sounds/Bg(fast)") as AudioClip);
        audioDic.Add(AudioType.Bg_moderate, Resources.Load("Sounds/Bg(moderate)") as AudioClip);
        audioDic.Add(AudioType.ButtonClicked, Resources.Load("Sounds/ButtonClick") as AudioClip);
        audioDic.Add(AudioType.Miss, Resources.Load("Sounds/Miss") as AudioClip);
        audioDic.Add(AudioType.ShootPerson, Resources.Load("Sounds/ShootPerson") as AudioClip);
        audioDic.Add(AudioType.Timer, Resources.Load("Sounds/Timer") as AudioClip);
        //设置音源组件
        _bgmAS.playOnAwake = false;
        _bgmAS.loop = true;
        _soundAS.playOnAwake = false;
        //设置刚开始播放的bgm
        PlayBGM(AudioType.Bg_moderate);
    }

    /// <summary>
    /// 用于播放一次性音效
    /// </summary>
    /// <param name="audioType">声音枚举</param>
    public void PlaySound(AudioType audioType)
    {
        _soundAS.PlayOneShot(audioDic.TryGet(audioType));
    }
    /// <summary>
    /// 用于播放背景音乐
    /// </summary>
    public void PlayBGM(AudioType audioType)
    {
        if (_bgmAS.isPlaying)
        {
            _bgmAS.Stop();
        }
        _bgmAS.clip = audioDic.TryGet(audioType);
        _bgmAS.Play();
    }

    /// <summary>
    /// 背景音乐音量控制
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void BGMVolumeController(float volume)
    {
        _bgmAS.volume = volume;
    }
    /// <summary>
    /// 音效音量控制
    /// </summary>
    /// <param name="volume">音量大小</param>
    public void SoundVolumeController(float volume)
    {
        _soundAS.volume = volume;
    }

    public void StopBGM()
    {
        _bgmAS.Stop();
    }
}
