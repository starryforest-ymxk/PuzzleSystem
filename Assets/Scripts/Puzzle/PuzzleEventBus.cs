using System;
using System.Collections.Generic;
using UnityEngine;

namespace PuzzleSystem.Puzzle
{
    public class PuzzleEventBus : MonoBehaviour
    {
        private readonly Dictionary<string, Action<object>> listeners = new Dictionary<string, Action<object>>();
        private event Action<string, object> globalListener;

        public void Register(string eventKey, Action<object> callback)
        {
            if (string.IsNullOrEmpty(eventKey) || callback == null)
            {
                return;
            }

            if (listeners.TryGetValue(eventKey, out var existing))
            {
                existing += callback;
                listeners[eventKey] = existing;
            }
            else
            {
                listeners[eventKey] = callback;
            }
        }

        public void Unregister(string eventKey, Action<object> callback)
        {
            if (string.IsNullOrEmpty(eventKey) || callback == null)
            {
                return;
            }

            if (listeners.TryGetValue(eventKey, out var existing))
            {
                existing -= callback;
                if (existing == null)
                {
                    listeners.Remove(eventKey);
                }
                else
                {
                    listeners[eventKey] = existing;
                }
            }
        }

        public void RegisterGlobal(Action<string, object> callback)
        {
            globalListener += callback;
        }

        public void UnregisterGlobal(Action<string, object> callback)
        {
            globalListener -= callback;
        }

        public void Raise(string eventKey, object payload = null)
        {
            if (listeners.TryGetValue(eventKey, out var handler))
            {
                handler?.Invoke(payload);
            }

            globalListener?.Invoke(eventKey, payload);
        }
    }
}
