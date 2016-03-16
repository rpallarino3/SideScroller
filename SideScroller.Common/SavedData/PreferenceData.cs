using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using SideScroller.Common.Enumerations;

namespace SideScroller.Common.SavedData
{
    public class PreferenceData
    {
        public PreferenceData()
        {
            ChangedKeyFunctions = new Dictionary<ControlFunctions, List<Keys>>();
            ChangedButtonFunctions = new Dictionary<ControlFunctions, List<Buttons>>();
        }

        public Dictionary<ControlFunctions, List<Keys>> ChangedKeyFunctions { get; set; }
        public Dictionary<ControlFunctions, List<Buttons>> ChangedButtonFunctions { get; set; }
    }
}
