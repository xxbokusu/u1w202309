using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Assertions;

namespace unity1week202309.Manager
{
    /*
     * <summery>
     * BGM/SEの再生と停止を管理する
     * 同時に再生するものの数だけAudioSourceを用意する
     * </summery>
     */
    public class SoundManager: MonoBehaviour {
        private static SoundManager _instance;
        public static SoundManager Instance { get { return _instance; } }

        private enum SoundState {
            None,
            Loading,
            Ready,
            Playing
        }
        private SoundState _currentState = SoundState.None;
        public bool IsReady { get { return _currentState == SoundState.Ready; } }
        public bool IsPlaying { get { return _currentState == SoundState.Playing; } }
        
        private Dictionary<String, AudioClip> _bgmDictionary = new Dictionary<String, AudioClip>() {
            {"maou_bgm_healing11b(moving)", null},
            {"Sparrow-Real_Ambi01-1", null}
        };
        private Dictionary<String, AudioClip> _seDictionary = new Dictionary<String, AudioClip>() {
            {"maou_se_sound_pc01(click)", null},
            {"maou_se_battle07(shoot)", null},
            {"maou_se_voice_bird01",null}
        };

        private Dictionary<String, AudioSource> _audioSourceDictionary = new Dictionary<String, AudioSource>();
        private const float BGMVolume = 0.5f;

        void Start() {
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            _currentState = SoundState.Loading;
            LoadAudioClipsAsync().Forget();
        }

        private async UniTaskVoid LoadAudioClipsAsync() {
            var bgmKeys = new List<string>(_bgmDictionary.Keys);
            foreach (var key in bgmKeys)
            {
                string filePath = "Audios/BGM/" + key;
                AudioClip clip = Resources.Load<AudioClip>(filePath);
                Assert.IsNotNull(clip, "SoundManager::loadSoundAsync()::clip is null! key = " + filePath);
                _bgmDictionary[key] = clip;

                // 再生用のAudioSourceを子オブジェクトとして生成
                GameObject child = new GameObject(key);
                child.transform.SetParent(transform);
                var audioSource = child.AddComponent<AudioSource>();
                // ループ再生設定
                audioSource.loop = true;
                _audioSourceDictionary.Add(key, audioSource);
            }
            // SEをロード
            var seKeys = new List<string>(_seDictionary.Keys);
            foreach (var key in seKeys)
            {
                // unityroomでのAddressablesのロードがうまくいかないので、Resourcesからロードする
                string filePath = "Audios/SE/" + key;
                AudioClip clip = Resources.Load<AudioClip>(filePath);
                Assert.IsNotNull(clip, "SoundManager::loadSoundAsync()::clip is null! key = " + filePath);
                _seDictionary[key] = clip;

                // 再生用のAudioSourceを子オブジェクトとして生成
                GameObject child = new GameObject(key);
                child.transform.SetParent(transform);
                var audioSource = child.AddComponent<AudioSource>();
                _audioSourceDictionary.Add(key, audioSource);
            }

            _currentState = SoundState.Ready;
        }
        
        // BGMを再生する
        public void PlayBGM(string bgmName)
        {
            if (!_bgmDictionary.ContainsKey(bgmName))
            {
                Debug.Util.Log("SoundManager::PlayBGM()::bgmName is not found!");
                return;
            }

            // 再生用のAudioSourceを取得して再生する
            AudioSource audioSource = _audioSourceDictionary[bgmName];
            if (audioSource == null)
            {
                Debug.Util.Log("SoundManager::PlayBGM()::AudioSource is not found!");
                return;
            }
            audioSource.clip = _bgmDictionary[bgmName];
            FadeInAsync(audioSource, this.GetCancellationTokenOnDestroy()).Forget();
            audioSource.Play();
            _currentState = SoundState.Playing;
        }

        // 音量を徐々に大きくすることでフェードインする
        private async UniTaskVoid FadeInAsync(AudioSource audioSource, CancellationToken token)
        {

            audioSource.volume = 0.0f;
            while (audioSource.volume < BGMVolume)
            {
                audioSource.volume += 0.01f;
                await UniTask.Delay(10);
            }
        }

        // BGMを停止する
        public void StopBGM(string bgmName)
        {
            if (_currentState != SoundState.Playing)
            {
                return;
            }

            // 音声をフェードアウトさせる
            AudioSource audioSource = _audioSourceDictionary[bgmName];
            if (audioSource == null)
            {
                Debug.Util.Log("SoundManager::StopBGM()::AudioSource is not found!");
                return;
            }
            FadeOutAsync(audioSource, this.GetCancellationTokenOnDestroy()).Forget();
            audioSource.Stop();
            _currentState = SoundState.Ready;
        }

        //フェードアウト
        private async UniTaskVoid FadeOutAsync(AudioSource audioSource, CancellationToken token)
        {
            while (audioSource.volume > 0.0f)
            {
                audioSource.volume -= 0.01f;
                await UniTask.Delay(10);
            }
        }

        // SEを再生する
        public void PlaySE(string seName)
        {
            if (!_seDictionary.ContainsKey(seName))
            {
                Debug.Util.Log("SoundManager::PlaySE()::seName is not found!");
                return;
            }

            // 再生用のAudioSourceを取得して再生する
            AudioSource audioSource = _audioSourceDictionary[seName];
            if (audioSource == null)
            {
                Debug.Util.Log("SoundManager::PlaySE()::AudioSource is not found!");
                return;
            }
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(_seDictionary[seName]);
        }
    }
}