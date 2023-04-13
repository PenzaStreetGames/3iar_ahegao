using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Level
{
    /**
     * Класс TileColor предназначен для
     * хранения возможных цветов тайлов в виде перечисления
     */
    public enum TileColor : int 
    {
        None = 0, /**< Стандартный цвет */
        Red = 1, /**< Красный цвет */
        Blue = 2, /**< Синий цвет */
        Green = 3, /**< Зеленый цвет */
        Yellow = 4, /**< Желтый цвет */
    }

    /**
     * Класс TileType предназначен для
     * хранения возможных вариантов состояния тайла по доступности
     */
    public enum TileType : int
    {
        Border = 0, /**< Граничный тайл */
        Open = 1, /**< Активный тайл */
        Blocked = 2, /**< Заблокированный тайл */
    }

    /**
     * Класс TileViewState предназначен для
     * хранения возможных вариантов состояния тайла по графическому отображению
     */
    public enum TileViewState : int
    {
        Active = 0, /**< Активный для отображения */
        Hover = 1, /**< Находящийся под курсором */
        Selected = 2, /**< Выбранный */
    }

    /**
     * Класс Tile.
     * Основная сущность в игровом процессе.
     * Представляет из себя место на игровом поле определенного цвета
     */
    public class Tile : MonoBehaviour
    {
        public FieldController fieldController; /**< Контроллер управления игровым полем */

        public TileColor tileColor; /**< Цвет тайла */
        public TileType tileType; /**< Тип тайла по доступности */
        public TileViewState tileViewState; /**< Тип тайла по отображению */
        public Vector2 position; /**< Координаты тайла */
        public bool selected; /**< Состояние выбранности тайла */
        public Color[] colors = new Color[Enum.GetValues(typeof(TileColor)).Length]; /**< Возможные цвета тайла */
        public float chosenShadowSharpness; /**< Затемнение тайла */
        public float chosenScale; /**< Масштаб тайла */

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        /**
         * Метод получения цвета тайла
         */
        public TileColor /**< Цвет тайла */ GetColor()
        {
            return tileColor;
        }
        
        /**
         * Метод задания цвета тайла
         */
        public void SetColor(TileColor color /**< Цвет тайла */)
        {
            tileColor = color;
            calculateEffects();
        }

        /**
         * Метод установления отображения тайла
         */
        public void SetViewState(TileViewState viewState /**< Состояние отображения тайла */)
        {
            tileViewState = viewState;
            calculateEffects();
        }

        /**
         * Метод проверки тайлов на соседство
         */
        public bool IsNeighbour(Tile other /**< Второй выбранный тайл */)
        {
            var l = position - other.position;
            return Math.Abs(l.magnitude - 1f) < 0.001f;
        }

        /**
         * Метод проверки тайлов на возможность перемещения
         */
        public bool CanSwapWith(Tile other /**< Второй выбранный тайл */)
        {
            if (!IsNeighbour(other))
                return false;
            if (other.tileType != TileType.Open || tileType != TileType.Open)
                return false;
            if (tileColor == other.tileColor)
                return false;
            return true;
        }

        private void calculateEffects()
        {
            Color baseColor = colors[(int)tileColor];
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            Color color = baseColor;
            Vector3 localScale = new Vector3(1, 1, 1);
            switch (tileViewState)
            {
                case TileViewState.Active:
                    break;
                case TileViewState.Hover:
                    color = new Color(
                        r: baseColor.r * chosenShadowSharpness,
                        g: baseColor.g * chosenShadowSharpness,
                        b: baseColor.b * chosenShadowSharpness,
                        a: 1.0f
                        );
                    break;
                case TileViewState.Selected:
                    color = new Color(
                        r: baseColor.r * chosenShadowSharpness,
                        g: baseColor.g * chosenShadowSharpness,
                        b: baseColor.b * chosenShadowSharpness,
                        a: 1.0f
                    );
                    localScale = new Vector3(chosenScale, chosenScale, chosenScale);
                    break;
            }
            spriteRenderer.color = color;
            transform.localScale = localScale;
        }

        private void OnMouseDown()
        {
            fieldController.HandleTileClick(this);
        }
        
        private void OnMouseEnter()
        {
            fieldController.HandleTileMouseEnter(this);
        }

        private void OnMouseExit()
        {
            fieldController.HandleTileMouseExit(this);
        }
    }
}