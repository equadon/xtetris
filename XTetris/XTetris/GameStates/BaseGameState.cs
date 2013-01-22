using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using XTetris.Engine;

namespace XTetris.GameStates
{
    public abstract partial class BaseGameState : GameState
    {
        #region Properties

        protected PlayerIndex PlayerIndexInControl { get; set; }

        protected BaseGameState TransitionTo { get; set; }

        protected bool Transitioning { get; set; }

        protected ChangeType ChangeType { get; set; }

        protected TimeSpan TransitionTimer { get; set; }
        protected TimeSpan TransitionInterval { get; set; }

        #endregion

        public BaseGameState(Game game, GameStateManager manager)
            : base(game, manager)
        {
            PlayerIndexInControl = PlayerIndex.One;
            TransitionInterval = TimeSpan.FromSeconds(0.5);
        }

        #region XNA Method Region

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            if (Transitioning)
            {
                TransitionTimer += gameTime.ElapsedGameTime;

                if (TransitionTimer >= TransitionInterval)
                {
                    Transitioning = false;
                    switch (ChangeType)
                    {
                        case ChangeType.Change:
                            StateManager.ChangeState(TransitionTo);
                            break;
                        case ChangeType.Pop:
                            StateManager.PopState();
                            break;
                        case ChangeType.Push:
                            StateManager.PushState(TransitionTo);
                            break;
                    }
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }

        #endregion

        public virtual void Transition(ChangeType change, BaseGameState gameState)
        {
            Transitioning = true;
            ChangeType = change;
            TransitionTo = gameState;
            TransitionTimer = TimeSpan.Zero;
        }
    }
}
