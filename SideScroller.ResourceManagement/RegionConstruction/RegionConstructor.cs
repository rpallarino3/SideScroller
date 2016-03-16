using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SideScroller.Common.Enumerations;
using SideScroller.Common.GameObjects;
using SideScroller.ResourceManagement.RegionConstruction.RegionFactories;

namespace SideScroller.ResourceManagement.RegionConstruction
{
    public class RegionConstructor
    {

        private Dictionary<RegionNames, RegionFactory> _regionFactories;

        public RegionConstructor()
        {
            _regionFactories = new Dictionary<RegionNames, RegionFactory>();
            _regionFactories.Add(RegionNames.Test1, new Test1RegionFactory());
            _regionFactories.Add(RegionNames.Test2, new Test2RegionFactory());
            _regionFactories.Add(RegionNames.Test3, new Test3RegionFactory());
        }

        public void ConstructRegion(RegionNames regionName, List<GameObject> gameObjects, List<GameObject> cameraCollisionObjects,
            List<GameObject> backgroundGameObjects, List<GameObject> foregroundGameObjects)
        {
            _regionFactories[regionName].ConstructRegion(gameObjects, cameraCollisionObjects, backgroundGameObjects, foregroundGameObjects);
        }
    }
}
