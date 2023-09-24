using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements;

namespace unity1week202309.Controller {
    /*
     * <summery>
     * カメラワークを制御するクラス
     * Titleから進行するとキャラとステージを横から写す
     * </summery>
     */
    public class CameraViewController : MonoBehaviour {
        private Camera _camera;
        private GameObject _charaObject;
        private Transform _charaTransform;
        private Vector3 _prePosition;
        private bool _isInitialized = false;
        
        private GirlController _girlController;
        private void Start() {
            _camera = GetComponent<Camera>();
            if (_camera == null) {
                Debug.Util.LogError("CameraViewController::Start()::_camera is null");
                return;
            }
            InitializeAsync(this.GetCancellationTokenOnDestroy()).Forget();
        }
        
        private async UniTaskVoid InitializeAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => _charaTransform != null, cancellationToken: token);
            
            _isInitialized = true;
            _prePosition = _charaTransform.position;
            MoveResultViewAsync(token).Forget();
        }
        
        public void SetCharaObject(GameObject charaObject) {
            _charaObject = charaObject;
            _charaTransform = _charaObject.transform;
            _girlController = _charaObject.GetComponent<GirlController>();
            if (_girlController == null) {
                Debug.Util.LogError("CameraViewController::SetCharaObject()::_girlController is null");
                return;
            }
        }
        
        private void Update() {
            if (!_isInitialized) return;
            
            // charaとの相対距離を保ちながらカメラを移動させる
            var charaPosition = _charaTransform.position;
            var diff = charaPosition - _prePosition;
            _camera.transform.position += diff;
            _prePosition = charaPosition;
        }

        private async UniTaskVoid MoveResultViewAsync(CancellationToken token) {
            await UniTask.WaitUntil(() => _girlController.IsResulting, cancellationToken: token);
            
            _camera.transform.DOMove(new Vector3(_charaTransform.position.x + 2.2f, 3.5f, _charaTransform.position.z + -0.35f), 1.0f);
            // _camera.transform.DOMove(new Vector3(-4.5f, 0.0f, 2.3f), 1.0f).SetRelative(true);
        }
    }
}