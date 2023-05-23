using Ui;
using UnityEngine;
using UnityEngine.Serialization;

public class MenuUiController : MonoBehaviour {
    public GameController gameController;
    public SceneType sceneType;
    public MenuPage menuPage;
    public GameObject mainMenuPanel;
    public GameObject chooseLevelPanel;

    // Start is called before the first frame update
    void Start() {
        SetMenuPage(MenuPage.MainMenu);
    }

    // Update is called once per frame
    void Update() {
    }

    public void SetMenuPage(MenuPage menuPage) {
        this.menuPage = menuPage;
        SetVisiblePanel();
    }

    public void SetVisiblePanel() {
        mainMenuPanel.SetActive(menuPage == MenuPage.MainMenu);
        chooseLevelPanel.SetActive(menuPage == MenuPage.LevelChoice);
    }
}
