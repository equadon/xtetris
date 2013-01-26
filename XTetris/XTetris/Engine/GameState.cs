using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Valekhz.Tetris.Engine
{
    public abstract partial class GameState : DrawableGameComponent
    {
        #region Properties

        public List<GameComponent> Components { get; protected set; }
        public GameState Tag { get; protected set; }
        public GameStateManager StateManager { get; private set; }

        #endregion

        public GameState(Game game, GameStateManager manager)
            : base(game)
        {
            StateManager = manager;

            Components = new List<GameComponent>();
            Tag = this;
        }

        #region XNA Methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (GameComponent component in Components)
                if (component.Enabled)
                    component.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            DrawableGameComponent drawComponent;

            foreach (GameComponent component in Components)
            {
                if (component is DrawableGameComponent)
                {
                    drawComponent = component as DrawableGameComponent;

                    if (drawComponent.Visible)
                        drawComponent.Draw(gameTime);
                }
            }

            base.Draw(gameTime);
        }

        #endregion

        #region GameState Methods

        protected internal virtual void StateChange(object sender, EventArgs e)
        {
            if (StateManager.CurrentState == Tag)
                Show();
            else
                Hide();
        }

        protected virtual void Show()
        {
            Visible = true;
            Enabled = true;

            foreach (GameComponent component in Components)
            {
                component.Enabled = true;
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent) component).Visible = true;
            }
        }

        protected virtual void Hide()
        {
            Visible = false;
            Enabled = false;

            foreach (GameComponent component in Components)
            {
                component.Enabled = false;
                if (component is DrawableGameComponent)
                    ((DrawableGameComponent) component).Visible = false;
            }
        }

        #endregion
    }
}
