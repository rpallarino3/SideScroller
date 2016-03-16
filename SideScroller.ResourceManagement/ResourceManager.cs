using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SideScroller.Common;
using SideScroller.Common.Enumerations;
using SideScroller.Common.SavedData;
using SideScroller.Common.GameObjects;

namespace SideScroller.ResourceManagement
{
    public class ResourceManager
    {
        private IServiceProvider _serviceProvider;
        private string _rootDirectory;

        private List<Texture2D> _playerTextures;
        private ContentManager _staticContentManager;
        private Dictionary<RegionNames, ContentManager> _regionContentManagers;
        private ContentManager _characterContentManager;
        private Dictionary<int, Texture2D> _characterTextures;
        private Dictionary<RegionNames, Dictionary<int, Texture2D>> _regionTextures;
        private Dictionary<RegionNames, List<GameObject>> _regionGameObjects;
        private Dictionary<RegionNames, List<GameObject>> _cameraCollisionObjects;
        private Dictionary<RegionNames, List<GameObject>> _bgGameObjects;
        private Dictionary<RegionNames, List<GameObject>> _fgGameObjects;
        private Dictionary<WeaponNames, Texture2D> _weaponTextures;

        private Queue<LoadMessage> _assetLoaderQueue;
        private AssetLoader _assetLoader;

        private Loading _loading;

        private MenuResourceManager _menuResourceManager;

        public ResourceManager(IServiceProvider serviceProvider, string rootDirectory)
        {
            _serviceProvider = serviceProvider;
            _rootDirectory = rootDirectory;

            _playerTextures = new List<Texture2D>();
            _staticContentManager = new ContentManager(serviceProvider, rootDirectory);
            _characterContentManager = new ContentManager(serviceProvider, rootDirectory);
            _characterTextures = new Dictionary<int, Texture2D>();
            _regionContentManagers = new Dictionary<RegionNames, ContentManager>();
            _regionTextures = new Dictionary<RegionNames, Dictionary<int, Texture2D>>();
            _regionGameObjects = new Dictionary<RegionNames, List<GameObject>>();
            _cameraCollisionObjects = new Dictionary<RegionNames, List<GameObject>>();
            _bgGameObjects = new Dictionary<RegionNames, List<GameObject>>();
            _fgGameObjects = new Dictionary<RegionNames, List<GameObject>>();
            _weaponTextures = new Dictionary<WeaponNames, Texture2D>();

            _menuResourceManager = new MenuResourceManager(_staticContentManager);

            _loading = new Loading();

            _assetLoaderQueue = new Queue<LoadMessage>();
            _assetLoader = new AssetLoader(_assetLoaderQueue, _regionContentManagers, _characterContentManager, _characterTextures, 
                _regionTextures, _regionGameObjects, _cameraCollisionObjects, _bgGameObjects, _fgGameObjects, _serviceProvider, _rootDirectory, _loading);

            var t = new Thread(() => _assetLoader.WaitForLoad());
            t.Start();
        }

        public void LoadAssets(LoadMessage message)
        {
            lock (_assetLoaderQueue)
            {
                Console.WriteLine("Putting load message onto the queue...");
                _loading.DoneLoading = false;
                _assetLoaderQueue.Enqueue(message);
            }
        }

        public void LoadPreferenceData()
        {
            _assetLoader.LoadPreferenceData();
        }

        public void LoadStaticContent()
        {
            _menuResourceManager.LoadContent();
            _playerTextures.Add(_staticContentManager.Load<Texture2D>("CharacterAnimations/PlayerCharacter/PlayerCharacterAtlas1"));
            _weaponTextures.Add(WeaponNames.TestPierce, _staticContentManager.Load<Texture2D>("WeaponTextures/TestPierceTexture"));
            _weaponTextures.Add(WeaponNames.TestBlunt, _staticContentManager.Load<Texture2D>("WeaponTextures/TestBluntTexture"));
            _weaponTextures.Add(WeaponNames.TestSlash, _staticContentManager.Load<Texture2D>("WeaponTextures/TestSlashTexture"));
        }

        public void UnloadAllContent()
        {
            _staticContentManager.Unload();
            _characterContentManager.Unload();

            foreach (var cm in _regionContentManagers.Values)
            {
                cm.Unload();
            }

            lock (_assetLoaderQueue)
            {
                _assetLoaderQueue.Clear();
            }

            _assetLoader.Shutdown();
        }

        public List<GameObject> GetAllObjectsFromCompoundDictionary(Dictionary<RegionNames, List<GameObject>> gameObjects)
        {
            var masterList = new List<GameObject>();

            foreach (var list in gameObjects.Values)
            {
                foreach (var item in list)
                {
                    masterList.Add(item);
                }
            }

            return masterList;
        }

        public MenuResourceManager MenuResourceManager
        {
            get { return _menuResourceManager; }
        }

        public Loading Loading
        {
            get { return _loading; }
        }

        public PreferenceData LastUsedPreferenceData
        {
            get { return _assetLoader.LastUsedPreferenceData; }
        }

        public SaveData LastUsedSaveData
        {
            get { return _assetLoader.LastUsedSaveData; }
        }

        public List<Texture2D> PlayerTextures
        {
            get { return _playerTextures; }
        }

        public Dictionary<RegionNames, Dictionary<int, Texture2D>> RegionTextures
        {
            get { return _regionTextures; }
        }

        public Dictionary<int, Texture2D> CharacterTextures
        {
            get { return _characterTextures; }
        }

        public Dictionary<RegionNames, List<GameObject>> RegionGameObjects
        {
            get { return _regionGameObjects; }
        }

        public Dictionary<RegionNames, List<GameObject>> CameraCollisionObjects
        {
            get { return _cameraCollisionObjects; }
        }

        public Dictionary<RegionNames, List<GameObject>> BgGameObjects
        {
            get { return _bgGameObjects; }
        }

        public Dictionary<RegionNames, List<GameObject>> FgGameObjects
        {
            get { return _fgGameObjects; }
        }

        public Dictionary<WeaponNames, Texture2D> WeaponTextures
        {
            get { return _weaponTextures; }
        }
    }
}
