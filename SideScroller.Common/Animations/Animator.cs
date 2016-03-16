using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SideScroller.Common.Animations
{
    public class Animator
    {
        private Dictionary<int, Animation> _animations;
        private Animation _currentAnimation;

        private bool _animationFinished;
        private int _animtionCounter;
        private int _frameCounter;

        public Animator(Dictionary<int, Animation> animations, int startingAnimation)
        {
            _animations = animations;
            _currentAnimation = _animations[startingAnimation];
        }

        public void AdvanceAnimation()
        {
            if (_animationFinished) // maybe shouldn't put this here to be safe?
            {
                return; 
            }

            if (_frameCounter == _currentAnimation.ImageTime - 1)
            {
                if (_animtionCounter == _currentAnimation.NumImages - 1)
                {
                    _animationFinished = true;
                }
                else
                {
                    _animtionCounter++;
                    _frameCounter = 0;
                }                
            }
            else
            {
                _frameCounter++;
            }
        }

        public void AdvanceAnimationReplay()
        {
            if (_animationFinished) // maybe shouldn't put this here to be safe?
            {
                _animtionCounter = 0;
                _frameCounter = 0;
                _animationFinished = _currentAnimation.NumImages == 1 ? true : false;
            }

            if (_frameCounter == _currentAnimation.ImageTime - 1)
            {
                if (_animtionCounter == _currentAnimation.NumImages - 1)
                {
                    _animationFinished = true;
                }
                else
                {
                    _animtionCounter++;
                    _frameCounter = 0;
                }
            }
            else
            {
                _frameCounter++;
            }
        }

        public void SetNewAnimation(int animation)
        {
            _currentAnimation = _animations[animation];
            _animtionCounter = 0;
            _frameCounter = 0;
            _animationFinished = _currentAnimation.NumImages == 1 ? true : false;
        }

        public void SetAnimationIfNotCurrent(int animation)
        {
            if (_currentAnimation.Row != animation)
                SetNewAnimation(animation);
        }

        public Dictionary<int, Animation> Animations
        {
            get { return _animations; }
        }

        public Animation CurrentAnimation
        {
            get { return _currentAnimation; }
        }

        public int AnimationCounter
        {
            get { return _animtionCounter; }
        }

        public bool AnimationFinished
        {
            get { return _animationFinished; }
        }
    }
}
