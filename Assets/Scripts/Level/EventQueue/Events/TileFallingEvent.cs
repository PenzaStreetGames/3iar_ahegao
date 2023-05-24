using Level.EventQueue.Events;
using Level.TileEntity;
using UnityEngine;
using Utils;

namespace Level.EventQueue {
    public class TileFallingEvent : LevelEvent, IGameEvent{
        public Tile Tile;
        public new float Delay = 0.2f;

        public TileFallingEvent(Tile tile) {
            base.Delay = Delay;
            Tile = tile;
        }

        public new void Release() {
            FieldController fieldController = FieldController.Instance;
            LevelEventQueue levelEventQueue = LevelEventQueue.Instance;

            fieldController.soundSource.Stop();
            fieldController.soundSource.PlayOneShot(fieldController.fallSound);

            var (x, y) = (Tile.position.X, Tile.position.Y);
            var tile = fieldController.Tiles[x, y];
            fieldController.ShiftTileDown(tile);

            if (x < fieldController.FieldSize.X - 2) {
                var tileUnder2 = fieldController.Tiles[x + 2, y];
                if (tileUnder2.tileType == TileType.Open && tileUnder2.tileColor == TileColor.None) {
                    var tileUnder = fieldController.Tiles[x + 1, y];
                    var tileUnderFallingEvent = new TileFallingEvent(tileUnder);
                    levelEventQueue.Enqueue(tileUnderFallingEvent, tileUnderFallingEvent.Delay);
                    var tileMovingEvent = new TileMovingEvent(tileUnder, tileUnder2, fieldController.movingTilePrefab);
                    levelEventQueue.Enqueue(tileMovingEvent, 0f);
                }
            }

            if (!Tile.UnderBorder(fieldController.Tiles)) {
                var tileAbove = fieldController.Tiles[x - 1, y];
                var nextFallingEvent = new TileFallingEvent(tileAbove);
                levelEventQueue.Enqueue(nextFallingEvent, Delay);
                var tileMovingEvent = new TileMovingEvent(tileAbove, tile, fieldController.movingTilePrefab);
                levelEventQueue.Enqueue(tileMovingEvent, 0f);
            }
            else {
                var tileFillingEvent = new TileFillingEvent(tile);
                levelEventQueue.Enqueue(tileFillingEvent, tileFillingEvent.Delay);
            }
        }

        public new GameEventType GetType() {
            return GameEventType.TileFalling;
        }
    }
}
