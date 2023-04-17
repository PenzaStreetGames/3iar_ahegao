using System;
using System.Collections.Generic;

namespace Utils {
    public class Pair<T, TU> : IComparable<Pair<T, TU>>, IEquatable<Pair<T, TU>>
        where T : IComparable<T>
        where TU : IComparable<TU> {
        T X { get; }
        TU Y { get; }

        public Pair(T x, TU y) {
            X = x;
            Y = y;
        }

        public int CompareTo(Pair<T, TU> other) {
            var firstComparision = X.CompareTo(other.X);

            return firstComparision != 0 ? firstComparision : Y.CompareTo(other.Y);
        }

        public bool Equals(Pair<T, TU> other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return EqualityComparer<T>.Default.Equals(X, other.X) && EqualityComparer<TU>.Default.Equals(Y, other.Y);
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            return obj.GetType() == this.GetType() && Equals((Pair<T, TU>)obj);
        }

        public override int GetHashCode() {
            return HashCode.Combine(X, Y);
        }

        public static bool operator <(Pair<T, TU> left, Pair<T, TU> right) {
            return left.CompareTo(right) < 0;
        }

        public static bool operator >(Pair<T, TU> left, Pair<T, TU> right) {
            return left.CompareTo(right) > 0;
        }

        public static bool operator ==(Pair<T, TU> left, Pair<T, TU> right) {
            if (left is not null) {
                return left.Equals(right);
            }

            return right is null;
        }

        public static bool operator !=(Pair<T, TU> left, Pair<T, TU> right) {
            return !(left == right);
        }

        public static bool operator <=(Pair<T, TU> left, Pair<T, TU> right) {
            return left < right || left == right;
        }

        public static bool operator >=(Pair<T, TU> left, Pair<T, TU> right) {
            return left > right || left == right;
        }

        public static Pair<T, TU> operator +(Pair<T, TU> left, Pair<T, TU> right) {
            dynamic leftX = left.X;
            dynamic leftY = left.Y;
            dynamic rightX = right.X;
            dynamic rightY = right.Y;

            return new Pair<T, TU>(leftX + leftY, rightX + rightY);
        }

        public static Pair<T, TU> operator -(Pair<T, TU> left, Pair<T, TU> right) {
            dynamic leftX = left.X;
            dynamic leftY = left.Y;
            dynamic rightX = right.X;
            dynamic rightY = right.Y;

            return new Pair<T, TU>(leftX - leftY, rightX - rightY);
        }

        public static long operator ^(Pair<T, TU> left, Pair<T, TU> right) {
            dynamic leftX = left.X;
            dynamic leftY = left.Y;
            dynamic rightX = right.X;
            dynamic rightY = right.Y;

            return Math.Abs(leftX - rightX) + Math.Abs(leftY - rightY);
        }
    }
}
