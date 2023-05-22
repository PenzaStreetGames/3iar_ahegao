using Db;
using Db.Entity;
using Level;
using Level.EventQueue;
using Level.TileEntity;
using UnityEngine;
using UnityEngine.Serialization;

public class LevelController : MonoBehaviour {
    public GameController gameController;
    public FieldController fieldController;
    public LevelEventQueue levelEventQueue;
    public int startTurnCount;
    public int turnCounter;
    public int destroyedTilesCounter;
    public int targetDestroyedTilesCount;
    public int score = 0;
    [FormerlySerializedAs("levelStatus")] public LevelProgressStage levelProgressStage;
    public static LevelController Instance;
    public TextFieldController scoreField;
    public TextFieldController turnsField;
    public TextFieldController destroyedTilesField;

    // Start is called before the first frame update
    void Start() {
        if (Instance == null) {
            Instance = this;
        }

        var save = SaveRepository.GetSave();
        if (save == default(SaveEntity)) {
            var level = LevelRepository.GetLevel();
            save = SaveEntity.MakeSaveFromLevel(level);
            SaveRepository.PersistSave(save);
        }

        fieldController.Init(save);
        SetScore(score);
        SetTurns(turnCounter);
        SetDestroyedTilesCount(destroyedTilesCounter);
    }

    // Update is called once per frame
    void Update() {
        CheckLevelStatus();
    }

    public LevelProgressStage GetLevelStatus() {
        var state = LevelProgressStage.StillPlaying;
        if (levelEventQueue.IsFieldStable()) {
            if (!CheckIfTurnsExists()) {
                state = LevelProgressStage.NoCombinationsLeftLose;
            }
            else if (CheckSuccessLevelEnd()) {
                state = LevelProgressStage.Win;
            }
            else if (CheckLevelEnd()) {
                state = LevelProgressStage.NoTurnsLeftLose;
            }
        }
        else {
            state = LevelProgressStage.UnstableField;
        }
        return state;
    }

    //Level restart function (in theory, in the future it should launch a dialog box with the Restart - Exit menu option).
    //Message - a message that will be displayed to the player at the end of the game in the future
    public void RestartLevel(string message) {
        Debug.LogWarning(message);

        fieldController.GenerateFieldWithGuaranteedCombination(
            SaveEntity.MakeSaveFromLevel(
                LevelRepository.GetLevel()
            )
        );
        Debug.Log("Reload Level. Resetting game state");
        ResetState();
    }

    void ResetState() {
        SetDestroyedTilesCount(0);
        SetTurns(startTurnCount);
        SetScore(0);
        fieldController.ResetField(SaveEntity.MakeSaveFromLevel(LevelRepository.GetLevel()));
    }

    //A function that performs all the logic after the player's turn
    public void CheckLevelStatus() {
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
            case LevelProgressStage.UnstableField:
            default:
                break;
        }
    }

    public bool CheckIfTurnsExists() {
        return fieldController.GetAllPossibleTurns().Count > 0;
    }

    public void DecrementTurnCounter() {
        if (turnCounter > 0) {
            SetTurns(turnCounter - 1);
        }
    }

    public void IncreaseDestroyedTilesCounter(int count) {
        SetDestroyedTilesCount(destroyedTilesCounter);
        Debug.Log($"Destroyed: {count}");
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
        SetScore(score + delta);
    }

    public void SetScore(int value) {
        score = value;
        scoreField.SetValue(value.ToString());
    }

    public void SetTurns(int value) {
        turnCounter = value;
        turnsField.SetValue(value.ToString());
    }

    public void SetDestroyedTilesCount(int value) {
        destroyedTilesCounter = value;
        // destroyedTilesField.SetValue(value.ToString());
    }
}
