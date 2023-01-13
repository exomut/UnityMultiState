using System;
using EXOMUT.MultiState;
using UnityEngine;
using UnityEngine.Events;

namespace EXOMUT.Example.Scripts
{
    // Declare the different states you will use here.
    [Flags]
    public enum CubeStates
    {
        Control = 1 << 0, 
        Spinning = 1 << 1, 
        Fall = 1 << 2, 
    }

    // Initialize the Multi State object with a "StateMonoBehaviour" and your new enum States
    public class CubeStateManager : StateMonoBehaviour<CubeStates>
    {
        public UnityEvent onEnterFall;
        public UnityEvent onExitFall;

        private CubeControls _controls;
        
        private void Move()
        {
            var xy = _controls.Move.Move.ReadValue<Vector2>();
            transform.position += new Vector3(xy.x, 0, xy.y) * Time.deltaTime;
        }

        // Do not use "Awake" instead use "StateAwake"
        protected override void StateAwake()
        {
            _controls = new CubeControls();
            _controls.Move.Enable();
        
            AddOnUpdate(CubeStates.Control, Move);
        
            // Add action to state that is called each update
            AddOnUpdate(CubeStates.Spinning, () => transform.Rotate(1,10 * Time.deltaTime,0));
        
            // Use a Unity Event to allow adding actions from the Unity editor.
            // This action is called each time the state is entered.
            AddOnEnter(CubeStates.Fall, () => onEnterFall?.Invoke());
            AddOnExit(CubeStates.Fall, () => onExitFall?.Invoke());
        }

        protected override void StateStart() { }
        protected override void StateUpdate() { }

        // Toggle a State from Enabled to Disabled or Disabled to Enabled
        public void ToggleSpinningState() => ToggleState(CubeStates.Spinning);
        public void ToggleControlState() => ToggleState(CubeStates.Control);
        public void ToggleFallState() => ToggleState(CubeStates.Fall);
    }
}