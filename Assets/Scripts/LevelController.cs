using Db;
using Db.Entity;
using Level;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelController : MonoBehaviour {
    public GameController gameController;
    public FieldController fieldController;
    public int startTurnCount;
    public int turnCounter;
    public int destroyedTilesCounter;
    public int targetDestroyedTilesCount;
    public int score = 0;
    [FormerlySerializedAs("levelStatus")] public LevelProgressStage levelProgressStage;
    public static LevelController Instance;


    // Start is called before the first frame update
    void Start() {
        if (Instance == null) {
            Instance = this;
        }
        var save = SaveRepository.GetSave(-1, -1);
        fieldController.Init(
            4, 4, save
        );
    }

    // Update is called once per frame
    void Update() {
    }

    public LevelProgressStage GetLevelStatus() {
        var state = LevelProgressStage.StillPlaying;
        if (!CheckIfTurnsExists()) {
            state = LevelProgressStage.NoCombinationsLeftLose;
        } else if (CheckSuccessLevelEnd()) {
            state = LevelProgressStage.Win;
        } else if (CheckLevelEnd()) {
            state = LevelProgressStage.NoTurnsLeftLose;
        }
        return state;
    }

    //Level restart function (in theory, in the future it should launch a dialog box with the Restart - Exit menu option).
    //Message - a message that will be displayed to the player at the end of the game in the future
    public void RestartLevel(string message) {
        Debug.Log(message);

        fieldController.GenerateFieldWithGuaranteedCombination();
        Debug.Log("Reload Level. Resetting game state");
        ResetState();

        Debug.Log("Making new save");
        SaveRepository.PersistSave(Save.MakeSaveFromData(fieldController.Tiles));
    }

    void ResetState() {
        turnCounter = startTurnCount;
        destroyedTilesCounter = 0;
        score = 0;
    }

    //A function that performs all the logic after the player's turn
    public void UpdateAfterPlayerTurn() {
        DecrementTurnCounter();
        levelProgressStage = GetLevelStatus();
        switch (levelProgressStage) {
            case LevelProgressStage.Win:
                RestartLevel("Congratulations! You have passed the level!");
                break;
            case LevelProgressStage.NoCombinationsLeftLose:
                RestartLevel("You've lost! Combinations have run out on the field.");
                break;
            case LevelProgressStage.NoTurnsLeftLose:
                RestartLevel("You've lost! You don't have turns left.");
                break;
            case LevelProgressStage.StillPlaying:
            default:
                break;
        }
    }

    public bool CheckIfTurnsExists() {
        return fieldController.GetAllPossibleTurns().Count > 0;
    }

    public void DecrementTurnCounter() {
        if (turnCounter > 0) {
            turnCounter--;
        }
    }

    public void IncreaseDestroyedTilesCounter(int count) {
        destroyedTilesCounter += count;
        Debug.Log($"Destroyed: {destroyedTilesCounter}");
    }

    public bool CheckLevelEnd() {
        if (turnCounter > 0) {
            return false;
        }

        return true;
    }

    public bool CheckSuccessLevelEnd() {
        if (destroyedTilesCounter >= targetDestroyedTilesCount) {
            return true;
        }

        return false;
    }

    public void IncreaseScoreForCombination(int combinationLength) {
        var delta = 0;
        if (combinationLength <= 2) {
            return;
        }
        delta = combinationLength switch {
            3 => 10,
            4 => 30,
            5 => 90,
            6 => 270,
            _ => 500
        };
        score += delta;
    }

    // Start is called before the first frame update
    void Start() {
        var level = LevelRepository.GetLevel();
        var save = SaveEntity.MakeSaveFromLevel(level);
        SaveRepository.PersistSave(save);

        fieldController.Init(
            4, 4, save
        );
    }

    // Update is called once per frame
    void Update() {
    }
}
