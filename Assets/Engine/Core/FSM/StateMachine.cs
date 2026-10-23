#region

using System;
using System.Collections.Generic;

#endregion

namespace MergeGridGame.Core.FSM
{
    public class StateMachine
    {
        private Dictionary<Type, IState> _states = new Dictionary<Type, IState>();
        private IState _currentState;

        public void RegisterState<T>(IState state) where T : IState
        {
            _states[typeof(T)] = state;
        }

        public void ChangeState<T>() where T : IState
        {
            _currentState?.Exit();

            if (_states.TryGetValue(typeof(T), out var newState))
            {
                _currentState = newState;
                _currentState.Enter();
            }
        }

        public void Update(float deltaTime)
        {
            _currentState?.Update(deltaTime);
        }
    }
}
