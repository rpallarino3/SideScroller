using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;

namespace SideScroller.Logic
{
    public class Fader
    {
        private readonly Color WHITE_COLOR = Color.White;
        private readonly Color DIM_COLOR = new Color(100, 100, 100);

        private List<Color> _fadeColors;

        private Color _drawColor;

        private bool _fadeOut;
        private bool _doneFading;
        private bool _fading;
        private int _fadeCounter;
        private bool _switchState;
        private bool _queuedFadeIn;

        private GameStates _queuedUpState;

        public Fader()
        {
            _fadeCounter = 0;
            _drawColor = WHITE_COLOR;

            _fadeColors = new List<Color>();
            _fadeColors.Add(Color.White);
            _fadeColors.Add(new Color(225, 225, 225));
            _fadeColors.Add(new Color(225, 225, 225));
            _fadeColors.Add(new Color(200, 200, 200));
            _fadeColors.Add(new Color(200, 200, 200));
            _fadeColors.Add(new Color(175, 175, 175));
            _fadeColors.Add(new Color(175, 175, 175));
            _fadeColors.Add(new Color(150, 150, 150));
            _fadeColors.Add(new Color(150, 150, 150));
            _fadeColors.Add(new Color(125, 125, 125));
            _fadeColors.Add(new Color(125, 125, 125));
            _fadeColors.Add(new Color(100, 100, 100));
            _fadeColors.Add(new Color(100, 100, 100));
            _fadeColors.Add(new Color(75, 75, 75));
            _fadeColors.Add(new Color(75, 75, 75));
            _fadeColors.Add(new Color(50, 50, 50));
            _fadeColors.Add(new Color(50, 50, 50));
            _fadeColors.Add(new Color(25, 25, 25));
            _fadeColors.Add(new Color(25, 25, 25));
            _fadeColors.Add(Color.Black);
        }

        public void ContinueFade(ref GameStates gameState)
        {
            if (_fadeOut)
            {
                if (_fadeCounter == _fadeColors.Count - 1)
                {
                    _fading = false;

                    if (_switchState)
                    {
                        gameState = _queuedUpState;
                    }
                }
                else
                {
                    _fadeCounter++;
                    _drawColor = _fadeColors[_fadeCounter];
                }
            }
            else
            {
                if (_fadeCounter == 0)
                {
                    _fading = false;
                }
                else
                {
                    _fadeCounter--;
                    _drawColor = _fadeColors[_fadeCounter];
                }
            }
        }

        public void FadeOut()
        {
            _queuedFadeIn = true;
            _switchState = false;
            _fading = true;
            _fadeOut = true;
            _fadeCounter = 0;
        }

        public void FadeOut(GameStates gameState)
        {
            _queuedFadeIn = true; // is this always true?
            _switchState = true;
            _queuedUpState = gameState;
            _fading = true;
            _fadeOut = true;
            _fadeCounter = 0;
        }

        public void FadeIn()
        {
            _queuedFadeIn = false;
            _fading = true;
            _fadeOut = false;
            _fadeCounter = _fadeColors.Count - 1;
        }

        public void Dim()
        {
            _drawColor = DIM_COLOR;
        }

        public void Brighten()
        {
            _drawColor = WHITE_COLOR;
        }

        public Color DrawColor
        {
            get { return _drawColor; }
        }

        public bool Fading
        {
            get { return _fading; }
            set { _fading = value; }
        }

        public bool FadingOut
        {
            get { return _fadeOut; }
        }

        public bool QueuedFadeIn
        {
            get { return _queuedFadeIn; }
        }
    }
}
