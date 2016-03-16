using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.GameObjects;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.CollisionBoxes
{
    public abstract class CollisionBox
    {
        protected Vector2 _anchorPoint;
        protected int _collisionMask; // this might need to be changed from an int
        protected GameObject _anchorObject;

        protected CollisionBox(GameObject anchorObject, Vector2 anchorPoint, int collisionMask)
        {
            _anchorObject = anchorObject;
            _anchorPoint = anchorPoint;
            _collisionMask = collisionMask;
        }

        public abstract bool Collide(Vector2 thisOffset, CollisionBox otherBox, Vector2 otherOffset);
        public abstract Vector2 ReboundInShallowest(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset);
        public abstract Vector2 ReboundInDirection(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset, Directions dir);
        public abstract List<Directions> GetCollisionDirections(Vector2 thisOffset, CollisionBox box, Vector2 otherOffset);

        public bool CheckCollisionMasks(CollisionBox box)
        {
            return ((_collisionMask & box.AnchorObject.MaskType) == box.AnchorObject.MaskType)
                && ((box.CollisionMask & _anchorObject.MaskType) == _anchorObject.MaskType);
        }

        public Vector2 AnchorPoint
        {
            get { return _anchorPoint; }
            set { _anchorPoint = value; }
        }

        public int CollisionMask
        {
            get { return _collisionMask; }
            set { _collisionMask = value; }
        }

        public GameObject AnchorObject
        {
            get { return _anchorObject; }
            set { _anchorObject = value; }
        }
    }
}
