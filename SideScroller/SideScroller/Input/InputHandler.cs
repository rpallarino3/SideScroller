using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using SideScroller.Common.SavedData;
using SideScroller.Common.Enumerations;

namespace SideScroller.Input
{
    public class InputHandler
    {
        private Dictionary<ControlFunctions, Control> _controls;

        public InputHandler(PreferenceData preferenceData)
        {
            _controls = new Dictionary<ControlFunctions, Control>();
            _controls.Add(ControlFunctions.Attack, new Control(new List<Keys>() { Keys.LeftShift }, new List<Buttons>() { Buttons.B }, true));
            _controls.Add(ControlFunctions.Crouch, new Control(new List<Keys>() { Keys.S }, new List<Buttons>() { Buttons.LeftThumbstickDown }, false));
            _controls.Add(ControlFunctions.Dash, new Control(new List<Keys>() { Keys.Q }, new List<Buttons>() { Buttons.X }, true));
            _controls.Add(ControlFunctions.ExitMenu, new Control(new List<Keys>() { Keys.Escape }, new List<Buttons>() { Buttons.Back }, true));
            _controls.Add(ControlFunctions.ContinueJump, new Control(new List<Keys>() { Keys.Space }, new List<Buttons>() { Buttons.A }, false));
            _controls.Add(ControlFunctions.Jump, new Control(new List<Keys>() { Keys.Space }, new List<Buttons>() { Buttons.A }, true));
            _controls.Add(ControlFunctions.MoveLeft, new Control(new List<Keys>() { Keys.A }, new List<Buttons>() { Buttons.LeftThumbstickLeft }, false));
            _controls.Add(ControlFunctions.MoveRight, new Control(new List<Keys>() { Keys.D }, new List<Buttons>() { Buttons.LeftThumbstickRight }, false));
            _controls.Add(ControlFunctions.Pause, new Control(new List<Keys>() { Keys.Enter }, new List<Buttons>() { Buttons.Start }, true));
            _controls.Add(ControlFunctions.SpecialAttack, new Control(new List<Keys>() { Keys.W, Keys.LeftShift },
                new List<Buttons>() { Buttons.LeftThumbstickUp, Buttons.B }, false));
            _controls.Add(ControlFunctions.Interact, new Control(new List<Keys>() { Keys.W }, new List<Buttons>() { Buttons.LeftThumbstickUp }, true));
            _controls.Add(ControlFunctions.Switch, new Control(new List<Keys>() { Keys.E }, new List<Buttons>() { Buttons.Y }, true));

            foreach (KeyValuePair<ControlFunctions, List<Keys>> k in preferenceData.ChangedKeyFunctions)
            {
                _controls[k.Key].UpdateKeyBinding(k.Value);
            }

            foreach (KeyValuePair<ControlFunctions, List<Buttons>> b in preferenceData.ChangedButtonFunctions)
            {
                _controls[b.Key].UpdateButtonBinding(b.Value);
            }
        }

        public void UpdateInputs(GamePadState gamePadState, KeyboardState keyboardState)
        {
            foreach (ControlFunctions cf in _controls.Keys)
            {
                _controls[cf].UpdateReady(gamePadState, keyboardState);
            }
        }

        public bool IsFunctionReady(ControlFunctions function)
        {
            return _controls[function].FunctionReady;
        }

        public void ClearInputs()
        {
            foreach (ControlFunctions cf in _controls.Keys)
            {
                _controls[cf].ClearFunction();
            }
        }
    }
}
