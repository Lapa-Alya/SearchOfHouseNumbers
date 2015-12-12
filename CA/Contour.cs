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
    }
}