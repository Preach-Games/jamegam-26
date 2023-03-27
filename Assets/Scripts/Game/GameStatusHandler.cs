using System;
using System.ComponentModel;
using UnityEngine;
using Object = System.Object;

namespace DungeonDraws.Game
{
    public sealed class GameStatusHandler
    {
        private static GameStatusHandler instance = null;
        private static readonly object padlock = new object();

        public event EventHandler OnLoading;

        public event EventHandler OnPaused;

        public event EventHandler OnDayStart;

        public event EventHandler OnDayReset;

        public void Load(Object sender, EventArgs e)
        {
            OnLoading?.Invoke(sender, e);
        }
        
        public void Pause(Object sender, EventArgs e)
        {
            OnPaused?.Invoke(sender, e);
        }

        public void DayStart(Object sender, EventArgs e)
        {
            OnDayStart?.Invoke(sender, e);
        }

        public void DayReset(Object sender, EventArgs e)
        {
            OnDayReset?.Invoke(sender, e);
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