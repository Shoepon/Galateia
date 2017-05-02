using MMF.DeviceManager;
using SlimDX.Direct3D10;
using D3D = SlimDX.Direct3D11;
using D2D = SlimDX.Direct2D;
using DW = SlimDX.DirectWrite;


namespace Galateia.Infra.Graphics
{
    public class GraphicDeviceManager : BasicGraphicDeviceManager
    {
        private D3D.BlendState customBlendState = null;
        private D2D.Factory d2dFactory;

        private DW.Factory dwFactory;

        /// <summary>
        ///     新しいインスタンスを初期化します．
        /// </summary>
        public GraphicDeviceManager() : base()
        {
        }

        /// <summary>
        ///     Direct2Dファクトリを取得します．
        /// </summary>
        public D2D.Factory D2DFactory
        {
            get { return d2dFactory; }
        }

        /// <summary>
        ///     DirectWriteファクトリを取得します．
        /// </summary>
        public DW.Factory DWFactory
        {
            get { return dwFactory; }
        }

        /// <summary>
        ///     デバイスを初期化します．
        /// </summary>
        public new void Load()
        {
            base.Load(false, D3D.DeviceCreationFlags.BgraSupport, DeviceCreationFlags.BgraSupport);

            //ラスタライザの設定を上書き
            var blendDesc = new D3D.BlendStateDescription();
            blendDesc.AlphaToCoverageEnable = true; // Alpha to Coverage を有効にする
            blendDesc.IndependentBlendEnable = false;
            for (int i = 0; i < blendDesc.RenderTargets.Length; i++)
            {
                blendDesc.RenderTargets[i].BlendEnable = true;
                blendDesc.RenderTargets[i].SourceBlend = D3D.BlendOption.SourceAlpha;
                blendDesc.RenderTargets[i].DestinationBlend = D3D.BlendOption.InverseSourceAlpha;
                blendDesc.RenderTargets[i].BlendOperation = D3D.BlendOperation.Add;
                blendDesc.RenderTargets[i].SourceBlendAlpha = D3D.BlendOption.One;
                blendDesc.RenderTargets[i].DestinationBlendAlpha = D3D.BlendOption.One; // ここを１にしないとアルファの合成が変
                blendDesc.RenderTargets[i].BlendOperationAlpha = D3D.BlendOperation.Add;
                blendDesc.RenderTargets[i].RenderTargetWriteMask = D3D.ColorWriteMaskFlags.All;
            }

            customBlendState = Context.OutputMerger.BlendState = D3D.BlendState.FromDescription(Device, blendDesc);

            d2dFactory = new D2D.Factory(D2D.FactoryType.Multithreaded);
            dwFactory = new DW.Factory(DW.FactoryType.Shared);
        }

        /// <summary>
        ///     デバイスを破棄します．
        /// </summary>
        public override void Dispose()
        {
            d2dFactory.Dispose();
            dwFactory.Dispose();
            customBlendState.Dispose();

            base.Dispose();
        }
    }
}