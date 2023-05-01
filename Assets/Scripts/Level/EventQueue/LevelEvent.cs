using System;
using UnityEngine;

namespace Level.EventQueue {
    public abstract class LevelEvent : IGameEvent {
        public float QueuingTime;
        public float ReleaseTime;
        public float Delay;

        public void Enqueue(float delay) {
            QueuingTime = Time.time;
            ReleaseTime = QueuingTime + delay;
            Delay = delay;
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

        public float GetDelay() {
            return Delay;
        }
    }
}
