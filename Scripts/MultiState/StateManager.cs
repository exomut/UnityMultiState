using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace EXOMUT.MultiState
{
    internal class StateManager
    {
        private Action<string> _onStateAdded;
        private Action<string> _onStateRemoved;
        private Action _onStateChanged;

        private bool _initialized;

        private readonly List<State> _states = new List<State>();

        internal void AddState(State state)
        {
            _states.Add(state);
            
            if (_initialized)
            {
                state.OnStateEnter?.Invoke();
                _onStateAdded?.Invoke(state.Name);
            }

            _onStateChanged?.Invoke();
        }
        
        internal void AddStates(params State[] states)
        {
            foreach (var state in states)
                AddState(state);
        }
        
        internal void ToggleState(State state)
        {
            if (HasState(state))
                RemoveState(state);
            else
                AddState(state);
        }
        
        internal void ToggleStates(params State[] states)
        {
            foreach (var state in states)
                ToggleState(state);
        }

        internal IEnumerable<State> GetStates() => _states;


        internal void ReplaceState(State state)
        {
            ClearStates();
            AddState(state);
        }

        internal void ReplaceStates(params State[] states)
        {
            ClearStates();
            AddStates(states);
        }

        internal bool RemoveState(State state)
        {
            var result = _states.Remove(state);
            state?.OnStateExit?.Invoke();
            
            _onStateRemoved?.Invoke(state?.Name);
            _onStateChanged?.Invoke();

            return result;
        }

        internal bool RemoveStates(params State[] states) => states.All(RemoveState);

        internal bool IsState(State state) => _states.Count == 1 && _states.Contains(state) && _initialized;
        internal bool HasState(State state) => _states.Contains(state) && _initialized;

        internal bool HasStates(params State[] states) => states.All(HasState);

        internal void Update()
        {
            if (!_initialized)
            {
                foreach (var state in _states)
                {
                    state.OnStateEnter?.Invoke();
                    _onStateAdded?.Invoke(state.Name);
                }

                _initialized = true;
            }

            foreach (var state in _states)
                state.OnStateUpdate?.Invoke();
        }

        internal GameObject SetActive(State state, GameObject gameObject)
        {
            if (HasState(state))
                gameObject.SetActive(true);

            void Activate()
            {
                if (gameObject == null)
                    state.OnStateEnter -= Activate;
                else
                    gameObject.SetActive(true);
            }

            state.OnStateEnter += Activate;

            void Deactivate()
            {
                if (gameObject == null)
                    state.OnStateExit -= Deactivate;
                else
                    gameObject.SetActive(false);
            }

            state.OnStateExit += Deactivate;

            return gameObject;
        }

        internal GameObject SetNotActive(State state, GameObject gameObject)
        {
            if (HasState(state))
                gameObject.SetActive(false);

            void Deactivate()
            {
                if (gameObject == null)
                    state.OnStateEnter -= Deactivate;
                else
                    gameObject.SetActive(false);
            }

            state.OnStateEnter += Deactivate;

            void Activate()
            {
                if (gameObject == null)
                    state.OnStateExit -= Activate;
                else
                    gameObject.SetActive(true);
            }

            state.OnStateExit += Activate;

            return gameObject;
        }

        internal void AddListener(State state, UnityEvent unityEvent, UnityAction action)
        {
            if (HasState(state))
                unityEvent.AddListener(action);

            state.OnStateEnter += () => unityEvent.AddListener(action);
            state.OnStateExit += () => unityEvent.RemoveListener(action);
        }
        
        internal void AddListenerToStates(UnityEvent unityEvent, UnityAction action, params State[] states)
        {
            foreach (var state in states)
                AddListener(state, unityEvent, action);
        }

        internal void AddListeners(State state, UnityEvent unityEvent, params UnityAction[] actions) =>
            actions.ToList().ForEach(a => AddListener(state, unityEvent, a));

        internal void AddListener<T>(State state, UnityEvent<T> unityEvent, UnityAction<T> action)
        {
            if (HasState(state))
                unityEvent.AddListener(action);

            state.OnStateEnter += () => unityEvent.AddListener(action);
            state.OnStateExit += () => unityEvent.RemoveListener(action);
        }
        
        internal void AddListenerToStates<T>(UnityEvent<T> unityEvent, UnityAction<T> action, params State[] states)
        {
            foreach (var state in states)
                AddListener(state, unityEvent, action);
        }

        internal void AddListeners<T>(State state, UnityEvent<T> unityEvent, params UnityAction<T>[] actions) =>
            actions.ToList().ForEach(a => AddListener(state, unityEvent, a));

        internal void AddOnEnter(State state, Action action, bool forceLateRun)
        {
            if (forceLateRun && _initialized && HasState(state))
                action();

            state.OnStateEnter += action;
        }

        internal void RemoveOnEnter(State state, Action action) => state.OnStateEnter -= action;

        internal void AddOnExit(State state, Action action) => state.OnStateExit += action;

        internal void RemoveOnExit(State state, Action action) => state.OnStateExit -= action;

        internal void AddOnUpdate(State state, Action action) => state.OnStateUpdate += action;

        internal void RemoveOnUpdate(State state, Action action) => state.OnStateUpdate -= action;

        internal void AddOnStateAdded(Action<string> action) => _onStateAdded += action;

        internal void RemoveOnStateAdded(Action<string> action) => _onStateAdded -= action;

        internal void AddOnStateRemoved(Action<string> action) => _onStateRemoved += action;

        internal void RemoveOnStateRemoved(Action<string> action) => _onStateRemoved -= action;

        internal void AddOnStateChanged(Action action) => _onStateChanged += action;

        internal void RemoveOnStateChanged(Action action) => _onStateChanged -= action;

        private void ClearStates()
        {
            foreach (var state in _states)
            {
                state?.OnStateExit?.Invoke();
                _onStateRemoved?.Invoke(state?.Name);
            }

            _states.Clear();
        }
    }
}