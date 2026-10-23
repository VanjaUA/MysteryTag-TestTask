#region

using MergeGridGame.Factories;
using System.Collections.Generic;
using UnityEngine;

#endregion

namespace MergeGridGame.Core.FSM
{
    public abstract class State : IState
    {
        protected readonly StateMachine _fsm;
        protected readonly DIContainer _diContainer;

        private readonly List<Object> _createdObjects = new();

        protected State(StateMachine fsm, DIContainer diContainer)
        {
            _fsm = fsm;
            _diContainer = diContainer;
        }

        public virtual void Enter() { }
        public virtual void Exit()
        {
            foreach (Object obj in _createdObjects)
            {
                if (obj == null)
                {
                    continue;
                }

                if (obj is GameObject go)
                {
                    GameObject.Destroy(go);
                    continue;
                }
                if (obj is Component component)
                {
                    GameObject.Destroy(component.gameObject);
                    continue;
                }
                Object.Destroy(obj);
            }
            _createdObjects.Clear();
        }
        public virtual void Update(float deltaTime) { }

        protected T CreateUIInState<T>(T uiPrefab) where T : Object
        {
            T uiElement = _diContainer.Resolve<UIFactory>().CreateUIElement(uiPrefab);
            _createdObjects.Add(uiElement);
            return uiElement;
        }

        protected T CreateInState<T>(T prefab, Vector3 position, Quaternion rotation) where T : Object
        {
            T instance = Object.Instantiate(prefab, position, rotation);
            _createdObjects.Add(instance);
            return instance;
        }

        protected void TrackToState(Object obj)
        {
            if (obj == null || _createdObjects.Contains(obj))
            {
                return;
            }
            _createdObjects.Add(obj);
        }
    }
}
