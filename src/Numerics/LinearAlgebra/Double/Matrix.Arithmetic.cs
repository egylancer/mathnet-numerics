﻿// <copyright file="Matrix.Arithmetic.cs" company="Math.NET">
// Math.NET Numerics, part of the Math.NET Project
// http://numerics.mathdotnet.com
// http://github.com/mathnet/mathnet-numerics
// http://mathnetnumerics.codeplex.com
//
// Copyright (c) 2009-2010 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

namespace MathNet.Numerics.LinearAlgebra.Double
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Properties;
    using Threading;

    /// <summary>
    /// Defines the base class for <c>Matrix</c> classes.
    /// </summary>
    public abstract partial class Matrix
    {
        /// <summary>
        /// Adds another matrix to this matrix. The result will be written into this matrix.
        /// </summary>
        /// <param name="other">The matrix to add to this matrix.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        public virtual void Add(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (other.RowCount != this.RowCount || other.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMatrixDimensions);
            }

            CommonParallel.For(
                0,
                this.RowCount,
                i =>
                {
                    for (var j = 0; j < this.ColumnCount; j++)
                    {
                        this.At(i, j, this.At(i, j) + other.At(i, j));
                    }
                });
        }

        /// <summary>
        /// Subtracts another matrix from this matrix. The result will be written into this matrix.
        /// </summary>
        /// <param name="other">The matrix to subtract.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If the two matrices don't have the same dimensions.</exception>
        public virtual void Subtract(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (other.RowCount != this.RowCount || other.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMatrixDimensions);
            }

            CommonParallel.For(
                0,
                this.RowCount,
                i =>
                {
                    for (var j = 0; j < this.ColumnCount; j++)
                    {
                        this.At(i, j, this.At(i, j) - other.At(i, j));
                    }
                });
        }

        /// <summary>
        /// Multiplies each element of this matrix with a scalar.
        /// </summary>
        /// <param name="scalar">The scalar to multiply with.</param>
        public virtual void Multiply(double scalar)
        {
            if (1.0.AlmostEqualInDecimalPlaces(scalar, 15))
            {
                return;
            }

            CommonParallel.For(
                0,
                this.RowCount,
                i =>
                {
                    for (var j = 0; j < this.ColumnCount; j++)
                    {
                        this.At(i, j, this.At(i, j) * scalar);
                    }
                });
        }

        /// <summary>
        /// Multiplies each element of the matrix by a scalar and places results into the result matrix.
        /// </summary>
        /// <param name="scalar">The scalar to multiply the matrix with.</param>
        /// <param name="result">The matrix to multiply.</param>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void Multiply(double scalar, Matrix result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != this.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "result");
            }

            if (result.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension, "result");
            }

            this.CopyTo(result);
            result.Multiply(scalar);
        }

        /// <summary>
        /// Multiplies this matrix by a vector and returns the result.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If rightSide is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <c>this.ColumnCount != rightSide.Count</c>.</exception>
        public virtual Vector Multiply(Vector rightSide)
        {
            var ret = this.CreateVector(this.RowCount);
            Multiply(rightSide, ret);
            return ret;
        }

        /// <summary>
        /// Multiplies this matrix with a vector and places the results into the result matrix.
        /// </summary>
        /// <param name="rightSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>result.Count != this.RowCount</strong>.</exception>
        /// <exception cref="ArgumentException">If <strong>this.ColumnCount != rightSide.Count</strong>.</exception>
        public virtual void Multiply(Vector rightSide, Vector result)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (this.ColumnCount != rightSide.Count)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "rightSide");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (this.RowCount != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            if (ReferenceEquals(rightSide, result))
            {
                var tmp = result.CreateVector(result.Count);
                Multiply(rightSide, tmp);
                tmp.CopyTo(result);
            }
            else
            {
                CommonParallel.For(
                    0,
                    this.RowCount,
                    i =>
                    {
                        double s = 0;
                        for (var j = 0; j != this.ColumnCount; j++)
                        {
                            s += this.At(i, j) * rightSide[j];
                        }

                        result[i] = s;
                    });               
            }
        }

        /// <summary>
        /// Left multiply a matrix with a vector ( = vector * matrix ).
        /// </summary>
        /// <param name="leftSide">The vector to multiply with.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>this.RowCount != leftSide.Count</strong>.</exception>
        public virtual Vector LeftMultiply(Vector leftSide)
        {
            var ret = this.CreateVector(this.ColumnCount);
            this.LeftMultiply(leftSide, ret);
            return ret;
        }

        /// <summary>
        /// Left multiply a matrix with a vector ( = vector * matrix ) and place the result in the result vector.
        /// </summary>
        /// <param name="leftSide">The vector to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If leftSide is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>result.Count != this.ColumnCount</strong>.</exception>
        /// <exception cref="ArgumentException">If <strong>this.RowCount != leftSide.Count</strong>.</exception>
        public virtual void LeftMultiply(Vector leftSide, Vector result)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (this.RowCount != leftSide.Count)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "leftSide");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (this.ColumnCount != result.Count)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            if (ReferenceEquals(leftSide, result))
            {
                var tmp = result.CreateVector(result.Count);
                this.LeftMultiply(leftSide, tmp);
                tmp.CopyTo(result);
            }
            else
            {
                CommonParallel.For(
                    0,
                    this.RowCount,
                    j =>
                    {
                        double s = 0;
                        for (var i = 0; i != leftSide.Count; i++)
                        {
                            s += leftSide[i] * this.At(i, j);
                        }

                        result[j] = s;
                    });               
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and places the results into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <param name="result">The result of the multiplication.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>this.Columns != other.Rows</strong>.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the this.Rows x other.Columns.</exception>
        public virtual void Multiply(Matrix other, Matrix result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (this.ColumnCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            if (result.RowCount != this.RowCount || result.ColumnCount != other.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            if (ReferenceEquals(this, result) || ReferenceEquals(other, result))
            {
                var tmp = result.CreateMatrix(result.RowCount, result.ColumnCount);
                Multiply(other, tmp);
                tmp.CopyTo(result);
            }
            else
            {
                CommonParallel.For(
                    0,
                    this.RowCount,
                    j =>
                    {
                        for (var i = 0; i != other.ColumnCount; i++)
                        {
                            double s = 0;
                            for (var l = 0; l < this.ColumnCount; l++)
                            {
                                s += this.At(j, l) * other.At(l, i);
                            }

                            result.At(j, i, s);
                        }
                    });               
            }
        }

        /// <summary>
        /// Multiplies this matrix with another matrix and returns the result.
        /// </summary>
        /// <param name="other">The matrix to multiply with.</param>
        /// <exception cref="ArgumentException">If <strong>this.Columns != other.Rows</strong>.</exception>        
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception>
        /// <returns>The result of the multiplication.</returns>
        public virtual Matrix Multiply(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (this.ColumnCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            var result = this.CreateMatrix(this.RowCount, other.ColumnCount);
            Multiply(other, result);
            return result;
        }

        /// <summary>
        /// Negates each element of this matrix.
        /// </summary>        
        public virtual void Negate()
        {
            this.Multiply(-1);
        }

        /// <summary>
        /// Negate each element of this matrix and place the results into the result matrix.
        /// </summary>
        /// <param name="result">The result of the negation.</param>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">if the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void Negate(Matrix result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != this.RowCount || result.ColumnCount != this.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            this.CopyTo(result);
            result.Negate();
        }

        /// <summary>
        /// Adds two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to add.</param>
        /// <param name="rightSide">The right matrix to add.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Matrix operator +(Matrix leftSide, Matrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.RowCount != rightSide.RowCount || leftSide.ColumnCount != rightSide.ColumnCount)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMatrixDimensions);
            }

            var ret = leftSide.Clone();
            ret.Add(rightSide);
            return ret;
        }

        /// <summary>
        /// Returns a <strong>Matrix</strong> containing the same values of rightSide. 
        /// </summary>
        /// <param name="rightSide">The matrix to get the values from.</param>
        /// <returns>A matrix containing a the same values as <paramref name="rightSide"/>.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Matrix operator +(Matrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.Clone();
        }

        /// <summary>
        /// Subtracts two matrices together and returns the results.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to subtract.</param>
        /// <param name="rightSide">The right matrix to subtract.</param>
        /// <returns>The result of the addition.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="leftSide"/> and <paramref name="rightSide"/> don't have the same dimensions.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Matrix operator -(Matrix leftSide, Matrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (leftSide.RowCount != rightSide.RowCount || leftSide.ColumnCount != rightSide.ColumnCount)
            {
                throw new ArgumentOutOfRangeException(Resources.ArgumentMatrixDimensions);
            }

            var ret = leftSide.Clone();
            ret.Subtract(rightSide);
            return ret;
        }

        /// <summary>
        /// Negates each element of the matrix.
        /// </summary>
        /// <param name="rightSide">The matrix to negate.</param>
        /// <returns>A matrix containing the negated values.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Matrix operator -(Matrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            var ret = rightSide.Clone();
            ret.Negate();
            return ret;
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> is <see langword="null" />.</exception>
        public static Matrix operator *(Matrix leftSide, double rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            var ret = leftSide.Clone();
            ret.Multiply(rightSide);
            return ret;
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> by a constant and returns the result.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The constant to multiply the matrix by.</param>
        /// <returns>The result of the multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Matrix operator *(double leftSide, Matrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            var ret = rightSide.Clone();
            ret.Multiply(leftSide);
            return ret;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <remarks>This operator will allocate new memory for the result. It will
        /// choose the representation of either <paramref name="leftSide"/> or <paramref name="rightSide"/> depending on which
        /// is denser.</remarks>
        /// <param name="leftSide">The left matrix to multiply.</param>
        /// <param name="rightSide">The right matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the dimensions of <paramref name="leftSide"/> or <paramref name="rightSide"/> don't conform.</exception>
        public static Matrix operator *(Matrix leftSide, Matrix rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            if (leftSide.ColumnCount != rightSide.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions);
            }

            return leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <strong>Matrix</strong> and a <see cref="Vector"/>.
        /// </summary>
        /// <param name="leftSide">The matrix to multiply.</param>
        /// <param name="rightSide">The vector to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector operator *(Matrix leftSide, Vector rightSide)
        {
            if (leftSide == null)
            {
                throw new ArgumentNullException("leftSide");
            }

            return leftSide.Multiply(rightSide);
        }

        /// <summary>
        /// Multiplies a <see cref="Vector"/> and a <strong>Matrix</strong>.
        /// </summary>
        /// <param name="leftSide">The vector to multiply.</param>
        /// <param name="rightSide">The matrix to multiply.</param>
        /// <returns>The result of multiplication.</returns>
        /// <exception cref="ArgumentNullException">If <paramref name="leftSide"/> or <paramref name="rightSide"/> is <see langword="null" />.</exception>
        public static Vector operator *(Vector leftSide, Matrix rightSide)
        {
            if (rightSide == null)
            {
                throw new ArgumentNullException("rightSide");
            }

            return rightSide.LeftMultiply(leftSide);
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <returns>A new matrix that is the pointwise multiplication of this matrix and <paramref name="other"/>.</returns>
        public virtual Matrix PointwiseMultiply(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            Matrix result = CreateMatrix(RowCount, ColumnCount);
            PointwiseMultiply(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise multiplies this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise multiply with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise multiplication.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this matrix and <paramref name="result"/> are not the same size.</exception>
        public virtual void PointwiseMultiply(Matrix other, Matrix result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            if (ColumnCount != result.ColumnCount || RowCount != result.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.At(i, j, At(i, j) * other.At(i, j));
                    }
                });
        }

        /// <summary>
        /// Pointwise add this matrix with another matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise add with this one.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <returns>A new matrix that is the pointwise addition of this matrix and <paramref name="other"/>.</returns>
        public virtual Matrix PointwiseAdd(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            Matrix result = CreateMatrix(RowCount, ColumnCount);
            PointwiseAdd(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise adds this matrix with another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise add with this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise addition.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this matrix and <paramref name="result"/> are not the same size.</exception>
        public virtual void PointwiseAdd(Matrix other, Matrix result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            if (ColumnCount != result.ColumnCount || RowCount != result.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.At(i, j, At(i, j) + other.At(i, j));
                    }
                });
        }

        /// <summary>
        /// Pointwise subtract another matrix from this matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise subtract from this one.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <returns>A new matrix that is the pointwise subtraction of this matrix and <paramref name="other"/>.</returns>
        public virtual Matrix PointwiseSubtract(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            Matrix result = CreateMatrix(RowCount, ColumnCount);
            PointwiseSubtract(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise subtract another matrix from this matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise subtract from this one.</param>
        /// <param name="result">The matrix to store the result of the pointwise addition.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this matrix and <paramref name="result"/> are not the same size.</exception>
        public virtual void PointwiseSubtract(Matrix other, Matrix result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            if (ColumnCount != result.ColumnCount || RowCount != result.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.At(i, j, At(i, j) - other.At(i, j));
                    }
                });
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise subtract this one by.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <returns>A new matrix that is the pointwise division of this matrix and <paramref name="other"/>.</returns>
        public virtual Matrix PointwiseDivide(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            Matrix result = CreateMatrix(RowCount, ColumnCount);
            PointwiseDivide(other, result);
            return result;
        }

        /// <summary>
        /// Pointwise divide this matrix by another matrix and stores the result into the result matrix.
        /// </summary>
        /// <param name="other">The matrix to pointwise divide this one by.</param>
        /// <param name="result">The matrix to store the result of the pointwise division.</param>
        /// <exception cref="ArgumentNullException">If the other matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception> 
        /// <exception cref="ArgumentException">If this matrix and <paramref name="other"/> are not the same size.</exception>
        /// <exception cref="ArgumentException">If this matrix and <paramref name="result"/> are not the same size.</exception>
        public virtual void PointwiseDivide(Matrix other, Matrix result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            if (ColumnCount != result.ColumnCount || RowCount != result.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.At(i, j, At(i, j) / other.At(i, j));
                    }
                });
        }

        /// <summary>
        /// Generates matrix with random elements.
        /// </summary>
        /// <param name="numberOfRows">Number of rows.</param>
        /// <param name="numberOfColumns">Number of columns.</param>
        /// <param name="distribution">Continuous Random Distribution or Source</param>
        /// <returns>
        /// An numberOfRows-by-numberOfColumns matrix with elements distributed according to the provided distribution.
        /// </returns>
        /// <exception cref="ArgumentException">If the parameter numberOfRows is not positive.</exception>
        /// <exception cref="ArgumentException">If the parameter numberOfColumns is not positive.</exception>
        public virtual Matrix Random(int numberOfRows, int numberOfColumns, MathNet.Numerics.Distributions.IContinuousDistribution distribution)
        {
            if (numberOfRows < 1)
            {
                throw new ArgumentException("numberOfRows", Resources.ArgumentMustBePositive);
            }

            if (numberOfColumns < 1)
            {
                throw new ArgumentException("numberOfColumns", Resources.ArgumentMustBePositive);
            }

            var matrix = CreateMatrix(numberOfRows, numberOfColumns);
            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    for (int i = 0; i < matrix.RowCount; i++)
                    {
                        matrix[i, j] = distribution.Sample();
                    }
                });

            return matrix;
        }

        /// <summary>
        /// Generates matrix with random elements.
        /// </summary>
        /// <param name="numberOfRows">Number of rows.</param>
        /// <param name="numberOfColumns">Number of columns.</param>
        /// <param name="distribution">Continuous Random Distribution or Source</param>
        /// <returns>
        /// An numberOfRows-by-numberOfColumns matrix with elements distributed according to the provided distribution.
        /// </returns>
        /// <exception cref="ArgumentException">If the parameter numberOfRows is not positive.</exception>
        /// <exception cref="ArgumentException">If the parameter numberOfColumns is not positive.</exception>
        public virtual Matrix Random(int numberOfRows, int numberOfColumns, MathNet.Numerics.Distributions.IDiscreteDistribution distribution)
        {
            if (numberOfRows < 1)
            {
                throw new ArgumentException("numberOfRows", Resources.ArgumentMustBePositive);
            }

            if (numberOfColumns < 1)
            {
                throw new ArgumentException("numberOfColumns", Resources.ArgumentMustBePositive);
            }

            var matrix = CreateMatrix(numberOfRows, numberOfColumns);
            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    for (int i = 0; i < matrix.RowCount; i++)
                    {
                        matrix[i, j] = distribution.Sample();
                    }
                });

            return matrix;
        }

        /// <summary>
        ///  Concatenates this matrix with the given matrix.
        /// </summary>
        /// <param name="right">The matrix to concatenate.</param>
        /// <returns>The combined matrix.</returns>
        public virtual Matrix Append(Matrix right)
        {
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.RowCount != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension);
            }

            Matrix result = CreateMatrix(RowCount, ColumnCount + right.ColumnCount);
            Append(right, result);
            return result;
        }

        /// <summary>
        /// Concatenates this matrix with the given matrix and places the result into the result matrix.
        /// </summary>
        /// <param name="right">The matrix to concatenate.</param>
        /// <param name="result">The combined matrix.</param>
        public virtual void Append(Matrix right, Matrix result)
        {
            if (right == null)
            {
                throw new ArgumentNullException("right");
            }

            if (right.RowCount != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension);
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.ColumnCount != (ColumnCount + right.ColumnCount) || result.RowCount != RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameColumnDimension);
            }

            CommonParallel.Invoke(
                () =>
                {
                    CommonParallel.For(
                       0,
                       this.RowCount,
                       i =>
                       {
                           CommonParallel.For(0, this.ColumnCount, j => result.At(i, j, At(i, j)));
                       });
                },
                () =>
                {
                    CommonParallel.For(
                       0,
                       right.RowCount,
                       i =>
                       {
                           CommonParallel.For(0, right.ColumnCount, j => result.At(i, j + ColumnCount, right.At(i, j)));
                       });                    
               });
        }

        /// <summary>
        /// Stacks this matrix on top of the given matrix and places the result into the result matrix.
        /// </summary>
        /// <param name="lower">The matrix to stack this matrix upon.</param>
        /// <returns>The combined matrix.</returns>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>upper.Columns != lower.Columns</strong>.</exception>
        public virtual Matrix Stack(Matrix lower)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            if (lower.ColumnCount != ColumnCount)
            {
                throw new ArgumentException("lower", Resources.ArgumentMatrixSameColumnDimension);
            }

            Matrix result = CreateMatrix(RowCount + lower.RowCount, ColumnCount);
            Stack(lower, result);
            return result;
        }

        /// <summary>
        /// Stacks this matrix on top of the given matrix and places the result into the result matrix.
        /// </summary>
        /// <param name="lower">The matrix to stack this matrix upon.</param>
        /// <param name="result">The combined matrix.</param>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If <strong>upper.Columns != lower.Columns</strong>.</exception>
        public virtual void Stack(Matrix lower, Matrix result)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }
            
            if (lower.ColumnCount != ColumnCount)
            {
                throw new ArgumentException("lower", Resources.ArgumentMatrixSameColumnDimension);
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != (RowCount + lower.RowCount) || result.ColumnCount != ColumnCount)
            {
                throw new ArgumentException("result", Resources.ArgumentMatrixDimensions);
            }

            CommonParallel.Invoke(
                () =>
                {
                    CommonParallel.For(
                        0,
                        this.RowCount,
                        i =>
                        {
                            CommonParallel.For(0, this.ColumnCount, j => result.At(i, j, At(i, j)));
                        });
                },
                () =>
                {
                    CommonParallel.For(
                        0,
                        lower.RowCount,
                        i =>
                        {
                            CommonParallel.For(0, lower.ColumnCount, j => result.At(i + RowCount, j, lower.At(i, j)));
                        });
                });
        }

        /// <summary>
        /// Computes the trace of this matrix.
        /// </summary>
        /// <returns>The trace of this matrix</returns>
        /// <exception cref=">ArgumentException">If the matrix is not square</exception>
        public virtual double Trace()
        {
            if (RowCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSquare);
            }

            double t = 0.0;
            for (int i = 0; i < RowCount; i++)
            {
                t += this[i, i];
            }

            return t;
        }

        /// <summary>
        /// Diagonally stacks his matrix on top of the given matrix. The new matrix is a M-by-N matrix, 
        /// where M = this.Rows + lower.Rows and N = this.Columns + lower.Columns.
        /// The values of off the off diagonal matrices/blocks are set to zero.
        /// </summary>
        /// <param name="lower">The lower, right matrix.</param>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <returns>the combined matrix</returns>
        public virtual Matrix DiagonalStack(Matrix lower)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            Matrix result = CreateMatrix(RowCount + lower.RowCount, ColumnCount + lower.ColumnCount);
            DiagonalStack(lower, result);
            return result;
        }

        /// <summary>
        /// Diagonally stacks his matrix on top of the given matrix and places the combined matrix into the result matrix.
        /// </summary>
        /// <param name="lower">The lower, right matrix.</param>
        /// <param name="result">The combined matrix</param>
        /// <exception cref="ArgumentNullException">If lower is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not (this.Rows + lower.rows) x (this.Columns + lower.Columns).</exception>
        public virtual void DiagonalStack(Matrix lower, Matrix result)
        {
            if (lower == null)
            {
                throw new ArgumentNullException("lower");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount + lower.RowCount || result.ColumnCount != ColumnCount + lower.ColumnCount)
            {
                throw new ArgumentException("result", Resources.ArgumentMatrixDimensions);
            }

            CommonParallel.Invoke(
                () =>
                {
                    CommonParallel.For(
                        0,
                        this.RowCount,
                        i =>
                        {
                            CommonParallel.For(0, this.ColumnCount, j => result.At(i, j, At(i, j)));
                        });
                },               
                () =>
                {
                    CommonParallel.For(
                        0,
                        lower.RowCount,
                        i =>
                        {
                            CommonParallel.For(0, lower.ColumnCount, j => result.At(i + RowCount, j + ColumnCount, lower.At(i, j)));
                        });
                });
        }

        /// <summary>
        /// Calculates the rank of the matrix
        /// </summary>
        /// <returns>effective numerical rank, obtained from SVD</returns>
        public virtual int Rank()
        {
            throw new NotImplementedException();
        }

        /// <summary>Calculates the condition number of this matrix.</summary>
        /// <returns>The condition number of the matrix.</returns>
        /// <remarks>The condition number is calculated using singular value decomposition.</remarks>
        public virtual double ConditionNumber()
        {
            throw new NotImplementedException();
        }

        /// <summary>Computes the determinant of this matrix.</summary>
        /// <returns>The determinant of this matrix.</returns>
        public virtual double Determinant()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Computes the Kronecker product of this matrix with the given matrix. The new matrix is M-by-N
        /// with M = this.Rows * lower.Rows and N = this.Columns * lower.Columns.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <exception cref="ArgumentNullException">If other is <see langword="null" />.</exception>
        /// <returns>The kronecker product of the two matrices.</returns>
        public virtual Matrix KroneckerProduct(Matrix other)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            Matrix result = CreateMatrix(RowCount * other.RowCount, ColumnCount * other.ColumnCount);
            KroneckerProduct(other, result);
            return result;
        }
        
        /// <summary>
        /// Computes the Kronecker product of this matrix with the given matrix. The new matrix is M-by-N
        /// with M = this.Rows * lower.Rows and N = this.Columns * lower.Columns.
        /// </summary>
        /// <param name="other">The other matrix.</param>
        /// <param name="result">The kronecker product of the two matrices.</param>
        /// <exception cref="ArgumentNullException">If other is <see langword="null" />.</exception>
        /// <exception cref="ArgumentNullException">If the result matrix is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not (this.Rows * lower.rows) x (this.Columns * lower.Columns).</exception>
        public virtual void KroneckerProduct(Matrix other, Matrix result)
        {
            if (other == null)
            {
                throw new ArgumentNullException("other");
            }

            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != (RowCount * other.RowCount) || result.ColumnCount != (ColumnCount * other.ColumnCount))
            {
                throw new ArgumentException("result", Resources.ArgumentMatrixDimensions);
            }

            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    for (int i = 0; i < RowCount; i++)
                    {
                        result.SetSubMatrix(i * other.RowCount, other.RowCount, j * other.ColumnCount, other.ColumnCount, At(i, j) * other);
                    }
                });
        }

        /// <summary>
        /// Normalizes the columns of a matrix.
        /// </summary>
        /// <param name="p">The norm under which to normalize the columns under.</param>
        /// <returns>A normalized version of the matrix.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the parameter p is not positive.</exception>
        public virtual Matrix NormalizeColumns(int p)
        {
            if (p < 1)
            {
                throw new ArgumentOutOfRangeException("p", Resources.ArgumentMustBePositive);
            }

            Matrix ret = Clone();
            CommonParallel.For(
                0,
                ColumnCount,
                i =>
                {
                    Vector coli = GetColumn(i);
                    double norm = coli.NormP(p);
                    for (int j = 0; j < RowCount; j++)
                    {
                        ret[j, i] = coli[j] / norm;
                    }
                });
            return ret;
        }

        /// <summary>
        /// Normalizes the rows of a matrix.
        /// </summary>
        /// <param name="p">The norm under which to normalize the rows under.</param>
        /// <returns>A normalized version of the matrix.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If the parameter p is not positive.</exception>
        public virtual Matrix NormalizeRows(int p)
        {
            if (p < 1)
            {
                throw new ArgumentOutOfRangeException("p", Resources.ArgumentMustBePositive);
            }

            Matrix ret = Clone();
            CommonParallel.For(
                0,
                ColumnCount,
                j =>
                {
                    Vector rowj = GetRow(j);
                    double norm = rowj.NormP(p);
                    for (int i = 0; i < RowCount; i++)
                    {
                        ret[i, j] = rowj[j] / norm;
                    }
                });

            return ret;
        }

        /// <summary>
        /// Returns an <see cref="IEnumerator{T}"/> that enumerates over the matrix columns.
        /// </summary>
        /// <returns>An <see cref="IEnumerator{T}"/> that enumerates over the matrix columns</returns>
        /// <seealso cref="IEnumerator{T}"/>
    }
}