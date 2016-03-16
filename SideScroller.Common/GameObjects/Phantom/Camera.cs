using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;
using SideScroller.Common.CollisionBoxes;
using SideScroller.Common.HelperClasses;

namespace SideScroller.Common.GameObjects.Phantom
{
    public class Camera : PhantomGameObject
    {
        private readonly int PAN_FRAMES = 60;

        private Vector2 _panLocation;
        private bool _panning;
        private Vector2 _panDistance;

        private bool _locked;

        public Camera(RegionNames region, List<int> imageIndex, Layer layer, Vector2 anchorPoint, float gravitationalConstant)
            : base(region, imageIndex, layer, anchorPoint, gravitationalConstant)
        {
            _position = new Vector2(0, 0);
            _panLocation = new Vector2(0, 0);
            _panning = false;
            _locked = false;
            _mask = GameConstants.PHANTOM_COLLISION_MASK;

            _collisionBoxes.Add(new RectangleCollisionBox(this, new Vector2(0, 0), 1, 1, _mask));
        }

        public override bool Collide(Vector2 currentObjectOffset, GameObject otherObject, Vector2 otherObjectOffset, Directions dir)
        {
            _rebounded = false;
            foreach (var collisionBox in _collisionBoxes)
            {
                foreach (var otherCollisionBox in otherObject.CollisionBoxes)
                {
                    var collided = collisionBox.Collide(currentObjectOffset, otherCollisionBox, otherObjectOffset);

                    if (collided)
                    {
                        var offset = otherCollisionBox.ReboundInDirection(currentObjectOffset, collisionBox, otherObjectOffset, dir);
                        _position += offset;
                        _rebounded = true;
                        return true;
                    }
                }
            }
            return false;
        }

        public void MoveCamera(Vector2 distance)
        {
            if (!_locked) _position += distance;
        }

        public void Pan(Vector2 location)
        {
            _panLocation = location;
            _panning = true;
            _panDistance = (_position - _panLocation) / PAN_FRAMES;
        }

        public void ContinuePan()
        {
            if ((Math.Abs(_position.X - _panLocation.X) <= _panDistance.X) &&
                (Math.Abs(_position.Y - _panLocation.Y) <= _panDistance.Y))
            {
                _position = _panLocation;
                _panning = false;
            }
            else
            {
                _position += _panDistance;
            }
        }

        public Vector2 PanLocation
        {
            get { return _panLocation; }
        }

        public bool Panning
        {
            get { return _panning; }
        }

        public bool Locked
        {
            get { return _locked; }
        }
    }
}
