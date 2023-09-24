using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScriptableObject;
using TMPro;
using unity1week202309.Controller;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

namespace unity1week202309.Manager {
    /*
     * <summery>
     * メインシーンの進行を管理する
     * 時間制限付きなので時間を管理し、残時間に応じて表現が変遷する
     * </summery>
     */
    class MainSceneManager : GameSceneManager {
        private enum MainSceneState {
            Initialize,
            Playing,
            Result,
        }

        private MainSceneState _currentState = MainSceneState.Initialize;
        public bool IsResult => _currentState == MainSceneState.Result;

        private bool CanTransition() {
            return IsResult;
        }

        GameObject _unityChan;
        private GirlController _girlController;
        [SerializeField] private CameraViewController _cameraViewController;

        [SerializeField] private float playingResource = 50.0f;

        // Result時に表示するPanel
        [SerializeField] private GameObject resultPanel;
        [SerializeField] private TextMeshProUGUI scoreText;

        public void WasteResource(float waste) {
            playingResource -= waste;
            if (playingResource < 0.0f) {
                playingResource = 0.0f;
            }
        }

        public bool IsResourceEmpty => playingResource <= 0.0f;

        [SerializeField] private ScoreScriptableObject scoreScriptableObject;

        public void AddScore(float score) {
            scoreScriptableObject.AddScore(score);
        }

        private void Start() {
            Initialize();
            TransitionAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        public override void Initialize() {
            if (_cameraViewController == null) {
                Debug.Util.LogError("MainSceneManager::Start()::_cameraViewController is null");
                return;
            }

            if (scoreScriptableObject == null) {
                Debug.Util.LogError("MainSceneManager::Start()::scoreScriptableObject is null");
                return;
            }

            if (resultPanel == null) {
                Debug.Util.LogError("MainSceneManager::Start()::resultPanel is null");
                return;
            }
            if (scoreText == null) {
                Debug.Util.LogError("MainSceneManager::Start()::scoreText is null");
                return;
            }

            resultPanel.SetActive(false);

            SoundManager.Instance.PlayBGM("Picnic-Xy02-2(Slow)");
            var groundCube = ObjectsManager.Instance.GetObject("Prefab/GroundCube");
            var unityChan = ObjectsManager.Instance.GetObject("Prefab/unitychan_dynamic");
            if (unityChan == null) {
                Debug.Util.LogError("MainSceneManager::Initialize()::unitychan is null");
                return;
            }

            Instantiate(groundCube, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity, transform);
            // 右向きに生成する
            _unityChan = Instantiate(unityChan, new Vector3(0.0f, 0.5f, 0.0f), Quaternion.Euler(0.0f, 90.0f, 0.0f),
                transform);
            _girlController = _unityChan.GetComponent<GirlController>();
            if (_girlController == null) {
                Debug.Util.LogError("MainSceneManager::Initialize()::_girlController is null");
                return;
            }

            _cameraViewController.SetCharaObject(_unityChan);
        }

        private async UniTaskVoid TransitionAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => !SceneTransitionManager.Instance.IsTransition, cancellationToken: token);
            _currentState = MainSceneState.Playing;

            await UniTask.WaitUntil(() => playingResource <= 0.0f, cancellationToken: token);
            _currentState = MainSceneState.Result;
            SoundManager.Instance.PlaySE("Crow-Real_part");
            SoundManager.Instance.StopBGM("Picnic-Xy02-2(Slow)");
            // Score表示用のTextMeshproを更新
            var score = (int)scoreScriptableObject.Score;
            scoreText.text = score.ToString();
            resultPanel.SetActive(true);

            // 3秒待ってスペースキーでリザルトへ
            await UniTask.Delay(3000, cancellationToken: token);
            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space), cancellationToken: token);
            SceneTransitionManager.Instance.ChangeScene(Scene.Title);
        }
    }
}