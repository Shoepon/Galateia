using System.Diagnostics;
using MMFUtil = MMF.Utility;

namespace Galateia.Infra.Graphics
{
    public class FPSCounterWithLimiter : MMFUtil.FPSCounter
    {
        private readonly Stopwatch _stopwatch = new Stopwatch();

        private int _fpsLimit;
        private double _millisecPerFrame;

        public int FpsLimit
        {
            get { return _fpsLimit; }
            set
            {
                _fpsLimit = value;
                _millisecPerFrame = value > 0 ? (1000.0/value) : 0;
            }
        }

        public bool ShouldRender
        {
            get { return _stopwatch.ElapsedMilliseconds >= _millisecPerFrame; }
        }

        public new void Start()
        {
            _stopwatch.Start();
            base.Start();
        }

        public new void CountFrame()
        {
            base.CountFrame();
            _stopwatch.Restart();
        }
    }
}