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
            var tileDataMatrix = new String[tiles.GetLength(0)];
            for (var i = 0; i < tiles.GetLength(0); i++) {
                var line = "";
                for (var j = 0; j < tiles.GetLength(1); j++) {
                    var tileData = tiles[i, j].GetTileData();
                    char symbol = tileData.TileType switch {
                        TileType.Border => TileType.Border.ToChar(),
                        TileType.Open => tileData.TileColor.ToChar(),
                        TileType.Blocked => TileType.Blocked.ToChar(),
                        _ => throw new ArgumentOutOfRangeException()
                    };
                    line += symbol;
                }
                tileDataMatrix[i] = line;
            }
            return JsonArraySerializer.SerializeArray(tileDataMatrix);
        }

        static TilePersistData[,] DecodeTilesMatrix(string encodedString) {
            var charMatrix = JsonArraySerializer.DeserializeArray<String>(encodedString);
            var tilePersistData = new TilePersistData[charMatrix.GetLength(0), charMatrix[0].Length];
            for (var i = 0; i < tilePersistData.GetLength(0); i++) {
                String line = charMatrix[i];
                for (var j = 0; j < tilePersistData.GetLength(1); j++) {
                    tilePersistData[i, j] = new TilePersistData {
                        X = i,
                        Y = j,
                        TileColor = TileColorExtensions.FromChar(line[j]),
                        TileType = TileTypeExtensions.FromChar(line[j])
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
