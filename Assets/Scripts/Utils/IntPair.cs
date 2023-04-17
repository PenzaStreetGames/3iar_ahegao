using System;
using System.Collections.Generic;

namespace Utils {
    public class IntPair : IComparable<IntPair>, IEquatable<IntPair> {
        int X { get; }
        int Y { get; }

        public IntPair(int x = 0, int y = 0) {
            X = x;
            Y = y;
        }

        public int CompareTo(IntPair other) {
            var firstComparision = X.CompareTo(other.X);
            return firstComparision != 0 ? firstComparision : Y.CompareTo(other.Y);
        }

        public bool Equals(IntPair other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return EqualityComparer<int>.Default.Equals(X, other.X) && EqualityComparer<int>.Default.Equals(Y, other.Y);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            return obj.GetType() == GetType() && Equals((IntPair)obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }

        public static bool operator <(IntPair left, IntPair right) {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(IntPair left, IntPair right) {
            return left.CompareTo(right) > 0;
        }

        public static bool operator ==(IntPair left, IntPair right) {
            if (left is not null) {
                return left.Equals(right);
            }

            return right is null;
        }

        public static bool operator !=(IntPair left, IntPair right) {
            return !(left == right);
        }

        public static bool operator <=(IntPair left, IntPair right) {
            return left < right || left == right;
        }

        public static bool operator >=(IntPair left, IntPair right) {
            return left > right || left == right;
        }

        public static IntPair operator +(IntPair left, IntPair right) {
            return new IntPair(left.X + right.X, left.Y + right.Y);
        }

        public static IntPair operator -(IntPair left, IntPair right) {
            return new IntPair(left.X - right.X, left.Y - right.Y);
        }

        public int ManhattanDistance(IntPair other) {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }
    }
}
