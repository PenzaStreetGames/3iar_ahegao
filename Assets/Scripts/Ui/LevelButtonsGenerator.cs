using TMPro;
using UnityEngine;

namespace Ui {
    public class LevelButtonsGenerator : MonoBehaviour {
        public int colsCount;
        public Vector3 leftTopCorner;
        public float distanceBetweenButtons;
        public GameObject levelButtonPrefab;
        public MenuController menuController;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void GenerateButtons(int startNumber, int amount) {
            while (transform.childCount > 0) {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
            for (var i = startNumber; i < startNumber + amount; i++) {
                (int x, int y) = ((i - startNumber) / colsCount, (i - startNumber) % colsCount);
                Vector3 spawnPosition = leftTopCorner + new Vector3(
                    y * distanceBetweenButtons,
                    -x * distanceBetweenButtons,
                    0
                    );
                GameObject levelButton = Instantiate(levelButtonPrefab, spawnPosition, Quaternion.identity, transform);
                ChooseLevelButtonContoller chooseLevelButtonController =
                    levelButton.GetComponent<ChooseLevelButtonContoller>();
                chooseLevelButtonController.levelNumber = i;
                chooseLevelButtonController.gameController = menuController.gameController;
                ButtonController buttonController = levelButton.GetComponent<ButtonController>();
                buttonController.menuUiController = menuController.menuUiController;
                TMP_Text label = levelButton.transform.GetChild(0).GetComponent<TMP_Text>();
                label.SetText(i.ToString());
            }
        }
    }
}
