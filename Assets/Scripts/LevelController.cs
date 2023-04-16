using Level;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public GameController gameController;

    public FieldController fieldController;

    //TODO: сделать счётчик ходов
    public int turnCounter;

    public int destroyedTilesCounter;

    public int targetDestroyedTilesCount;
    //TODO: метод уменьшения кол-ва ходов

    public void CheckLevelState() {
        if (CheckIfTurnsExists()) {
            Debug.Log("Ходов нема");
        }
        if (CheckSuccessLevelEnd()) {
            Debug.Log("Вы победили");
        }
        else if (CheckLevelEnd()) {
            Debug.Log("Ходы закончились");
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


    // Start is called before the first frame update
    void Start() {
        fieldController.Init(
            5, 5
        );
    }

    // Update is called once per frame
    void Update() {
    }
}
