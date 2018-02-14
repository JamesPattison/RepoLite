using System;
using System.Collections.Generic;

namespace RepoLite.Commands
{
    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(object data)
        {
            Data = data;
        }

        public object Data { get; }
    }

    public enum CommandType
    {
        /// <summary>
        /// The event manager can expect this event to be fired only once.
        /// As such it should hold onto any firing and notify any late listeners.
        /// </summary>
        Single,

        /// <summary>
        /// A standard style .NET event.
        /// </summary>
        Standard
    }

    public class CommandManager
    {
        #region [Singleton Logic]

        private static volatile CommandManager _instance;
        private static readonly object SyncRoot = new object();

        private CommandManager() { }

        private static CommandManager Instance
        {
            get
            {
                if (_instance != null)
                    return _instance;

                lock (SyncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new CommandManager();
                    }
                }

                return _instance;
            }
        }

        #endregion

        private readonly IDictionary<string, object> _firedSingleEvents
            = new Dictionary<string, object>();
        private readonly IDictionary<string, EventHandler<CommandEventArgs>> _handlerMap
            = new Dictionary<string, EventHandler<CommandEventArgs>>();

        public static void FireCommand(string eventId, object commandData, CommandType commandType)
        {
            if (commandType == CommandType.Single)
            {
                // TODO: this could be in twice should we update the
                // eventdata?
                lock (Instance._firedSingleEvents)
                {
                    Instance._firedSingleEvents[eventId] = commandData;
                }
            }

            EventHandler<CommandEventArgs> handler;
            lock (Instance._handlerMap)
            {
                Instance._handlerMap.TryGetValue(eventId, out handler);
            }
            if (handler == null)
                return;

            var args = new CommandEventArgs(commandData);
            handler(Instance, args);
        }

        public static void SubscribeCommand(string eventId, EventHandler<CommandEventArgs> handler)
        {
            bool fireNow;
            object data;
            lock (Instance._firedSingleEvents)
            {
                fireNow = Instance._firedSingleEvents.TryGetValue(eventId, out data);
            }
            if (fireNow)
            {
                handler(Instance, new CommandEventArgs(data));
            }
            else
            {
                lock (Instance._handlerMap)
                {
                    if (!Instance._handlerMap.ContainsKey(eventId))
                    {
                        Instance._handlerMap[eventId] = handler;
                    }
                    else
                    {
                        Instance._handlerMap[eventId] += handler;
                    }
                }
            }
        }

        public static void UnsubscribeCommand(string eventId, EventHandler<CommandEventArgs> handler)
        {
            lock (Instance._handlerMap)
            {
                if (!Instance._handlerMap.TryGetValue(eventId, out var existingHandler))
                    return;

                // ReSharper disable once DelegateSubtraction
                existingHandler -= handler;
                if (existingHandler == null)
                {
                    Instance._handlerMap.Remove(eventId);
                }
                else
                {
                    Instance._handlerMap[eventId] = existingHandler;
                }
            }
        }
    }
}
