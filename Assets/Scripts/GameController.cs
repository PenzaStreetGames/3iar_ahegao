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
    }

    // Update is called once per frame
    void Update() {
    }

    public void QuitLevel() {
        Debug.Log("Level exit button was pressed");
    }

    public void StartLevel(int levelNumber) {
        Debug.Log("Start level button was pressed");
    }

    public void SoundToggle() {
        levelController.fieldController.soundSource.volume =
            levelController.fieldController.soundSource.volume != 0 ? 0 : (float)0.1;
    }

    public void MusicToggle() {
        levelController.fieldController.musicSource.volume =
            levelController.fieldController.musicSource.volume != 0 ? 0 : (float)0.1;
    }

}
