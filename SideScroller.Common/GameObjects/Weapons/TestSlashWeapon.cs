using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.GameObjects.Weapons
{
    public class TestSlashWeapon : SlashingWeapon
    {

        public TestSlashWeapon(RegionNames region, List<int> imageIndexes, Layer layer, Vector2 anchorPoint) :
            base(region, imageIndexes, Layer.MidMidground, anchorPoint, 5, 1, new Vector2(150, 150))
        {
            FillBoxOffsets();
        }

        private void FillBoxOffsets()
        {
        }
    }
}
