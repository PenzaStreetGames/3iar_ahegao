using Level.TileEntity;
using Utils;

namespace Level.EventQueue {
    public class TileFillingEvent : LevelEvent, IGameEvent {
        public Tile Tile;
        public float Delay = 0.5f;

        public TileFillingEvent(Tile tile) {
            Tile = tile;
        }

        public new void Release() {
            FieldController fieldController = FieldController.Instance;

            var (x, y) = (Tile.position.X, Tile.position.Y);
            var tile = fieldController.Tiles[x, y];
            fieldController.RandomFillEmptyTile(tile);
        }

        public new GameEventType GetType() {
            return GameEventType.TileFilling;
        }
    }
}
