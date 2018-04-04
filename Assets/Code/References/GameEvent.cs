using UnityEngine;
using System.Collections.Generic;

namespace Assets.Code.References
{

    [CreateAssetMenu(fileName = "Event.asset", menuName = "Status 92/Game Event", order = 3)]
    public class GameEvent : ScriptableObject
    {

        private List<GameEventListener> listeners = new List<GameEventListener>();

        public Object DefaultParameter;

        public void Raise()
        {
            for (int i = listeners.Count - 1; i >= 0; --i)
            {
                listeners[i].OnEventRaised();
            }
        }

        public void RegisterListener(GameEventListener listener)
        {
            listeners.Add(listener);
        }

        public void UnregisterListener(GameEventListener listener)
        {
            listeners.Remove(listener);
        }

    }
}
