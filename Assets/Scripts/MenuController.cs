using System;
using UnityEngine;

public class MenuController : MonoBehaviour {
    public GameController gameController;
    public MenuUiController menuUiController;

    public void Awake() {
        GameObject main = GameObject.Find("Main");
        gameController = main.GetComponent<GameController>();
        gameController.menuController = this;
    }

    public void Start() {

    }

    public void Update() {

    }
}
