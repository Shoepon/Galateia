using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Galateia.CSC;
using MMDFileParser.PMXModelParser;
using MMF.Bone;
using MMF.Model.PMX;
using SlimDX;

namespace Galateia.Ghost.Demeanor
{
    public class ModelData : IDisposable
    {
        private bool _disposed;

        public string Name { get; private set; }
        public PMXModel Model { get; private set; }
        public ModelMotionManager MotionManager { get; private set; }
        public CubeF ModelCube { get; private set; }

        private readonly PMXBone _baloonReferenceBone;
        private readonly PMXBone _centerReferenceBone;

        public Vector3 BaloonReferencePoint
        {
            get
            {
                return Vector3.TransformCoordinate(_baloonReferenceBone.Position, _baloonReferenceBone.GlobalPose) +
                       Model.Transformer.Position;
            }
        }

        public Vector3 CenterReferencePoint
        {
            get
            {
                return Vector3.TransformCoordinate(_centerReferenceBone.Position, _centerReferenceBone.GlobalPose) +
                      Model.Transformer.Position;
            }
        }

        public ModelData(string name, PMXModel model, IShell shell)
        {
            Name = name;
            Model = model;
            shell.LoadModel(model);
            MotionManager = new ModelMotionManager(model);
            ModelCube = CalculateModelCube(model);
            _baloonReferenceBone = model.Skinning.BoneDictionary["頭"];
            _centerReferenceBone = model.Skinning.BoneDictionary["下半身"];
        }

        ~ModelData()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Model.Dispose();
                Model = null;
                MotionManager = null;

                _disposed = true;
            }
        }

        /// <summary>
        ///     モデルの初期状態における外接直方体を計算します．
        /// </summary>
        /// <param name="model">対称のモデル．</param>
        /// <returns>外接直方体．</returns>
        private static CubeF CalculateModelCube(PMXModel model)
        {
            VertexList list = model.Model.VertexList;

            float left, top, right, bottom, front, back;
            VertexData first = list.Vertexes.First();
            left = right = first.Position.X;
            top = bottom = first.Position.Y;
            front = back = first.Position.Z;

            foreach (VertexData v in list.Vertexes)
            {
                left = v.Position.X < left ? v.Position.X : left;
                top = top < v.Position.Y ? v.Position.Y : top;
                right = right < v.Position.X ? v.Position.X : right;
                bottom = v.Position.Y < bottom ? v.Position.Y : bottom;
                front = v.Position.Z < front ? v.Position.Z : front;
                back = back < v.Position.Z ? v.Position.Z : back;
            }

            return new CubeF
            {
                X = left,
                Width = right - left,
                Y = bottom,
                Height = top - bottom,
                Z = front,
                Depth = back - front
            };
        }
    }
}
