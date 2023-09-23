using UnityEngine;
using unity1week202309.Manager;

namespace ScriptableObject {
    /*
     * <summery>
     * ゲーム全体で共用する音声設定を管理するScriptableObject
     * </summery>
     */
    [CreateAssetMenu(fileName = "GameConfig", menuName = "ScriptableObject/GameConfig")]
    public class ConfigScriptableObject : UnityEngine.ScriptableObject {
        [SerializeField] private float bgmVolumeRate = 1.0f;

        // BGMの音量倍率変更を反映して、再生中のBGMの音量を変更する
        public void SetBGMVolumeRate(float volume) {
            bgmVolumeRate = volume;
            SoundManager.Instance.ResetBGMVolume();
        }

        public float GetBGMVolumeRate() {
            return bgmVolumeRate;
        }
    }
}