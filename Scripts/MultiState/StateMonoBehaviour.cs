using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace EXOMUT.MultiState
{
    /// <summary>
    /// StateManager is a Multi-State manager with callbacks on entering and exiting states.
    /// </summary>
    public abstract class StateMonoBehaviour<TStates> : MonoBehaviour where TStates : Enum
    {
        private readonly StateManager _manager = new StateManager();
        private readonly Dictionary<string, State> _states = new Dictionary<string, State>();

        private State StringToState(string state) => _states[state];
        private static string EnumToString(Enum state) => state.ToString();
        private State[] StringToStates(IEnumerable<string> states) => states.Select(StringToState).ToArray();

        private static string[] EnumsToStrings<T>(IEnumerable<T> states) where T : Enum =>
            states.Select(s => s.ToString()).ToArray();

        
        [HeaderAttribute("Initial States: Can not be hot changed in play mode.")]
        public TStates behaviourState;
        
        public void ReloadStates() => ReplaceStates(Enums.FlagsToList<TStates>(behaviourState).ToArray());

        protected void Awake()
        {
            NewStatesFromEnum<TStates>();
            ReloadStates();
            StateAwake();
        }
        /// <summary>
        /// Method used to setup states
        /// </summary>
        protected abstract void StateAwake();

        protected void Start() => StateStart();
        protected abstract void StateStart();
        
        protected void Update()
        {
            _manager.Update();
            StateUpdate();
        }
        protected abstract void StateUpdate();

        /// <summary>
        /// Creates a New State in the Database to be used.
        /// </summary>
        /// <param name="stateName"></param>
        /// <exception cref="DuplicateNameException"></exception>
        public void NewState(string stateName)
        {
            if (_states.ContainsKey(stateName))
                throw new DuplicateNameException($"The State name '{stateName}' already exists");

            _states[stateName] = new State(stateName);
        }

        /// <summary>
        /// Creates multiple New States in the Database to be used.
        /// </summary>
        /// <param name="states"></param>
        public void NewStates(IEnumerable<string> states) => states.ToList().ForEach(NewState);

        /// <summary>
        /// Creates States Automatically from an Enum
        /// </summary>
        /// <typeparam name="T">Type of Enum</typeparam>
        private void NewStatesFromEnum<T>() where T : Enum
        {
            foreach (var item in Enum.GetNames(typeof(T)))
                NewState(item);
        }

        /// <summary>
        /// Destroys and removes a state.
        /// </summary>
        /// <remarks>onExitState will be called.</remarks>
        /// <param name="state"></param>
        public void DestroyState(string state)
        {
            RemoveState(state);
            _states.Remove(state);
        }

        /// <summary>
        /// Adds a single State to the StateManager.
        /// </summary>
        /// <remarks>
        /// Any State added during the Awake or Setup stage of a MonoBehaviour will have its OnStateEnter called during the first frame of Update.
        /// </remarks>
        /// <param name="state"></param>
        public void AddState(string state) => _manager.AddState(StringToState(state));

        public void AddState(TStates state) => AddState(EnumToString(state));

        /// <summary>
        /// Adds Multiple States to the StateManager.
        /// </summary>
        /// <remarks>
        /// Any State added during the Awake or Setup stage of a MonoBehaviour will have its OnStateEnter called during the first frame of Update.
        /// </remarks>
        /// <param name="states"></param>
        public void AddStates(params string[] states) => _manager.AddStates(StringToStates(states));

        public void AddStates(params TStates[] states) => AddStates(EnumsToStrings(states));
        
        /// <summary>
        /// Toggles a single State to the StateManager.
        /// </summary>
        /// <remarks>
        /// Any State added during the Awake or Setup stage of a MonoBehaviour will have its OnStateEnter called during the first frame of Update.
        /// </remarks>
        /// <param name="state"></param>
        public void ToggleState(string state) => _manager.ToggleState(StringToState(state));

        public void ToggleState(TStates state) => ToggleState(EnumToString(state));

        /// <summary>
        /// Toggles Multiple States in the StateManager.
        /// </summary>
        /// <remarks>
        /// Any State added during the Awake or Setup stage of a MonoBehaviour will have its OnStateEnter called during the first frame of Update.
        /// </remarks>
        /// <param name="states"></param>
        public void ToggleStates(params string[] states) => _manager.ToggleStates(StringToStates(states));
        
        public void ToggleStates(params TStates[] states) => ToggleStates(EnumsToStrings(states));

        /// <summary>
        /// Gets all states currently active.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetActiveStates() => _manager.GetStates().Select(s => s.Name);

        /// <summary>
        /// Gets a list of all states that can be used.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAvailableStates() => _states.Keys;

        /// <summary>
        /// Gets all non-active states.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetNonActiveStates() =>
            GetActiveStates().Where(s => GetAvailableStates().Contains(s));

        /// <summary>
        /// Replaces all states and adds the given state
        /// </summary>
        /// <param name="state"></param>
        public void ChangeState(string state) => ReplaceState(state);

        public void ChangeState(TStates state) => ChangeState(EnumToString(state));

        /// <summary>
        /// Removes all states and adds a single State to the StateManager.
        /// </summary>
        /// <remarks>
        /// Any State added during the Awake or Setup stage of a MonoBehaviour will have its OnStateEnter called during the first frame of Update.
        /// </remarks>
        /// <param name="state"></param>
        public void ReplaceState(string state) => _manager.ReplaceState(StringToState(state));

        public void ReplaceState(TStates state) => ReplaceState(EnumToString(state));

        /// <summary>
        /// Removes all states and adds multiple States to the StateManager.
        /// </summary>
        /// <remarks>
        /// Any State added during the Awake or Setup stage of a MonoBehaviour will have its OnStateEnter called during the first frame of Update.
        /// </remarks>
        /// <param name="states"></param>
        public void ReplaceStates(params string[] states) => _manager.ReplaceStates(StringToStates(states));
        
        public void ReplaceStates(params TStates[] states) => ReplaceStates(EnumsToStrings(states));

        /// <summary>
        /// Removes the State from the StateManager.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>true: if state was found and removed.</returns>
        public bool RemoveState(string state) => _manager.RemoveState(StringToState(state));

        public bool RemoveState(TStates state) => RemoveState(EnumToString(state));

        /// <summary>
        /// Removes multiple States from the StateManager.
        /// </summary>
        /// <param name="states"></param>
        /// <returns>
        /// true: if all states found and removed.
        /// Warning:
        /// false: will be returned if one or more States is removed although one or more States was not found.
        /// </returns>
        public bool RemoveStates(params string[] states) => _manager.RemoveStates(StringToStates(states));

        public bool RemoveStates(params TStates[] states) => RemoveStates(EnumsToStrings(states));
        
        /// <summary>
        /// Checks if State is the only state in StateManager.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>true: if State is the only one found.</returns>
        public bool IsState(string state) => _manager.IsState(StringToState(state));
        
        public bool IsState(TStates state) => IsState(EnumToString(state));

        /// <summary>
        /// Checks if State is in StateManager.
        /// </summary>
        /// <param name="state"></param>
        /// <returns>true: if State found.</returns>
        public bool HasState(string state) => _manager.HasState(StringToState(state));

        public bool HasState(TStates state) => HasState(EnumToString(state));

        /// <summary>
        /// Checks if multiples States are all in the StateManager.
        /// </summary>
        /// <param name="states"></param>
        /// <returns>true: only if all States found.</returns>
        public bool HasStates(params string[] states) => _manager.HasStates(StringToStates(states));

        public bool HasStates(params TStates[] states) => HasStates(EnumsToStrings(states));

        /// <summary>
        /// Sets Game Object to Active when state is entered and to not active when state is exited.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="go"></param>
        public GameObject SetActive(string state, GameObject go) => _manager.SetActive(StringToState(state), go);

        public GameObject SetActive(TStates state, GameObject go) => SetActive(EnumToString(state), go);

        /// <summary>
        /// Sets Game Object to Not Active when state is entered and to active when state is exited.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="go"></param>
        public GameObject SetNotActive(string state, GameObject go) => _manager.SetNotActive(StringToState(state), go);

        public GameObject SetNotActive(TStates state, GameObject go) => SetNotActive(EnumToString(state), go);

        /// <summary>
        /// Automatically adds and removes listener when entering and exiting state.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="unityEvent"></param>
        /// <param name="action"></param>
        public void AddListener(string state, UnityEvent unityEvent, UnityAction action) =>
            _manager.AddListener(StringToState(state), unityEvent, action);

        public void AddListener(TStates state, UnityEvent unityEvent, UnityAction action) =>
            AddListener(EnumToString(state), unityEvent, action);

        public void AddListenerToStates(UnityEvent unityEvent, UnityAction action, params string[] states) =>
            _manager.AddListenerToStates(unityEvent, action, StringToStates(states));
        public void AddListenerToStates(UnityEvent unityEvent, UnityAction action, params TStates[] states) =>
            AddListenerToStates(unityEvent, action, EnumsToStrings(states));

        public void AddListeners(string state, UnityEvent unityEvent, params UnityAction[] actions) =>
            _manager.AddListeners(StringToState(state), unityEvent, actions);

        public void AddListeners(TStates state, UnityEvent unityEvent, params UnityAction[] actions) =>
            AddListeners(EnumToString(state), unityEvent, actions);

        /// <summary>
        /// Automatically adds and removes listener when entering and exiting state.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="unityEvent"></param>
        /// <param name="action"></param>
        public void AddListener<T>(string state, UnityEvent<T> unityEvent, UnityAction<T> action) =>
            _manager.AddListener(StringToState(state), unityEvent, action);

        public void AddListener<T>(TStates state, UnityEvent<T> unityEvent, UnityAction<T> action) =>
            AddListener(EnumToString(state), unityEvent, action);
        
        public void AddListenerToStates<T>(UnityEvent<T> unityEvent, UnityAction<T> action, params string[] states) =>
            _manager.AddListenerToStates(unityEvent, action, StringToStates(states));
        public void AddListenerToStates<T>(UnityEvent<T> unityEvent, UnityAction<T> action, params TStates[] states) =>
            AddListenerToStates(unityEvent, action, EnumsToStrings(states));

        /// <summary>
        /// Automatically adds and removes multiple listeners when entering and exiting state.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="unityEvent"></param>
        /// <param name="actions"></param>
        /// <typeparam name="T"></typeparam>
        public void AddListeners<T>(string state, UnityEvent<T> unityEvent, params UnityAction<T>[] actions) =>
            _manager.AddListeners(StringToState(state), unityEvent, actions);

        public void AddListeners<T>(TStates state, UnityEvent<T> unityEvent, params UnityAction<T>[] actions) =>
            AddListeners(EnumToString(state), unityEvent, actions);

        /// <summary>
        /// Adds callback that will be invoked on entering state.
        /// Will not be called until just before first OnStateUpdate after declaring StateMachine to allow
        /// Awake and Start methods to be executed across all MonoBehaviours.
        /// </summary>
        public void AddOnEnter(string state, Action action, bool forceLateRun = false) =>
            _manager.AddOnEnter(StringToState(state), action, forceLateRun);

        public void AddOnEnter(TStates state, Action action, bool forceLateRun = false) =>
            AddOnEnter(EnumToString(state), action, forceLateRun);

        /// <summary>
        /// Removes a delegate from the OnEnter callback.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        public void RemoveOnEnter(string state, Action action) => _manager.RemoveOnEnter(StringToState(state), action);

        public void RemoveOnEnter(TStates state, Action action) => RemoveOnEnter(EnumToString(state), action);

        /// <summary>
        /// Adds callback that will be invoked on exiting state.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        public void AddOnExit(string state, Action action) => _manager.AddOnExit(StringToState(state), action);

        public void AddOnExit(TStates state, Action action) => AddOnExit(EnumToString(state), action);

        /// <summary>
        /// Removes a delegate from the OnExit callback.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        public void RemoveOnExit(string state, Action action) => _manager.RemoveOnExit(StringToState(state), action);

        public void RemoveOnExit(TStates state, Action action) => RemoveOnExit(EnumToString(state), action);

        /// <summary>
        /// Adds a callback that will be invoked when the StateMachine.Update() method is called each frame.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        public void AddOnUpdate(string state, Action action) => _manager.AddOnUpdate(StringToState(state), action);

        public void AddOnUpdate(TStates state, Action action) => AddOnUpdate(EnumToString(state), action);

        /// <summary>
        /// Removes a delegate from the OnUpdate callback.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="action"></param>
        public void RemoveOnUpdate(string state, Action action) =>
            _manager.RemoveOnUpdate(StringToState(state), action);

        public void RemoveOnUpdate(TStates state, Action action) => RemoveOnUpdate(EnumToString(state), action);

        /// <summary>
        /// Adds a callback that will be invoked when any state is added.
        /// </summary>
        /// <param name="action"></param>
        public void AddOnStateAdded(Action<string> action) => _manager.AddOnStateAdded(action);

        /// <summary>
        /// Removes a delegate from the OnStateAdded callback.
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnStateAdded(Action<string> action) => _manager.RemoveOnStateRemoved(action);

        /// <summary>
        /// Adds a callback that will be invoked when any state is removed. State name is passed to the action.
        /// </summary>
        /// <param name="action"></param>
        public void AddOnStateRemoved(Action<string> action) => _manager.AddOnStateRemoved(action);

        /// <summary>
        /// Removes a delegate from the OnStateRemoved callback.
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnStateRemoved(Action<string> action) => _manager.RemoveOnStateRemoved(action);

        /// <summary>
        /// Adds a callback that will be invoked when any changes to the state manager has been made.
        /// </summary>
        /// <param name="action"></param>
        public void AddOnStateChanged(Action action) => _manager.AddOnStateChanged(action);

        /// <summary>
        /// Removes a delegate from the OnStateChanged callback.
        /// </summary>
        /// <param name="action"></param>
        public void RemoveOnStateChanged(Action action) => _manager.RemoveOnStateChanged(action);
    }
}