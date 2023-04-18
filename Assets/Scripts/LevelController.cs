using Db;
using Level;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public GameController gameController;

    public FieldController fieldController;

    public int startTurnCount;

    public int turnCounter;

    public int destroyedTilesCounter;

    public int targetDestroyedTilesCount;

    public int score = 0;

    public LevelStatus levelStatus;



    public LevelStatus GetLevelStatus() {
        var state = LevelStatus.StillPlaying;
        if (!CheckIfTurnsExists()) {
            state = LevelStatus.NoCombinationsLeftLose;
        } else if (CheckSuccessLevelEnd()) {
            state = LevelStatus.Win;
        } else if (CheckLevelEnd()) {
            state = LevelStatus.NoTurnsLeftLose;
        }
        return state;
    }

    //Level restart function (in theory, in the future it should launch a dialog box with the Restart - Exit menu option).
    //Message - a message that will be displayed to the player at the end of the game in the future
    public void RestartLevel(string message) {
        ResetState();
        fieldController.GenerateFieldWithGuaranteedCombination();
        Debug.Log(message);
    }

    void ResetState() {
        turnCounter = startTurnCount;
        destroyedTilesCounter = 0;
        score = 0;
    }

    //A function that performs all the logic after the player's turn
    public void UpdateAfterPlayerTurn() {
        DecrementTurnCounter();
        levelStatus = GetLevelStatus();
        switch (levelStatus) {
            case LevelStatus.Win:
                RestartLevel("Congratulations! You have passed the level!");
                break;
            case LevelStatus.NoCombinationsLeftLose:
                RestartLevel("You've lost! You have no turns left.");
                break;
            case LevelStatus.NoTurnsLeftLose:
                RestartLevel("You've lost! Combinations have run out on the field.");
                break;
            case LevelStatus.StillPlaying:
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
        var save = SaveRepository.GetSave(-1, -1);

        fieldController.Init(
            4, 4, save
        );
    }

    // Update is called once per frame
    void Update() {
    }
}
