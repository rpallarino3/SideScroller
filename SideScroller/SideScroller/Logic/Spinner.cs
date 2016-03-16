using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Animations;
using SideScroller.Common.Enumerations;
using SideScroller.ResourceManagement;

namespace SideScroller.Logic
{
    public class Spinner
    {
        private readonly int SPIN_MIN_LENGTH = 20;

        private ResourceManager _resourceManager;
        private Fader _fader;

        private bool _spinning;
        private Animator _animator;
        private GameStates _queuedUpState;

        private bool _fullScreenSpin;
        private RegionNames _regionImageToShow;

        private int _spinCounter;

        public Spinner(ResourceManager resourceManager, Fader fader)
        {
            _resourceManager = resourceManager;
            _fader = fader;

            var animationDictionary = new Dictionary<int, Animation>();
            animationDictionary.Add(0, new Animation(0, 6, 3, new Vector2(60, 60)));
            _animator = new Animator(animationDictionary, 0);

            _regionImageToShow = RegionNames.Unknown;
            _spinCounter = 0;
        }

        public void BeginSpin(GameStates spinEndState)
        {
            _spinning = true;
            _queuedUpState = spinEndState;
            _fullScreenSpin = false;
            _spinCounter = 0;
        }

        public void BeginSpin(GameStates spinEndState, RegionNames region)
        {
            _spinning = true;
            _queuedUpState = spinEndState;
            _fullScreenSpin = true;
            _spinCounter = 0;
            _regionImageToShow = region;
        }

        public void ContinueSpin(ref GameStates gameState)
        {
            if (_fullScreenSpin)
            {
                if (_resourceManager.Loading._doneLoading && _spinCounter >= SPIN_MIN_LENGTH)
                {
                    gameState = _queuedUpState;
                    //_fader.FadeIn();
                    _spinning = false;
                }
                else
                {
                    _spinCounter++;
                    _animator.AdvanceAnimationReplay();
                }
            }
            else
            {
                if (_resourceManager.Loading._doneLoading)
                {
                    gameState = _queuedUpState;
                    //_fader.FadeIn();
                    _spinning = false;
                }
                else
                {
                    _spinCounter++;
                    _animator.AdvanceAnimationReplay();
                }
            }
        }

        public bool Spinning
        {
            get { return _spinning; }
        }

        public Animator Animator
        {
            get { return _animator; }
        }

        public bool FullScreenSpin
        {
            get { return _fullScreenSpin; }
        }

        public RegionNames RegionImageToShow
        {
            get { return _regionImageToShow; }
        }
    }
}
