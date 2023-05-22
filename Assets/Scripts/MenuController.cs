using Ui;
using UnityEngine;

public class MenuController : MonoBehaviour {
    public GameController gameController;
    public SceneType sceneType;
    public MainMenuPage mainMenuPage;
    public GameObject mainMenuPanel;
    public GameObject chooseLevelPanel;
    public GameObject[] panels;

    // Start is called before the first frame update
    void Start() {
        panels = new[] { mainMenuPanel, chooseLevelPanel };
        SetMenuPage(MainMenuPage.MainMenu);
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetMenuPage(MainMenuPage menuPage) {
        mainMenuPage = menuPage;
        SetVisiblePanel();
    }

    public void SetVisiblePanel() {
        foreach (var panel in panels) {
            panel.SetActive(false);
        }
        switch (mainMenuPage) {
            case MainMenuPage.MainMenu:
                mainMenuPanel.SetActive(true);
                break;
            case MainMenuPage.LevelChoice:
                chooseLevelPanel.SetActive(true);
                break;
        }
    }
}
