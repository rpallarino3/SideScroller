using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using SideScroller.Common.Enumerations;
using SideScroller.Common.Animations;

namespace SideScroller.Common.GameObjects.Phantom
{
    public class BackgroundImageObject : PhantomGameObject
    {

        public BackgroundImageObject(RegionNames region, List<int> imageIndex, Layer layer, Vector2 anchorPoint, Vector2 size)
            : base(region, imageIndex, layer, anchorPoint, 0)
        {
            var animationDictionary = new Dictionary<int, Animation>();
            animationDictionary.Add(0, new Animation(0, 1, 1, size));

            _animator = new Animator(animationDictionary, 0);
        }
    }
}
