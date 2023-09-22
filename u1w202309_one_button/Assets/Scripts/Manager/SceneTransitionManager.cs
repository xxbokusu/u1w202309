using System;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Threading;
using unity1week202309.Object;

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
        private static SceneTransitionManager _instance = null;
        public static SceneTransitionManager Instance {
            get {
                return _instance;
            }
        }
        
        private Scene _currentScene = Scene.Initialize;
        private Scene _nextScene = Scene.Title;
        private bool _isTransition;
        private bool _isInitialized = false;
        
        // シーン遷移時にFade out/inをするためのCanvas
        [SerializeField] private FadeCanvas _fadeCanvas;
        // クレジット表示用のCanvasGroup.最初に一度だけFade outする
        [SerializeField] private CanvasGroup _creditCanvasGroup;
        
        void Awake() {
            if (_instance == null) {
                _instance = this;
                DontDestroyOnLoad(gameObject);
            } else {
                Destroy(gameObject);
            }
            if (_fadeCanvas == null) {
                Debug.Util.LogError("SceneTransitionManager::Awake()::fadeCanvas is null");
                return;
            }
            if (_creditCanvasGroup == null) {
                Debug.Util.LogError("SceneTransitionManager::Awake()::creditCanvasGroup is null");
                return;
            }
            _creditCanvasGroup.alpha = 1.0f;
        }
        
        public void ChangeScene(Scene targetScene) {
            _nextScene = targetScene;
            LoadSceneAsync(_nextScene, this.GetCancellationTokenOnDestroy()).Forget();
        }

        // クレジットのフェードアウトを見てから初回遷移をする
        private void Update() {
            if (!_isInitialized && _creditCanvasGroup.alpha <= 0.0f) {
                _isInitialized = true;
                ChangeScene(_nextScene);
            } else if (!_isInitialized) {
                _creditCanvasGroup.alpha -= 0.01f;
            }
        }

        //非同期的に次のシーンを読み込んで遷移する. 読み込み開始でフェードアウト, 読み込み完了でフェードイン
        private async UniTaskVoid LoadSceneAsync(Scene targetScene, CancellationToken token) {
            _fadeCanvas.SceneFadeOutAsync(token).Forget();
            await UniTask.WaitUntil(() => !_fadeCanvas.IsFadeOut, cancellationToken: token);

            Debug.Util.LogFormat("SceneTransitionManager::loadSceneAsync()::scene transition {0} > {1}", this._currentScene ,targetScene);
            if (_currentScene != Scene.Initialize && SceneManager.GetSceneByName(_currentScene.ToString()).IsValid())
            {
                await SceneManager.UnloadSceneAsync(_currentScene.ToString());
            }
            var progress =SceneManager.LoadSceneAsync(_nextScene.ToString(), LoadSceneMode.Additive);
            _isTransition = true;

            await UniTask.WaitUntil(() => progress.isDone, cancellationToken: token);
            _isTransition = false;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(_nextScene.ToString()));
            _fadeCanvas.SceneFadeInAsync(token).Forget();
            _currentScene = targetScene;
        }
    }
}