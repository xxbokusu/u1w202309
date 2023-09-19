namespace Assets.Scripts.Manager
{
    /*
     * <summery>
     * タイトルシーンの進行を管理する
     * </summery>
     */
    public class TitleSceneManager: ISceneManager
    {
        private enum TitleSceneState
        {
            Waiting,
            Working
        }
        private TitleSceneState _state = TitleSceneState.Waiting;
        public bool IsWorking { get { return _state == TitleSceneState.Working; } }

        public void Initialize()
        {
            _state = TitleSceneState.Working;
        }
        
        public void WatchSceneState()
        {
            switch (_state)
            {
                case TitleSceneState.Waiting:
                    break;
                case TitleSceneState.Working:
                    break;
            }
        }
    }
}