#region

using MergeGridGame.Core;
using MergeGridGame.Core.FSM;
using MergeGridGame.ScriptableObjects;

#endregion

namespace MergeGridGame.States
{
    public class LoadingState : State
    {
        private const float LoadingDuration = 0.5f;
        private float _timer;

        public LoadingState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer)
        {
        }

        public override void Enter()
        {
            _timer = 0f;

            PrefabRegistry prefabRegistry = _diContainer.Resolve<PrefabRegistry>();
            _ = CreateUIInState(prefabRegistry.Get("LoadingScreenUI"));
        }

        public override void Update(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= LoadingDuration)
            {
                _fsm.ChangeState<GameplayState>();
            }
        }
    }
}
