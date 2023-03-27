using System;
using System.ComponentModel;

namespace DungeonDraws.Game
{
    public enum GameStatus
    {
        Loading,
        Paused,
        DayStart,
        DayReset
    }
    
    public sealed class GameStatusHandler
    {
        private EventHandlerList _gameStatusEvents = new EventHandlerList();
        private static GameStatusHandler instance = null;
        private static readonly object padlock = new object();
        
        public event EventHandler OnLoading
        {
            add
            {
                _gameStatusEvents.AddHandler(GameStatus.Loading, value);
            }
            remove
            {
                _gameStatusEvents.RemoveHandler(GameStatus.Loading, value);
            }
        }
        
        public event EventHandler OnPaused
        {
            add
            {
                _gameStatusEvents.AddHandler(GameStatus.Paused, value);
            }
            remove
            {
                _gameStatusEvents.RemoveHandler(GameStatus.Paused, value);
            }
        }
        
        public event EventHandler OnDayStart
        {
            add
            {
                _gameStatusEvents.AddHandler(GameStatus.DayStart, value);
            }
            remove
            {
                _gameStatusEvents.RemoveHandler(GameStatus.DayStart, value);
            }
        }
        
        public event EventHandler OnDayReset
        {
            add
            {
                _gameStatusEvents.AddHandler(GameStatus.DayReset, value);
            }
            remove
            {
                _gameStatusEvents.RemoveHandler(GameStatus.DayReset, value);
            }
        }

        public void Load(Object sender, EventArgs e)
        {
            ProcessEvent(sender, e, GameStatus.Loading);
        }
        
        public void Pause(Object sender, EventArgs e)
        {
            ProcessEvent(sender, e, GameStatus.Paused);
        }

        public void DayStart(Object sender, EventArgs e)
        {
            ProcessEvent(sender, e, GameStatus.DayStart);
        }

        public void DayReset(Object sender, EventArgs e)
        {
            ProcessEvent(sender, e, GameStatus.DayReset);
        }

        private void ProcessEvent(Object sender, EventArgs e, GameStatus gs)
        {
            EventHandler gameEventDelegate = (EventHandler)_gameStatusEvents[gs];
            gameEventDelegate(sender, e);
        }
        
        private GameStatusHandler()
        {
            //Event handler instanced
        }
        
        public static GameStatusHandler Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new GameStatusHandler();
                    }
                    return instance;
                }
            }
        }
    }
}