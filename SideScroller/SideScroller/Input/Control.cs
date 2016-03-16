using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SideScroller.Common.Enumerations;
using Microsoft.Xna.Framework.Input;

namespace SideScroller.Input
{
    public class Control
    {
        private bool _functionReady;
        private bool _released;
        private List<Keys> _keys;
        private List<Buttons> _buttons;
        private bool _releaseBeforeRepress;

        public Control(List<Keys> keys, List<Buttons> buttons, bool releaseBeforeRepress)
        {
            _functionReady = false;
            _released = false;
            _keys = keys;
            _buttons = buttons;
            _releaseBeforeRepress = releaseBeforeRepress;
        }

        public void UpdateKeyBinding(List<Keys> newKeys)
        {
            _keys = newKeys;
        }

        public void UpdateButtonBinding(List<Buttons> newButtons)
        {
            _buttons = newButtons;
        }

        public void UpdateReady(GamePadState gamePadState, KeyboardState keyboardState)
        {
            bool allButtonsDown = true;
            bool allKeysDown = true;

            foreach (Buttons b in _buttons)
            {
                if (!gamePadState.IsButtonDown(b))
                {
                    allButtonsDown = false;
                    break;
                }
            }

            foreach (Keys k in _keys)
            {
                if (!keyboardState.IsKeyDown(k))
                {
                    allKeysDown = false;
                    break;
                }
            }

            if (!allKeysDown && !allButtonsDown)
            {
                _functionReady = false;
                _released = true;
            }
            else
            {
                if (_releaseBeforeRepress)
                {
                    if (_released)
                    {
                        _released = false;
                        _functionReady = true;
                    }
                    else
                    {
                        _functionReady = false;
                    }
                }
                else
                {
                    _functionReady = true;
                }
                _released = false;
            }
        }

        public void ClearFunction()
        {
            _functionReady = false;
        }

        public bool FunctionReady
        {
            get { return _functionReady; }
        }
    }
}
