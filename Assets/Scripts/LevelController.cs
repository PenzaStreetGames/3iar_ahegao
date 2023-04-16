using Level;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameController gameController;

    public FieldController fieldController;
    //TODO: сделать счётчик ходов
    public int turnCounter;

    public int destroyedTilesCounter;

    public int targetDestroyedTilesCount;
    //TODO: метод уменьшения кол-ва ходов

    public void MakeTurn()
    {
        DecreaseTurnCounter();
        if (CheckSuccessLevelEnd())
        {
            Debug.Log("Вы победили");
        } else if (CheckLevelEnd())
        {
            Debug.Log("Ходы закончились");
        }
    }
    public void DecreaseTurnCounter()
    {
        if (turnCounter > 0)
        {
            turnCounter--;
        }
    }

    public bool CheckLevelEnd()
    {
        if (turnCounter > 0)
        {
            return false;
        }

        return true;
    }

    public bool CheckSuccessLevelEnd()
    {
        if (destroyedTilesCounter >= targetDestroyedTilesCount)
        {
            return true;
        }
        return false;
    }


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }
}
