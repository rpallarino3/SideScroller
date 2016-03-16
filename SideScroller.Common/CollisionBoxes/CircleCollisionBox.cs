using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.GameObjects;
using SideScroller.Common.Enumerations;
using SideScroller.Common.HelperClasses;

namespace SideScroller.Common.CollisionBoxes
{
    public class CircleCollisionBox : CollisionBox
    {
        private float _radius;

        public CircleCollisionBox(GameObject anchorObject, Vector2 anchorPoint, float radius, int mask)
            : base(anchorObject, anchorPoint, mask)
        {
            _radius = radius;
        }

        public override bool Collide(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset)
        {
            // might run into the +1 problem again so chekc here to make sure
            if (!CheckCollisionMasks(box)) return false;

            if (box is CircleCollisionBox)
            {
                var radCombine = _radius + ((CircleCollisionBox)box).Radius;
                var distanceBetweenCenters = Vector2.Distance(_anchorPoint + _anchorObject.Position + thisOffset,
                    box.AnchorPoint + box.AnchorObject.Position + otherOffset);

                if (distanceBetweenCenters <= radCombine)
                {
                    return true;
                }

            }
            else if (box is RectangleCollisionBox)
            {
                var width = ((RectangleCollisionBox)box).Width;
                var height = ((RectangleCollisionBox)box).Height;

                var topLeftCorner = box.AnchorPoint + box.AnchorObject.Position + otherOffset;
                var topRightCorner = box.AnchorPoint + box.AnchorObject.Position + otherOffset + new Vector2(width, 0);
                var bottomLeftCorner = box.AnchorPoint + box.AnchorObject.Position + otherOffset + new Vector2(0, height);
                var bottomRightCorner = box.AnchorPoint + box.AnchorObject.Position + otherOffset + new Vector2(width, height);

                var position = _anchorPoint + _anchorObject.Position + thisOffset;

                if (position.Y >= topLeftCorner.Y &&
                    position.Y <= bottomLeftCorner.Y &&
                    position.X <= topRightCorner.X &&
                    position.X >= topLeftCorner.X)
                {
                    return true;
                }

                if (Vector2.Distance(topLeftCorner, position) <= _radius ||
                    Vector2.Distance(topRightCorner, position) <= _radius ||
                    Vector2.Distance(bottomLeftCorner, position) <= _radius ||
                    Vector2.Distance(bottomRightCorner, position) <= _radius)
                {
                    return true;
                }

                if (topLeftCorner.Y - position.Y <= _radius && position.X >= topLeftCorner.X && position.X <= topRightCorner.X)
                {
                    return true;
                }
                if (position.Y - bottomRightCorner.Y <= _radius && position.X >= topLeftCorner.X && position.X <= topRightCorner.X)
                {
                    return true;
                }
                if (topLeftCorner.X - position.X <= _radius && position.Y >= topLeftCorner.Y && position.Y <= bottomRightCorner.Y)
                {
                    return true;
                }
                if (position.X - topRightCorner.X <= _radius && position.Y >= topLeftCorner.Y && position.Y <= bottomRightCorner.Y)
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

            if (box is CircleCollisionBox)
            {
                var currentCenter = _anchorPoint + _anchorObject.Position + thisOffset;
                var boxCenter = box.AnchorPoint + box.AnchorObject.Position + otherOffset;
                var boxRadius = ((CircleCollisionBox)box).Radius;
                var distance = Vector2.Distance(currentCenter, boxCenter);

                // other circle is complete inside this one
                if (distance + boxRadius > 0 && distance < _radius)
                {
                    // not sure what to do here
                }
                else if (distance + _radius > 0 && distance < boxRadius) // this is completely inside other?
                {
                    // not sure what to do here either
                }

                if (currentCenter.X <= boxCenter.X)
                {
                    xDist = currentCenter.X + _radius - (boxCenter.X - boxRadius);
                    reboundXDirection = -1;
                }
                else
                {
                    xDist = boxCenter.X + boxRadius - (currentCenter.X - _radius);
                    reboundXDirection = 1;
                }

                if (currentCenter.Y <= boxCenter.Y)
                {
                    yDist = currentCenter.Y + _radius - (boxCenter.Y - boxRadius);
                    reboundYDirection = 1;
                }
                else
                {
                    yDist = boxCenter.Y + boxRadius - (currentCenter.Y - _radius);
                    reboundYDirection = -1;
                }
            }
            else if (box is RectangleCollisionBox)
            {
                var currentCenter = _anchorPoint + _anchorObject.Position + thisOffset;
                var boxLocation = box.AnchorPoint + box.AnchorObject.Position + otherOffset;
                var boxWidth = ((RectangleCollisionBox)box).Width;
                var boxHeight = ((RectangleCollisionBox)box).Height;

                // this means that square is inside circle on x
                if (boxLocation.X >= currentCenter.X - _radius && boxLocation.X + boxWidth <= currentCenter.X + _radius)
                {
                    xDist = 0;
                    reboundXDirection = 0;
                }
                else if (boxLocation.X < currentCenter.X - _radius)
                {
                    xDist = boxLocation.X + boxWidth - (currentCenter.X - _radius);
                    reboundXDirection = -1;
                }
                else
                {
                    xDist = currentCenter.X + _radius - boxLocation.X;
                    reboundXDirection = 1;
                }

                if (boxLocation.Y >= currentCenter.Y - _radius && boxLocation.Y + boxHeight <= currentCenter.Y + _radius)
                {
                    yDist = 0;
                    reboundYDirection = 0;
                }
                else if (boxLocation.Y < currentCenter.Y - _radius)
                {
                    yDist = boxLocation.Y + boxHeight - (currentCenter.Y - _radius);
                    reboundYDirection = -1;
                }
                else
                {
                    yDist = currentCenter.Y + _radius - boxLocation.Y;
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
            var boxLocation = box.AnchorPoint + box.AnchorObject.Position + otherOffset;

            if (box is RectangleCollisionBox)
            {
                var boxWidth = ((RectangleCollisionBox)box).Width;
                var boxHeight = ((RectangleCollisionBox)box).Height;

                if (dir == Directions.Up)
                {
                    return new Vector2(0, -(boxLocation.Y + boxHeight - (thisLocation.Y - _radius)));
                }
                else if (dir == Directions.Down)
                {
                    return new Vector2(0, thisLocation.Y + _radius - boxLocation.Y);
                }
                else if (dir == Directions.Left)
                {
                    return new Vector2(-(boxLocation.X + boxWidth - (thisLocation.X - _radius)), 0);
                }
                else if (dir == Directions.Right)
                {
                    return new Vector2(thisLocation.X + _radius - boxLocation.X, 0);
                }
            }
            else if (box is CircleCollisionBox)
            {
                var boxRadius = ((CircleCollisionBox)box).Radius;

                if (dir == Directions.Up)
                {
                    return new Vector2(0, -(boxLocation.Y + boxRadius - (thisLocation.X - _radius)));
                }
                else if (dir == Directions.Down)
                {
                    return new Vector2(0, thisLocation.Y + _radius - (boxLocation.Y - boxRadius));
                }
                else if (dir == Directions.Left)
                {
                    return new Vector2(-(boxLocation.X + boxRadius - (thisLocation.X - _radius)), 0);
                }
                else if (dir == Directions.Right)
                {
                    return new Vector2(thisLocation.X + _radius - (boxLocation.X - boxRadius), 0);
                }
            }

            return new Vector2(0, 0);
        }

        public override List<Directions> GetCollisionDirections(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset)
        {
            var directions = new List<Directions>();

            var topLeftCorner = _anchorObject.Position + thisOffset - new Vector2(_radius, _radius);
            var botRightCorner = topLeftCorner + new Vector2(_radius, _radius);

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

            if (_anchorObject.VelocityPerFrame.X > 0)
            {
                if (botRightCorner.X > otherTopLeftCorner.X && topLeftCorner.X < otherTopLeftCorner.X)
                    directions.Add(Directions.Right);
            }
            else if (_anchorObject.VelocityPerFrame.X < 0)
            {
                if (topLeftCorner.X < otherBottomRightCorner.X && botRightCorner.X > otherBottomRightCorner.X)
                    directions.Add(Directions.Left);
            }

            if (_anchorObject.VelocityPerFrame.Y > 0)
            {
                if (botRightCorner.Y > otherTopLeftCorner.Y && topLeftCorner.Y > otherTopLeftCorner.Y)
                    directions.Add(Directions.Down);
            }
            else if (_anchorObject.VelocityPerFrame.Y < 0)
            {
                if (topLeftCorner.Y < otherBottomRightCorner.Y && botRightCorner.Y > otherBottomRightCorner.Y)
                    directions.Add(Directions.Up);
            }

            return directions;
        }

        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
    }
}
