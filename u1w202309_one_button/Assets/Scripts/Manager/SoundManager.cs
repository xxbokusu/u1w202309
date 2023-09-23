using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using ScriptableObject;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace unity1week202309.Manager {
    /*
     * <summery>
     * BGM/SEの再生と停止を管理する
     * 同時に再生するものの数だけAudioSourceを用意する
     * </summery>
     */
    public class SoundManager : MonoBehaviour {
        public static SoundManager Instance { get; private set; }

        private enum SoundState {
            None,
            Loading,
            Ready,
            Playing
        }

        private SoundState _currentState = SoundState.None;

        public bool IsReady => _currentState == SoundState.Ready;
        public bool IsPlaying => _currentState == SoundState.Playing;

        private Dictionary<string, AudioClip> _bgmDictionary = new() {
            { "Sparrow-Real_Ambi01-1", null },
        };

        private Dictionary<string, AudioClip> _seDictionary = new() {
            { "maou_se_voice_bird01", null },
            { "Crow-Real_Ambi01-1", null }
        };

        private Dictionary<string, AudioSource> _audioSourceDictionary = new();

        private const float BGMVolumeBaseMax = 1.0f;
        private float _bgmVolumeBase = 0.0f;

        // ScriptableObjectから音量の倍率を取得する
        [SerializeField] private ConfigScriptableObject configScriptableObject;

        void Start() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else {
                Destroy(gameObject);
            }

            if (configScriptableObject == null) {
                Debug.Util.LogError("SoundManager::Start()::configScriptableObject is null");
                return;
            }

            _currentState = SoundState.Loading;
            LoadAudioClipsAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async UniTaskVoid LoadAudioClipsAsync(CancellationToken token) {
            var bgmRequestDictionary = new Dictionary<string, ResourceRequest>();
            foreach (var key in _bgmDictionary.Keys) {
                var req = Resources.LoadAsync<AudioClip>("Audios/BGM/" + key);
                bgmRequestDictionary.Add(key, req);
            }

            var seRequestDictionary = new Dictionary<string, ResourceRequest>();
            foreach (var key in _seDictionary.Keys) {
                var req = Resources.LoadAsync<AudioClip>("Audios/SE/" + key);
                seRequestDictionary.Add(key, req);
            }

            await UniTask.WaitUntil(() => bgmRequestDictionary.Values.All(req => req.isDone), cancellationToken: token);
            foreach (var key in bgmRequestDictionary.Keys) {
                var clip = bgmRequestDictionary[key].asset as AudioClip;
                Assert.IsNotNull(clip, "SoundManager::loadSoundAsync()::clip is null! key = " + key);
                _bgmDictionary[key] = clip;

                // 再生用のAudioSourceを子オブジェクトとして生成
                var child = new GameObject(key);
                child.transform.SetParent(transform);
                var audioSource = child.AddComponent<AudioSource>();
                // ループ再生設定
                audioSource.volume = BGMVolumeBaseMax;
                audioSource.loop = true;
                _audioSourceDictionary.Add(key, audioSource);
            }

            await UniTask.WaitUntil(() => seRequestDictionary.Values.All(req => req.isDone), cancellationToken: token);
            foreach (var key in seRequestDictionary.Keys) {
                var clip = seRequestDictionary[key].asset as AudioClip;
                Assert.IsNotNull(clip, "SoundManager::loadSoundAsync()::clip is null! key = " + key);
                _seDictionary[key] = clip;

                // 再生用のAudioSourceを子オブジェクトとして生成
                var child = new GameObject(key);
                child.transform.SetParent(transform);
                var audioSource = child.AddComponent<AudioSource>();
                _audioSourceDictionary.Add(key, audioSource);
            }

            _currentState = SoundState.Ready;
        }

        // BGMを再生する
        public void PlayBGM(string bgmName) {
            if (!_bgmDictionary.ContainsKey(bgmName)) {
                Debug.Util.Log("SoundManager::PlayBGM()::bgmName is not found!");
                return;
            }

            // 再生用のAudioSourceを取得して再生する
            var audioSource = _audioSourceDictionary[bgmName];
            if (audioSource == null) {
                Debug.Util.Log("SoundManager::PlayBGM()::AudioSource is not found!");
                return;
            }

            audioSource.clip = _bgmDictionary[bgmName];
            FadeInAsync(audioSource, this.GetCancellationTokenOnDestroy()).Forget();
            audioSource.Play();
            _currentState = SoundState.Playing;
        }

        private bool CanSetVolume(AudioSource audioSource, float volume) {
            if (volume < 0.0f) return false;
            if (volume > ReflectVolumeRate(BGMVolumeBaseMax)) {
                audioSource.volume = ReflectVolumeRate(BGMVolumeBaseMax);
                return false;
            }

            return true;
        }
        private float ReflectVolumeRate(float volume) {
            return volume * configScriptableObject.GetBGMVolumeRate();
        }

        // 再生中のaudioSourceの音量を変更する
        public void ResetBGMVolume() {
            if (_currentState != SoundState.Playing) {
                return;
            }

            Debug.Util.LogFormat("SoundManager::ResetBGMVolume()::Reset BGM Volume to {0}", configScriptableObject.GetBGMVolumeRate());
            foreach (var audioSource in _audioSourceDictionary.Values) {
                audioSource.volume = ReflectVolumeRate(_bgmVolumeBase);
            }
        }

        // 音量を徐々に大きくすることでフェードインする
        private async UniTaskVoid FadeInAsync(AudioSource audioSource, CancellationToken token) {
            while (CanSetVolume(audioSource, _bgmVolumeBase + 0.01f)) {
                audioSource.volume = ReflectVolumeRate(_bgmVolumeBase + 0.01f);
                _bgmVolumeBase += 0.01f;
                await UniTask.Delay(10);
            }
        }

        // BGMを停止する
        public void StopBGM(string bgmName) {
            if (_currentState != SoundState.Playing) {
                return;
            }

            // 音声をフェードアウトさせる
            AudioSource audioSource = _audioSourceDictionary[bgmName];
            if (audioSource == null) {
                Debug.Util.Log("SoundManager::StopBGM()::AudioSource is not found!");
                return;
            }

            FadeOutAsync(audioSource, this.GetCancellationTokenOnDestroy()).Forget();
            audioSource.Stop();
            _currentState = SoundState.Ready;
        }

        //フェードアウト
        private async UniTaskVoid FadeOutAsync(AudioSource audioSource, CancellationToken token) {
            while (CanSetVolume(audioSource, _bgmVolumeBase - 0.01f)) {
                audioSource.volume = ReflectVolumeRate(_bgmVolumeBase - 0.01f);
                _bgmVolumeBase -= 0.01f;
                await UniTask.Delay(10);
            }
        }

        // SEを再生する
        public void PlaySE(string seName) {
            if (!_seDictionary.ContainsKey(seName)) {
                Debug.Util.Log("SoundManager::PlaySE()::seName is not found!");
                return;
            }

            // 再生用のAudioSourceを取得して再生する
            AudioSource audioSource = _audioSourceDictionary[seName];
            if (audioSource == null) {
                Debug.Util.Log("SoundManager::PlaySE()::AudioSource is not found!");
                return;
            }

            audioSource.volume = ReflectVolumeRate(_bgmVolumeBase);
            audioSource.PlayOneShot(_seDictionary[seName]);
        }
    }
}