using Level.TileEntity;
using UnityEditor.SceneTemplate;
using UnityEngine;

namespace Level.EventQueue.Events {
    public class TileMovingEvent : LevelEvent, IGameEvent {
        public Tile TileFrom;
        public Tile TileTo;
        public GameObject TilePrefab;

        public TileMovingEvent(Tile tileFrom, Tile tileTo, GameObject tilePrefab) {
            TileFrom = tileFrom;
            TileTo = tileTo;
            TilePrefab = tilePrefab;
        }

        public new void Release() {
            var movingTile = Object.Instantiate(TilePrefab, TileFrom.transform);
            var movingTileScript = movingTile.GetComponent<MovingTile>();
            movingTileScript.SetFields(TileFrom, TileTo);
        }

        public new GameEventType GetType() {
            return GameEventType.TileMoving;
        }
    }
}
