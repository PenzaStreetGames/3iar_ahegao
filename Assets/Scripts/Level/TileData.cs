namespace Level
{
    public class TileData
    {
        int X;
        int Y;
        TileColor TileColor;
        TileType TileType;

        public int GetX() => X;

        public void SetX(int value) => X = value;

        public int GetY() => Y;

        public void SetY(int value) => Y = value;

        public TileColor GetTileColor() => TileColor;

        public void SetTileColor(TileColor value) => TileColor = value;

        public TileType GetTileType() => TileType;

        public void SetTileType(TileType value) => TileType = value;

        public TileData(int x, int y, TileColor tileColor, TileType tileType)
        {
            X = x;
            Y = y;
            TileColor = tileColor;
            TileType = tileType;
        }
    }
}
