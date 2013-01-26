using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Valekhz.ScreenManagement;
using Valekhz.Tetris.Screens;

namespace Valekhz.Tetris
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TetrisGame : Microsoft.Xna.Framework.Game
    {
        public const int BlockSize = 30;

        public const int BoardPaddingTop = 0;
        public const int BoardPaddingSide = 15;

        public const int BlocksWide = 10;
        public const int BlocksHigh = 22;

        public const int ScreenWidth = 620 + BoardPaddingSide * 2;
        public const int ScreenHeight = 690 + BoardPaddingSide * 2;

        public static bool Debug = false;

        public static readonly Color ShapeIColor = new Color(0, 255, 255);
        public static readonly Color ShapeJColor = new Color(0, 0, 255);
        public static readonly Color ShapeLColor = new Color(255, 170, 0);
        public static readonly Color ShapeOColor = new Color(255, 255, 0);
        public static readonly Color ShapeSColor = new Color(0, 255, 0);
        public static readonly Color ShapeTColor = new Color(153, 0, 255);
        public static readonly Color ShapeZColor = new Color(255, 0, 0);

        private readonly GraphicsDeviceManager _graphics;

        public Rectangle ScreenRectangle { get; private set; }

        // Screen Management
        private readonly TetrisScreenManager _screenManager;
        private readonly ScreenFactory _screenFactory;

        public TetrisGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;

            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            Window.Title = "Tetris";

            _screenFactory = new ScreenFactory();
            Services.AddService(typeof(IScreenFactory), _screenFactory);

            _screenManager = new TetrisScreenManager(Services);
            Components.Add(_screenManager);

            // Add initial screens
            _screenManager.AddScreen(new BackgroundScreen(), null);
            _screenManager.AddScreen(new MainMenuScreen(), null);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            ScreenRectangle = new Rectangle(0, 0, ScreenWidth, ScreenHeight);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.BurlyWood);

            base.Draw(gameTime);
        }
    }
}
