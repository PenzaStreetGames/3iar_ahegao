using Db.Serialization;
using Level;

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
            return JsonArraySerializer.Serialize2DArray(tiles);
        }

        static Tile[,] DecodeTilesMatrix(string encodedString)
        {
            return JsonArraySerializer.Deserialize2DArray<Tile>(encodedString);
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

        public Tile[,] GetDecodedFieldState()
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
