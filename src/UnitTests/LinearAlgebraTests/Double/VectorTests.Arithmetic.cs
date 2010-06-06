﻿// <copyright file="VectorTests.Arithmetic.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
// Copyright (c) 2009-2010 Math.NET
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.UnitTests.LinearAlgebraTests.Double
{
    using System;
    using LinearAlgebra.Double;
    using MbUnit.Framework;

    public abstract partial class VectorTests
    {
        [Test]
        public void CanCallPlus()
        {
            var vector = this.CreateVector(this._data);
            var other = vector.Plus();
            Assert.AreSame(vector, other, "Should be the same vector");
        }

        [Test]
        public void OperatorPlusThrowsArgumentNullExceptionWhenCallOnNullVector()
        {
            Vector vector = null;
            Vector other = null;
            Assert.Throws<ArgumentNullException>(() => other = +vector);
        }

        [Test]
        public void CanCallUnaryPlusOperator()
        {
            var vector = this.CreateVector(this._data);
            var other = +vector;
            Assert.AreSame(vector, other, "Should be the same vector");
        }

        [Test]
        [MultipleAsserts]
        public void CanAddScalarToVector()
        {
            var vector = this.CreateVector(this._data);
            vector.Add(2.0);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] + 2.0, vector[i]);
            }

            vector.Add(0.0);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] + 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanAddScalarToVectorUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Add(2.0, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] + 2.0, result[i]);
            }

            vector.Add(0.0, result);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], result[i]);
            }
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenAddingScalarWithNullResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            Assert.Throws<ArgumentNullException>(() => vector.Add(0.0, null));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenAddingScalarWithWrongSizeResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            var result = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Add(0.0, result));
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenAddingTwoVectorsAndOneIsNull()
        {
            var vector = this.CreateVector(this._data);
            Assert.Throws<ArgumentNullException>(() => vector.Add(null));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenAddingTwoVectorsOfDifferingSize()
        {
            var vector = this.CreateVector(this._data.Length);
            var other = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Add(other));
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenAddingTwoVectorsAndResultIsNull()
        {
            var vector = this.CreateVector(this._data.Length);
            var other = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentNullException>(() => vector.Add(other, null));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenAddingTwoVectorsAndResultIsDifferentSize()
        {
            var vector = this.CreateVector(this._data.Length);
            var other = this.CreateVector(this._data.Length);
            var result = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Add(other, result));
        }

        [Test]
        public void AdditionOperatorThrowsArgumentNullExpectionIfAVectorIsNull()
        {
            Vector a = null;
            var b = this.CreateVector(this._data.Length);
            Assert.Throws<ArgumentNullException>(() => a += b);

            a = b;
            b = null;
            Assert.Throws<ArgumentNullException>(() => a += b);
        }

        [Test]
        public void AdditionOperatorThrowsArgumentExpectionIfVectorsAreDifferentSize()
        {
            var a = this.CreateVector(this._data.Length);
            var b = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => a += b);
        }

        [Test]
        public void CanAddTwoVectors()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            vector.Add(other);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanAddTwoVectorsUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Add(other, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i], other[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] * 2.0, result[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanAddTwoVectorsUsingOperator()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            var result = vector + other;

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i], other[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] * 2.0, result[i]);
            }
        }

        [Test]
        public void CanAddVectorToItself()
        {
            var vector = this.CreateVector(this._data);
            vector.Add(vector);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanAddVectorToItselfUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Add(vector, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] * 2.0, result[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanAddTwoVectorsUsingItselfAsResultVector()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            vector.Add(other, vector);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], other[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }
        }

        [Test]
        public void CanCallNegate()
        {
            var vector = this.CreateVector(this._data);
            var other = vector.Negate();
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(-this._data[i], other[i]);
            }
        }

        [Test]
        public void OperatorNegateThrowsArgumentNullExceptionWhenCallOnNullVector()
        {
            Vector vector = null;
            Vector other = null;
            Assert.Throws<ArgumentNullException>(() => other = -vector);
        }

        [Test]
        public void CanCallUnaryNegationOperator()
        {
            var vector = this.CreateVector(this._data);
            var other = -vector;
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(-this._data[i], other[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanSubtractScalarFromVector()
        {
            var vector = this.CreateVector(this._data);
            vector.Subtract(2.0);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] - 2.0, vector[i]);
            }

            vector.Subtract(0.0);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] - 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanSubtractScalarFromVectorUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Subtract(2.0, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] - 2.0, result[i]);
            }

            vector.Subtract(0.0, result);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], result[i]);
            }
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenSubtractingScalarWithNullResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            Assert.Throws<ArgumentNullException>(() => vector.Subtract(0.0, null));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenSubtractingScalarWithWrongSizeResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            var result = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Subtract(0.0, result));
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenSubtractingTwoVectorsAndOneIsNull()
        {
            var vector = this.CreateVector(this._data);
            Assert.Throws<ArgumentNullException>(() => vector.Subtract(null));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenSubtractingTwoVectorsOfDifferingSize()
        {
            var vector = this.CreateVector(this._data.Length);
            var other = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Subtract(other));
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenSubtractingTwoVectorsAndResultIsNull()
        {
            var vector = this.CreateVector(this._data.Length);
            var other = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentNullException>(() => vector.Subtract(other, null));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenSubtractingTwoVectorsAndResultIsDifferentSize()
        {
            var vector = this.CreateVector(this._data.Length);
            var other = this.CreateVector(this._data.Length);
            var result = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Subtract(other, result));
        }

        [Test]
        public void SubtractionOperatorThrowsArgumentNullExpectionIfAVectorIsNull()
        {
            Vector a = null;
            var b = this.CreateVector(this._data.Length);
            Assert.Throws<ArgumentNullException>(() => a -= b);

            a = b;
            b = null;
            Assert.Throws<ArgumentNullException>(() => a -= b);
        }

        [Test]
        public void SubtractionOperatorThrowsArgumentExpectionIfVectorsAreDifferentSize()
        {
            var a = this.CreateVector(this._data.Length);
            var b = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => a -= b);
        }

        [Test]
        public void CanSubtractTwoVectors()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            vector.Subtract(other);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(0.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanSubtractTwoVectorsUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Subtract(other, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i], other[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(0.0, result[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanSubtractTwoVectorsUsingOperator()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            var result = vector - other;

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i], other[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(0.0, result[i]);
            }
        }

        [Test]
        public void CanSubtractVectorFromItself()
        {
            var vector = this.CreateVector(this._data);
            vector.Subtract(vector);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(0.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanSubtractVectorFromItselfUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Subtract(vector, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(0.0, result[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanSubtractTwoVectorsUsingItselfAsResultVector()
        {
            var vector = this.CreateVector(this._data);
            var other = this.CreateVector(this._data);
            vector.Subtract(other, vector);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], other[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(0.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanDivideVectorByScalar()
        {
            var vector = this.CreateVector(this._data);
            vector.Divide(2.0);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] / 2.0, vector[i]);
            }

            vector.Divide(1.0);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] / 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanDivideVectorByScalarUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Divide(2.0, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] / 2.0, result[i]);
            }

            vector.Divide(1.0, result);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], result[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanMultiplyVectorByScalar()
        {
            var vector = this.CreateVector(this._data);
            vector.Multiply(2.0);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }

            vector.Multiply(1.0);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanMultiplyVectorByScalarUsingResultVector()
        {
            var vector = this.CreateVector(this._data);
            var result = this.CreateVector(this._data.Length);
            vector.Multiply(2.0, result);

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], vector[i], "Making sure the original vector wasn't modified.");
                Assert.AreEqual(this._data[i] * 2.0, result[i]);
            }

            vector.Multiply(1.0, result);
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i], result[i]);
            }
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenMultiplyingScalarWithNullResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            Assert.Throws<ArgumentNullException>(() => vector.Multiply(1.0, null));
        }

        [Test]
        public void ThrowsArgumentNullExceptionWhenDividingScalarWithNullResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            Assert.Throws<ArgumentNullException>(() => vector.Divide(1.0, null));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenMultiplyingScalarWithWrongSizeResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            var result = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Multiply(0.0, result));
        }

        [Test]
        public void ThrowsArgumentExceptionWhenDividingScalarWithWrongSizeResultVector()
        {
            var vector = this.CreateVector(this._data.Length);
            var result = this.CreateVector(this._data.Length + 1);
            Assert.Throws<ArgumentException>(() => vector.Divide(0.0, result));
        }

        [Test]
        [MultipleAsserts]
        public void CanMultiplyVectorByScalarUsingOperators()
        {
            var vector = this.CreateVector(this._data);
            vector = vector * 2.0;

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }

            vector = vector * 1.0;
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }

            vector = this.CreateVector(this._data);
            vector = 2.0 * vector;

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }

            vector = 1.0 * vector;
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] * 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void CanDivideVectorByScalarUsingOperators()
        {
            var vector = this.CreateVector(this._data);
            vector = vector / 2.0;

            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] / 2.0, vector[i]);
            }

            vector = vector / 1.0;
            for (var i = 0; i < this._data.Length; i++)
            {
                Assert.AreEqual(this._data[i] / 2.0, vector[i]);
            }
        }

        [Test]
        [MultipleAsserts]
        public void OperatorMultiplyThrowsArgumentNullExceptionWhenVectorIsNull()
        {
            Vector vector = null;
            Vector result = null;
            Assert.Throws<ArgumentNullException>(() => result = vector * 2.0);
            Assert.Throws<ArgumentNullException>(() => result = 2.0 * vector);
        }

        [Test]
        public void OperatorDivideThrowsArgumentNullExceptionWhenVectorIsNull()
        {
            Vector vector = null;
            Assert.Throws<ArgumentNullException>(() => vector = vector / 2.0);
        }

        [Test]
        public void CanDotProduct()
        {
            var dataA = this.CreateVector(this._data);
            var dataB = this.CreateVector(this._data);

            Assert.AreEqual(55.0, dataA.DotProduct(dataB));
        }

        [Test]
        [ExpectedArgumentNullException]
        public void DotProductThrowsExceptionWhenArgumentIsNull()
        {
            var dataA = this.CreateVector(this._data);
            Vector dataB = null;

            dataA.DotProduct(dataB);
        }

        [Test]
        [ExpectedArgumentException]
        public void DotProductThrowsExceptionWhenArgumentHasDifferentSize()
        {
            var dataA = this.CreateVector(this._data);
            var dataB = this.CreateVector(new double[] { 1, 2, 3, 4, 5, 6 });

            dataA.DotProduct(dataB);
        }

        [Test]
        public void CanDotProductUsingOperator()
        {
            var dataA = this.CreateVector(this._data);
            var dataB = this.CreateVector(this._data);

            Assert.AreEqual(55.0, dataA * dataB);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void OperatorDotProductThrowsExceptionWhenLeftArgumentIsNull()
        {
            var dataA = this.CreateVector(this._data);
            Vector dataB = null;

            var d = dataA * dataB;
        }

        [Test]
        [ExpectedArgumentNullException]
        public void OperatorDotProductThrowsExceptionWhenRightArgumentIsNull()
        {
            Vector dataA = null;
            var dataB = this.CreateVector(this._data);

            var d = dataA * dataB;
        }

        [Test]
        [ExpectedArgumentException]
        public void OperatorDotProductThrowsExceptionWhenArgumentHasDifferentSize()
        {
            var dataA = this.CreateVector(this._data);
            var dataB = this.CreateVector(new double[] { 1, 2, 3, 4, 5, 6 });

            var d = dataA * dataB;
        }

        [Test]
        public void PointWiseMultiply()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            var result = CreateVector(vector1.Count);
            vector1.PointWiseMultiply(vector2, result);
            for (var i = 0; i < vector1.Count; i++)
            {
                Assert.AreEqual(this._data[i] * this._data[i], result[i]);
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void PointWiseMultiplyWithOtherNullShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            Vector vector2 = null;
            var result = CreateVector(vector1.Count);
            vector1.PointWiseMultiply(vector2, result);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void PointWiseMultiplyWithResultNullShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            Vector result = null;
            vector1.PointWiseMultiply(vector2, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void PointWiseMultiplyWithInvalidResultLengthShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            var result = this.CreateVector(vector1.Count + 1);
            vector1.PointWiseMultiply(vector2, result);
        }

        [Test]
        public void PointWiseMultiplyWithResult()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            vector1.PointWiseMultiply(vector2);
            for (var i = 0; i < vector1.Count; i++)
            {
                Assert.AreEqual(this._data[i] * this._data[i], vector1[i]);
            }
        }
       
        [Test]
        public void PointWiseDivide()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            var result = CreateVector(vector1.Count);
            vector1.PointWiseDivide(vector2, result);
            for (var i = 0; i < vector1.Count; i++)
            {
                Assert.AreEqual(this._data[i] / this._data[i], result[i]);
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void PointWiseDivideWithOtherNullShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            Vector vector2 = null;
            var result = CreateVector(vector1.Count);
            vector1.PointWiseDivide(vector2, result);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void PointWiseDivideWithResultNullShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            Vector result = null;
            vector1.PointWiseDivide(vector2, result);
        }

        [Test]
        [ExpectedArgumentException]
        public void PointWiseDivideWithInvalidResultLengthShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            var result = this.CreateVector(vector1.Count + 1);
            vector1.PointWiseDivide(vector2, result);
        }

        [Test]
        public void PointWiseDivideWithResult()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = vector1.Clone();
            vector1.PointWiseDivide(vector2);
            for (var i = 0; i < vector1.Count; i++)
            {
                Assert.AreEqual(this._data[i] / this._data[i], vector1[i]);
            }
        }

        [Test]
        public void CanCalculateDyadicProduct()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = this.CreateVector(this._data);
            Matrix m = Vector.DyadicProduct(vector1, vector2);
            for (var i = 0; i < vector1.Count; i++)
            {
                for (var j = 0; j < vector2.Count; j++)
                {
                    Assert.AreEqual(m[i, j], vector1[i] * vector2[j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void DyadicProductWithFirstParameterNullShouldThrowException()
        {
            Vector vector1 = null;
            var vector2 = this.CreateVector(this._data);
            Vector.DyadicProduct(vector1, vector2);
        }

        [Test]
        [ExpectedArgumentNullException]
        public void DyadicProductWithSecondParameterNullShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            Vector vector2 = null;
            Vector.DyadicProduct(vector1, vector2);
        }

        [Test]
        public void CanCalculateTensorMultiply()
        {
            var vector1 = this.CreateVector(this._data);
            var vector2 = this.CreateVector(this._data);
            var m = vector1.TensorMultiply(vector2);
            for (var i = 0; i < vector1.Count; i++)
            {
                for (var j = 0; j < vector2.Count; j++)
                {
                    Assert.AreEqual(m[i, j], vector1[i] * vector2[j]);
                }
            }
        }

        [Test]
        [ExpectedArgumentNullException]
        public void TensorMultiplyWithNullParameterNullShouldThrowException()
        {
            var vector1 = this.CreateVector(this._data);
            Vector vector2 = null;
            vector1.TensorMultiply(vector2);
        }
    }
}