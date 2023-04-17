using System;

namespace Level.TileEntity {
    public enum TileColor {
        None = 0,
        Red = 1,
        Blue = 2,
        Green = 3,
        Yellow = 4
    }

    public static class TileColorExtensions {
        const char NoneChar = '!';
        const char RedChar = 'R';
        const char BlueChar = 'B';
        const char GreenChar = 'G';
        const char YellowChar = 'Y';

        public static char ToChar(this TileColor tileColor) {
            return tileColor switch {
                TileColor.None => NoneChar,
                TileColor.Red => RedChar,
                TileColor.Blue => BlueChar,
                TileColor.Green => GreenChar,
                TileColor.Yellow => YellowChar,
                _ => throw new ArgumentOutOfRangeException(nameof(tileColor), tileColor, null)
            };
        }

        public static TileColor FromChar(char colour) {
            return colour switch {
                NoneChar => TileColor.None,
                RedChar => TileColor.Red,
                BlueChar => TileColor.Blue,
                GreenChar => TileColor.Green,
                YellowChar => TileColor.Yellow,
                _ => TileColor.None
            };
        }
    }
}
