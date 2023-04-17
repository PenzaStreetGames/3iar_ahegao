using Level;
using UnityEngine;

public class LevelController : MonoBehaviour {
    public GameController gameController;

    public FieldController fieldController;

    public int turnCounter;

    public int destroyedTilesCounter;

    public int targetDestroyedTilesCount;


    public LevelStatusEnum CheckLevelState() {
        if (CheckIfTurnsExists()) {
            Debug.Log("Не осталось комбинаций на поле. Вы проиграли!");
            return LevelStatusEnum.NoCombinationsLeftLose;
        }
        if (CheckSuccessLevelEnd()) {
            Debug.Log("Вы победили");
            return LevelStatusEnum.Win;
        }
        if (CheckLevelEnd()) {
            Debug.Log("Закончились ходы. Вы проиграли!");
            return LevelStatusEnum.NoTurnsLeftLose;
        }
        return LevelStatusEnum.StillPlaying;
    }

    //Функция перезапуска уровня (по идее в будущем должно запускать диалоговое окно с выбором Перезапустить - Выйти в меню).
    //Message - сообщение, которое будет в будущем выводиться игроку при завершении игры
    public void RestartLevel(string message) {
        fieldController.Init(5,5);
        Debug.Log(message);
    }

    //Функция, выполняющая всю логику после хода игрока
    public void UpdateAfterPlayerTurn(int destroyedTiles) {
        IncreaseDestroyedTilesCounter(destroyedTiles);
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
