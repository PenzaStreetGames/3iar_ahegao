using System;
using Db.Serialization;
using Level.TileEntity;

namespace Db.Entity {
    public class SaveEntity {
        readonly string FieldState;
        readonly long LevelId;
        readonly long UserId;

        SaveEntity(long levelId, long userId, string fieldState) {
            LevelId = levelId;
            UserId = userId;
            FieldState = fieldState;
        }

        static string EncodeTilesMatrix(Tile[,] tiles) {
            var tileDataMatrix = new string[tiles.GetLength(0)];
            for (var i = 0; i < tiles.GetLength(0); i++) {
                var line = "";
                for (var j = 0; j < tiles.GetLength(1); j++) {
                    var tileData = tiles[i, j].GetTileData();
                    var symbol = tileData.TileType switch {
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
            var charMatrix = JsonArraySerializer.DeserializeArray<string>(encodedString);
            var tilePersistData = new TilePersistData[charMatrix.GetLength(0), charMatrix[0].Length];
            for (var i = 0; i < tilePersistData.GetLength(0); i++) {
                var line = charMatrix[i];
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

        public static SaveEntity MakeSaveFromData(string fieldState, long levelId, long userId) {
            return new SaveEntity(levelId, userId, fieldState);
        }

        public static SaveEntity MakeSaveFromData(Tile[,] tiles, long levelId = -1, long userId = -1) {
            return new SaveEntity(levelId, userId, EncodeTilesMatrix(tiles));
        }

        public static SaveEntity MakeSaveFromLevel(LevelEntity level, long userId = -1) {
            return new SaveEntity(
                fieldState: level.GetEncodedLevel(),
                levelId: level.GetLevelId(),
                userId: userId
            );
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
