using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace ContourAnalysisNS
{
    /// <summary>
    /// Contour
    /// </summary>
    /// <remarks>Vector of complex numbers</remarks>
    [Serializable]
    public class Contour
    {
        Complex[] array;
        public Rectangle SourceBoundingRect;

        public Contour(int capacity)
        {
            array = new Complex[capacity];
        }

        protected Contour()
        {
        }

        public int Count
        {
            get
            {
                return array.Length;
            }
        }

        public Complex this[int i]
        {
            get { return array[i]; }
            set { array[i] = value; }
        }

        public Contour(IList<Point> points, int startIndex, int count)
            : this(count)
        {
            int minX = points[startIndex].X;
            int minY = points[startIndex].Y;
            int maxX = minX;
            int maxY = minY;
            int endIndex = startIndex + count;

            for (int i = startIndex; i < endIndex; i++)
            {
                var p1 = points[i];
                var p2 = i == endIndex - 1 ? points[startIndex] : points[i + 1];
                array[i] = new Complex(p2.X - p1.X, -p2.Y + p1.Y);

                if (p1.X > maxX) maxX = p1.X;
                if (p1.X < minX) minX = p1.X;
                if (p1.Y > maxY) maxY = p1.Y;
                if (p1.Y < minY) minY = p1.Y;
            }

            SourceBoundingRect = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        public Contour(IList<Point> points)
            : this(points, 0, points.Count)
        {
        }

        public Contour Clone()
        {
            Contour result = new Contour();
            result.array = (Complex[])array.Clone();
            return result;
        }
        /// <summary>
        /// Returns R^2 of difference of norms
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public double DiffR2(Contour c)
        {
            double max1 = 0;
            double max2 = 0;
            double sum = 0;
            for (int i = 0; i < Count; i++)
            {
                double v1 = array[i].Norma;
                double v2 = c.array[i].Norma;
                if (v1 > max1) max1 = v1;
                if (v2 > max2) max2 = v2;
                double v = v1 - v2;
                sum += v * v;
            }
            double max = Math.Max(max1, max2);
            return 1 - sum / Count / max / max;
        }

        public double Norma
        {
            get
            {
                double result = 0;
                foreach (var c in array)
                    result += c.NormaSquare;
                return Math.Sqrt(result);
            }
        }

        /// <summary>
        /// Scalar product
        /// </summary>
        public unsafe Complex Dot(Contour c, int shift)
        {
            var count = Count;
            double sumA = 0;
            double sumB = 0;
            fixed (Complex* ptr1 = &array[0])
            fixed (Complex* ptr2 = &c.array[shift])
            fixed (Complex* ptr22 = &c.array[0])
            fixed (Complex* ptr3 = &c.array[c.Count - 1])
            {
                Complex* p1 = ptr1;
                Complex* p2 = ptr2;
                for (int i = 0; i < count; i++)
                {
                    Complex x1 = *p1;
                    Complex x2 = *p2;
                    sumA += x1.a * x2.a + x1.b * x2.b;
                    sumB += x1.b * x2.a - x1.a * x2.b;

                    p1++;
                    if (p2 == ptr3)
                        p2 = ptr22;
                    else
                        p2++;
                }
            }
            return new Complex(sumA, sumB);
        }
    }
}