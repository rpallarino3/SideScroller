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
    public class RoamLogicHandler
    {
        private ResourceManager _resourceManager;
        private InputHandler _inputHandler;
        private Camera _camera;
        private PlayerCharacter _player;
        private Fader _fader;

        private Dictionary<RegionNames, Vector2> _backgroundAnchorLocations;
        private PlayerLogicHandler _playerLogicHandler;

        public RoamLogicHandler(ResourceManager resourceManager, InputHandler inputHandler, Camera camera, PlayerCharacter player, Fader fader)
        {
            _resourceManager = resourceManager;
            _inputHandler = inputHandler;
            _camera = camera;
            _player = player;
            _fader = fader;
            _backgroundAnchorLocations = new Dictionary<RegionNames, Vector2>();
            _playerLogicHandler = new PlayerLogicHandler(resourceManager, inputHandler, camera, player, fader);
        }

        public void UpdateRoamLogic(ref GameStates gameState, ref bool paused)
        {
            _playerLogicHandler.UpdatePlayerLogic(ref gameState, ref paused);

            if (!paused)
            {
                UpdateCameraLogic();
                UpdateBackgroundLogic();
                //Console.WriteLine(_backgroundAnchorLocations[RegionNames.Test2]);
            }
        }

        private void UpdateCameraLogic()
        {
            // set camera x to player x

            if (_camera.Locked)
            {
            }
            else if (_camera.Panning)
            {
            }
            else
            {
                List<GameObject> cameraCollisionObjects = new List<GameObject>();

                foreach (var cameraCollisionList in _resourceManager.CameraCollisionObjects.Values)
                {
                    foreach (var item in cameraCollisionList)
                    {
                        cameraCollisionObjects.Add(item);
                    }
                }

                // might need to adjust the camera position based on if it is anothe region?
                // or is the camera always in the same region as the player?

                Directions dir = _camera.Position.Y > _player.Position.Y ? Directions.Down : Directions.Up;
                _camera.Position = new Vector2(_camera.Position.X, _player.Position.Y);

                foreach (var cameraCollisionObject in cameraCollisionObjects)
                {
                    if (_camera.Collide(RegionLayout.RegionsToLoadWithOffsets[_player.Region][_player.Region], cameraCollisionObject,
                        RegionLayout.RegionsToLoadWithOffsets[_player.Region][cameraCollisionObject.Region], dir))
                    {
                        break;
                    }
                }

                dir = _camera.Position.X > _player.Position.X ? Directions.Right : Directions.Left;
                _camera.Position = new Vector2(_player.Position.X, _camera.Position.Y);

                foreach (var cameraCollisionObject in cameraCollisionObjects)
                {
                    if (_camera.Collide(RegionLayout.RegionsToLoadWithOffsets[_player.Region][_player.Region], cameraCollisionObject,
                        RegionLayout.RegionsToLoadWithOffsets[_player.Region][cameraCollisionObject.Region], dir))
                    {
                        break;
                    }
                }

                dir = _camera.Position.Y > _player.Position.Y ? Directions.Down : Directions.Up;
                _camera.Position = new Vector2(_camera.Position.X, _player.Position.Y);

                foreach (var cameraCollisionObject in cameraCollisionObjects)
                {
                    if (_camera.Collide(RegionLayout.RegionsToLoadWithOffsets[_player.Region][_player.Region], cameraCollisionObject,
                        RegionLayout.RegionsToLoadWithOffsets[_player.Region][cameraCollisionObject.Region], dir))
                    {
                        break;
                    }
                }

                dir = _camera.Position.X > _player.Position.X ? Directions.Right : Directions.Left;
                _camera.Position = new Vector2(_player.Position.X, _camera.Position.Y);

                foreach (var cameraCollisionObject in cameraCollisionObjects)
                {
                    if (_camera.Collide(RegionLayout.RegionsToLoadWithOffsets[_player.Region][_player.Region], cameraCollisionObject,
                        RegionLayout.RegionsToLoadWithOffsets[_player.Region][cameraCollisionObject.Region], dir))
                    {
                        break;
                    }
                }
            }
        }

        private void UpdateBackgroundLogic()
        {
            #region Update background anchors

            _backgroundAnchorLocations.Clear();

            foreach (RegionNames r in RegionLayout.RegionsToLoadWithOffsets[_player.Region].Keys) // something is funky in here
            {
                float anchorX = 0;
                float anchorY = 0;

                var currentRegionOffset = RegionLayout.RegionsToLoadWithOffsets[_player.Region][r];
                var currentRegionSize = RegionLayout.RegionSizes[r];
                var currentRegionBGSize = RegionLayout.BackgroundSizes[r];

                //var regionAdjustedPlayerPosition = _player.Position - currentRegionOffset - RegionLayout.SCREEN_SIZE_IN_GAME_UNITS / 2;
                // the camera position is in the middle of the screen?
                // yes such that 0, 0 of screen = logic.Camera.Position - GameConstants.SCREEN_SIZE_IN_GAME_UNITS / 2 + new Vector2(0, 1);
                var regionAdjustedPlayerPosition = _camera.Position - currentRegionOffset + new Vector2(0, 1) - GameConstants.SCREEN_SIZE_IN_GAME_UNITS / 2;

                var excessSize = (currentRegionBGSize - GameConstants.SCREEN_SIZE_IN_GAME_UNITS) / 2;
                var topLeftBackgroundBound = currentRegionOffset; // i don't think this is right
                var bottomRightBackgroundBound = currentRegionOffset + currentRegionSize - currentRegionBGSize; // i don't think this is right
                var scrollStopTopLeft = topLeftBackgroundBound + excessSize * GameConstants.BG_SCROLL_RATIO;
                var scrollStopBottomRight = bottomRightBackgroundBound - excessSize * GameConstants.BG_SCROLL_RATIO;

                if (currentRegionBGSize.X < GameConstants.SCREEN_SIZE_IN_GAME_UNITS.X ||
                    currentRegionBGSize.X > currentRegionSize.X / GameConstants.BG_SCROLL_RATIO + GameConstants.SCREEN_SIZE_IN_GAME_UNITS.X)
                {
                    Console.WriteLine("Background image for region " + r.ToString() + " is too wide to scroll properly.");

                    anchorX = currentRegionOffset.X - excessSize.X;
                }
                else if (regionAdjustedPlayerPosition.X < topLeftBackgroundBound.X)
                {
                    anchorX = topLeftBackgroundBound.X;
                }
                else if (regionAdjustedPlayerPosition.X < scrollStopTopLeft.X)
                {
                    anchorX = (regionAdjustedPlayerPosition.X - topLeftBackgroundBound.X) / GameConstants.BG_SCROLL_RATIO;
                }
                else if (regionAdjustedPlayerPosition.X < scrollStopBottomRight.X)
                {
                    anchorX = regionAdjustedPlayerPosition.X - excessSize.X;
                }
                else if (regionAdjustedPlayerPosition.X < bottomRightBackgroundBound.X)
                {
                    anchorX = (regionAdjustedPlayerPosition.X - scrollStopBottomRight.X) / GameConstants.BG_SCROLL_RATIO;
                }
                else
                {
                    anchorX = bottomRightBackgroundBound.X;
                }

                if (currentRegionBGSize.Y < GameConstants.SCREEN_SIZE_IN_GAME_UNITS.Y ||
                    currentRegionBGSize.Y > currentRegionSize.Y / GameConstants.BG_SCROLL_RATIO + GameConstants.SCREEN_SIZE_IN_GAME_UNITS.Y)
                {
                    Console.WriteLine("Background image for region " + r.ToString() + " is too tall to scroll properly.");

                    anchorY = currentRegionOffset.Y - excessSize.Y;
                }
                else if (regionAdjustedPlayerPosition.Y < topLeftBackgroundBound.Y)
                {
                    anchorY = topLeftBackgroundBound.Y;
                }
                else if (regionAdjustedPlayerPosition.Y < scrollStopTopLeft.Y)
                {
                    anchorY = (regionAdjustedPlayerPosition.Y - topLeftBackgroundBound.Y) / GameConstants.BG_SCROLL_RATIO;
                }
                else if (regionAdjustedPlayerPosition.Y < scrollStopBottomRight.Y)
                {
                    anchorY = regionAdjustedPlayerPosition.Y - excessSize.Y;
                }
                else if (regionAdjustedPlayerPosition.Y < bottomRightBackgroundBound.Y)
                {
                    anchorY = (regionAdjustedPlayerPosition.Y - scrollStopBottomRight.Y) / GameConstants.BG_SCROLL_RATIO;
                }
                else
                {
                    anchorY = bottomRightBackgroundBound.Y;
                }

                _backgroundAnchorLocations.Add(r, new Vector2(anchorX, anchorY));
            }

            #endregion
        }

        public Dictionary<RegionNames, Vector2> BackgroundAnchorLocations
        {
            get { return _backgroundAnchorLocations; }
        }
    }
}
