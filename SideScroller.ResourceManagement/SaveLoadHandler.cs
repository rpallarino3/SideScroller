using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Storage;
using SideScroller.Common;
using SideScroller.Common.SavedData;

namespace SideScroller.ResourceManagement
{
    public class SaveLoadHandler
    {

        private IAsyncResult result;

        public void RequestSave(string fileName, SaveData data)
        {
            try
            {
                IAsyncResult device = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                device.AsyncWaitHandle.WaitOne();
                StorageDevice storageDevice = StorageDevice.EndShowSelector(device);

                if (storageDevice.IsConnected && storageDevice != null)
                {
                    IAsyncResult result = storageDevice.BeginOpenContainer("Save Game", null, null);
                    result.AsyncWaitHandle.WaitOne();
                    StorageContainer container = storageDevice.EndOpenContainer(result);
                    result.AsyncWaitHandle.Close();

                    if (container.FileExists(fileName))
                    {
                        container.DeleteFile(fileName);
                    }

                    Stream stream = container.CreateFile(fileName);
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                    serializer.Serialize(stream, data);
                    stream.Close();
                    container.Dispose();
                }
                Console.WriteLine("Save complete!");
            }
            catch
            {
                Console.WriteLine("ERROR! SAVE FAILED!!!");
            }

        }

        public SaveData RequestLoad(string fileName)
        {
            try
            {
                IAsyncResult device = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                device.AsyncWaitHandle.WaitOne();
                StorageDevice storageDevice = StorageDevice.EndShowSelector(device);

                if (storageDevice.IsConnected && storageDevice != null)
                {
                    IAsyncResult result = storageDevice.BeginOpenContainer("A Troll in the Hay Data", null, null);
                    result.AsyncWaitHandle.WaitOne();
                    StorageContainer container = storageDevice.EndOpenContainer(result);
                    result.AsyncWaitHandle.Close();

                    if (!container.FileExists(fileName))
                    {
                        container.Dispose();
                        Console.WriteLine("No save data found for file " + fileName + ". Creating new save data.");
                        return new SaveData();
                    }

                    Stream stream = container.OpenFile(fileName, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                    SaveData saveData = (SaveData)serializer.Deserialize(stream);
                    stream.Close();
                    container.Dispose();
                    Console.WriteLine("Load complete!");
                    return saveData;
                }
                return null;
            }
            catch
            {
                Console.WriteLine("ERROR! LOAD FAILED!!!");
                return null;
            }
        }

        public void SavePreferenceData(PreferenceData preferenceData)
        {
            try
            {
                IAsyncResult device = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                device.AsyncWaitHandle.WaitOne();
                StorageDevice storageDevice = StorageDevice.EndShowSelector(device);

                if (storageDevice.IsConnected && storageDevice != null)
                {
                    IAsyncResult result = storageDevice.BeginOpenContainer("A Troll in the Hay Data", null, null);
                    result.AsyncWaitHandle.WaitOne();
                    StorageContainer container = storageDevice.EndOpenContainer(result);
                    result.AsyncWaitHandle.Close();

                    if (container.FileExists(""))//come edit the file name
                    {
                        container.DeleteFile("");
                    }

                    Stream stream = container.CreateFile("");
                    XmlSerializer serializer = new XmlSerializer(typeof(PreferenceData));
                    serializer.Serialize(stream, preferenceData);
                    stream.Close();
                    container.Dispose();
                }
                Console.WriteLine("Preference data save complete!");
            }
            catch
            {
                Console.WriteLine("ERROR! PREFERENCE SAVE FAILED!!!");
            }
        }

        public PreferenceData LoadPreferenceData()
        {
            try
            {
                IAsyncResult device = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);
                device.AsyncWaitHandle.WaitOne();
                StorageDevice storageDevice = StorageDevice.EndShowSelector(device);

                if (storageDevice.IsConnected && storageDevice != null)
                {
                    IAsyncResult result = storageDevice.BeginOpenContainer("A Troll in the Hay Data", null, null);
                    result.AsyncWaitHandle.WaitOne();
                    StorageContainer container = storageDevice.EndOpenContainer(result);
                    result.AsyncWaitHandle.Close();

                    if (!container.FileExists("")) //come edit the filename
                    {
                        container.Dispose();
                        Console.WriteLine("No preference data found, creating new preference data.");
                        return new PreferenceData();
                    }

                    Stream stream = container.OpenFile("", FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(PreferenceData));
                    PreferenceData prefData = (PreferenceData)serializer.Deserialize(stream);
                    stream.Close();
                    container.Dispose();
                    Console.WriteLine("Preference load complete!");
                    return prefData;
                }
                return null;
            }
            catch
            {
                Console.WriteLine("ERROR! PREFERENCE LOAD FAILED!!!");
                return null;
            }
        }
    }
}
