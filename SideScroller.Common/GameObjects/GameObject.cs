using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.CollisionBoxes;
using SideScroller.Common.Animations;
using SideScroller.Common.Enumerations;
using SideScroller.Common.HelperClasses;

namespace SideScroller.Common.GameObjects
{
    public abstract class GameObject
    {
        protected RegionNames _region;

        protected List<CollisionBox> _collisionBoxes;

        protected Vector2 _position;
        protected Vector2 _velocityPerFrame;
        protected Vector2 _accelerationPerFrame;

        protected List<int> _imageIndexes;
        protected Layer _layer;

        protected Vector2 _anchorPoint;

        protected float _gravitationalConstant;

        protected Animator _animator;

        protected bool _rebounded;

        protected int _maskType;
        protected int _mask;

        public GameObject(RegionNames region, List<int> imageIndexes, Layer layer, Vector2 anchorPoint, float gravitationalConstant)
        {
            _region = region;
            _imageIndexes = imageIndexes;
            _layer = layer;
            _gravitationalConstant = gravitationalConstant;
            _anchorPoint = anchorPoint;
            _position = anchorPoint; // is this right?
            _collisionBoxes = new List<CollisionBox>(); // we might not want to do this here

            _velocityPerFrame = new Vector2(0, 0);
            _accelerationPerFrame = new Vector2(0, 0);
        }

        #region Methods

        #region Collision

        public virtual bool Collide(Vector2 currentObjectOffset, GameObject otherObject, Vector2 otherObjectOffset)
        {
            _rebounded = false;
            foreach (var box in _collisionBoxes)
            {
                foreach (var otherBox in otherObject.CollisionBoxes)
                {
                    if (box.Collide(currentObjectOffset, otherBox, otherObjectOffset))
                    {
                        this.HandleCollisionWithObject(currentObjectOffset, box, otherObject, otherObjectOffset, otherBox);
                        otherObject.HandleCollisionWithObject(otherObjectOffset, otherBox, this, currentObjectOffset, box);
                        return true;
                    }
                }
            }
            return false;
        }

        // collide with a rebound in a specific direction
        public virtual bool Collide(Vector2 currentObjectOffset, GameObject otherObject, Vector2 otherObjectOffset, Directions dir)
        {
            _rebounded = false;
            foreach (var box in _collisionBoxes)
            {
                foreach (var otherBox in otherObject.CollisionBoxes)
                {
                    if (box.Collide(currentObjectOffset, otherBox, otherObjectOffset))
                    {
                        this.HandleCollisionWithObject(currentObjectOffset, box, otherObject, otherObjectOffset, otherBox, dir);
                        otherObject.HandleCollisionWithObject(otherObjectOffset, otherBox, this, currentObjectOffset, box, dir);
                        return true;
                    }
                }
            }
            return false;
        }

        public virtual void HandleCollisionWithObject(Vector2 currentObjectOffset, CollisionBox collidingBox, GameObject otherObject,
            Vector2 otherObjectOffset, CollisionBox otherCollidingBox)
        {
        }


        public virtual void HandleCollisionWithObject(Vector2 currentObjectOffset, CollisionBox collidingBox, GameObject otherObject,
            Vector2 otherObjectOffset, CollisionBox otherCollidingBox, Directions dir)
        {
        }

        #endregion

        #region Physics

        public void UpdatePosition()
        {
            _position += _velocityPerFrame;
        }

        public virtual void DecayXVelocity(float multiplier)
        {
            if (Math.Abs(_velocityPerFrame.X * multiplier) < 0.1)
            {
                _velocityPerFrame.X = 0;
            }
            else
            {
                _velocityPerFrame.X = _velocityPerFrame.X * multiplier;
            }
        }

        public virtual void UpdateVelocity()
        {
            if (Math.Abs(_velocityPerFrame.X + _accelerationPerFrame.X) <= GameConstants.MAX_RUN_VELOCITY_IN_X)
            {
                _velocityPerFrame.X += _accelerationPerFrame.X;
            }
            
            if (Math.Abs(_velocityPerFrame.Y + _accelerationPerFrame.Y) <= GameConstants.MAX_RUN_VELOCITY_IN_Y)
            {
                _velocityPerFrame.Y += _accelerationPerFrame.Y;
            }

            // so after we update the velocity, we want to set the acceleration to 0
            _accelerationPerFrame = new Vector2(0, 0);
        }

        public void ApplyVelocityVector(Vector2 velocity)
        {
            _velocityPerFrame += velocity;
        }

        public void ApplyAccelerationVector(Vector2 acceleration)
        {
            _accelerationPerFrame += acceleration;
        }

        public void ApplyGravity(Vector2 acceleration)
        {
            _accelerationPerFrame += acceleration * _gravitationalConstant;
        }

        #endregion

        #endregion

        #region Properties

        public int Mask
        {
            get { return _mask; }
            set { _mask = value; }
        }

        public int MaskType
        {
            get { return _maskType; }
        }

        public List<CollisionBox> CollisionBoxes
        {
            get { return _collisionBoxes; }
        }

        public RegionNames Region
        {
            get { return _region; }
            set { _region = value; }
        }

        public Layer Layer
        {
            get { return _layer; }
        }

        public List<int> ImageIndexes
        {
            get { return _imageIndexes; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Vector2 VelocityPerFrame
        {
            get { return _velocityPerFrame; }
        }

        public Vector2 AccelerationPerFrame
        {
            get { return _accelerationPerFrame; }
        }

        public Animator Animator
        {
            get { return _animator; }
        }

        public bool Rebounded
        {
            get { return _rebounded; }
        }

        #endregion
    }
}
