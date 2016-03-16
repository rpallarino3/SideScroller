using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;
using SideScroller.Common.HelperClasses;

namespace SideScroller.Common.GameObjects.Phantom
{
    public class CameraCollisionObject : PhantomGameObject
    {
        public CameraCollisionObject(RegionNames region, List<int> imageIndex, Layer layer, Vector2 anchorPoint, float gravitationalConstant) 
            : base(region, imageIndex, layer, anchorPoint, gravitationalConstant)
        {
            _mask = GameConstants.PHANTOM_COLLISION_MASK;
        }
    }
}
