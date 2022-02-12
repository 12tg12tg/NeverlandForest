using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SoundType
{
    BG_Intro, BG_Main, BG_Battle
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance => instance;

    public bool muteBGM;
    public bool muteSF;

    [SerializeField] private AudioSource bgm_Player;
    [SerializeField] private List<AudioSource> sfx_Players;

    [SerializeField] private List<AudioClip> soundList;

    // 재생
    public void Play(SoundType type)
    {
        switch (type)
        {
            case SoundType.BG_Intro:
                break;
            case SoundType.BG_Main:
                break;
            case SoundType.BG_Battle:
                break;
            default:
                break;
        }
    }

    /*
     * 플레이 함수
     *  PlayOneShot : 한번재생
     *  Play : 가지고 있는 클립을 반복재생
     */

    private void PlayBGM(AudioClip clip)
    {
        if (bgm_Player.isPlaying)
            bgm_Player.Stop();
        bgm_Player.clip = clip;
        bgm_Player.Play();
    }

    private void PlaySFX(AudioClip clip)
    {
        var cound = sfx_Players.Count;
        for (int i = 0; i < cound; i++)
        {
            if(!sfx_Players[i].isPlaying)
            {
                sfx_Players[i].PlayOneShot(clip);
            }
        }
    }

    // 설정 ============================================
    public void SetMuteBGM(bool isMute)
    {

    }

    public void SetMuteSFX(bool isMute)
    {

    }

    public void SetVolumeBGM(float value)
    {

    }

    public void SetVolumeSFX(float value)
    {

    }
}