using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Level.TileEntity
{

    public class Tile : MonoBehaviour
    {
        public FieldController fieldController;

        public TileColor tileColor;
        public TileType tileType;
        public Vector2 position;

        public Color[] colors = new Color[Enum.GetValues(typeof(TileColor)).Length];
        public TileViewState tileViewState;
        public float chosenShadowSharpness;
        public float chosenScale;

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
        }

        void OnMouseDown()
        {
            fieldController.HandleTileClick(this);
        }

        void OnMouseEnter()
        {
            FieldController.HandleTileMouseEnter(this);
        }

        void OnMouseExit()
        {
            FieldController.HandleTileMouseExit(this);
        }

        public TilePersistData GetTileData()
        {
            var tileData = new TilePersistData
            {
                X = (int)position.x,
                Y = (int)position.y,
                TileColor = tileColor,
                TileType = tileType
            };

            return tileData;
        }

        public TileColor GetColor()
        {
            return tileColor;
        }

        public void SetColor(TileColor color)
        {
            tileColor = color;
            CalculateEffects();
        }

        public void SetViewState(TileViewState viewState)
        {
            tileViewState = viewState;
            CalculateEffects();
        }

        bool IsNeighbour(Tile other)
        {
            var l = position - other.position;
            return Math.Abs(l.magnitude - 1f) < 0.001f;
        }

        public bool CanSwapWith(Tile other)
        {
            if (!IsNeighbour(other))
            {
                return false;
            }

            if (other.tileType != TileType.Open || tileType != TileType.Open)
            {
                return false;
            }

            return tileColor != other.tileColor;
        }

        //TODO: сделать проверку на комбинации с соседями
        public bool CheckCombination()
        {
            if (tileType == TileType.Open)
            {
                //смотрим соседние клетки справа
                if (position.x < fieldController.fieldSize.x - 2)
                {

                }

                if (position.y < fieldController.fieldSize.y - 2)
                {

                }

            }
            return false;
        }

        void CalculateEffects()
        {
            var baseColor = colors[(int)tileColor];
            var spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
            var color = baseColor;
            var localScale = new Vector3(1, 1, 1);
            switch (tileViewState)
            {
                case TileViewState.Active:
                    break;
                case TileViewState.Hover:
                    color = new Color(
                        baseColor.r * chosenShadowSharpness,
                        baseColor.g * chosenShadowSharpness,
                        baseColor.b * chosenShadowSharpness,
                        1.0f
                    );
                    break;
                case TileViewState.Selected:
                    color = new Color(
                        baseColor.r * chosenShadowSharpness,
                        baseColor.g * chosenShadowSharpness,
                        baseColor.b * chosenShadowSharpness,
                        1.0f
                    );
                    localScale = new Vector3(chosenScale, chosenScale, chosenScale);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            spriteRenderer.color = color;
            transform.localScale = localScale;
        }
    }
}
