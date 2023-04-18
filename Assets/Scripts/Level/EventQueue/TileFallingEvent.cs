using Level.TileEntity;
using Utils;

namespace Level.EventQueue {
    public class TileFallingEvent : LevelEvent, IGameEvent{
        public Tile Tile;
        public float Delay = 0.2f;

        public TileFallingEvent(Tile tile) {
            Tile = tile;
        }

        public new void Release() {
            FieldController fieldController = FieldController.Instance;
            LevelEventQueue levelEventQueue = LevelEventQueue.Instance;
            var (x, y) = (Tile.position.X, Tile.position.Y);
            var tile = fieldController.Tiles[x, y];
            fieldController.ShiftTileDown(tile);
            if (x > 0) {
                var tileAbove = fieldController.Tiles[x - 1, y];
                var nextFallingEvent = new TileFallingEvent(tileAbove);
                levelEventQueue.Enqueue(nextFallingEvent, Delay);
            }
            else if (x == 0) {
                var tileFillingEvent = new TileFillingEvent(tile);
                levelEventQueue.Enqueue(tileFillingEvent, tileFillingEvent.Delay);
            }
            if (x < fieldController.FieldSize.X - 2) {
                var tileUnder2 = fieldController.Tiles[x + 2, y];
                if (tileUnder2.tileType == TileType.Open && tileUnder2.tileColor == TileColor.None) {
                    var tileUnder = fieldController.Tiles[x + 1, y];
                    var tileUnderFallingEvent = new TileFallingEvent(tileUnder);
                    levelEventQueue.Enqueue(tileUnderFallingEvent, tileUnderFallingEvent.Delay);
                }
            }
        }

        public new GameEventType GetType() {
            return GameEventType.TileFalling;
        }
    }
}
