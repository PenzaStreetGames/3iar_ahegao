using System;
using Db.Serialization;
using Level.TileEntity;

namespace Db.Entity {
    public class LevelEntity {
        readonly string Level;
        readonly long LevelId;

        LevelEntity(long levelId, string level) {
            LevelId = levelId;
            Level = level;
        }

        static string EncodeTilesMatrix(Tile[,] tiles) {
            var tileDataMatrix = new string[tiles.GetLength(0)];
            for (var i = 0; i < tiles.GetLength(0); i++) {
                var line = "";
                for (var j = 0; j < tiles.GetLength(1); j++) {
                    var tileData = tiles[i, j].GetTileData();
                    var symbol = tileData.TileType switch {
                        TileType.Border => TileType.Border.ToChar(),
                        TileType.Open => TileColor.None.ToChar(),
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

        public static LevelEntity MakeLevelFromData(string level, long levelId) {
            return new LevelEntity(levelId, level);
        }

        public static LevelEntity MakeLevelFromData(Tile[,] tiles, long levelId = -1) {
            return new LevelEntity(levelId, EncodeTilesMatrix(tiles));
        }

        public string GetEncodedLevel() {
            return Level;
        }

        public TilePersistData[,] GetDecodedLevel() {
            return DecodeTilesMatrix(Level);
        }

        public long GetLevelId() {
            return LevelId;
        }
    }
}
