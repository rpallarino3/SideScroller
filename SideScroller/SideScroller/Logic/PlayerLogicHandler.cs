using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;
using SideScroller.Common.GameObjects;
using SideScroller.Common.GameObjects.Characters;
using SideScroller.Common.GameObjects.Phantom;
using SideScroller.Common.HelperClasses;
using SideScroller.ResourceManagement;
using SideScroller.Input;
using SideScroller.Graphics;

namespace SideScroller.Logic
{
    public class PlayerLogicHandler
    {
        private ResourceManager _resourceManager;
        private InputHandler _inputHandler;
        private Camera _camera;
        private PlayerCharacter _player;
        private Fader _fader;

        public PlayerLogicHandler(ResourceManager resourceManager, InputHandler inputHandler, Camera camera, PlayerCharacter player, Fader fader)
        {
            _resourceManager = resourceManager;
            _inputHandler = inputHandler;
            _camera = camera;
            _player = player;
            _fader = fader;
        }

        public void UpdatePlayerLogic(ref GameStates gameState, ref bool paused)
        {
            var controlFunctions = Enum.GetValues(typeof(ControlFunctions)).Cast<ControlFunctions>().OrderByDescending(x => x).ToList();
            _player.ResetReboundDistance();
            _player.ResetBools();
            _player.Animator.AdvanceAnimationReplay();

            for (int i = 0; i < controlFunctions.Count; i++)
            {
                var controlFunction = controlFunctions[i];

                if (!_inputHandler.IsFunctionReady(controlFunction))
                    continue;

                if (!InputValidator.IsInputValidForState(controlFunction, _player.State))
                    continue;

                ExecuteAction(controlFunction, ref gameState, ref paused);

                if (paused)
                    break;
            }

            _player.CheckContinueAttack();

            _player.ApplyGravity(GameConstants.GRAVITATIONAL_PULL);
            //Console.Write("Accel: " + _player.AccelerationPerFrame);

            _player.UpdateFacingDirection();

            _player.UpdateVelocity();
            //Console.Write(" Velocity: " + _player.VelocityPerFrame);

            _player.UpdatePosition();
            //Console.WriteLine(" Position: " + _player.Position);

            if (InputValidator.IsInputValidForState(ControlFunctions.ContinueJump, _player.State))
                _player.CheckContinueJump(_inputHandler.IsFunctionReady(ControlFunctions.ContinueJump));

            // check collisions down here

            var collisionCheckCount = 0;

            while (collisionCheckCount < GameConstants.MAX_COLLISION_TRIES)
            {
                collisionCheckCount++;

                var gameObjects = _resourceManager.GetAllObjectsFromCompoundDictionary(_resourceManager.RegionGameObjects);

                foreach (var go in gameObjects)
                {
                    var collided = _player.Collide(RegionLayout.RegionsToLoadWithOffsets[_player.Region][_player.Region], go,
                        RegionLayout.RegionsToLoadWithOffsets[_player.Region][go.Region]);

                    // do we do anything next or just let the player object handle it?
                }

                if (!_player.Rebounded)
                {
                    break;
                }
            }

            if (_player.State == CharacterStates.Attack)
            {
                // check the collision of the boxes on the weapon
                // damage stuff and stop attack if necessary
            }

            // then update the player animation
            // the updating of the player animation should be done entirely within the player object
            // leave logic compartmentalized within the object
        }

        private void ExecuteAction(ControlFunctions controlFunction, ref GameStates gameState, ref bool paused)
        {
            switch (controlFunction)
            {
                case ControlFunctions.ExitMenu:
                    ExitMenuFunction(ref gameState, ref paused);
                    break;
                case ControlFunctions.Pause:
                    PauseMenuFunction(ref gameState, ref paused);
                    break;
                case ControlFunctions.SpecialAttack:
                    SpecialAttackFunction();
                    break;
                case ControlFunctions.Attack:
                    AttackFunction();
                    break;
                case ControlFunctions.Interact:
                    InteractFunction();
                    break;
                case ControlFunctions.ContinueJump:
                    ContinueJumpFunction();
                    break;
                case ControlFunctions.Jump:
                    JumpFunction();
                    break;
                case ControlFunctions.Dash:
                    DashFunction();
                    break;
                case ControlFunctions.MoveLeft:
                case ControlFunctions.MoveRight:
                case ControlFunctions.Crouch:
                    MoveFunction(controlFunction);
                    break;
                case ControlFunctions.Switch:
                    SwitchFunction();
                    break;
                default:
                    Console.WriteLine("Not sure how we got to an invalid control function.");
                    break;
            }
        }

        private void ExitMenuFunction(ref GameStates gameState, ref bool paused)
        {
            _fader.Dim();
            gameState = GameStates.ExitMenu;
            paused = true;
        }

        private void PauseMenuFunction(ref GameStates gameState, ref bool paused)
        {
            _fader.FadeOut(GameStates.Menu);
            paused = true;
        }

        private void SpecialAttackFunction()
        {
            // check to see if cd is ready on weapon
            // if ready, begin to execute special attack and don't do anything else?
            // if not ready, do nothing and continue
        }

        private void AttackFunction()
        {
            _player.ExecuteAttack();
        }

        private void InteractFunction()
        {
            // need to go through all phantom objects and maybe some standard objects to see if we can interact
            // if we find one, begin interact and don't do anything else?
            // if one is not found, do nothing and continue
        }

        private void ContinueJumpFunction()
        {
            _player.ContinueJump();
        }

        private void JumpFunction()
        {
            _player.ExecuteJump();
        }

        private void DashFunction()
        {
            // should apply a velocity vector in the direction the player is facing
            // or just make velocity go to a certain value in the direction the player is facing
            _player.ExecuteDash();
        }

        private void MoveFunction(ControlFunctions function)
        {
            Directions dir;

            if (function == ControlFunctions.MoveLeft)
                dir = Directions.Left;
            else if (function == ControlFunctions.MoveRight)
                dir = Directions.Right;
            else
                dir = Directions.Down;

            _player.ExecuteMove(dir);
        }

        private void SwitchFunction()
        {
        }
    }
}
