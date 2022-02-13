using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SoundType
{   
    //BG
    BG_Intro, BG_Main, BG_Battle,BG_Hunt, BG_Camp,
    //Se
    Se_win,Se_Fail,Se_GameWin,Se_GameOver,Se_Axe,Se_Spade,Se_Hand,Se_Button,Se_Diary,
    Se_Attack,Se_lightSingleAttack, Se_lightWide_attack,Se_lightForceMove,
    Se_Monster_hitted, Se_Character_hitted,
    Se_bowSingleShot, Se_bowMultyShot, Se_knockBack, Se_Walk, Se_MonsterTurnStart,
    Se_BetTypeMonsterAttack,
    Se_SpiderTypeMonsterAttack,
    Se_BigPlantTypeMonsterAttack,
    Se_GhostTypeMonsterAttack,
    Se_SmallTypeMonsterAttack,
    Se_OilFulling
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager instance;
    public static SoundManager Instance => instance;

    private bool muteBGM;
    public bool MuteBgm
    {
        get => muteBGM;
        set { muteBGM = value; }
    }
    private bool muteSF;
    public bool MuteSf
    {
        get => muteSF;
        set { muteSF = value; }
    }
    [SerializeField] private AudioSource bgm_Player;
    [SerializeField] private List<AudioSource> sfx_Players;
    [SerializeField] private AudioSource walkSoundPlayer;

    [SerializeField] private Scrollbar bgmSlider;
    [SerializeField] private Scrollbar soundSlider;

    private float bgmVolume;
    public float BgmVolume
    {
        get => bgmVolume;
        set { bgmVolume = value; }
    }
    private float sfVolume;
    public float SFVoulme
    {
        get => sfVolume;
        set { sfVolume = value; }
    }
    public AudioSource WalkSoundPlayer
    {
        get => walkSoundPlayer;
        set
        {
            walkSoundPlayer = value;
        }
    }

    [SerializeField] private List<AudioClip> soundList;

    public void Awake()
    {
        instance ??= this;
    }

    // ���
    public void Play(SoundType type)
    {
        switch (type)
        {
            case SoundType.BG_Intro:
            case SoundType.BG_Main:
            case SoundType.BG_Battle:
            case SoundType.BG_Hunt:
            case SoundType.BG_Camp:
                if (!muteBGM)
                {
                    PlayBGM(soundList[(int)type]);
                }
                break;
            case SoundType.Se_win:
            case SoundType.Se_Fail:
            case SoundType.Se_GameWin:
            case SoundType.Se_GameOver:
            case SoundType.Se_Axe:
            case SoundType.Se_Spade:
            case SoundType.Se_Hand:
            case SoundType.Se_Button:
            case SoundType.Se_Diary:
            case SoundType.Se_Attack:
            case SoundType.Se_lightSingleAttack:
            case SoundType.Se_lightWide_attack:
            case SoundType.Se_lightForceMove:
            case SoundType.Se_Monster_hitted:
            case SoundType.Se_Character_hitted:
            case SoundType.Se_bowSingleShot:
            case SoundType.Se_bowMultyShot:
            case SoundType.Se_knockBack:
            case SoundType.Se_Walk:
            case SoundType.Se_MonsterTurnStart:
            case SoundType.Se_BetTypeMonsterAttack:
            case SoundType.Se_SpiderTypeMonsterAttack:
            case SoundType.Se_BigPlantTypeMonsterAttack:
            case SoundType.Se_GhostTypeMonsterAttack:
            case SoundType.Se_SmallTypeMonsterAttack:
            case SoundType.Se_OilFulling:
                if (!muteSF)
                {
                    PlaySFX(soundList[(int)type]);
                }
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

    public void PlayWalkSound(bool isOn)
    {
        if (isOn)
        {
            if (!walkSoundPlayer.isPlaying)
            {
                walkSoundPlayer.Play();
            }
        }
        else
        {
            walkSoundPlayer.Stop();
        }
    }


    // ���� ============================================
    public void SetMuteBGM(bool isMute)
    {
        isMute =muteBGM;
        if (isMute)
        {
            SetVolumeBGM(0f);
        }
        else
        {
            SetVolumeBGM(bgmSlider.value);
        }

        Vars.UserData.isBgmMute = isMute;

    }

    public void SetMuteSFX(bool isMute)
    {
        isMute = muteSF;
        if (isMute)
        {
            SetVolumeSFX(0f);
        }
        else
        {
            SetVolumeSFX(soundSlider.value);
        }
        Vars.UserData.isSfMute = isMute;

    }

    public void SetVolumeBGM(float volume)
    {
        bgm_Player.volume = volume;
        Vars.UserData.bgmVolume = bgm_Player.volume;
    }

    public void SetVolumeSFX(float volume)
    {
        for (int i = 0; i < sfx_Players.Count; i++)
        {
            sfx_Players[i].volume = volume;
        }
        Vars.UserData.sfVoulme = volume;

    }
}