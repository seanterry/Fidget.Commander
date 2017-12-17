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
using Xunit;

namespace Fidget.Commander
{
    /// <summary>
    /// Tests of the unit structure.
    /// </summary>

    public class UnitStructureTests
    {
        /// <summary>
        /// Tests of the non-generic CompareTo methods.
        /// </summary>

        public class CompareTo_NonGeneric
        {
            /// <summary>
            /// Instance under test.
            /// </summary>

            IComparable Instance = new Unit();

            [Fact]
            public void ReturnsZero_WhenComparedToSelf()
            {
                var actual = Instance.CompareTo( Instance );
                Assert.Equal( 0, actual );
            }

            [Fact]
            public void ReturnsZero_WhenComparedToDefault()
            {
                var actual = Instance.CompareTo( Unit.Default );
                Assert.Equal( 0, actual );
            }
        }

        /// <summary>
        /// Tests of the generic CompareTo method.
        /// </summary>

        public class CompareTo_Generic
        {
            /// <summary>
            /// Instance under test.
            /// </summary>

            IComparable<Unit> Instance = new Unit();

            [Fact]
            public void ReturnsZero_WhenComparedToSelf()
            {
                var actual = Instance.CompareTo( (Unit)Instance );
                Assert.Equal( 0, actual );
            }

            [Fact]
            public void ReturnsZero_WhenComparedToDefault()
            {
                var actual = Instance.CompareTo( Unit.Default );
                Assert.Equal( 0, actual );
            }
        }

        /// <summary>
        /// Tests of the non-generic Equals method.
        /// </summary>

        public class Equals_NonGeneric
        {
            /// <summary>
            /// Instance under test.
            /// </summary>

            object Instance = new Unit();

            [Fact]
            public void ReturnsTrue_WhenComparedToSelf()
            {
                var actual = Instance.Equals( Instance );
                Assert.True( actual );
            }

            [Fact]
            public void ReturnsTrue_WhenComparedToDefault()
            {
                var actual = Instance.Equals( Unit.Default );
                Assert.True( actual );
            }

            [Fact]
            public void ReturnsFalse_WhenComparedToOtherType()
            {
                var actual = Instance.Equals( this );
                Assert.False( actual );
            }
        }

        /// <summary>
        /// Tests of the generic Equals operator.
        /// </summary>

        public class Equals_Generic
        {
            /// <summary>
            /// Instance under test.
            /// </summary>

            IEquatable<Unit> Instance = new Unit();

            [Fact]
            public void ReturnsTrue_WhenComparedToSelf()
            {
                var actual = Instance.Equals( (Unit)Instance );
                Assert.True( actual );
            }

            [Fact]
            public void ReturnsTrue_WhenComparedToDefault()
            {
                var actual = Instance.Equals( Unit.Default );
                Assert.True( actual );
            }
        }

        /// <summary>
        /// Tests of the GetHashCode method.
        /// </summary>

        public class GetHashCode_Override
        {
            [Fact]
            public void ReturnsZero()
            {
                var unit = new Unit();
                var actual = unit.GetHashCode();

                Assert.Equal( 0, actual );
            }
        }

        /// <summary>
        /// Tests of the equality operator.
        /// </summary>

        public class EqualityOperator
        {
            /// <summary>
            /// Instance under test.
            /// </summary>

            Unit Instance = new Unit();

            [Fact]
            public void ReturnsTrue_WhenComparedToSelf()
            {
                #pragma warning disable CS1718
                Assert.True( Instance == Instance );
                #pragma warning restore CS1718

                // xunit should also recognize as equal
                Assert.Equal( Instance, Instance );
            }

            [Fact]
            public void ReturnsTrue_WhenComparedToDefault()
            {
                Assert.True( Instance == Unit.Default );
                Assert.Equal( Instance, Unit.Default );
            }
        }

        /// <summary>
        /// Tests of the inequality operator.
        /// </summary>

        public class InequalityOperator
        {
            /// <summary>
            /// Instance under test.
            /// </summary>

            Unit Instance = new Unit();

            [Fact]
            public void ReturnsFalse_WhenComparedToSelf()
            {
                #pragma warning disable CS1718
                Assert.False( Instance != Instance );
                #pragma warning restore CS1718
            }

            [Fact]
            public void ReturnsFalse_WhenComparedToDefault()
            {
                Assert.False( Instance != Unit.Default );
            }
        }
    }
}