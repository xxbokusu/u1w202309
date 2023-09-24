using System.Collections;
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

        private Dictionary<GirlState, string> StateNameByState = new() {
            { GirlState.Walking, "WALK00_F" },
            { GirlState.Running, "RUN00_F" },
        };
        private Transform _charaTransform;
        private Animator _animator;
        private bool paramIsWalking = false;
        public bool ParamIsWalking => paramIsWalking;
        public bool IsWalkingState => _animator.GetCurrentAnimatorStateInfo(0).IsName(StateNameByState[GirlState.Walking]);

        private bool paramIsRunning = false;
        public bool ParamIsRunning => paramIsRunning;
        public bool IsRunningState => _animator.GetCurrentAnimatorStateInfo(0).IsName(StateNameByState[GirlState.Running]);

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
            ChangeStateByInputAsync(this.GetCancellationTokenOnDestroy());
        }

        private async void Update() {
            if (!IsWalkingState) return;

            var moveVector = new Vector3(1, 0, 0);
            if (IsRunningState) moveVector *= 2;

            _charaTransform.DOMove(moveVector, 1).SetRelative(true);
            await UniTask.Delay(1000);
        }

        private async void ChangeStateByInputAsync(CancellationToken token) {
            while (true) {
                await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
                paramIsWalking = true;
                _animator.SetBool("IsWalking", paramIsWalking);
            
                await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
                paramIsRunning = true;
                _animator.SetBool("IsRunning", paramIsRunning);
            
                await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
                paramIsRunning = false;
                _animator.SetBool("IsRunning", paramIsRunning);
            
                await UniTask.WaitUntil(() => Input.GetKeyUp(KeyCode.Space), cancellationToken: token);
                paramIsWalking = false;
                _animator.SetBool("IsWalking", paramIsWalking);
            }
        }
    }
}