using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.HelperClasses
{
    public static class RegionLayout
    {
        private static Dictionary<RegionNames, Vector2> _regionSizes;
        private static Dictionary<RegionNames, Dictionary<RegionNames, Vector2>> _regionsToLoadWithOffsets;
        private static Dictionary<RegionNames, Vector2> _backgroundSizes;

        static RegionLayout()
        {
            _regionsToLoadWithOffsets = new Dictionary<RegionNames, Dictionary<RegionNames, Vector2>>();
            _regionSizes = new Dictionary<RegionNames, Vector2>();
            _backgroundSizes = new Dictionary<RegionNames, Vector2>();

            _regionSizes.Add(RegionNames.Unknown, new Vector2(0, 0));
            _regionsToLoadWithOffsets.Add(RegionNames.Unknown, new Dictionary<RegionNames, Vector2>());
            _backgroundSizes.Add(RegionNames.Unknown, new Vector2(0, 0));

            #region Test1
            _regionSizes.Add(RegionNames.Test1, new Vector2(2000, 2000));
            _backgroundSizes.Add(RegionNames.Test1, new Vector2(1280, 720));
            var regionTest1Dictionary = new Dictionary<RegionNames, Vector2>();
            regionTest1Dictionary.Add(RegionNames.Test1, new Vector2(0, 0));
            regionTest1Dictionary.Add(RegionNames.Test2, new Vector2(2000, 0));

            _regionsToLoadWithOffsets.Add(RegionNames.Test1, regionTest1Dictionary);
            #endregion

            #region Test2
            _regionSizes.Add(RegionNames.Test2, new Vector2(4000, 2000));
            _backgroundSizes.Add(RegionNames.Test2, new Vector2(1500, 1000));
            var regionTest2Dictionary = new Dictionary<RegionNames, Vector2>();
            regionTest2Dictionary.Add(RegionNames.Test2, new Vector2(0, 0));
            regionTest2Dictionary.Add(RegionNames.Test1, new Vector2(-2000, 0));
            regionTest2Dictionary.Add(RegionNames.Test3, new Vector2(4000, -2000));

            _regionsToLoadWithOffsets.Add(RegionNames.Test2, regionTest2Dictionary);
            #endregion

            #region Test3
            _regionSizes.Add(RegionNames.Test3, new Vector2(3000, 4000));
            _backgroundSizes.Add(RegionNames.Test3, new Vector2(1500, 2000));
            var regionTest3Dictionary = new Dictionary<RegionNames, Vector2>();
            regionTest3Dictionary.Add(RegionNames.Test3, new Vector2(0, 0));
            regionTest3Dictionary.Add(RegionNames.Test2, new Vector2(-4000, 2000));

            _regionsToLoadWithOffsets.Add(RegionNames.Test3, regionTest3Dictionary);
            #endregion
        }

        public static Dictionary<RegionNames, Vector2> RegionSizes
        {
            get { return _regionSizes; }
        }

        public static Dictionary<RegionNames, Dictionary<RegionNames, Vector2>> RegionsToLoadWithOffsets
        {
            get { return _regionsToLoadWithOffsets; }
        }

        public static Dictionary<RegionNames, Vector2> BackgroundSizes
        {
            get { return _backgroundSizes; }
        }
    }
}
