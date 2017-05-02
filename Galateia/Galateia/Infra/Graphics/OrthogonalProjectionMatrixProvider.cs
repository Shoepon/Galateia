using System;
using MMF.Matricies.Projection;
using SlimDX;

namespace Galateia.Infra.Graphics
{
    /// <summary>
    ///     直交変換のカメラ管理クラス
    /// </summary>
    public class OrthogonalProjectionMatrixProvider : IProjectionMatrixProvider
    {
        private float height;
        private Matrix projectionMatrix = Matrix.Identity;
        private float width;
        private float zFar, zNear;

        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.AspectRatio);
            }
        }

        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.AspectRatio);
            }
        }

        /// <summary>
        ///     プロジェクション行列
        /// </summary>
        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        public float AspectRatio
        {
            get { return width/height; }
            set { throw new NotSupportedException(); }
        }

        public float Fovy
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public void InitializeProjection(float width, float height, float znear, float zfar)
        {
            this.width = width;
            this.height = height;
            zNear = znear;
            zFar = zfar;
            UpdateProjection();
        }

        public float ZFar
        {
            get { return zFar; }
            set
            {
                zFar = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.ZFar);
            }
        }

        public float ZNear
        {
            get { return zNear; }
            set
            {
                zNear = value;
                UpdateProjection();
                NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType.ZNear);
            }
        }

        public event EventHandler<ProjectionMatrixChangedEventArgs> ProjectionMatrixChanged;

        /// <summary>
        ///     プロジェクション行列を更新する
        /// </summary>
        private void UpdateProjection()
        {
            projectionMatrix = Matrix.OrthoLH(width, height, zNear, zFar);
        }

        /// <summary>
        ///     プロジェクション行列が変更されたことを通知する．
        /// </summary>
        /// <param name="type">変更されたパラメータ</param>
        private void NotifyProjectMatrixChanged(ProjectionMatrixChangedVariableType type)
        {
            if (ProjectionMatrixChanged != null)
                ProjectionMatrixChanged(this, new ProjectionMatrixChangedEventArgs(type));
        }
    }
}