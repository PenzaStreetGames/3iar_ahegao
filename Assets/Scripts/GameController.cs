using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType : int
{
    Menu = 0,
    Level = 1
}
public class GameController : MonoBehaviour
{
    public LevelController levelController;
    public MenuController menuController;
    public SceneType sceneType = SceneType.Level;

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
