using System.Collections.Generic;
using MMF.Model.PMX;
using MMF.Motion;

namespace Galateia.Ghost.Demeanor
{
    /// <summary>
    ///     モーションを管理します
    /// </summary>
    public class ModelMotionManager
    {
        private readonly PMXModel model;

        /// <summary>
        ///     モーションと名前の連想辞書
        /// </summary>
        private readonly Dictionary<string, IMotionProvider> motions = new Dictionary<string, IMotionProvider>();

        /// <summary>
        ///     管理オブジェクトを初期化します．
        /// </summary>
        /// <param name="model">モーションを関連付けるモデル．</param>
        public ModelMotionManager(PMXModel model)
        {
            this.model = model;
        }

        /// <summary>
        ///     指定した名前のモーションを取得します．
        /// </summary>
        /// <param name="name">モーション名．</param>
        /// <returns>モーションプロバイダ．</returns>
        public IMotionProvider this[string name]
        {
            get { return motions[name]; }
        }

        /// <summary>
        ///     名前とモーションファイルを指定して，モーションを読み込みます．
        /// </summary>
        /// <param name="name">モーション名．</param>
        /// <param name="path">ファイルのパス．</param>
        public void LoadMotion(string name, string path)
        {
            IMotionProvider motion = model.MotionManager.AddMotionFromFile(path, false);
            motions.Add(name, motion);
        }
    }
}