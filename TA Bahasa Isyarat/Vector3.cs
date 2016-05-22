using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace TA_Bahasa_Isyarat
{
    public class Vector3
    {
        private double x;

        public double X
        {
            get { return x; }
            set { x = value; }
        }

        private double y;

        public double Y
        {
            get { return y; }
            set { y = value; }
        }

        private double z;

        public double Z
        {
            get { return z; }
            set { z = value; }
        }


        public Vector3()
        {
            SetVector(0, 0, 0);
        }

        public Vector3(double x, double y, double z)
        {
            SetVector(x, y, z);
        }

        public void SetVector(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void SetVector(SkeletonPoint point)
        {
            this.x = point.X;
            this.y = point.Y;
            this.z = point.Z;
        }

        public double Magnitude()
        {
            return Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }

        public Vector3 Normalize()
        {
            return new Vector3(this.x / this.Magnitude(), this.y / this.Magnitude(), this.z / this.Magnitude());
        }

        public static double DotProduct(Vector3 u, Vector3 v)
        {
            return u.x * v.x + u.y * v.y + u.z * v.z;
        }

        public static Vector3 CrossProduct(Vector3 u, Vector3 v)
        {
            Vector3 crossProduct = new Vector3(u.y * v.z - u.z * v.y,
                                             u.z * v.x - u.x * v.z,
                                             u.x * v.y - u.y * v.x);
            return crossProduct;
        }

        public static double Distance(Vector3 u, Vector3 v)
        {
            return Math.Sqrt((u.x - v.x) * (u.x - v.x) + (u.y - v.y) * (u.y - v.y) + (u.z - v.z) * (u.z - v.z));
        }

        public static Vector3 operator +(Vector3 u, Vector3 v)
        {
            return new Vector3(u.x + v.x, u.y + v.y, u.z + v.z);
        }

        public static Vector3 operator -(Vector3 u, Vector3 v)
        {
            return new Vector3(u.x - v.x, u.y - v.y, u.z - v.z);
        }

        public static Vector3 operator *(double n, Vector3 u)
        {
            return new Vector3(u.x * n, u.y * n, u.z * n);
        }

        public static Vector3 operator *(Vector3 u, double n)
        {
            return new Vector3(u.x * n, u.y * n, u.z * n);
        }

        public static Vector3 operator /(Vector3 u, double n)
        {
            return new Vector3(u.x / n, u.y / n, u.z / n);
        }

        public static Vector3 operator /(double n, Vector3 u)
        {
            return new Vector3(u.x / n, u.y / n, u.z / n);
        }
    }
}

