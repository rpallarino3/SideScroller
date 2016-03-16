using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.GameObjects;
using SideScroller.Common.GameObjects.Phantom;
using SideScroller.Common.Enumerations;

namespace SideScroller.ResourceManagement.RegionConstruction.RegionFactories
{
    public class Test3RegionFactory : RegionFactory
    {

        public override void ConstructRegion(List<GameObject> gameObjects, List<GameObject> cameraCollisionObjects,
            List<GameObject> backgroundGameObjects, List<GameObject> foregroundGameObjects)
        {
            var bgImageList = new List<int>() { 1 };
            var bgImage = new BackgroundImageObject(RegionNames.Test3, bgImageList, Layer.BackBackground, new Vector2(0, 0), new Vector2(1500, 2000));
            backgroundGameObjects.Add(bgImage);
        }
    }
}
