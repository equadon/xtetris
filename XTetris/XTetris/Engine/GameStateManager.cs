using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Valekhz.Tetris.Engine
{
    public enum ChangeType { Change, Pop, Push }

    public class GameStateManager : GameComponent
    {
        public event EventHandler OnStateChange;

        #region Fields & Properties

        private const int StartDrawOrder = 5000;
        private const int DrawOrderInc = 100;

        private readonly Stack<GameState> _gameStates = new Stack<GameState>();
        private int _drawOrder;

        public GameState CurrentState
        {
            get { return _gameStates.Peek(); }
        }

        #endregion

        public GameStateManager(Game game)
            : base(game)
        {
            _drawOrder = StartDrawOrder;
        }

        #region XNA Methods

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        #endregion

        #region GameState Methods

        public void PopState()
        {
            if (_gameStates.Count > 0)
            {
                RemoveState();
                _drawOrder -= DrawOrderInc;

                if (OnStateChange != null)
                    OnStateChange(this, null);
            }
        }

        private void RemoveState()
        {
            GameState state = _gameStates.Peek();

            OnStateChange -= state.StateChange;
            Game.Components.Remove(state);
            _gameStates.Pop();
        }

        public void PushState(GameState newState)
        {
            _drawOrder += DrawOrderInc;
            newState.DrawOrder = _drawOrder;

            AddState(newState);

            if (OnStateChange != null)
                OnStateChange(this, null);
        }

        private void AddState(GameState newState)
        {
            _gameStates.Push(newState);

            Game.Components.Add(newState);

            OnStateChange += newState.StateChange;
        }

        public void ChangeState(GameState newState)
        {
            while (_gameStates.Count > 0)
                RemoveState();

            newState.DrawOrder = StartDrawOrder;
            _drawOrder = StartDrawOrder;

            AddState(newState);

            if (OnStateChange != null)
                OnStateChange(this, null);
        }

        #endregion
    }
}
