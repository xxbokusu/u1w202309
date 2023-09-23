using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unity1week202309.Controller {
    public class GirlController : MonoBehaviour {
        private Animator _animator;
        private bool isWalking = false;
        public bool IsWalking => isWalking;

        private bool isRunning = false;
        public bool IsRunning => isRunning;
        void Start() {
            _animator = GetComponent<Animator>();
        }

        void Update() {
            if (!Input.GetKeyUp(KeyCode.Space)) return;
            if (!isWalking) {
                isWalking = true;
                _animator.SetBool("IsWalking", isWalking);
                return;
            }
            if (!isRunning) {
                isRunning = true;
                _animator.SetBool("IsRunning", isRunning);
                return;
            }
            
            isWalking = false;
            isRunning = false;
        }
    }
}