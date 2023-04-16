using Db.Serialization;
using Level.TileEntity;

namespace Db.Entity
{
    public class Save
    {
        readonly string FieldState;
        readonly long LevelId;
        readonly long UserId;

        Save(long levelId, long userId, string fieldState)
        {
            LevelId = levelId;
            UserId = userId;
            FieldState = fieldState;
        }

        Save(string fieldState)
        {
            LevelId = -1;
            UserId = -1;
            FieldState = fieldState;
        }

        static string EncodeTilesMatrix(Tile[,] tiles)
        {
            var tileDataMatrix = new TilePersistData[tiles.GetLength(0), tiles.GetLength(1)];
            for (var i = 0; i < tileDataMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < tileDataMatrix.GetLength(1); j++)
                {
                    tileDataMatrix[i, j] = tiles[i, j].GetTileData();
                }
            }

            return JsonArraySerializer.Serialize2DArray(tileDataMatrix);
        }

        static TilePersistData[,] DecodeTilesMatrix(string encodedString)
        {
            return JsonArraySerializer.Deserialize2DArray<TilePersistData>(encodedString);
        }

        public static Save MakeSaveFromData(long levelId, long userId, Tile[,] tiles)
        {
            return new Save(levelId, userId, EncodeTilesMatrix(tiles));
        }

        public static Save MakeSaveFromData(Tile[,] tiles)
        {
            return new Save(EncodeTilesMatrix(tiles));
        }

        public string GetEncodedFieldState()
        {
            return FieldState;
        }

        public TilePersistData[,] GetDecodedFieldState()
        {
            return DecodeTilesMatrix(FieldState);
        }

        public long GetLevelId()
        {
            return LevelId;
        }

        public long GetUserId()
        {
            return UserId;
        }
    }
}
