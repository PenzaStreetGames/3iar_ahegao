using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor.Tilemaps;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Level
{
    /**
     * Класс FieldController предназначен для управления
     * игровым полем игры "три в ряд"
     */
    public class FieldController : MonoBehaviour
    {
        public Tile[,] Tiles; /**< Матрица тайлов игрового поля */
        public Vector2 fieldSize; /**< Размерность игрового поля */
        public GameObject tilePrefab; /**< Шаблон тайла игровго поля */
        public float tileStep = 1f; /**< Шаг расстановки тайлов на игровом поле */
        public Tile chosenTile; /**< Выбранный тайл */
        // Start is called before the first frame update
        
        /**
         * Метод,вызываемый перед первым обновлением кадра.
         * Генерирует и устанавливает тайлы на игровом поле
         */
        void Start()
        {
            Tiles = new Tile[(int)fieldSize.x, (int)fieldSize.y];
            CreateTiles();
            GenerateField();
        }

        private void CreateTiles()
        {
            Vector2 leftTopCorner = new Vector2(
                -(fieldSize.x / 2) * tileStep,
                (fieldSize.y / 2) * tileStep
            );
            for (var i = 0; i < fieldSize.x; i++)
            {
                for (var j = 0; j < fieldSize.y; j++)
                {
                    Vector3 point = new Vector3(
                        leftTopCorner.x + (j + 0.5f) * tileStep,
                        leftTopCorner.y - (i + 0.5f) * tileStep,
                        0
                    );
                    Tiles[i, j] = CreateTile(i, j, point);
                }
            }
        }

        private Tile CreateTile(int i, int j, Vector3 point)
        {
            var tileObject = Instantiate(tilePrefab, point, Quaternion.identity);
            tileObject.transform.parent = gameObject.transform;
            tileObject.name = $"Tile {i}-{j}";
            var tile = tileObject.GetComponent<Tile>();
            tile.fieldController = this;
            tile.position = new Vector2(i, j);
            tile.SetColor(TileColor.None);
            return tile;
        }

        private void GenerateField()
        {
            // var colors = Enum.GetValues(typeof(TileColor));
            var ccolors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow };
            for (int i = 0; i < fieldSize.x; i++)
            {
                for (int j = 0; j < fieldSize.y; j++)
                {
                    var tile = Tiles[i, j];
                    tile.tileType = TileType.Open;
                    // TileColor color = (TileColor) colors.GetValue(Random.Range(0, colors.Length));
                    // tile.SetColor(color);
                    // tile.SetColor(null);
                    int colorIndex = (i + j + Random.Range(0, ccolors.Length)) % ccolors.Length;
                    tile.SetColor(ccolors[colorIndex]);
                    while (CheckCombination(Tiles))
                    {
                        colorIndex = (colorIndex + 1) % ccolors.Length;
                        tile.SetColor(ccolors[colorIndex]);
                    }
                }
            }
        }
        
        // Функция для проверки наличия комбинации из трех и более одинаковых тайлов
        private bool CheckCombination(Tile[,] field)
        {
            int numRows = field.GetLength(0);
            int numColumns = field.GetLength(1);

            // Проверяем горизонтальные совпадения
            for (int i = 0; i < numRows; i++)
            {
                for (int j = 0; j < numColumns - 2; j++)
                {
                    if (field[i, j].GetColor() != TileColor.None && field[i, j].GetColor() == field[i, j + 1].GetColor() && field[i, j].GetColor()== field[i, j + 2].GetColor())
                    {
                        Debug.Log($"hor {i} {j} {field[i, j].GetColor()}");
                        return true;
                    }
                }
            }

            // Проверяем вертикальные совпадения
            for (int i = 0; i < numRows - 2; i++)
            {
                for (int j = 0; j < numColumns; j++)
                {
                    if (field[i, j].GetColor() != TileColor.None && field[i, j].GetColor() == field[i + 1, j].GetColor() && field[i, j].GetColor() == field[i + 2, j].GetColor())
                    {
                        Debug.Log($"vert {i} {j} {field[i, j].GetColor()}");
                        return true;
                    }
                }
            }

            return false;
        }
        
        // Update is called once per frame
        /**
         * Метод помощи в отрисовке. Вызывается один раз при отрисовке каждого кадра
         */
        void Update()
        {
        }

        /**
         * Метод обработки нажатия на тайл.
         * Служит для реализации игровой механики по перестановке тайлов на поле
         * при наступлении события нажатия.
         */
        public void HandleTileClick(Tile tile /**< Объект нажатого тайла */)
        {
            Debug.Log($"Click {tile.gameObject.name}");
            if (chosenTile != null)
            {
                if (chosenTile.CanSwapWith(tile))
                {
                    SwapTiles(chosenTile, tile);
                    tile.SetViewState(TileViewState.Active);
                    chosenTile.SetViewState(TileViewState.Active);
                    chosenTile = null;
                    return;
                }
                chosenTile.SetViewState(TileViewState.Active);
            }
            tile.SetViewState(TileViewState.Selected);
            chosenTile = tile;
        }

        /**
         * Метод, меняющий местами два тайлла-аргумента
         */
        public void SwapTiles(Tile tile1 /**< Первый тайл для перемещения */, Tile tile2 /**< Второй тайл для перемещения */)
        {
            (tile1.tileColor, tile2.tileColor) = (tile2.tileColor, tile1.tileColor);
        }

        /**
         * Метод обработки события наведения курсора на тайл.
         * Служит для графического выделения заинтересовавшего пользователя тайла
         */
        public void HandleTileMouseEnter(Tile tile /**< Тайл, находящийся под курсором игрока */)
        {
            if (tile.tileViewState == TileViewState.Active)
                tile.SetViewState(TileViewState.Hover);
            Debug.Log($"Cursor enter {tile.gameObject.name}");
        }

        /**
         * Метод обработки события выведения курсора за границы тайла.
         * Служит для отмены граффических эффектов наведения курсора на тайл
         */
        public void HandleTileMouseExit(Tile tile /**< Тайл, вышедший из под наблюдения */)
        {
            if (tile.tileViewState == TileViewState.Hover)
                tile.SetViewState(TileViewState.Active);
            Debug.Log($"Cursor exit {tile.gameObject.name}");
        }
    }
}