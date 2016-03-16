using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;
using SideScroller.Common.HelperClasses;

namespace SideScroller.Common.GameObjects
{
    public abstract class TerrainObject : GameObject
    {
        protected float _friction;

        public TerrainObject(RegionNames region, List<int> imageIndex, Layer layer, Vector2 anchorPoint, float friction)
            : base(region, imageIndex, layer, anchorPoint, 0)
        {
            _maskType = GameConstants.TERRAIN_COLLISION_MASK;
            _friction = friction;
        }

        public float Friction
        {
            get { return _friction; }
        }
    }
}
