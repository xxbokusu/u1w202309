using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace unity1week202309.Controller {
    public class GirlController : MonoBehaviour {
        private enum GirlState {
            Waiting,
            Walking,
            Running,
        }

        private readonly Dictionary<GirlState, string> _stateNameByState = new() {
            { GirlState.Walking, "WALK00_F" },
            { GirlState.Running, "RUN00_F" },
        };

        private Transform _charaTransform;
        private Animator _animator;
        public bool ParamIsWalking { get; private set; } = false;

        public bool IsWalkingState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Walking]);

        public bool ParamIsRunning { get; private set; } = false;

        public bool IsRunningState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Running]);

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

            ChangeStateByInputAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }

        private async void Update() {
            if (!IsWalkingState) return;

            var moveVector = new Vector3(1, 0, 0);
            if (IsRunningState) moveVector *= 2;

            _charaTransform.DOMove(moveVector, 1).SetRelative(true);
            await UniTask.Delay(1000);
        }

        private async UniTaskVoid ChangeStateByInputAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
            ParamIsWalking = true;
            _animator.SetBool("IsWalking", ParamIsWalking);

            await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
            ParamIsRunning = true;
            _animator.SetBool("IsRunning", ParamIsRunning);

            await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
            ParamIsRunning = false;
            _animator.SetBool("IsRunning", ParamIsRunning);

            await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
            ParamIsWalking = false;
            _animator.SetBool("IsWalking", ParamIsWalking);
        }
    }
}