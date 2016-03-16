using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SideScroller.ResourceManagement;
using SideScroller.Input;
using SideScroller.Common.Enumerations;

namespace SideScroller.Logic
{
    public class MenuLogicHandler
    {
        private readonly int FRAME_INPUT_BUFFER = 10;

        private ResourceManager _resourceManager;
        private InputHandler _inputHandler;
        private Fader _fader;

        private int _frameCounter;

        public MenuLogicHandler(ResourceManager resourceManager, InputHandler inputHandler, Fader fader)
        {
            _resourceManager = resourceManager;
            _inputHandler = inputHandler;
            _fader = fader;

            _frameCounter = 0;
        }

        public void UpdatePauseMenuLogic(ref GameStates gameState, ref bool paused)
        {
            // might want to change the frame counter for this
            if (_frameCounter == 0)
            {
                if (_inputHandler.IsFunctionReady(ControlFunctions.Pause))
                {
                    _fader.FadeOut(GameStates.Roam);
                }
            }
            else
            {
                _frameCounter--;
            }
        }

        public void UpdateExitMenuLogic(ref GameStates gameState, ref bool paused)
        {
            if (_frameCounter == 0)
            {
                if (_inputHandler.IsFunctionReady(ControlFunctions.ExitMenu))
                {
                    _fader.Brighten();
                    gameState = GameStates.Roam;
                    paused = false;
                }
            }
            else
            {
                _frameCounter--;
            }
        }

        public void EnterMenu()
        {
            _frameCounter = FRAME_INPUT_BUFFER;
        }
    }
}
