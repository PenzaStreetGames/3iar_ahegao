using Db;
using UnityEngine;

public enum SceneType {
    Menu = 0,
    Level = 1
}

public class GameController : MonoBehaviour {
    public LevelController levelController;
    public MenuController menuController;
    public SceneType sceneType = SceneType.Level;

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start() {
        LevelRepository.InitDb();
        SaveRepository.InitDb();
    }

    // Update is called once per frame
    void Update() {
    }
}
