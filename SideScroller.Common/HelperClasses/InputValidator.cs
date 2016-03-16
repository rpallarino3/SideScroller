using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.HelperClasses
{
    public static class InputValidator
    {
        private static Dictionary<ControlFunctions, List<CharacterStates>> _validInputStates;

        static InputValidator()
        {
            _validInputStates = new Dictionary<ControlFunctions, List<CharacterStates>>();

            _validInputStates.Add(ControlFunctions.ExitMenu, new List<CharacterStates>() { CharacterStates.Attack, CharacterStates.Cast,
                CharacterStates.Dash, CharacterStates.Jump, CharacterStates.Push, CharacterStates.Recoil, CharacterStates.Stand, CharacterStates.Walk });
            _validInputStates.Add(ControlFunctions.Pause, new List<CharacterStates>() { CharacterStates.Attack, CharacterStates.Cast,
                CharacterStates.Dash, CharacterStates.Jump, CharacterStates.Push, CharacterStates.Recoil, CharacterStates.Stand, CharacterStates.Walk });
            _validInputStates.Add(ControlFunctions.SpecialAttack, new List<CharacterStates>() { CharacterStates.Dash,
                CharacterStates.Jump, CharacterStates.Push, CharacterStates.Stand, CharacterStates.Walk });
            _validInputStates.Add(ControlFunctions.Attack, new List<CharacterStates>() { CharacterStates.Stand, CharacterStates.Walk,
                CharacterStates.Jump, CharacterStates.Dash, CharacterStates.Push });
            _validInputStates.Add(ControlFunctions.Interact, new List<CharacterStates>() { CharacterStates.Cast, CharacterStates.Dash,
                CharacterStates.Jump, CharacterStates.Push, CharacterStates.Stand, CharacterStates.Walk });
            _validInputStates.Add(ControlFunctions.ContinueJump, new List<CharacterStates>() { CharacterStates.Jump });
            _validInputStates.Add(ControlFunctions.Jump, new List<CharacterStates>() { CharacterStates.Stand, CharacterStates.Walk, CharacterStates.Dash,
                CharacterStates.Push });
            _validInputStates.Add(ControlFunctions.Dash, new List<CharacterStates>() { CharacterStates.Stand, CharacterStates.Walk, CharacterStates.Push });
            _validInputStates.Add(ControlFunctions.MoveLeft, new List<CharacterStates>() { CharacterStates.Stand, CharacterStates.Walk, CharacterStates.Push,
                CharacterStates.Jump });
            _validInputStates.Add(ControlFunctions.MoveRight, new List<CharacterStates>() { CharacterStates.Stand, CharacterStates.Walk, CharacterStates.Push,
                CharacterStates.Jump });
            _validInputStates.Add(ControlFunctions.Switch, new List<CharacterStates>() { CharacterStates.Stand, CharacterStates.Walk, CharacterStates.Push });
            _validInputStates.Add(ControlFunctions.Crouch, new List<CharacterStates>() { CharacterStates.Stand, CharacterStates.Walk, CharacterStates.Push,
                CharacterStates.Jump });
        }

        public static bool IsInputValidForState(ControlFunctions function, CharacterStates state)
        {
            return _validInputStates[function].Contains(state);
        }
    }
}
