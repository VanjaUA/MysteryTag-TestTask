#region 

using MergeGridGame.Core.FSM;
using MergeGridGame.Factories;
using MergeGridGame.ScriptableObjects;
using MergeGridGame.Services;
using MergeGridGame.States;
using MergeGridGame.UI;
using UnityEngine;
using UnityEngine.EventSystems;

#endregion

namespace MergeGridGame.Core
{
    public class GameStarter : MonoBehaviour
    {
        [SerializeField] private PrefabRegistry prefabRegistry;
        //[SerializeField] private SoundRegistry soundRegistry;

        private StateMachine _stateMachine;
        private DIContainer _diContainer;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            Input.multiTouchEnabled = false;

            _diContainer = new DIContainer();

            BootstrapInfrastructure();
            RegisterServices();
            RegisterFactories();

            _stateMachine = new StateMachine();
            RegisterFSMStates();

            _stateMachine.ChangeState<LoadingState>();
        }

        private void Update()
        {
            _stateMachine.Update(Time.deltaTime);
        }

        private void BootstrapInfrastructure()
        {
            Camera mainCamera = Instantiate(prefabRegistry.MainCameraPrefab);
            mainCamera.name = "[MainCamera]";

            EventSystem eventSystem = Instantiate(prefabRegistry.EventSystemPrefab);
            eventSystem.name = "[EventSystem]";

            UIRoot uiRoot = Instantiate(prefabRegistry.UIRootPrefab);
            uiRoot.name = "[UIRoot]";
            uiRoot.Initialize(mainCamera);

            _diContainer.Register(mainCamera);
            _diContainer.Register(uiRoot);
        }

        private void RegisterServices()
        {
            _diContainer.Register(prefabRegistry);
            //_diContainer.Register(soundRegistry);

            SaveService saveService = new();
            _diContainer.Register<ISaveService>(saveService);
            //_diContainer.Register<IScoreService>(new ScoreService(saveService));
            //_diContainer.Register<ISoundService>(new SoundService(audioRoot));
        }

        private void RegisterFactories()
        {
            UIRoot uiRoot = _diContainer.Resolve<UIRoot>();
            _diContainer.Register(new UIFactory(uiRoot.ScreensContainer));
        }

        private void RegisterFSMStates()
        {
            _stateMachine.RegisterState<LoadingState>(new LoadingState(_stateMachine, _diContainer));
            _stateMachine.RegisterState<GameplayState>(new GameplayState(_stateMachine, _diContainer));
        }
    }
}
