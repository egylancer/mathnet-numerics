﻿// <copyright file="Matrix.cs" company="Math.NET">
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
    using System.Text;
    using Properties;
    using MathNet.Numerics.Threading;

    /// <summary>
    /// Defines the base class for <c>Matrix</c> classes.
    /// </summary>
#if !SILVERLIGHT
    [Serializable]
#endif
    public abstract partial class Matrix : 
#if SILVERLIGHT
   IFormattable, IEquatable<Matrix>
#else
 IFormattable, IEquatable<Matrix>, ICloneable
#endif
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="rows">
        /// The number of rows.
        /// </param>
        /// <param name="columns">
        /// The number of columns.
        /// </param>
        protected Matrix(int rows, int columns)
        {
            if (rows <= 0)
            {
                throw new ArgumentOutOfRangeException(Resources.MatrixRowsMustBePositive);
            }

            if (columns <= 0)
            {
                throw new ArgumentOutOfRangeException(Resources.MatrixColumnsMustBePositive);
            }

            RowCount = rows;
            ColumnCount = columns;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Matrix"/> class.
        /// </summary>
        /// <param name="order">
        /// The order of the matrix.
        /// </param>
        protected Matrix(int order)
        {
            if (order <= 0)
            {
                throw new ArgumentOutOfRangeException(Resources.MatrixRowsOrColumnsMustBePositive);
            }

            RowCount = order;
            ColumnCount = order;
        }

        /// <summary>
        /// Gets the number of columns.
        /// </summary>
        /// <value>The number of columns.</value>
        public virtual int ColumnCount { get; private set; }

        /// <summary>
        /// Gets the number of rows.
        /// </summary>
        /// <value>The number of rows.</value>
        public virtual int RowCount { get; private set; }

        /// <summary>
        /// Gets or sets the value at the given row and column.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <value>The double value to get or set.</value>
        /// <remarks>This method is ranged checked. <see cref="At(int,int)"/> and <see cref="At(int,int,double)"/>
        /// to get and set values without range checking.</remarks>
        public virtual double this[int row, int column]
        {
            get
            {
                RangeCheck(row, column);
                return At(row, column);
            }

            set
            {
                RangeCheck(row, column);
                At(row, column, value);
            }
        }

        /// <summary>
        /// Retrieves the requested element without range checking.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <returns>
        /// The requested element.
        /// </returns>
        public abstract double At(int row, int column);

        /// <summary>
        /// Sets the value of the given element.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        /// <param name="value">
        /// The value to set the element to.
        /// </param>
        public abstract void At(int row, int column, double value);

        /// <summary>
        /// Creates a clone of this instance.
        /// </summary>
        /// <returns>
        /// A clone of the instance.
        /// </returns>
        public virtual Matrix Clone()
        {
            var result = CreateMatrix(RowCount, ColumnCount);
            CopyTo(result);
            return result;
        }

        /// <summary>
        /// Copies the elements of this matrix to the given matrix.
        /// </summary>
        /// <param name="target">
        /// The matrix to copy values into.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If target is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If this and the target matrix do not have the same dimensions..
        /// </exception>
        public virtual void CopyTo(Matrix target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (ReferenceEquals(this, target))
            {
                return;
            }

            if (RowCount != target.RowCount || ColumnCount != target.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "target");
            }

            // TODO this assumes that all entries matter; if "this" is a sparse matrix,
            // we might be able to optimize the copying a bit.
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    target.At(i, j, At(i, j));
                }
            }
        }

        /// <summary>
        /// Creates a <strong>Matrix</strong> for the given number of rows and columns.
        /// </summary>
        /// <param name="numberOfRows">
        /// The number of rows.
        /// </param>
        /// <param name="numberOfColumns">
        /// The number of columns.
        /// </param>
        /// <returns>
        /// A <strong>Matrix</strong> with the given dimensions.
        /// </returns>
        /// <remarks>
        /// Creates a matrix of the same matrix type as the current matrix.
        /// </remarks>
        public abstract Matrix CreateMatrix(int numberOfRows, int numberOfColumns);

        /// <summary>
        /// Creates a <see cref="Vector"/> with a the given dimension.
        /// </summary>
        /// <param name="size">The size of the vector.</param>
        /// <returns>
        /// A <see cref="Vector"/> with the given dimension.
        /// </returns>
        /// <remarks>
        /// Creates a vector of the same type as the current matrix.
        /// </remarks>
        public abstract Vector CreateVector(int size);

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return ToString(null, null);
        }

        /// <summary>
        /// Copies a row into an <see cref="Vector"/>.
        /// </summary>
        /// <param name="index">The row to copy.</param>
        /// <returns>A <see cref="Vector"/> containing the copied elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        public virtual Vector GetRow(int index)
        {
            Vector ret = CreateVector(ColumnCount);
            GetRow(index, 0, ColumnCount, ret);
            return ret;
        }

        /// <summary>
        /// Copies a row into to the given <see cref="Vector"/>.
        /// </summary>
        /// <param name="index">The row to copy.</param>
        /// <param name="result">The <see cref="Vector"/> to copy the row into.</param>
        /// <exception cref="ArgumentNullException">If the result vector is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <b>this.Columns != result.Count</b>.</exception>
        public virtual void GetRow(int index, Vector result)
        {
            GetRow(index, 0, ColumnCount, result);
        }

        /// <summary>
        /// Copies the requested row elements into a new <see cref="Vector"/>.
        /// </summary>
        /// <param name="rowIndex">The row to copy elements from.</param>
        /// <param name="columnIndex">The column to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A <see cref="Vector"/> containing the requested elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</item>
        /// <item><paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</item>
        /// <item><c>(columnIndex + length) &gt;= Columns.</c></item></list></exception>        
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        public virtual Vector GetRow(int rowIndex, int columnIndex, int length)
        {
            Vector ret = CreateVector(length);
            GetRow(rowIndex, columnIndex, length, ret);
            return ret;
        }

        /// <summary>
        /// Copies the requested row elements into a new <see cref="Vector"/>.
        /// </summary>
        /// <param name="rowIndex">The row to copy elements from.</param>
        /// <param name="columnIndex">The column to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="result">The <see cref="Vector"/> to copy the column into.</param>
        /// <exception cref="ArgumentNullException">If the result <see cref="Vector"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>        
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>        
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> + <paramref name="length"/>  
        /// is greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <strong>result.Count &lt; length</strong>.</exception>
        public virtual void GetRow(int rowIndex, int columnIndex, int length, Vector result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (rowIndex >= RowCount || rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (columnIndex >= ColumnCount || columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            if (columnIndex + length > ColumnCount)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            if (result.Count < length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            for (int i = columnIndex, j = 0; i < columnIndex + length; i++, j++)
            {
                result[j] = At(rowIndex, i);
            }
        }

        /// <summary>
        /// Copies a column into a new <see cref="Vector"/>.
        /// </summary>
        /// <param name="index">The column to copy.</param>
        /// <returns>A <see cref="Vector"/> containing the copied elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        public virtual Vector GetColumn(int index)
        {
            Vector result = CreateVector(RowCount);
            GetColumn(index, 0, RowCount, result);
            return result;
        }

        /// <summary>
        /// Copies a column into to the given <see cref="Vector"/>.
        /// </summary>
        /// <param name="index">The column to copy.</param>
        /// <param name="result">The <see cref="Vector"/> to copy the column into.</param>
        /// <exception cref="ArgumentNullException">If the result <see cref="Vector"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is negative,
        /// or greater than or equal to the number of columns.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <b>this.Rows != result.Count</b>.</exception>
        public virtual void GetColumn(int index, Vector result)
        {
            GetColumn(index, 0, RowCount, result);
        }

        /// <summary>
        /// Copies the requested column elements into a new <see cref="Vector"/>.
        /// </summary>
        /// <param name="columnIndex">The column to copy elements from.</param>
        /// <param name="rowIndex">The row to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <returns>A <see cref="Vector"/> containing the requested elements.</returns>
        /// <exception cref="ArgumentOutOfRangeException">If:
        /// <list><item><paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</item>
        /// <item><paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</item>
        /// <item><c>(rowIndex + length) &gt;= Rows.</c></item></list>
        /// </exception>        
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        public virtual Vector GetColumn(int columnIndex, int rowIndex, int length)
        {
            Vector result = CreateVector(length);
            GetColumn(columnIndex, rowIndex, length, result);
            return result;
        }

        /// <summary>
        /// Copies the requested column elements into the given vector.
        /// </summary>
        /// <param name="columnIndex">The column to copy elements from.</param>
        /// <param name="rowIndex">The row to start copying from.</param>
        /// <param name="length">The number of elements to copy.</param>
        /// <param name="result">The <see cref="Vector"/> to copy the column into.</param>
        /// <exception cref="ArgumentNullException">If the result <see cref="Vector"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="columnIndex"/> is negative,
        /// or greater than or equal to the number of columns.</exception>        
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> is negative,
        /// or greater than or equal to the number of rows.</exception>        
        /// <exception cref="ArgumentOutOfRangeException">If <paramref name="rowIndex"/> + <paramref name="length"/>  
        /// is greater than or equal to the number of rows.</exception>
        /// <exception cref="ArgumentException">If <paramref name="length"/> is not positive.</exception>
        /// <exception cref="ArgumentOutOfRangeException">If <strong>result.Count &lt; length</strong>.</exception>
        public virtual void GetColumn(int columnIndex, int rowIndex, int length, Vector result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (columnIndex >= ColumnCount || columnIndex < 0)
            {
                throw new ArgumentOutOfRangeException("columnIndex");
            }

            if (rowIndex >= RowCount || rowIndex < 0)
            {
                throw new ArgumentOutOfRangeException("rowIndex");
            }

            if (rowIndex + length > RowCount)
            {
                throw new ArgumentOutOfRangeException("length");
            }

            if (length < 1)
            {
                throw new ArgumentException(Resources.ArgumentMustBePositive, "length");
            }

            if (result.Count < length)
            {
                throw new ArgumentException(Resources.ArgumentVectorsSameLength, "result");
            }

            for (int i = rowIndex, j = 0; i < rowIndex + length; i++, j++)
            {
                result[j] = At(i, columnIndex);
            }
        }

        /// <summary>
        /// Returns a new matrix containing the lower triangle of this matrix.
        /// </summary>
        /// <returns>The lower triangle of this matrix.</returns>        
        public virtual Matrix GetLowerTriangle()
        {
            Matrix ret = CreateMatrix(RowCount, ColumnCount);
            CommonParallel.For(0, ColumnCount, j =>
            {
                for (int i = j; i < RowCount; i++)
                {
                    ret.At(i, j, At(i, j));
                }
            });
            return ret;
        }

        /// <summary>
        /// Puts the lower triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void GetLowerTriangle(Matrix result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            CommonParallel.For(0, ColumnCount, j =>
            {
                for (int i = 0; i < RowCount; i++)
                {
                    if (i >= j)
                    {
                        result.At(i, j, At(i, j));
                    }
                    else
                    {
                        result.At(i, j, 0);
                    }
                }
            });
        }
        
        /// <summary>
        /// Returns a new matrix containing the upper triangle of this matrix.
        /// </summary>
        /// <returns>The upper triangle of this matrix.</returns>   
        public virtual Matrix GetUpperTriangle()
        {
            Matrix ret = CreateMatrix(RowCount, ColumnCount);
            CommonParallel.For(0, ColumnCount, j =>
            {
                for (int i = 0; i <= j; i++)
                {
                    ret.At(i, j, At(i, j));
                }
            });
            return ret;
        }

        /// <summary>
        /// Puts the upper triangle of this matrix into the result matrix.
        /// </summary>
        /// <param name="result">Where to store the lower triangle.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="result"/> is <see langword="null" />.</exception>
        /// <exception cref="ArgumentException">If the result matrix's dimensions are not the same as this matrix.</exception>
        public virtual void GetUpperTriangle(Matrix result)
        {
            if (result == null)
            {
                throw new ArgumentNullException("result");
            }

            if (result.RowCount != RowCount || result.ColumnCount != ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixDimensions, "result");
            }

            CommonParallel.For(0, ColumnCount, j =>
            {
                for (int i = 0; i < RowCount; i++)
                {
                    if (i <= j)
                    {
                        result.At(i, j, At(i, j));
                    }
                    else
                    {
                        result.At(i, j, 0);
                    }
                }
            });
        }

        #region Implemented Interfaces

#if !SILVERLIGHT
        #region ICloneable

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
#endif

        #region IEquatable<Matrix>

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">
        /// An object to compare with this object.
        /// </param>
        /// <returns>
        /// true if the current object is equal to the <paramref name="other"/> parameter; otherwise, false.
        /// </returns>
        public bool Equals(Matrix other)
        {
            // Reject equality when the argument is null or has a different shape.
            if (other == null)
            {
                return false;
            }

            if (ColumnCount != other.ColumnCount || RowCount != other.RowCount)
            {
                return false;
            }

            // Accept if the argument is the same object as this.
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // If all else fails, perform element wise comparison.
            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    if (At(row, column) != other.At(row, column))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region IFormattable

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">
        /// The format to use.
        /// </param>
        /// <param name="formatProvider">
        /// The format provider to use.
        /// </param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            var stringBuilder = new StringBuilder();
            for (var row = 0; row < RowCount; row++)
            {
                for (var column = 0; column < ColumnCount; column++)
                {
                    stringBuilder.Append(At(row, column).ToString(format, formatProvider));
                    if (column != ColumnCount - 1)
                    {
                        stringBuilder.Append(formatProvider.GetTextInfo().ListSeparator);
                    }
                }

                if (row != RowCount - 1)
                {
                    stringBuilder.Append(Environment.NewLine);
                }
            }

            return stringBuilder.ToString();
        }

        #endregion

        #endregion

        /// <summary>
        /// Ranges the check.
        /// </summary>
        /// <param name="row">
        /// The row of the element.
        /// </param>
        /// <param name="column">
        /// The column of the element.
        /// </param>
        private void RangeCheck(int row, int column)
        {
            if (row < 0 || row >= RowCount)
            {
                throw new ArgumentOutOfRangeException("row");
            }

            if (column < 0 || column >= ColumnCount)
            {
                throw new ArgumentOutOfRangeException("column");
            }
        }

        #region System.Object overrides

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Matrix);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            var hashNum = Math.Min(RowCount * ColumnCount, 25);
            long hash = 0;
            for (var i = 0; i < hashNum; i++)
            {
                var col = i % ColumnCount;
                var row = (i - col) / RowCount;

#if SILVERLIGHT
                hash ^= Precision.DoubleToInt64Bits(this[row, col]);
#else
                hash ^= BitConverter.DoubleToInt64Bits(this[row, col]);
#endif
            }

            return BitConverter.ToInt32(BitConverter.GetBytes(hash), 4);
        }

        #endregion

        /// <summary>
        /// Sets all values to zero.
        /// </summary>
        public virtual void Clear()
        {
            for (int i = 0; i < RowCount; i++)
            {
                for (int j = 0; j < ColumnCount; j++)
                {
                    At(i, j, 0);
                }
            }
        }

        /// <summary>
        /// Returns the transpose of this matrix.
        /// </summary>        
        /// <returns>The transpose of this matrix.</returns>
        public virtual Matrix Transpose()
        {
            Matrix ret = CreateMatrix(ColumnCount, RowCount);
            for (int j = 0; j < ColumnCount; j++)
            {
                for (int i = 0; i < RowCount; i++)
                {
                    ret.At(j, i, At(i, j));
                }
            }

            return ret;
        }
    }
}