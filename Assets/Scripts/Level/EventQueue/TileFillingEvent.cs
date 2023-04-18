using Level.TileEntity;
using Utils;

namespace Level.EventQueue {
    public class TileFillingEvent : LevelEvent, IGameEvent {
        public Tile Tile;
        public float Delay = 0.2f;

        public TileFillingEvent(Tile tile) {
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
            }
        }

        public new GameEventType GetType() {
            return GameEventType.TileFilling;
        }
    }
}
