using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace SideScroller.ResourceManagement
{
    public class MenuResourceManager
    {
        private ContentManager _contentManager;

        private Texture2D _inGameExitMenuBackground;

        public MenuResourceManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }
        
        public void LoadContent()
        {
            _inGameExitMenuBackground = _contentManager.Load<Texture2D>("MenuTextures/ExitMenuBackground");
        }

        public Texture2D InGameExitMenuBackground
        {
            get { return _inGameExitMenuBackground; }
        }
    }
}
