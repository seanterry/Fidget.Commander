/*  Copyright 2017 Sean Terry

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

        http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License. 
*/

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