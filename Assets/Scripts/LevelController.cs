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



    public LevelStatusEnum CheckLevelState() {
        var state = LevelStatusEnum.StillPlaying;
        if (!CheckIfTurnsExists()) {
            state = LevelStatusEnum.NoCombinationsLeftLose;
        } else if (CheckSuccessLevelEnd()) {
            state = LevelStatusEnum.Win;
        } else if (CheckLevelEnd()) {
            state = LevelStatusEnum.NoTurnsLeftLose;
        }
        return state;
    }

    //Функция перезапуска уровня (по идее в будущем должно запускать диалоговое окно с выбором Перезапустить - Выйти в меню).
    //Message - сообщение, которое будет в будущем выводиться игроку при завершении игры
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

    //Функция, выполняющая всю логику после хода игрока
    public void UpdateAfterPlayerTurn() {
        DecrementTurnCounter();
        switch (CheckLevelState()) {
            case LevelStatusEnum.Win:
                RestartLevel("Поздравляем! Вы прошли уровень!");
                break;
            case LevelStatusEnum.NoCombinationsLeftLose:
                RestartLevel("Вы проиграли! У вас не осталось ходов.");
                break;
            case LevelStatusEnum.NoTurnsLeftLose:
                RestartLevel("Вы проиграли! На поле закончились комбинации.");
                break;
            case LevelStatusEnum.StillPlaying:
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
        Debug.Log($"Уничтожено: {destroyedTilesCounter}");
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
