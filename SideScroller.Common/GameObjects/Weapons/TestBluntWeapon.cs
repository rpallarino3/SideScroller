using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.GameObjects.Weapons
{
    public class TestBluntWeapon : BluntingWeapon
    {

        public TestBluntWeapon(RegionNames region, List<int> imageIndexes, Layer layer, Vector2 anchorPoint) :
            base(region, imageIndexes, Layer.MidMidground, anchorPoint, 10, 3, new Vector2(300, 200))
        {
            FillBoxOffsets();
        }

        private void FillBoxOffsets()
        {
        }
    }
}
