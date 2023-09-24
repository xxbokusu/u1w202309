using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObject {
    /*
     * <summery>
     * プレイ時のスコアを管理するScriptableObject
     * 最終的にはrankingに送信する
     * </summery>
     */
    [CreateAssetMenu(fileName = "Score", menuName = "ScriptableObject/Score")]
    public class ScoreScriptableObject : UnityEngine.ScriptableObject {
        [SerializeField] private float score = 0;
        public void AddScore(float addScore) {
            score += addScore;
        }
        public float Score {
            get { return score; }
            set { score = value; }
        }
    }
}