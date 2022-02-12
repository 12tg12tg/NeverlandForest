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

    // ���
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
     * �÷��� �Լ�
     *  PlayOneShot : �ѹ����
     *  Play : ������ �ִ� Ŭ���� �ݺ����
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
                break;
            }
        }
    }

    // ���� ============================================
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