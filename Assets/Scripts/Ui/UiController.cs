using System;
using UnityEngine;

namespace UI {
    public class UiController : MonoBehaviour {
        public void Start() {

        }

        public void Update() {

        }

        public void HandleUiEvent(UiEventType uiEventType) {
            Debug.Log(uiEventType);
        }
    }
}
