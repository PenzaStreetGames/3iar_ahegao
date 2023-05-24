using Level.TileEntity;
using UnityEngine;

namespace Level.EventQueue.Events {
    public class TileCreatingEvent : LevelEvent, IGameEvent {
        public Tile Tile;
        public TilePersistData TileData;
        public new float Delay = 0.05f;

        public TileCreatingEvent(Tile tile, TilePersistData tileData) {
            base.Delay = Delay;
            Tile = tile;
            TileData = tileData;
        }

        public new void Release() {
            var colors = new[] { TileColor.Red, TileColor.Blue, TileColor.Green, TileColor.Yellow };
            var (i, j) = (Tile.position.X, Tile.position.Y);
            var fieldController = Tile.fieldController;
            fieldController.Tiles[i, j].SetTileType(TileData.TileType);

            fieldController.soundSource.Stop();
            fieldController.soundSource.PlayOneShot(fieldController.createSound);

            if (TileData.TileColor == TileColor.None) {
                var colorIndex = Random.Range(0, colors.Length);
                Tile.SetColor(colors[colorIndex]);
                while (Tile.HaveCombinations(fieldController.Tiles)) {
                    colorIndex = (colorIndex + 1) % colors.Length;
                    Tile.SetColor(colors[colorIndex]);
                }
            }
            else {
                Tile.SetFromTilePersistData(TileData);
            }
        }

        public new GameEventType GetType() {
            return GameEventType.TileCreating;
        }
    }
}
