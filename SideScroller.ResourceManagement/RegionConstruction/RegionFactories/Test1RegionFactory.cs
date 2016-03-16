using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.GameObjects;
using SideScroller.Common.GameObjects.Phantom;
using SideScroller.Common.GameObjects.Terrain;
using SideScroller.Common.Enumerations;
using SideScroller.Common.CollisionBoxes;
using SideScroller.Common.Animations;
using SideScroller.Common.HelperClasses;

namespace SideScroller.ResourceManagement.RegionConstruction.RegionFactories
{
    public class Test1RegionFactory : RegionFactory
    {

        public override void ConstructRegion(List<GameObject> gameObjects, List<GameObject> cameraCollisionObjects,
            List<GameObject> backgroundGameObjects, List<GameObject> foregroundGameObjects)
        {
            var terrainFloorListLeft = new List<int>() { 0 };
            var floorTerrainLeft = new NormalStationaryTerrainObject(RegionNames.Test1, terrainFloorListLeft, Layer.BackMidground,
                new Vector2(0, 1900), new Vector2(1000, 100));
            floorTerrainLeft.CollisionBoxes.Add(new RectangleCollisionBox(floorTerrainLeft, new Vector2(0, 0), 1000, 100,
                GameConstants.STANDARD_COLLISION_MASK + GameConstants.TERRAIN_COLLISION_MASK));
            var terrainFloorListRight = new List<int>() { 1 };
            var floorTerrainRight = new NormalStationaryTerrainObject(RegionNames.Test1, terrainFloorListRight, Layer.BackMidground,
                new Vector2(1000, 1900), new Vector2(1000, 100));
            floorTerrainRight.CollisionBoxes.Add(new RectangleCollisionBox(floorTerrainRight, new Vector2(0, 0), 1000, 100,
                GameConstants.STANDARD_COLLISION_MASK + GameConstants.TERRAIN_COLLISION_MASK));

            gameObjects.Add(floorTerrainLeft);
            gameObjects.Add(floorTerrainRight);

            var camCollisionObjectImage = new List<int>() { -1 };
            var camCollisionObject1 = new CameraCollisionObject(RegionNames.Test1, camCollisionObjectImage, Layer.FrontMidground, new Vector2(0, 0), 0);
            camCollisionObject1.CollisionBoxes.Add(new RectangleCollisionBox(camCollisionObject1, new Vector2(0, 0), 640, 2000, camCollisionObject1.Mask));
            camCollisionObject1.CollisionBoxes.Add(new RectangleCollisionBox(camCollisionObject1, new Vector2(640, 0), 1600, 360, camCollisionObject1.Mask));
            camCollisionObject1.CollisionBoxes.Add(new RectangleCollisionBox(camCollisionObject1, new Vector2(640, 1640), 1600, 360, camCollisionObject1.Mask));

            cameraCollisionObjects.Add(camCollisionObject1);

            var bgImageList = new List<int>() { 2 };
            var bgImage = new BackgroundImageObject(RegionNames.Test1, bgImageList, Layer.BackBackground, new Vector2(0, 0), new Vector2(1280, 720));
            backgroundGameObjects.Add(bgImage);
        }
    }
}
