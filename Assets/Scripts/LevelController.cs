using Db;
using Level;
using UnityEditor.PackageManager;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public GameController gameController;

    public FieldController fieldController;

    public int turnCounter;

    public int destroyedTilesCounter;

    public int targetDestroyedTilesCount;
    public int score;


    public void MakeTurn() {
        DecreaseTurnCounter();
        if (CheckSuccessLevelEnd()) {
            Debug.Log("Вы победили");
        }
        else if (CheckLevelEnd()) {
            Debug.Log("Ходы закончились");
        }
    }

    public void DecreaseTurnCounter() {
        if (turnCounter > 0) {
            turnCounter--;
        }
    }

    public void IncreaseDestroyedTilesCounter(int count) {
        destroyedTilesCounter += count;
        Debug.Log($"Уничтожено всего: {destroyedTilesCounter}");
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
            Debug.LogError("Combination less than 3");
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
            8, 8, save
        );
    }

    // Update is called once per frame
    void Update() {
    }
}
