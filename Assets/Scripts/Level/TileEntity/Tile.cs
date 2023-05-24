using System;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Level.TileEntity {
    public class Tile : MonoBehaviour {
        public FieldController fieldController;

        public TileColor tileColor;
        public TileType tileType;
        public IntPair position;

        public Color[] colors = new Color[Enum.GetValues(typeof(TileColor)).Length];
        public Sprite[] sprites = new Sprite[Enum.GetValues(typeof(TileColor)).Length];
        public TileViewState tileViewState;
        public float chosenShadowSharpness;
        public float chosenScale;
        public SpriteRenderer spriteRenderer;

        // Start is called before the first frame update
        void Start() {
        }

        // Update is called once per frame
        void Update() {
        }

        void OnMouseDown() {
            if (tileType != TileType.Open) {
                return;
            }

            fieldController.HandleTileClick(this);
        }

        void OnMouseEnter() {
            if (fieldController.levelController.levelProgressStage != LevelProgressStage.StillPlaying)
                return;
            if (tileType != TileType.Open) {
                return;
            }

            FieldController.HandleTileMouseEnter(this);
        }

        void OnMouseExit() {
            if (tileType != TileType.Open) {
                return;
            }

            FieldController.HandleTileMouseExit(this);
        }

        public TilePersistData GetTileData() {
            var tileData = new TilePersistData {
                X = position.X,
                Y = position.Y,
                TileColor = tileColor,
                TileType = tileType
            };

            return tileData;
        }

        public TileColor GetColor() {
            return tileColor;
        }

        public void SetColor(TileColor color) {
            tileColor = color;
            CalculateEffects();
        }


        public void SetTileType(TileType type) {
            tileType = type;
            CalculateEffects();
        }

        public void SetViewState(TileViewState viewState) {
            tileViewState = viewState;
            CalculateEffects();
        }

        bool IsNeighbour(Tile other) {
            return position.ManhattanDistance(other.position) == 1;
        }

        public bool CanSwapWith(Tile[,] tiles, Tile other) {
            if (!IsNeighbour(other))
                return false;
            if (other.tileType != TileType.Open || tileType != TileType.Open)
                return false;
            if (tileColor == other.tileColor)
                return false;
            if (!MakesCombinationWhenSwappedWith(tiles, other))
                return false;
            return true;
        }

        public bool CanFallDown(Tile[,] tiles) {
            int x = position.X, y = position.Y;
            if (x >= tiles.GetLength(0) - 1)
                return false;
            var other = tiles[x + 1, y];
            if (other.tileType != TileType.Open)
                return false;
            return other.tileColor == TileColor.None;
        }

        public bool InTopLayer(Tile[,] tiles) {
            var (x, y) = (position.X, position.Y);
            for (int i = x; i > 0; i--) {
                var tileAbove = tiles[i - 1, y];
                if (tileAbove.tileType is TileType.Open or TileType.Blocked) {
                    return false;
                }
            }
            return true;
        }

        public bool UnderBorder(Tile[,] tiles) {
            if (position.X == 0)
                return true;
            Tile above = tiles[position.X - 1, position.Y];
            if (above.tileType == TileType.Border)
                return true;
            return false;
        }

        public bool MakesCombinationWhenSwappedWith(Tile[,] tiles, Tile other) {
            var flag = false;
            FieldController.SwapTileColors(this, other);
            if (HaveCombinations(tiles) || other.HaveCombinations(tiles)) {
                flag = true;
            }
            FieldController.SwapTileColors(this, other);
            return flag;
        }

        public HashSet<HashSet<IntPair>> GetTurns(Tile[,] tiles) {
            var res = new HashSet<HashSet<IntPair>>();
            var turns = new IntPair[]{new (-1,0),new (1,0),new (0,-1),new (0,1)};
            foreach (var turn in turns) {
                var anotherTilePosition = turn + position;
                var x1 = anotherTilePosition.X;
                var y1 = anotherTilePosition.Y;
                if (x1 >= 0 && x1 < tiles.GetLength(0) &&
                    y1 >= 0 && y1 < tiles.GetLength(1) &&
                    MakesCombinationWhenSwappedWith(tiles, tiles[x1, y1])) {
                    res.Add(new HashSet<IntPair> {new(position.X, position.Y), new(x1, y1)});
                }
            }
            return res;
        }

        public bool HaveCombinations(Tile[,] tiles) {
            if (tileType != TileType.Open)
                return false;
            if (tileColor == TileColor.None)
                return false;

            int vertical = 1, horizontal = 1;
            bool top = true, bottom = true, left = true, right = true;
            int x1 = position.X, y1 = position.Y;
            for (int i = 1; i < 3; i++) {
                if (top && x1 - i >= 0) {
                    int x2 = x1 - i, y2 = y1;
                    //Debug.Log($"{fieldController.Tiles[x2,y2]}");
                    var other = tiles[x2, y2];
                    if (other.tileType != TileType.Open || other.tileColor != tileColor)
                        top = false;
                    else {
                        vertical++;
                        if (vertical >= 3)
                            return true;
                    }
                }

                if (bottom && x1 + i < tiles.GetLength(0)) {
                    int x2 = x1 + i, y2 = y1;
                    var other = tiles[x2, y2];
                    if (other.tileType != TileType.Open || other.tileColor != tileColor)
                        bottom = false;
                    else {
                        vertical++;
                        if (vertical >= 3)
                            return true;
                    }
                }

                if (left && y1 - i >= 0) {
                    int x2 = x1, y2 = y1 - i;
                    var other = tiles[x2, y2];
                    if (other.tileType != TileType.Open || other.tileColor != tileColor)
                        left = false;
                    else {
                        horizontal++;
                        if (horizontal >= 3)
                            return true;
                    }
                }

                if (right && y1 + i < tiles.GetLength(1)) {
                    int x2 = x1, y2 = y1 + i;
                    var other = tiles[x2, y2];
                    if (other.tileType != TileType.Open || other.tileColor != tileColor)
                        right = false;
                    else {
                        horizontal++;
                        if (horizontal >= 3)
                            return true;
                    }
                }
            }

            return false;
        }

        void CalculateEffects() {
            var baseColor = Color.white;
            var color = baseColor;
            var sprite = sprites[(int)tileColor];
            //var localScale = new Vector3(1, 1, 1);
            switch (tileViewState) {
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
                    //localScale = new Vector3(chosenScale, chosenScale, chosenScale);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (tileType == TileType.Border) {
                color = new Color(0, 0, 0, 0);
            }

            spriteRenderer.color = color;
            spriteRenderer.sprite = sprite;
            //transform.localScale = localScale;
        }

        public void SetFromTilePersistData(TilePersistData data) {
            SetColor(data.TileColor);
            SetTileType(data.TileType);
        }
    }
}
