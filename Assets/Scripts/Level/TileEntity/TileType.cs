using System;

namespace Level.TileEntity {
    public enum TileType {
        Border = 0,
        Open = 1,
        Blocked = 2
    }

    public static class TileTypeExtensions {
        const char BorderChar = '#';
        const char OpenChar = '*';
        const char BlockedChar = '@';

        public static char ToChar(this TileType tileType) {
            return tileType switch {
                TileType.Border => BorderChar,
                TileType.Open => OpenChar,
                TileType.Blocked => BlockedChar,
                _ => throw new ArgumentOutOfRangeException(nameof(tileType), tileType, null)
            };
        }

        public static TileType FromChar(char type) {
            return type switch {
                BorderChar => TileType.Border,
                OpenChar => TileType.Open,
                BlockedChar => TileType.Blocked,
                _ => TileType.Open
            };
        }
    }
}
