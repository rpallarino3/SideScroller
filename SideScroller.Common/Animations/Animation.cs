using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SideScroller.Common.Animations
{
    public class Animation
    {
        private int _row;
        private int _numImages;
        private int _imageTime;
        private Vector2 _imageSize;

        public Animation(int row, int numImages, int imageTime, Vector2 imageSize)
        {
            _row = row;
            _numImages = numImages;
            _imageTime = imageTime;
            _imageSize = imageSize;
        }

        public int Row
        {
            get { return _row; }
        }

        public int NumImages
        {
            get { return _numImages; }
        }

        public int ImageTime
        {
            get { return _imageTime; }
        }

        public Vector2 ImageSize
        {
            get { return _imageSize; }
        }
    }
}
