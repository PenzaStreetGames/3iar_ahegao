using Db;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public enum SceneType {
    Menu = 0,
    Level = 1
}

public class GameController : MonoBehaviour {
    public LevelController levelController;
    public MenuController menuController;
    public SceneType sceneType = SceneType.Level;
    public int levelNumber;
    public int levelsCompleted;
    public int totalLevels;
    public static GameController instance;

    void Awake() {
        if (instance == null) {
            instance = this;
        }
        else {
            DestroyImmediate(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start() {
        SetSceneType(sceneType);
    }

    // Update is called once per frame
    void Update() {
    }

    public void QuitLevel() {
        LoadScene(SceneType.Menu);
        Debug.Log("Level exit button was pressed");
    }

    public void SetSceneType(SceneType type) {
        sceneType = type;
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

    public void LoadScene(SceneType type) {
        if (type == SceneType.Level && sceneType == SceneType.Menu) {
            Debug.Log("Loading Level");
            sceneType = type;
            SceneManager.LoadScene("Scenes/Level");
        } else if (type == SceneType.Menu && sceneType == SceneType.Level) {
            Debug.Log("Loading Menu");
            sceneType = type;
            SceneManager.LoadScene("Scenes/Menu");
        }
    }
}
