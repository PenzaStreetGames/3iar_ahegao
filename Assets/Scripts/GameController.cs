using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * Класс, отображающий состояние сцены
 */
public enum SceneType : int
{
    Menu = 0, /**< Меню */
    Level = 1 /**< Уровень */
}

/**
 * Класс управления состоянием игры
 */
public class GameController : MonoBehaviour
{
    public LevelController levelController; /**< Контроллер управления уровнями */
    public MenuController menuController; /**< Контрллер управления меню */
    public SceneType sceneType = SceneType.Level; /**< Тип сцены */

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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
