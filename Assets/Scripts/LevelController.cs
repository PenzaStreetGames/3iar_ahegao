using Level;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public GameController gameController;

    public FieldController fieldController;
    //TODO: сделать счётчик ходов
    public int turnCounter;

    //TODO: метод уменьшения кол-ва ходов
    public void DecreaseTurnCounter()
    {
        if (turnCounter > 0)
        {
            turnCounter--;
        }

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
