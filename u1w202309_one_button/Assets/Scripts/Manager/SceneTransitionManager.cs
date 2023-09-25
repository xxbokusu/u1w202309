using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading;
using unity1week202309.Object;
using UnityEngine.Serialization;

namespace unity1week202309.Manager
{
    public enum Scene
    {
        Initialize, // 初期値
        Title,
        Main,
        Result,
        Option
    }

    /*
     * <summery>
     * シーン遷移を管理するクラス
     * </summery>
     */
    public class SceneTransitionManager: MonoBehaviour {
        public static SceneTransitionManager Instance { get; private set; } = null;

        private Scene _currentScene = Scene.Initialize;
        private Scene _nextScene = Scene.Title;
        private bool _isTransition;
        public bool IsTransition => _isTransition;

        private bool _isInitialized = false;
        
        // シーン遷移時にFade out/inをするためのCanvas
        [SerializeField] private FadeCanvas fadeCanvas;
        // クレジット表示用のCanvasGroup.最初に一度だけFade outする
        [SerializeField] private CanvasGroup creditCanvasGroup;
        
        void Awake() {
            if (Instance == null) {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            if (fadeCanvas == null) {
                Debug.Util.LogError("SceneTransitionManager::Awake()::fadeCanvas is null");
                return;
            }
            if (creditCanvasGroup == null) {
                Debug.Util.LogError("SceneTransitionManager::Awake()::creditCanvasGroup is null");
                return;
            }
            creditCanvasGroup.alpha = 1.0f;
        }
        
        public void ChangeScene(Scene targetScene) {
            _nextScene = targetScene;
            LoadSceneAsync(_nextScene, this.GetCancellationTokenOnDestroy()).Forget();
        }

        // クレジットのフェードアウトを見てから初回遷移をする
        private void Update() {
            if (_isInitialized) return;

            creditCanvasGroup.alpha -= 0.01f;
            if (creditCanvasGroup.alpha > 0.0f) return;

            _isInitialized = true;
            creditCanvasGroup.gameObject.SetActive(false);
            ChangeScene(_nextScene);
        }

        //非同期的に次のシーンを読み込んで遷移する. 読み込み開始でフェードアウト, 読み込み完了でフェードイン
        private async UniTaskVoid LoadSceneAsync(Scene targetScene, CancellationToken token) {
            fadeCanvas.SceneFadeOutAsync(token).Forget();
            await UniTask.WaitUntil(() => !fadeCanvas.IsFadeOut, cancellationToken: token);

            Debug.Util.LogFormat("SceneTransitionManager::loadSceneAsync()::scene transition {0} > {1}", this._currentScene ,targetScene);
            if (_currentScene != Scene.Initialize && SceneManager.GetSceneByName(_currentScene.ToString()).IsValid())
            {
                await SceneManager.UnloadSceneAsync(_currentScene.ToString());
            }
            var progress =SceneManager.LoadSceneAsync(_nextScene.ToString(), LoadSceneMode.Additive);
            _isTransition = true;

            await UniTask.WaitUntil(() => progress.isDone, cancellationToken: token);
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_nextScene.ToString()));
            fadeCanvas.SceneFadeInAsync(token).Forget();
            _currentScene = targetScene;
            
            await UniTask.WaitUntil(() => !fadeCanvas.IsInFade, cancellationToken: token);
            _isTransition = false;
        }
    }
}