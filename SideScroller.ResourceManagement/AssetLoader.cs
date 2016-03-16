using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SideScroller.Common;
using SideScroller.Common.Enumerations;
using SideScroller.Common.SavedData;
using SideScroller.Common.GameObjects;
using SideScroller.ResourceManagement.RegionConstruction;

namespace SideScroller.ResourceManagement
{
    public class AssetLoader
    {
        private RegionConstructor _regionConstructor;

        private Dictionary<RegionNames, ContentManager> _regionContentManagers;
        private ContentManager _characterContentManager;
        private Dictionary<int, Texture2D> _characterTextures;
        private Dictionary<RegionNames, Dictionary<int, Texture2D>> _regionTextures;
        private Dictionary<RegionNames, List<GameObject>> _regionGameObjects;
        private Dictionary<RegionNames, List<GameObject>> _cameraCollisionObjects;
        private Dictionary<RegionNames, List<GameObject>> _bgGameObjects;
        private Dictionary<RegionNames, List<GameObject>> _fgGameObjects;
        private Queue<LoadMessage> _assetLoaderQueue;
        private IServiceProvider _serviceProvider;
        private string _rootDirectory;

        private Loading _loading;
        private bool _continueLooping;

        private SaveLoadHandler _saveLoadHandler;
        private SaveData _lastUsedSaveData;

        private PreferenceData _lastUsedPreferenceData;

        public AssetLoader(Queue<LoadMessage> assetLoaderQueue, Dictionary<RegionNames, ContentManager> regionContentManagers,
            ContentManager characterContentManager, Dictionary<int, Texture2D> characterTextures,
            Dictionary<RegionNames, Dictionary<int, Texture2D>> regionTextures, Dictionary<RegionNames, List<GameObject>> regionGameObjects,
            Dictionary<RegionNames, List<GameObject>> cameraCollisionObjects, Dictionary<RegionNames, List<GameObject>> bgGameObjects,
            Dictionary<RegionNames, List<GameObject>> fgGameObjects,
            IServiceProvider serviceProvider, string rootDirectory, Loading loading)
        {
            _regionConstructor = new RegionConstructor();
            _regionContentManagers = regionContentManagers;
            _characterContentManager = characterContentManager;
            _characterTextures = characterTextures;
            _regionTextures = regionTextures;
            _regionGameObjects = regionGameObjects;
            _cameraCollisionObjects = cameraCollisionObjects;
            _bgGameObjects = bgGameObjects;
            _fgGameObjects = fgGameObjects;
            _saveLoadHandler = new SaveLoadHandler();
            _assetLoaderQueue = assetLoaderQueue;
            _serviceProvider = serviceProvider;
            _rootDirectory = rootDirectory;

            _loading = loading;
            _continueLooping = true;
        }

        public void WaitForLoad()
        {
            while (_continueLooping)
            {
                if (_assetLoaderQueue.Count > 0)
                {
                    LoadMessage assetsToLoad;
                    lock (_assetLoaderQueue)
                    {
                        assetsToLoad = _assetLoaderQueue.Dequeue();
                    }
                    _loading.DoneLoading = false;
                    ConsumeLoadMessage(assetsToLoad);
                }
            }
        }

        public void ConsumeLoadMessage(LoadMessage message)
        {
            if (message is SaveLoadMessage)
            {
            }
            else
            {
                var newContentManagers = new Dictionary<RegionNames, ContentManager>();
                var newCharacterContentManager = new ContentManager(_serviceProvider, _rootDirectory);
                var newCharacterTextures = new Dictionary<int, Texture2D>();
                var newRegionTextures = new Dictionary<RegionNames, Dictionary<int, Texture2D>>();
                var newGameObjects = new Dictionary<RegionNames, List<GameObject>>();
                var newCameraObjects = new Dictionary<RegionNames, List<GameObject>>();
                var newBgObjects = new Dictionary<RegionNames, List<GameObject>>();
                var newFgObjects = new Dictionary<RegionNames, List<GameObject>>();
                foreach (RegionNames r in ((RegionLoadMessage)message).RegionsToLoad)
                {
                    LoadRegion(r, newContentManagers, newRegionTextures, newGameObjects, newCameraObjects, newBgObjects, newFgObjects);
                }

                if (((RegionLoadMessage)message).SwapOutCharacterHandler)
                {
                    LoadCharacters(((RegionLoadMessage)message).CharacterThemeToLoad, newCharacterContentManager, newCharacterTextures);
                }

                lock (_loading.Sync)
                {
                    foreach (RegionNames r in ((RegionLoadMessage)message).RegionsToUnload)
                    {
                        _regionContentManagers[r].Unload();
                        _regionContentManagers.Remove(r);
                        _regionTextures.Remove(r);
                        _regionGameObjects.Remove(r);
                        _cameraCollisionObjects.Remove(r);
                        _bgGameObjects.Remove(r);
                        _fgGameObjects.Remove(r);

                        if (((RegionLoadMessage)message).SwapOutCharacterHandler)
                        {
                            _characterContentManager.Unload();
                        }

                    }

                    foreach (RegionNames r in newContentManagers.Keys)
                    {
                        _regionContentManagers.Add(r, newContentManagers[r]);
                        _regionTextures.Add(r, newRegionTextures[r]);
                        _regionGameObjects.Add(r, newGameObjects[r]);
                        _cameraCollisionObjects.Add(r, newCameraObjects[r]);
                        _bgGameObjects.Add(r, newBgObjects[r]);
                        _fgGameObjects.Add(r, newFgObjects[r]);
                        
                        if (((RegionLoadMessage)message).SwapOutCharacterHandler)
                        {
                            _characterContentManager = newCharacterContentManager;
                            _characterTextures = newCharacterTextures;
                        }
                    }
                }
            }
            _loading.DoneLoading = true;
        }

        public void LoadCharacters(CharacterTheme theme, ContentManager newCharacterContentManager, Dictionary<int, Texture2D> newTextures)
        {
            var themeName = Enum.GetName(typeof(CharacterTheme), theme);
            var file = "CharacterAnimations/CharacterThemeLoad/" + themeName + "ThemeCharacters";

            var fileNames = newCharacterContentManager.Load<Dictionary<int, string>>(file);

            foreach (var index in fileNames.Keys)
            {
                var texture = newCharacterContentManager.Load<Texture2D>("CharacterAnimations/" + fileNames[index]);
                newTextures.Add(index, texture);
            }
        }

        public void LoadRegion(RegionNames regionName, Dictionary<RegionNames, ContentManager> contentManagers,
            Dictionary<RegionNames, Dictionary<int, Texture2D>> regionTextures,
            Dictionary<RegionNames, List<GameObject>> regionGameObjects,
            Dictionary<RegionNames, List<GameObject>> regionCameraObjects,
            Dictionary<RegionNames, List<GameObject>> regionBackgroundObjects,
            Dictionary<RegionNames, List<GameObject>> regionForegroundObjects)
        {
            var regionContentManager = new ContentManager(_serviceProvider, _rootDirectory);
            var newRegionTextures = new Dictionary<int, Texture2D>();

            var stringRegionName = Enum.GetName(typeof(RegionNames), regionName);
            var filePrefix = "EnvironmentTextures/" + stringRegionName + "/";

            var fileNames = regionContentManager.Load<Dictionary<int, string>>(filePrefix + stringRegionName + "Names");

            foreach (var file in fileNames.Keys)
            {
                newRegionTextures.Add(file, regionContentManager.Load<Texture2D>(filePrefix + fileNames[file]));
            }

            var gameObjects = new List<GameObject>();
            var cameraObjects = new List<GameObject>();
            var bgObjects = new List<GameObject>();
            var fgObjects = new List<GameObject>();

            _regionConstructor.ConstructRegion(regionName, gameObjects, cameraObjects, bgObjects, fgObjects);

            contentManagers.Add(regionName, regionContentManager);
            regionTextures.Add(regionName, newRegionTextures);
            regionGameObjects.Add(regionName, gameObjects);
            regionCameraObjects.Add(regionName, cameraObjects);
            regionBackgroundObjects.Add(regionName, bgObjects);
            regionForegroundObjects.Add(regionName, fgObjects);
        }

        public void Shutdown()
        {
            _continueLooping = false;
        }

        public void LoadPreferenceData()
        {
            _lastUsedPreferenceData = _saveLoadHandler.LoadPreferenceData();
        }

        public SaveData LastUsedSaveData
        {
            get
            {
                if (_lastUsedSaveData != null)
                {
                    lock (_lastUsedSaveData) // not quite sure we even need to lock on this
                    {
                        return _lastUsedSaveData;
                    }
                }
                else
                {
                    _lastUsedSaveData = new SaveData();
                    return _lastUsedSaveData;
                }
            }
        }

        public PreferenceData LastUsedPreferenceData
        {
            get
            {
                if (_lastUsedSaveData != null)
                {
                    lock (_lastUsedPreferenceData)
                    {
                        return _lastUsedPreferenceData;
                    }
                }
                else
                {
                    _lastUsedPreferenceData = new PreferenceData();
                    return _lastUsedPreferenceData;
                }
            }
        }
    }
}
