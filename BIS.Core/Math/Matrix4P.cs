#region

using BIS.Core.Streams;

#endregion

namespace BIS.Core.Math
{
    /// <summary>
    ///     Layout:
    ///     [m00, m01, m02, m03]
    ///     [m10, m11, m12, m13]
    ///     [m20, m21, m22, m23]
    ///     [ 0 , 0  , 0  , 1  ]
    /// </summary>
    public struct Matrix4P
    {
        private Matrix3P orientation;
        private Vector3P position;

        public Matrix3P Orientation => orientation;
        public Vector3P Position => position;

        public float this[int row, int col]
        {
            get => col == 3 ? position[row] : orientation[col][row];

            set
            {
                if (col == 3)
                    position[row] = value;
                else
                    orientation[row, col] = value;
            }
        }

        public Matrix4P(float val) : this(new Matrix3P(val), new Vector3P(val))
        {
        }

        public Matrix4P(BinaryReaderEx input) : this(new Matrix3P(input), new Vector3P(input))
        {
        }

        private Matrix4P(Matrix3P orientation, Vector3P position)
        {
            this.orientation = orientation;
            this.position = position;
        }

        public static Matrix4P operator *(Matrix4P a, Matrix4P b)
        {
            Matrix4P res = new Matrix4P();

            float x = b[0, 0];
            float y = b[1, 0];
            float z = b[2, 0];
            res[0, 0] = a[0, 0] * x + a[0, 1] * y + a[0, 2] * z;
            res[1, 0] = a[1, 0] * x + a[1, 1] * y + a[1, 2] * z;
            res[2, 0] = a[2, 0] * x + a[2, 1] * y + a[2, 2] * z;

            x = b[0, 1];
            y = b[1, 1];
            z = b[2, 1];
            res[0, 1] = a[0, 0] * x + a[0, 1] * y + a[0, 2] * z;
            res[1, 1] = a[1, 0] * x + a[1, 1] * y + a[1, 2] * z;
            res[2, 1] = a[2, 0] * x + a[2, 1] * y + a[2, 2] * z;

            x = b[0, 2];
            y = b[1, 2];
            z = b[2, 2];
            res[0, 2] = a[0, 0] * x + a[0, 1] * y + a[0, 2] * z;
            res[1, 2] = a[1, 0] * x + a[1, 1] * y + a[1, 2] * z;
            res[2, 2] = a[2, 0] * x + a[2, 1] * y + a[2, 2] * z;

            x = b.position.X;
            y = b.position.Y;
            z = b.position.Z;
            res.position.X = a[0, 0] * x + a[0, 1] * y + a[0, 2] * z + a.position.X;
            res.position.Y = a[1, 0] * x + a[1, 1] * y + a[1, 2] * z + a.position.Y;
            res.position.Z = a[2, 0] * x + a[2, 1] * y + a[2, 2] * z + a.position.Z;

            return res;
        }

        public static Matrix4P ReadMatrix4Quat16b(BinaryReaderEx input)
        {
            Quaternion quat = Quaternion.ReadCompressed(input);
            ShortFloat x = new ShortFloat(input.ReadUInt16());
            ShortFloat y = new ShortFloat(input.ReadUInt16());
            ShortFloat z = new ShortFloat(input.ReadUInt16());

            return new Matrix4P(quat.AsRotationMatrix(), new Vector3P(x, y, z));
        }

        public void Write(BinaryWriterEx output)
        {
            Orientation.Write(output);
            Position.Write(output);
        }

        public override string ToString()
        {
            return
                $@"{this[0, 0]}, {this[0, 1]}, {this[0, 2]}, {this[0, 3]},
{this[1, 0]}, {this[1, 1]}, {this[1, 2]}, {this[1, 3]},
{this[2, 0]}, {this[2, 1]}, {this[2, 2]}, {this[2, 3]},
{this[3, 0]}, {this[3, 1]}, {this[3, 2]}, 1";
        }
    }
}