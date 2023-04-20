using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine;

namespace Level.EventQueue {
    public class LevelEventQueue : MonoBehaviour {
        public List<IGameEvent> PlannedEvents = new();
        public static LevelEventQueue Instance;

        void Start() {
            if (Instance == null) {
                Instance = this;
            }
        }

        void Update() {
            var releasedEvents = new List<IGameEvent>();
            foreach (var plannedEvent in PlannedEvents) {
                if (plannedEvent.CanReleased()) {
                    releasedEvents.Add(plannedEvent);
                }
            }
            for (int i = 0; i < releasedEvents.Count; i++) {
                var releasedEvent = releasedEvents[i];
                releasedEvent.Release();
                PlannedEvents.Remove(releasedEvent);
            }
        }

        public void Enqueue(IGameEvent levelEvent, float delay) {
            // Debug.Log($"Enqueue call {levelEvent.GetType()}");
            levelEvent.Enqueue(delay);
            PlannedEvents.Add(levelEvent);
        }

        public bool IsFieldStable() {
            foreach (var plannedEvent in PlannedEvents) {
                switch (plannedEvent.GetType()) {
                    case GameEventType.TileFalling:
                        return false;
                    case GameEventType.TileFilling:
                        return false;
                    case GameEventType.CombinationSquashing:
                        return false;
                }
            }
            return true;
        }
    }
}
