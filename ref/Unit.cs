using System;

namespace Fidget.Commander
{
    /// <summary>
    /// Struture that represents a void result (Mimics the type of the same name in languages like Scala).
    /// </summary>

    public struct Unit : IEquatable<Unit>, IComparable<Unit>, IComparable
    {
        /// <summary>
        /// Gets the default instance of the type.
        /// </summary>

        public static Unit Default { get; } = new Unit();

        /// <summary>
        /// Ensures that comparisons to other instances are always equal.
        /// </summary>

        int IComparable<Unit>.CompareTo( Unit other ) => 0;

        /// <summary>
        /// Ensures that comparisons to other instances are always equal.
        /// </summary>

        int IComparable.CompareTo( object obj ) => 0;

        /// <summary>
        /// Ensures that comparisons to other instances are always equal.
        /// </summary>

        bool IEquatable<Unit>.Equals( Unit other ) => true;

        /// <summary>
        /// Ensures that comparisons to other instances are always equal.
        /// </summary>

        public override bool Equals( object obj ) => obj is Unit;

        /// <summary>
        /// Ensures that comparisons to other instances are always equal.
        /// </summary>

        public override int GetHashCode() => 0;

        /// <summary>
        /// Ensures that comparisons to other instances are always equal.
        /// </summary>

        public static bool operator ==( Unit first, Unit second ) => true;

        /// <summary>
        /// Ensures that comparisons to other instances are always equal.
        /// </summary>

        public static bool operator !=( Unit first, Unit second ) => false;
    }
}
