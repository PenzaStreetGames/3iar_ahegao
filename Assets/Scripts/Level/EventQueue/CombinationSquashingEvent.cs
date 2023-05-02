using System.Collections.Generic;
using Level.TileEntity;
using UnityEngine.UIElements;
using Utils;

namespace Level.EventQueue {
    public class CombinationSquashingEvent : LevelEvent, IGameEvent {
        public HashSet<Tile> Tiles;
        public float Delay = 0.2f;

        public CombinationSquashingEvent(HashSet<Tile> tiles) {
            Tiles = tiles;
        }

        public new void Release() {
            LevelController levelController = LevelController.Instance;
            FieldController fieldController = FieldController.Instance;
            LevelEventQueue levelEventQueue = LevelEventQueue.Instance;

            levelController.IncreaseScoreForCombination(Tiles.Count);
            levelController.IncreaseDestroyedTilesCounter(Tiles.Count);
            foreach (var tile in Tiles) {
                var (x, y) = (tile.position.X, tile.position.Y);
                if (tile.tileType == TileType.Open && tile.tileColor != TileColor.None) {
                    tile.SetColor(TileColor.None);
                }
                if (!tile.InTopLayer()) {
                    var tileAbove = fieldController.Tiles[x - 1, y];
                    if (tileAbove.tileType == TileType.Open && tileAbove.tileColor != TileColor.None) {
                        var tileFallingEvent = new TileFallingEvent(tileAbove);
                        levelEventQueue.Enqueue(tileFallingEvent, tileFallingEvent.Delay);
                    }

                }
                else {
                    var tileFillingEvent = new TileFillingEvent(tile);
                    levelEventQueue.Enqueue(tileFillingEvent, tileFillingEvent.Delay);
                }
            }
        }

        public new GameEventType GetType() {
            return GameEventType.CombinationSquashing;
        }
    }
}