using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace unity1week202309.Controller {
    public class GirlController : MonoBehaviour {
        private Animator _animator;
        bool isRunning = false;
        void Start() {
            _animator = GetComponent<Animator>();
        }

        void Update() {
            isRunning = true;
            _animator.SetBool("IsRunning", isRunning);
        }
    }
}