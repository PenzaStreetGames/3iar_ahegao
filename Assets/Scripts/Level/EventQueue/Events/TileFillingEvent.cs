using Level.EventQueue.Events;
using Level.TileEntity;
using Utils;

namespace Level.EventQueue {
    public class TileFillingEvent : LevelEvent, IGameEvent {
        public Tile Tile;
        public new float Delay = 0.2f;

        public TileFillingEvent(Tile tile) {
            base.Delay = Delay;
            Tile = tile;
        }

        public new void Release() {
            FieldController fieldController = FieldController.Instance;
            LevelEventQueue levelEventQueue = LevelEventQueue.Instance;

            var (x, y) = (Tile.position.X, Tile.position.Y);
            var tile = fieldController.Tiles[x, y];
            fieldController.RandomFillEmptyTile(tile);
            var tileUnder = fieldController.Tiles[x + 1, y];
            if (tileUnder.tileType == TileType.Open && tileUnder.tileColor == TileColor.None) {
                var tileFallingEvent = new TileFallingEvent(tile);
                levelEventQueue.Enqueue(tileFallingEvent, tileFallingEvent.Delay);
                var tileMovingEvent = new TileMovingEvent(tile, tileUnder, fieldController.movingTilePrefab);
                levelEventQueue.Enqueue(tileMovingEvent, 0f);
            }
        }

        public new GameEventType GetType() {
            return GameEventType.TileFilling;
        }
    }
}
