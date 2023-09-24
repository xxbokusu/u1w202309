using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using ScriptableObject;
using unity1week202309.Manager;
using UnityEngine;

namespace unity1week202309.Controller {
    public class GirlController : MonoBehaviour {
        private enum GirlState {
            Waiting,
            Walking,
            Running,
            Resulting
        }
        private GirlState _state = GirlState.Waiting;
        private readonly Dictionary<GirlState, float> _wasteResourceByState = new() {
            { GirlState.Waiting , 2.0f},
            { GirlState.Walking , 3.0f},
            { GirlState.Running , 4.0f},
            { GirlState.Resulting , 0.0f},
        };

        private readonly Dictionary<GirlState, string> _stateNameByState = new() {
            { GirlState.Waiting, "WAIT00" },
            { GirlState.Walking, "WALK00_F" },
            { GirlState.Running, "RUN00_F" },
        };

        private Transform _charaTransform;
        private Animator _animator;
        public bool IsWaitingState => _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Waiting]);
        public bool ParamIsWalking { get; private set; } = false;

        public bool IsWalkingState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Walking]);

        public bool ParamIsRunning { get; private set; } = false;

        public bool IsRunningState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Running]);

        public bool IsResulting => _state == GirlState.Resulting;

        private MainSceneManager _mainSceneManager;
        
        void Start() {
            _animator = GetComponent<Animator>();
            _charaTransform = GetComponent<Transform>();
            if (_animator == null) {
                Debug.Util.LogError("GirlController::Start()::_animator is null");
                return;
            }

            if (_charaTransform == null) {
                Debug.Util.LogError("GirlController::Start()::_charaTransform is null");
                return;
            }

            _mainSceneManager = GetComponentInParent<MainSceneManager>();
            if (_mainSceneManager == null) {
                Debug.Util.LogError("GirlController::Start()::_mainSceneManager is null");
                return;
            }

            ChangeStateByInputAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async void Update() {
            _mainSceneManager.WasteResource(_wasteResourceByState[_state] * Time.deltaTime);
            if (IsWaitingState) return;
            if (IsResulting) return;
            if (_mainSceneManager.IsResult) {
                _state = GirlState.Resulting;
                ParamIsWalking = false;
                ParamIsRunning = false;
                _animator.SetBool("IsWalking", ParamIsWalking);
                _animator.SetBool("IsRunning", ParamIsRunning);
                return;
            }
            
            var moveVector = new Vector3(0.5f, 0, 0);
            if (IsRunningState) moveVector *= 3;

            _charaTransform.DOMove(moveVector, 1).SetRelative(true);
            // 移動距離をintに変換してスコアに加算する
            _mainSceneManager.AddScore(moveVector.magnitude);
            await UniTask.Delay(1000);
        }

        private async UniTaskVoid ChangeStateByInputAsync(CancellationToken token) {
            while (true) {
                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && !IsResulting, cancellationToken: token);
                ParamIsWalking = true;
                _state = GirlState.Walking;
                _animator.SetBool("IsWalking", ParamIsWalking);

                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && !IsResulting, cancellationToken: token);
                ParamIsRunning = true;
                _state = GirlState.Running;
                _animator.SetBool("IsRunning", ParamIsRunning);

                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && !IsResulting, cancellationToken: token);
                ParamIsRunning = false;
                _state = GirlState.Walking;
                _animator.SetBool("IsRunning", ParamIsRunning);

                await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Space) && !IsResulting, cancellationToken: token);
                ParamIsWalking = false;
                _state = GirlState.Waiting;
                _animator.SetBool("IsWalking", ParamIsWalking);
            }
        }
    }
}