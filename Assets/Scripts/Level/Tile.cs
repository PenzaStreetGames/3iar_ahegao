using System;
using UnityEngine;

namespace Level
{
    public enum TileColor
    {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4
    }

    public enum TileType
    {
        Border = 0,
        Open = 1,
        Blocked = 2
    }

    public enum TileViewState
    {
        Active = 0,
        Hover = 1,
        Selected = 2
    }

    [Serializable]
    public class Tile : MonoBehaviour
    {
        public FieldController fieldController;

        public TileColor tileColor;
        public TileType tileType;
        public TileViewState tileViewState;
        public Vector2 position;
        public Color[] colors = new Color[Enum.GetValues(typeof(TileColor)).Length];
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

        public TileData GetTileData()
        {
            var tileData = new TileData
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
