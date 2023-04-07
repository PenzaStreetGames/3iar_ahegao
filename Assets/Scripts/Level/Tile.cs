using UnityEngine;
using UnityEngine.EventSystems;

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

    public class Tile : MonoBehaviour
    {
        public FieldController fieldController;
        public TileColor tileColor;
        public TileType tileType;
        public Vector2 position;
        public bool selected;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}