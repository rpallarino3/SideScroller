using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using SideScroller.Logic;
using SideScroller.ResourceManagement;
using SideScroller.Common.Enumerations;
using SideScroller.Common.HelperClasses;
using SideScroller.Common.GameObjects;
using SideScroller.Common.GameObjects.Characters;

namespace SideScroller.Graphics
{
    public class GraphicsHandler
    {
        public Vector2 _screenRatio;

        public void Draw(SpriteBatch sb, LogicHandler logic, ResourceManager resourceManager, int screenWidth, int screenHeight)
        {
            var screenRatioX = ((float)screenWidth) / GameConstants.SCREEN_SIZE_IN_GAME_UNITS.X;
            var screenRatioY = ((float)screenHeight) / GameConstants.SCREEN_SIZE_IN_GAME_UNITS.Y;

            _screenRatio = new Vector2(screenRatioX, screenRatioY);

            if (logic.GameState == GameStates.StartMenu)
            {
            }
            else if (logic.GameState == GameStates.ExitMenu)
            {
                lock (resourceManager.Loading.Sync)
                {
                    DrawExitMenu(sb, resourceManager);
                    DrawRoam(sb, logic, resourceManager);
                }
            }
            else if (logic.GameState == GameStates.Roam)
            {
                lock (resourceManager.Loading.Sync)
                {
                    DrawRoam(sb, logic, resourceManager);
                }
            }
        }

        private void DrawExitMenu(SpriteBatch sb, ResourceManager resourceManager)
        {
            sb.Draw(resourceManager.MenuResourceManager.InGameExitMenuBackground,
                new Vector2(515, 160), new Rectangle(0, 0, 250, 400), Color.White, 0, new Vector2(0, 0), _screenRatio, SpriteEffects.None,
                ((float)((int)Layer.FrontForeground)) / GameConstants.NUM_LAYERS);
        }

        private void DrawRoam(SpriteBatch sb, LogicHandler logic, ResourceManager resourceManager)
        {
            var currentRegion = logic.Player.Region;

            var cameraTopLeftMidGround = logic.Camera.Position - GameConstants.SCREEN_SIZE_IN_GAME_UNITS / 2 + new Vector2(0, 1);

            #region Player
            var playerLocation = logic.Player.Position;
            var playerImageSize = logic.Player.Animator.CurrentAnimation.ImageSize;

            if (DoesShowOnScreen(cameraTopLeftMidGround, playerLocation, playerImageSize))
            {
                var playerDrawLocation = playerLocation - cameraTopLeftMidGround;
                var playerDrawRectangle = new Rectangle((int)(logic.Player.Animator.AnimationCounter * playerImageSize.X),
                    (int)(logic.Player.Animator.CurrentAnimation.Row * playerImageSize.Y),
                    (int)playerImageSize.X, (int)playerImageSize.Y);

                var playerTexture = resourceManager.PlayerTextures
                    [GetTextureID(logic.Player.ImageIndexes, playerImageSize, logic.Player.Animator.CurrentAnimation.Row)];

                sb.Draw(
                    playerTexture,
                    playerDrawLocation * _screenRatio,
                    playerDrawRectangle,
                    logic.DrawColor,
                    0,
                    new Vector2(0, 0),
                    _screenRatio,
                    SpriteEffects.None,
                    ((float)((int)logic.Player.Layer))/GameConstants.NUM_LAYERS);
            }

            if (logic.Player.EquippedWeapon != null)
            {
                var weap = logic.Player.EquippedWeapon;
                var weapImageSize = weap.Animator.CurrentAnimation.ImageSize;

                if (DoesShowOnScreen(cameraTopLeftMidGround, weap.Position, weapImageSize))
                {
                    var weapDrawLocation = weap.Position - cameraTopLeftMidGround;
                    var weapDrawRectangle = new Rectangle((int)(weap.Animator.AnimationCounter * weapImageSize.X),
                        (int)(weap.Animator.CurrentAnimation.Row * weapImageSize.Y),
                        (int)weapImageSize.X, (int)weapImageSize.Y);

                    var weaponTexture = resourceManager.WeaponTextures[logic.Player.EquippedWeapon.Name];

                    sb.Draw(weaponTexture,
                        weapDrawLocation * _screenRatio,
                        weapDrawRectangle,
                        logic.DrawColor,
                        0,
                        new Vector2(0, 0),
                        _screenRatio,
                        SpriteEffects.None,
                        ((float)((int)weap.Layer)) / GameConstants.NUM_LAYERS);
                }

            }
            #endregion

            #region Midground Objects

            foreach (var r in resourceManager.RegionGameObjects.Keys)
            {
                foreach (var midGameObject in resourceManager.RegionGameObjects[r])
                {
                    var gameObjectLocation = midGameObject.Position + RegionLayout.RegionsToLoadWithOffsets[r][midGameObject.Region];
                    var imageSize = midGameObject.Animator.CurrentAnimation.ImageSize;

                    if (DoesShowOnScreen(cameraTopLeftMidGround, gameObjectLocation, imageSize))
                    {
                        var drawLocation = gameObjectLocation - cameraTopLeftMidGround;
                        var drawRectangle = new Rectangle((int)(midGameObject.Animator.AnimationCounter * imageSize.X),
                            (int)(midGameObject.Animator.CurrentAnimation.Row * imageSize.Y),
                            (int)imageSize.X, (int)imageSize.Y);

                        Texture2D texture;

                        if (midGameObject is Character)
                        {
                            texture = resourceManager.CharacterTextures
                                [GetTextureID(midGameObject.ImageIndexes, imageSize, midGameObject.Animator.CurrentAnimation.Row)];
                        }
                        else
                        {
                            texture = resourceManager.RegionTextures[midGameObject.Region]
                                [GetTextureID(midGameObject.ImageIndexes, imageSize, midGameObject.Animator.CurrentAnimation.Row)];
                        }

                        sb.Draw(
                            texture,
                            drawLocation * _screenRatio,
                            drawRectangle,
                            logic.DrawColor,
                            0,
                            new Vector2(0, 0),
                            _screenRatio,
                            SpriteEffects.None,
                            ((float)((int)midGameObject.Layer))/GameConstants.NUM_LAYERS);
                    }
                }
            }

            #endregion

            #region Background Objects

            foreach (var r in resourceManager.BgGameObjects.Keys)
            {
                foreach (var bgGameObject in resourceManager.BgGameObjects[r])
                {
                    var gameObjectLocation = (bgGameObject.Position + logic.RoamLogicHandler.BackgroundAnchorLocations[bgGameObject.Region] +
                        RegionLayout.RegionsToLoadWithOffsets[r][bgGameObject.Region]);
                    var imageSize = bgGameObject.Animator.CurrentAnimation.ImageSize;

                    if (DoesShowOnScreen(cameraTopLeftMidGround, gameObjectLocation, imageSize))
                    {
                        var drawLocation = gameObjectLocation - cameraTopLeftMidGround;
                        var drawRectangle = new Rectangle((int)(bgGameObject.Animator.AnimationCounter * imageSize.X),
                            (int)(bgGameObject.Animator.CurrentAnimation.Row * imageSize.Y),
                            (int)imageSize.X, (int)imageSize.Y);

                        // assume character objects can never be in the background
                        var texture = resourceManager.RegionTextures[bgGameObject.Region]
                            [GetTextureID(bgGameObject.ImageIndexes, imageSize, bgGameObject.Animator.CurrentAnimation.Row)];

                        sb.Draw(
                            texture,
                            drawLocation * _screenRatio,
                            drawRectangle,
                            logic.DrawColor,
                            0f,
                            new Vector2(0, 0),
                            _screenRatio,
                            SpriteEffects.None,
                            ((float)((int)bgGameObject.Layer))/GameConstants.NUM_LAYERS);

                    }
                }
            }

            #endregion

            #region ForegroundObjects
            #endregion
        }

        private bool DoesShowOnScreen(Vector2 screenTopLeft, Vector2 objectLocation, Vector2 objectSize)
        {
            if (objectLocation.X + objectSize.X <= screenTopLeft.X ||
                screenTopLeft.X + GameConstants.SCREEN_SIZE_IN_GAME_UNITS.X <= objectLocation.X ||
                objectLocation.Y + objectSize.Y <= screenTopLeft.Y ||
                screenTopLeft.Y + GameConstants.SCREEN_SIZE_IN_GAME_UNITS.Y <= objectLocation.Y)
            {
                return false;
            }

            return true;
        }

        private int GetTextureID(List<int> indexes, Vector2 imageSize, int animationIndex)
        {
            var yValue = (animationIndex + 1) * imageSize.Y;
            var textureIndex = (int)(yValue / GameConstants.MAX_TEXTURE_SIZE.Y); // i think this should work

            return indexes[textureIndex];
        }
    }
}
