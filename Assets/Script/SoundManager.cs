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

    public bool muteBGM;
    public bool muteSF;

    [SerializeField] private AudioSource bgm_Player;
    [SerializeField] private List<AudioSource> sfx_Players;
    [SerializeField] private AudioSource walkSoundPlayer;
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
        instance = this;
    }

    // 재생
    public void Play(SoundType type)
    {
        switch (type)
        {
            case SoundType.BG_Intro:
                PlayBGM(soundList[0]);
                break;
            case SoundType.BG_Main:
                PlayBGM(soundList[1]);
                break;
            case SoundType.BG_Battle:
                PlayBGM(soundList[2]);
                break;
            case SoundType.BG_Hunt:
                PlayBGM(soundList[3]);
                break;
            case SoundType.BG_Camp:
                PlayBGM(soundList[4]);
                break;
            case SoundType.Se_win:
                PlaySFX(soundList[5]);
                break;
            case SoundType.Se_Fail:
                PlaySFX(soundList[6]);
                break;
            case SoundType.Se_GameWin:
                PlaySFX(soundList[7]);
                break;
            case SoundType.Se_GameOver:
                PlaySFX(soundList[8]);
                break;
            case SoundType.Se_Axe:
                PlaySFX(soundList[9]);
                break;
            case SoundType.Se_Spade:
                PlaySFX(soundList[10]);
                break;
            case SoundType.Se_Hand:
                PlaySFX(soundList[11]);
                break;
            case SoundType.Se_Button:
                PlaySFX(soundList[12]);
                break;
            case SoundType.Se_Diary:
                PlaySFX(soundList[13]);
                break;
            case SoundType.Se_Attack:
                PlaySFX(soundList[14]);
                break;
            case SoundType.Se_lightSingleAttack:
                PlaySFX(soundList[15]);
                break;
            case SoundType.Se_lightWide_attack:
                PlaySFX(soundList[16]);
                break;
            case SoundType.Se_lightForceMove:
                PlaySFX(soundList[17]);
                break;
            case SoundType.Se_Monster_hitted:
                PlaySFX(soundList[18]);
                break;
            case SoundType.Se_Character_hitted:
                PlaySFX(soundList[19]);
                break;
            case SoundType.Se_bowSingleShot:
                PlaySFX(soundList[20]);
                break;
            case SoundType.Se_bowMultyShot:
                PlaySFX(soundList[21]);
                break;
            case SoundType.Se_knockBack:
                PlaySFX(soundList[22]);
                break;
            case SoundType.Se_Walk:
                PlaySFX(soundList[23]);
                break;
            case SoundType.Se_MonsterTurnStart:
                PlaySFX(soundList[24]);
                break;
            case SoundType.Se_BetTypeMonsterAttack:
                PlaySFX(soundList[25]);
                break;
            case SoundType.Se_SpiderTypeMonsterAttack:
                PlaySFX(soundList[26]);
                break;
            case SoundType.Se_BigPlantTypeMonsterAttack:
                PlaySFX(soundList[27]);
                break;
            case SoundType.Se_GhostTypeMonsterAttack:
                PlaySFX(soundList[28]);
                break;
            case SoundType.Se_SmallTypeMonsterAttack:
                PlaySFX(soundList[29]);
                break;
            case SoundType.Se_OilFulling:
                PlaySFX(soundList[30]);
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