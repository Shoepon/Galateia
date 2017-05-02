using SlimDX;

namespace Galateia.Ghost.Demeanor
{
    public struct CubeF
    {
        public float Depth;
        public float Height;
        public float Width;
        public float X;
        public float Y;
        public float Z;

        public Vector3 Center
        {
            get { return new Vector3(X + Width/2, Y + Height/2, Z + Depth/2); }
        }
    }
}