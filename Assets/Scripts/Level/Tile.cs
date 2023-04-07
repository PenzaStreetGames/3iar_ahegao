using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace Level
{
    public enum TileColor : int 
    {
        Red = 0,
        Blue = 1,
        Green = 2,
        Yellow = 3,
    }

    public enum TileType : int
    {
        Border = 0,
        Open = 1,
        Blocked = 2,
    }

    public enum TileViewState : int
    {
        Active = 0,
        Hover = 1,
        Selected = 2,
    }

    public class Tile : MonoBehaviour
    {
        public FieldController fieldController;

        public TileColor tileColor;
        public TileType tileType;
        public TileViewState tileViewState;
        public Vector2 position;
        public bool selected;
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

        public void SetColor(TileColor color)
        {
            tileColor = color;
            calculateEffects();
        }

        public void SetViewState(TileViewState viewState)
        {
            tileViewState = viewState;
            calculateEffects();
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