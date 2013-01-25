using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using XTetris.Engine;
using XTetris.GameStates;

namespace XTetris
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

        public static bool Debug = true;

        public static readonly Color IShapeColor = new Color(0, 255, 255);
        public static readonly Color JShapeColor = new Color(0, 0, 255);
        public static readonly Color LShapeColor = new Color(255, 170, 0);
        public static readonly Color OShapeColor = new Color(255, 255, 0);
        public static readonly Color SShapeColor = new Color(0, 255, 0);
        public static readonly Color TShapeColor = new Color(153, 0, 255);
        public static readonly Color ZShapeColor = new Color(255, 0, 0);

        private readonly GraphicsDeviceManager _graphics;
        private readonly GameStateManager _stateManager;

        public SpriteBatch SpriteBatch { get; private set; }

        public Rectangle ScreenRectangle { get; private set; }

        // Game states
        public GamePlayState GamePlayState { get; private set; }
        public GameOverState GameOverState { get; private set; }

        public GameStateManager StateManager
        {
            get { return _stateManager; }
        }

        public TetrisGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            _graphics.PreferredBackBufferWidth = ScreenWidth;
            _graphics.PreferredBackBufferHeight = ScreenHeight;

            Content.RootDirectory = "Content";

            IsMouseVisible = true;

            Window.Title = "Tetris";

            Components.Add(new InputHandler(this));

            _stateManager = new GameStateManager(this);
            Components.Add(StateManager);

            GamePlayState = new GamePlayState(this, StateManager);
            GameOverState = new GameOverState(this, StateManager);

            StateManager.ChangeState(GamePlayState);
            //StateManager.ChangeState(GameOverState);
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
            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);
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
