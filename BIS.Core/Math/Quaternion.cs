#region

using System.IO;

#endregion

namespace BIS.Core.Math
{
    public class Quaternion
    {
        public Quaternion()
        {
            W = 1f;
            X = 0f;
            Y = 0f;
            Z = 0f;
        }

        public Quaternion(float x, float y, float z, float w)
        {
            W = w;
            X = x;
            Y = y;
            Z = z;
        }

        public float X { get; private set; }

        public float Y { get; private set; }

        public float Z { get; private set; }

        public float W { get; private set; }

        public Quaternion Inverse
        {
            get
            {
                Normalize();
                return Conjugate;
            }
        }

        public Quaternion Conjugate => new Quaternion(-X, -Y, -Z, W);

        public static Quaternion ReadCompressed(BinaryReader input)
        {
            float x = (float)(-input.ReadInt16() / 16384d);
            float y = (float)(input.ReadInt16() / 16384d);
            float z = (float)(-input.ReadInt16() / 16384d);
            float w = (float)(input.ReadInt16() / 16384d);

            return new Quaternion(x, y, z, w);
        }

        public static Quaternion operator *(Quaternion a, Quaternion b)
        {
            float w = a.W * b.W - a.X * b.X - a.Y * b.Y - a.Z * b.Z;
            float x = a.W * b.X + a.X * b.W + a.Y * b.Z - a.Z * b.Y;
            float y = a.W * b.Y - a.X * b.Z + a.Y * b.W + a.Z * b.X;
            float z = a.W * b.Z + a.X * b.Y - a.Y * b.X + a.Z * b.W;
            return new Quaternion(x, y, z, w);
        }

        public void Normalize()
        {
            float n = (float)(1 / System.Math.Sqrt(X * X + Y * Y + Z * Z + W * W));
            X *= n;
            Y *= n;
            Z *= n;
            W *= n;
        }

        public Vector3P Transform(Vector3P xyz)
        {
            Quaternion vQ = new Quaternion(xyz.X, xyz.Y, xyz.Z, 0);
            Quaternion vQnew = this * vQ * Inverse;
            return new Vector3P(vQnew.X, vQnew.Y, vQnew.Z);
        }

        /// <summary>
        ///     for unit quaternions only?
        /// </summary>
        /// <returns></returns>
        public Matrix3P AsRotationMatrix()
        {
            Matrix3P rotMatrix = new Matrix3P();

            double xy = X * Y;
            double wz = W * Z;
            double wx = W * X;
            double wy = W * Y;
            double xz = X * Z;
            double yz = Y * Z;
            double zz = Z * Z;
            double yy = Y * Y;
            double xx = X * X;
            rotMatrix[0, 0] = (float)(1 - 2 * (yy + zz)); //1-2y2-2z2// need .997
            rotMatrix[0, 1] = (float)(2 * (xy - wz)); //2xy-2wz     -0.033
            rotMatrix[0, 2] = (float)(2 * (xz + wy)); ////  2xz+2wy//0.063
            rotMatrix[1, 0] = (float)(2 * (xy + wz)); //2xy+2wz  0.024
            rotMatrix[1, 1] = (float)(1 - 2 * (xx + zz)); //1-2x2-2z2
            rotMatrix[1, 2] = (float)(2 * (yz - wx)); //2yz+2wx////////////////
            rotMatrix[2, 0] = (float)(2 * (xz - wy)); //2xz-2wy
            rotMatrix[2, 1] = (float)(2 * (yz + wx)); //2yz-2wx/////////
            rotMatrix[2, 2] = (float)(1 - 2 * (xx + yy)); //1-2x2-2y2

            return rotMatrix;
        }
    }
}