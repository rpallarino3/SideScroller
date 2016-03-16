using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Animations;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.GameObjects.Characters
{
    public class Character : StandardGameObject
    {
        protected Directions _facingDirection;

        public Character(RegionNames region, List<int> imageIndex, Layer layer, Vector2 anchorPoint, float gravitationalConstant)
            : base(region, imageIndex, layer, anchorPoint, gravitationalConstant)
        {
            _facingDirection = Directions.Left;
        }

        public void UpdateFacingDirectionBasedOnVelocity()
        {
            if (_velocityPerFrame.X > 0)
            {
                _facingDirection = Directions.Right;
            }
            else if (_velocityPerFrame.X < 0)
            {
                _facingDirection = Directions.Left;
            }
        }

        public Directions FacingDirection
        {
            get { return _facingDirection; }
            set { _facingDirection = value; }
        }
    }
}
