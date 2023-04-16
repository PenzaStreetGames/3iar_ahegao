using System;
using Db.Serialization;
using Level.TileEntity;

namespace Db.Entity {
    public class Save {
        readonly string FieldState;
        readonly long LevelId;
        readonly long UserId;

        Save(long levelId, long userId, string fieldState) {
            LevelId = levelId;
            UserId = userId;
            FieldState = fieldState;
        }

        static string EncodeTilesMatrix(Tile[,] tiles) {
            var tileDataMatrix = new char[tiles.GetLength(0), tiles.GetLength(1)];
            for (var i = 0; i < tileDataMatrix.GetLength(0); i++) {
                for (var j = 0; j < tileDataMatrix.GetLength(1); j++) {
                    var tileData = tiles[i, j].GetTileData();
                    tileDataMatrix[i, j] = tileData.TileType switch {
                        TileType.Border => TileType.Border.ToChar(),
                        TileType.Open => tileData.TileColor.ToChar(),
                        TileType.Blocked => TileType.Blocked.ToChar(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                }
            }

            return JsonArraySerializer.Serialize2DArray(tileDataMatrix);
        }

        static TilePersistData[,] DecodeTilesMatrix(string encodedString) {
            var charMatrix = JsonArraySerializer.Deserialize2DArray<char>(encodedString);
            var tilePersistData = new TilePersistData[charMatrix.GetLength(0),charMatrix.GetLength(1)];
            for (var i = 0; i < tilePersistData.GetLength(0); i++) {
                for (var j = 0; j < tilePersistData.GetLength(1); j++) {
                    tilePersistData[i, j] = new TilePersistData {
                        X = i,
                        Y = j,
                        TileColor = TileColorExtensions.FromChar(charMatrix[i, j]),
                        TileType = TileTypeExtensions.FromChar(charMatrix[i, j])
                    };
                }
            }

            return tilePersistData;
        }

        public static Save MakeSaveFromData(string fieldState, long levelId, long userId) {
            return new Save(levelId, userId, fieldState);
        }

        public static Save MakeSaveFromData(Tile[,] tiles, long levelId = -1, long userId = -1) {
            return new Save(levelId, userId, EncodeTilesMatrix(tiles));
        }

        public string GetEncodedFieldState() {
            return FieldState;
        }

        public TilePersistData[,] GetDecodedFieldState() {
            return DecodeTilesMatrix(FieldState);
        }

        public long GetLevelId() {
            return LevelId;
        }

        public long GetUserId() {
            return UserId;
        }
    }
}
