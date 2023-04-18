using System;
using UnityEngine;

namespace Level.EventQueue {
    public abstract class LevelEvent : IGameEvent {
        public float QueuingTime;
        public float ReleaseTime;

        public void Enqueue(float delay) {
            QueuingTime = Time.time;
            ReleaseTime = QueuingTime + delay;
        }

        public bool CanReleased() {
            if (ReleaseTime == 0f)
                return false;
            return Time.time >= ReleaseTime;
        }

        public void Release() {
            throw new NotImplementedException();
        }

        public new GameEventType GetType() {
            throw new NotImplementedException();
        }
    }
}
