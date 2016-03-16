using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.GameObjects;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.CollisionBoxes
{
    public class RectangleCollisionBox : CollisionBox
    {
        private float _width;
        private float _height;

        public RectangleCollisionBox(GameObject anchorObject, Vector2 anchorPoint, float width, float height, int mask)
            : base(anchorObject, anchorPoint, mask)
        {
            _width = width;
            _height = height;
        }

        public override bool Collide(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset)
        {
            // might run into the +1 problem again so chekc here to make sure
            if (!CheckCollisionMasks(box)) return false;

            var boxPosition = box.AnchorPoint + box.AnchorObject.Position + otherOffset;
            var topLeftCorner = _anchorPoint + _anchorObject.Position + thisOffset;
            var topRightCorner = _anchorPoint + _anchorObject.Position + thisOffset + new Vector2(_width, 0);
            var bottomLeftCorner = _anchorPoint + _anchorObject.Position + thisOffset + new Vector2(0, _height);
            var bottomRightCorner = _anchorPoint + _anchorObject.Position + thisOffset + new Vector2(_width, _height);

            if (box is CircleCollisionBox)
            {
                var radius = ((CircleCollisionBox)box).Radius;

                if (boxPosition.Y >= topLeftCorner.Y &&
                    boxPosition.Y <= bottomLeftCorner.Y &&
                    boxPosition.X <= topRightCorner.X &&
                    boxPosition.X >= topLeftCorner.X)
                {
                    return true;
                }

                if (Vector2.Distance(topLeftCorner, boxPosition) <= radius ||
                    Vector2.Distance(topRightCorner, boxPosition) <= radius ||
                    Vector2.Distance(bottomLeftCorner, boxPosition) <= radius ||
                    Vector2.Distance(bottomRightCorner, boxPosition) <= radius)
                {
                    return true;
                }

                if (topLeftCorner.Y - boxPosition.Y <= radius && boxPosition.X >= topLeftCorner.X && boxPosition.X <= topRightCorner.X)
                {
                    return true;
                }
                if (boxPosition.Y - bottomRightCorner.Y <= radius && boxPosition.X >= topLeftCorner.X && boxPosition.X <= topRightCorner.X)
                {
                    return true;
                }
                if (topLeftCorner.X - boxPosition.X <= radius && boxPosition.Y >= topLeftCorner.Y && boxPosition.Y <= bottomRightCorner.Y)
                {
                    return true;
                }
                if (boxPosition.X - topRightCorner.X <= radius && boxPosition.Y >= topLeftCorner.Y && boxPosition.Y <= bottomRightCorner.Y)
                {
                    return true;
                }
            }
            else if (box is RectangleCollisionBox)
            {
                if ((topRightCorner.X <= (box.AnchorPoint.X + box.AnchorObject.Position.X + otherOffset.X)) ||
                    (box.AnchorPoint.X + box.AnchorObject.Position.X + otherOffset.X + ((RectangleCollisionBox)box).Width <= topLeftCorner.X) ||
                    (bottomLeftCorner.Y <= (box.AnchorPoint.Y + box.AnchorObject.Position.Y + otherOffset.Y)) ||
                    (box.AnchorPoint.Y + box.AnchorObject.Position.Y + otherOffset.Y + ((RectangleCollisionBox)box).Height <= topLeftCorner.Y))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public override Vector2 ReboundInShallowest(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset)
        {
            // might run into the +1 problem again so chekc here to make sure
            float xDist = 0;
            float yDist = 0;
            float reboundXDirection = 0;
            float reboundYDirection = 0;

            if (box is RectangleCollisionBox)
            {
                var currentBoxX = _anchorPoint.X + _anchorObject.Position.X + thisOffset.X;
                var boxX = box.AnchorPoint.X + box.AnchorObject.Position.X + otherOffset.X;
                if (currentBoxX < boxX)
                {
                    // this means that other box is entirely within this box in the x direction
                    if (currentBoxX + _width > boxX + ((RectangleCollisionBox)box).Width)
                    {
                        xDist = 0;
                        reboundXDirection = 0;
                    }
                    else
                    {
                        xDist = boxX + ((RectangleCollisionBox)box).Width - currentBoxX;
                        reboundXDirection = -1;
                    }
                }
                else
                {
                    // this means that this box is entirely within the width of the other in the x direction
                    if (boxX + ((RectangleCollisionBox)box).Width > currentBoxX + _width)                    
                    {
                        xDist = 0;
                        reboundXDirection = 0;
                    }
                    else
                    {
                        xDist = boxX + ((RectangleCollisionBox)box).Width - currentBoxX;
                        reboundXDirection = -1;
                    }
                }

                var currentBoxY = _anchorPoint.Y + _anchorObject.Position.Y + thisOffset.Y;
                var boxY = box.AnchorPoint.Y + box.AnchorObject.Position.Y + otherOffset.Y;
                if (currentBoxY < boxY) // this means that current is above other
                {
                    // this means that other box is entirely within this box in the y direction
                    if (currentBoxY + _height > boxY + ((RectangleCollisionBox)box).Height)
                    
                    {
                        yDist = 0;
                        reboundYDirection = 0;
                    }
                    else
                    {
                        yDist = currentBoxY + _height - boxY;
                        reboundYDirection = -1;
                    }
                }
                else
                {
                    // this means that this box is entirely within the width of the other in the y direction
                    if (boxY + ((RectangleCollisionBox)box).Height > currentBoxY + _height)
                    {
                        yDist = 0;
                        reboundYDirection = 0;
                    }
                    else
                    {
                        yDist = boxY + ((RectangleCollisionBox)box).Height - currentBoxY;
                        reboundYDirection = -1;
                    }
                }
            }
            else if (box is CircleCollisionBox)
            {
                var currentBoxX = _anchorPoint.X + _anchorObject.Position.X + thisOffset.X;
                var currentBoxY = _anchorPoint.Y + _anchorObject.Position.Y + thisOffset.Y;
                var boxCenter = ((CircleCollisionBox)box).AnchorPoint + box.AnchorObject.Position + otherOffset;
                var boxRadius = ((CircleCollisionBox)box).Radius;

                if (boxCenter.X - boxRadius >= currentBoxX && boxCenter.X + boxRadius <= currentBoxX + _width)
                {
                    xDist = 0;
                    reboundXDirection = 0;
                }
                else if (boxCenter.X - boxRadius < currentBoxX)
                {
                    xDist = boxCenter.X + boxRadius - currentBoxX;
                    reboundXDirection = -1;
                }
                else
                {
                    xDist = currentBoxX + _width - (boxCenter.X - boxRadius);
                    reboundXDirection = 1;
                }

                if (boxCenter.Y - boxRadius >= currentBoxY && boxCenter.Y + boxRadius <= currentBoxY + _height)
                {
                    yDist = 0;
                    reboundYDirection = 0;
                }
                else if (boxCenter.Y - boxRadius < currentBoxY)
                {
                    yDist = boxCenter.Y + boxRadius - currentBoxY;
                    reboundYDirection = -1;
                }
                else
                {
                    yDist = currentBoxY + _height - (boxCenter.Y - boxRadius);
                    reboundYDirection = 1;
                }
            }

            if (xDist < yDist && xDist > 0)
            {
                return new Vector2(xDist * reboundXDirection, 0);
            }
            else
            {
                if (yDist > 0)
                {
                    return new Vector2(0, yDist * reboundYDirection);
                }
            }

            return new Vector2(0, 0);
        }

        public override Vector2 ReboundInDirection(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset, Directions dir)
        {
            var thisLocation = _anchorObject.Position + _anchorPoint + thisOffset;
            var boxLocation = box.AnchorObject.Position + box.AnchorPoint + otherOffset;

            if (box is RectangleCollisionBox)
            {
                var boxWidth = ((RectangleCollisionBox)box).Width;
                var boxHeight = ((RectangleCollisionBox)box).Height;

                if (dir == Directions.Up)
                {
                    return new Vector2(0, -(boxLocation.Y + boxHeight - thisLocation.Y));
                }
                else if (dir == Directions.Down)
                {
                    return new Vector2(0, thisLocation.Y + _height - boxLocation.Y);
                }
                else if (dir == Directions.Left)
                {
                    return new Vector2(-(boxLocation.X + boxWidth - thisLocation.X), 0);
                }
                else if (dir == Directions.Right)
                {
                    return new Vector2(thisLocation.X + _width - boxLocation.X, 0);
                }
            }
            else if (box is CircleCollisionBox)
            {
                var boxRadius = ((CircleCollisionBox)box).Radius;

                if (dir == Directions.Up)
                {
                    return new Vector2(0, -(boxLocation.Y + boxRadius - thisLocation.Y));
                }
                else if (dir == Directions.Down)
                {
                    return new Vector2(0, thisLocation.Y + _height - (boxLocation.Y - boxRadius));
                }
                else if (dir == Directions.Left)
                {
                    return new Vector2(-(boxLocation.X + boxRadius - thisLocation.X), 0);
                }
                else if (dir == Directions.Right)
                {
                    return new Vector2(thisLocation.X + _width - (boxLocation.X - boxRadius), 0);
                }
            }

            return new Vector2(0, 0);
        }

        public override List<Directions> GetCollisionDirections(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset)
        {
            var directions = new List<Directions>();

            var topLeftCorner = _anchorObject.Position + thisOffset;
            var botRightCorner = topLeftCorner + new Vector2(_width, _height);

            Vector2 otherTopLeftCorner = new Vector2(0, 0);
            Vector2 otherBottomRightCorner = new Vector2(0, 0);

            if (box is RectangleCollisionBox)
            {
                otherTopLeftCorner = box.AnchorObject.Position + otherOffset;
                otherBottomRightCorner = otherTopLeftCorner + new Vector2(((RectangleCollisionBox)box).Width, ((RectangleCollisionBox)box).Height);
            }
            else if (box is CircleCollisionBox)
            {
                otherTopLeftCorner = box.AnchorObject.Position + thisOffset - new Vector2(((CircleCollisionBox)box).Radius, ((CircleCollisionBox)box).Radius);
                otherBottomRightCorner = box.AnchorObject.Position + thisOffset - new Vector2(((CircleCollisionBox)box).Radius, ((CircleCollisionBox)box).Radius);
            }

            // the problem here arises in the fact that the player is being pulled down into the block
            // if the player isn't completely on top of the block this will result i saying that the player
            // collided with the blockf rom a horizontal directions when they actually didn't
            // i think the below velocity check will fix this

            if (_anchorObject.VelocityPerFrame.X > 0) 
            {
                if (botRightCorner.X > otherTopLeftCorner.X && topLeftCorner.X < otherTopLeftCorner.X
                    && botRightCorner.X - _anchorObject.VelocityPerFrame.X <= otherTopLeftCorner.X)
                    directions.Add(Directions.Right);
            }
            else if (_anchorObject.VelocityPerFrame.X < 0)
            {
                if (topLeftCorner.X < otherBottomRightCorner.X && botRightCorner.X > otherBottomRightCorner.X
                    && topLeftCorner.X - _anchorObject.VelocityPerFrame.X >= otherBottomRightCorner.X)
                    directions.Add(Directions.Left);
            }

            if (_anchorObject.VelocityPerFrame.Y > 0)
            {
                if (botRightCorner.Y > otherTopLeftCorner.Y && topLeftCorner.Y < otherTopLeftCorner.Y)
                    directions.Add(Directions.Down);
            }
            else if (_anchorObject.VelocityPerFrame.Y < 0)
            {
                if (topLeftCorner.Y < otherBottomRightCorner.Y && botRightCorner.Y > otherBottomRightCorner.Y)
                    directions.Add(Directions.Up);
            }

            return directions;
        }

        public float Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public float Height
        {
            get { return _height; }
            set { _height = value; }
        }
    }
}
