using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;

using Valekhz.ScreenManagement;

namespace Valekhz.Tetris.Engine
{
    class TetrisScreenManager : ScreenManager
    {
        const string SettingsFilename = "Settings.xml";

        /// <summary>
        /// Only needed if this ScreenManager is loaded with the game.
        /// </summary>
        public TetrisGame Game { get; set; }

        public SpriteFont DebugFont { get; private set; }

        public Settings Settings { get; private set; }

        public TetrisScreenManager(IServiceProvider serviceProvider)
            : base(serviceProvider)
        {
        }

        protected override void LoadContent()
        {
            Font = Content.Load<SpriteFont>(@"Fonts\Menu");
            DebugFont = Content.Load<SpriteFont>(@"Fonts\Debug");
            BlankTexture = Content.Load<Texture2D>(@"Textures\blank");

            Settings = LoadSettings();

            base.LoadContent();
        }

        private StorageContainer GetContainer()
        {
            IAsyncResult result = StorageDevice.BeginShowSelector(PlayerIndex.One, null, null);

            result.AsyncWaitHandle.WaitOne();

            StorageDevice device = StorageDevice.EndShowSelector(result);

            result = device.BeginOpenContainer("SaveSettings", null, null);
            result.AsyncWaitHandle.WaitOne();

            StorageContainer container = device.EndOpenContainer(result);

            result.AsyncWaitHandle.Close();

            return container;
        }

        public Settings LoadSettings()
        {
            StorageContainer container = GetContainer();

            if (!container.FileExists(SettingsFilename))
            {
                container.Dispose();
                return new Settings();
            }

            Stream stream = container.OpenFile(SettingsFilename, FileMode.Open);
            XmlSerializer serializer = new XmlSerializer(typeof(Settings));

            Settings settings = (Settings) serializer.Deserialize(stream);

            stream.Close();
            container.Dispose();

            return settings;
        }

        public void SaveSettings()
        {
            StorageContainer container = GetContainer();

            if (container.FileExists(SettingsFilename))
                container.DeleteFile(SettingsFilename);

            Stream stream = container.CreateFile(SettingsFilename);

            XmlSerializer serializer = new XmlSerializer(typeof(Settings));
            serializer.Serialize(stream, Settings);

            stream.Close();
            container.Dispose();
        }
    }
}
