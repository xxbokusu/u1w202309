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
            WarmUp,
            Walking,
            Running,
            Exhausted,
            Resulting
        }

        private GirlState _state = GirlState.Waiting;

        private readonly Dictionary<GirlState, float> _wasteResourceByState = new() {
            { GirlState.Waiting, 1.0f },
            { GirlState.WarmUp, 2.0f },
            { GirlState.Walking, 3.0f },
            { GirlState.Running, 4.0f },
            { GirlState.Exhausted, 4.0f },
            { GirlState.Resulting, 0.0f },
        };

        private readonly Dictionary<GirlState, string> _stateNameByState = new() {
            { GirlState.Waiting, "WAIT00" },
            { GirlState.WarmUp, "WAIT02" },
            { GirlState.Walking, "WALK00_F" },
            { GirlState.Running, "RUN00_F" },
            { GirlState.Exhausted, "REFLESH00" },
        };

        private Transform _charaTransform;
        private Animator _animator;

        public bool IsWaitingState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Waiting]);

        public bool IsWarmUpState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.WarmUp]);

        public bool IsWalkingState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Walking]);
        
        private bool ParamIsExhausted { get; set; } = false;

        public bool IsRunningState =>
            _animator.GetCurrentAnimatorStateInfo(0).IsName(_stateNameByState[GirlState.Running]);

        public bool IsResulting => _state == GirlState.Resulting;

        private MainSceneManager _mainSceneManager;
        private PowerChargerController _powerChargerController;

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

            _powerChargerController = GameObject.Find("PowerCharger").GetComponent<PowerChargerController>();
            if (_powerChargerController == null) {
                Debug.Util.LogError("GirlController::Start()::_powerChargerController is null");
                return;
            }

            _powerChargerController.Activate();
        }

        private async void Update() {
            _mainSceneManager.WasteResource(_wasteResourceByState[_state] * Time.deltaTime);
            if (_mainSceneManager.IsResult) {
                _state = GirlState.Resulting;
                _animator.SetBool("IsWalking", false);
                _animator.SetBool("IsRunning", false);
                _animator.SetBool("IsWarmUp", false);
                return;
            }

            if (_powerChargerController.Power > 15.0f) {
                _state = GirlState.Exhausted;
                ParamIsExhausted = true;
                _animator.SetBool("IsExthausted", true);
            }

            if (ParamIsExhausted) {
                if (_powerChargerController.Power < 1.0f) {
                    ParamIsExhausted = false;
                    _animator.SetBool("IsExthausted", false);
                    _state = GirlState.Waiting;
                }
                else {
                    return;
                }
            }

            switch (_powerChargerController.Power) {
                case > 10.0f:
                    _state = GirlState.Running;
                    _animator.SetBool("IsRunning", true);
                    break;
                case > 2.0f:
                    _state = GirlState.Walking;
                    _animator.SetBool("IsWalking", true);
                    break;
                case > 1.0f:
                    _state = GirlState.WarmUp;
                    _animator.SetBool("IsWalking", false);
                    _animator.SetBool("IsRunning", false);
                    _animator.SetBool("IsWarmUp", true);
                    break;
                default:
                    _state = GirlState.Waiting;
                    _animator.SetBool("IsWalking", false);
                    _animator.SetBool("IsRunning", false);
                    _animator.SetBool("IsWarmUp", false);
                    break;
            }

            if (IsWaitingState) return;
            if (IsWarmUpState) return;
            if (IsResulting) return;

            var moveVector = new Vector3(0.5f, 0, 0);
            if (IsRunningState) moveVector *= 3;

            _charaTransform.DOMove(moveVector, 1).SetRelative(true);
            // 移動距離をintに変換してスコアに加算する
            _mainSceneManager.AddScore(moveVector.magnitude);
            await UniTask.Delay(1000);
        }

        // stateに応じてキャラのFace rigを切り替える
        public void ChangeFaceRig() {
            switch (_state) {
                case GirlState.Waiting:
                    _animator.SetTrigger("IsWaiting");
                    break;
                case GirlState.Walking:
                    _animator.SetTrigger("IsWalking");
                    break;
                case GirlState.Running:
                    _animator.SetTrigger("IsRunning");
                    break;
                case GirlState.Exhausted:
                    _animator.SetTrigger("IsExthausted");
                    break;
                case GirlState.Resulting:
                    _animator.SetTrigger("IsResulting");
                    break;
            }
        }
    }
}