using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SideScroller.ResourceManagement
{
    public class Loading
    {
        public object Sync = new object();

        public volatile bool _doneLoading = true;

        public bool DoneLoading
        {
            get { return _doneLoading; }
            set { _doneLoading = value; }
        }
    }
}
