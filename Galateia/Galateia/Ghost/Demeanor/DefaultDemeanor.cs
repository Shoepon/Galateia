using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Aphrodite;
using Galateia.CSC;
using MMDFileParser.PMXModelParser;
using MMF.Bone;
using MMF.DeviceManager;
using MMF.Model;
using MMF.Model.PMX;
using MMF.Motion;
using SlimDX;

namespace Galateia.Ghost.Demeanor
{
    public class DefaultDemeanor : IDemeanor
    {
        private readonly static Regex CommandRegex = new Regex(@"^\s*(?<cmd>[a-zA-Z0-9]+)(\s+(?<arg>[^,]+)(\s*,\s*(?<arg>[^,]+))*)?");

        private IShell _shell;
        private readonly ModelDataCollection _models = new ModelDataCollection();
        

        /// <summary>
        /// システムのロードが完了したときに呼び出されます．
        /// </summary>
        /// <param name="shell">シェルのインタフェースを指定します．</param>
        public void Loaded(IShell shell)
        {
            _shell = shell;
        }

        /// <summary>
        /// レンダリング作業の直前に呼び出されます．
        /// </summary>
        /// <param name="rectangles">描画するX-Y平面上の矩形を指定します．</param>
        public void PreRender(out RectangleF rectangles)
        {
            try
            {
                var data = _models.CurrentModelData;
                rectangles = new RectangleF(
                    data.CenterReferencePoint.X - data.ModelCube.Width,
                    data.CenterReferencePoint.Y - data.ModelCube.Height,
                    data.ModelCube.Width*2.0f, data.ModelCube.Height*2.0f
                    );
            }
            catch (InvalidOperationException)
            {
                rectangles = new RectangleF(0, 0, 1, 1);
            }
        }

        /// <summary>
        /// バルーンを表示する際の基準点のワールド座標を取得します．
        /// </summary>
        public PointF BaloonReferencePoint
        {
            get
            {   
                try
                {
                    var p = _models.CurrentModelData.BaloonReferencePoint;
                    return new PointF(p.X, p.Y);
                }
                catch (InvalidOperationException)
                {
                    return PointF.Empty;
                }
            }
        }

        /// <summary>
        /// システムが終了しようとする際に呼び出されます．
        /// </summary>
        public void Disposing()
        {
            foreach (var m in _models)
                m.Dispose();
        }

        /// <summary>
        /// 動作に関するコマンドを受け付けます．
        /// </summary>
        /// <param name="command">入力された文字列を指定します．</param>
        public void Command(string command)
        {
            var match = CommandRegex.Match(command);
            if (!match.Success)
                return;

            var cmd = match.Groups["cmd"].Captures[0].Value;
            var args = match.Groups["arg"].Success ? (from Capture cap in match.Groups["arg"].Captures select cap.Value).ToArray() : new string[0];

            switch (cmd)
            {
                case "load":
                    OnLoad(args);
                    break;
                case "play":
                    OnPlay(args);
                    break;
                case "moveto":
                    OnMoveTo(args);
                    break;
                case "move":
                    OnMove(args);
                    break;
            }
        }

        private void OnLoad(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("load [name],[file path]");

            string name = args[0];
            string path = args[1];

            switch (Path.GetExtension(path))
            {
                case ".vmd":
                    _models.CurrentModelData.MotionManager.LoadMotion(name, path);
                    break;
                case ".pmx":
                    _models.Add(new ModelData(name, PMXModel.FromFile(path), _shell));
                    _shell.LoadModel(_models[name].Model);
                    _shell.WorldSpace.AddResource(_models[name].Model);
                    _models.SetCurrent(name);
                    break;
            }
        }

        private void OnPlay(string[] args)
        {
            string name;
            ActionAfterMotion aam = ActionAfterMotion.Nothing;

            switch (args.Length)
            {
                case 1:
                    name = args[0];
                    break;
                case 2:
                    name = args[0];
                    if (args[1] == "repeat")
                        aam = ActionAfterMotion.Replay;
                    break;
                default:
                    return;
            }

            _models.CurrentModelData.Model.MotionManager.ApplyMotion(_models.CurrentModelData.MotionManager[name], 0, aam);
        }

        private void OnMoveTo(string[] args)
        {
            Vector3 pos = _models.CurrentModelData.Model.Transformer.Position;
            if (args.Length > 0)
                float.TryParse(args[0], out pos.X);
            if (args.Length > 1)
                float.TryParse(args[1], out pos.Y);
            if (args.Length > 2)
                float.TryParse(args[2], out pos.Z);

            _models.CurrentModelData.Model.Transformer.Position = pos;
        }

        private void OnMove(string[] args)
        {
            Vector3 delta = Vector3.Zero;
            if (args.Length > 0)
                float.TryParse(args[0], out delta.X);
            if (args.Length > 1)
                float.TryParse(args[1], out delta.Y);
            if (args.Length > 2)
                float.TryParse(args[2], out delta.Z);

            _models.CurrentModelData.Model.Transformer.Position += delta;
        }
    }
}
