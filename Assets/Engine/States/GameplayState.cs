#region
using MergeGridGame.Core;
using MergeGridGame.Core.FSM;
using MergeGridGame.Models;
using MergeGridGame.Presenters;
using MergeGridGame.ScriptableObjects;
using MergeGridGame.Services;
using MergeGridGame.UI;
using UnityEngine;
#endregion

namespace MergeGridGame.States
{
    public class GameplayState : State
    {
        private const string CurrentLevelSaveKey = "CurrentLevelIndex";

        private GameplayPresenter _gameplayPresenter;
        private VictoryScreenView _victoryScreenView;
        private ISoundService _soundService;
        private SoundRegistry _soundRegistry;

        public GameplayState(StateMachine fsm, DIContainer diContainer) : base(fsm, diContainer)
        {
        }

        public override void Enter()
        {
            PrefabRegistry prefabRegistry = _diContainer.Resolve<PrefabRegistry>();
            _soundRegistry = _diContainer.Resolve<SoundRegistry>();
            UIRoot uiRoot = _diContainer.Resolve<UIRoot>();
            ISaveService saveService = _diContainer.Resolve<ISaveService>();
            _soundService = _diContainer.Resolve<ISoundService>();

            AllLevelsConfigSO allLevelsConfig = prefabRegistry.AllLevelsConfig;
            int savedLevelIndex = saveService.LoadInt(CurrentLevelSaveKey, 0);

            if (savedLevelIndex >= allLevelsConfig.LevelsCount)
            {
                savedLevelIndex = 0;
            }

            LevelConfigSO levelConfig = allLevelsConfig.GetLevel(savedLevelIndex);

            GameObject screenPrefab = prefabRegistry.Get("GameplayScreenUI");
            GameObject screenObj = CreateUIInState(screenPrefab);
            GameplayScreenView screenView = screenObj.GetComponent<GameplayScreenView>();

            GridModel gridModel = new(levelConfig.Columns, levelConfig.Rows);

            _gameplayPresenter = new GameplayPresenter(
                gridModel,
                screenView,
                levelConfig,
                savedLevelIndex,
                allLevelsConfig.LevelsCount,
                prefabRegistry,
                _soundRegistry,
                uiRoot,
                saveService,
                _soundService);

            _gameplayPresenter.OnLevelCompleted += HandleLevelCompleted;
            _gameplayPresenter.Initialize();
        }

        public override void Update(float deltaTime)
        {
            _gameplayPresenter?.Update(deltaTime);
        }

        public override void Exit()
        {
            if (_gameplayPresenter != null)
            {
                _gameplayPresenter.OnLevelCompleted -= HandleLevelCompleted;
                _gameplayPresenter.Dispose();
                _gameplayPresenter = null;
            }

            if (_victoryScreenView != null)
            {
                _victoryScreenView.OnNextLevelClicked -= HandleNextLevelClicked;
                _victoryScreenView = null;
            }

            base.Exit();
        }

        private void HandleLevelCompleted(string currentTimeFormatted, string bestTimeFormatted)
        {
            _soundService.PlaySound(_soundRegistry.Get("Victory"));

            PrefabRegistry prefabRegistry = _diContainer.Resolve<PrefabRegistry>();
            GameObject victoryPrefab = prefabRegistry.Get("VictoryScreenUI");

            GameObject victoryObj = CreateUIInState(victoryPrefab);
            _victoryScreenView = victoryObj.GetComponent<VictoryScreenView>();

            _victoryScreenView.OnNextLevelClicked += HandleNextLevelClicked;
            _victoryScreenView.Setup(currentTimeFormatted, bestTimeFormatted);
        }

        private void HandleNextLevelClicked()
        {
            _soundService.PlaySound(_soundRegistry.Get("Button"));
            _fsm.ChangeState<LoadingState>();
        }
    }
}