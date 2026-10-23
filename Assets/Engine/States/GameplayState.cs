#region
using MergeGridGame.Core;
using MergeGridGame.Core.FSM;
using MergeGridGame.Models;
using MergeGridGame.Presenters;
using MergeGridGame.ScriptableObjects;
using MergeGridGame.UI;
using UnityEngine;
#endregion

namespace MergeGridGame.States
{
    public class GameplayState : State
    {
        private GameplayPresenter _gameplayPresenter;

        public GameplayState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer)
        {
        }

        public override void Enter()
        {
            PrefabRegistry prefabRegistry = _diContainer.Resolve<PrefabRegistry>();
            UIRoot uiRoot = _diContainer.Resolve<UIRoot>();
            LevelConfigSO levelConfig = prefabRegistry.defaultLevelConfig;

            GameObject screenPrefab = prefabRegistry.Get("GameplayScreenUI");
            GameObject screenObj = CreateUIInState(screenPrefab);
            GameplayScreenView screenView = screenObj.GetComponent<GameplayScreenView>();

            GridModel gridModel = new(levelConfig.Columns, levelConfig.Rows);

            _gameplayPresenter = new GameplayPresenter(
                gridModel,
                screenView,
                levelConfig,
                prefabRegistry,
                uiRoot);

            _gameplayPresenter.Initialize();
        }

        public override void Exit()
        {
            _gameplayPresenter?.Dispose();
            _gameplayPresenter = null;

            base.Exit();
        }
    }
}