using System;

namespace Galateia.Infra.Config
{
    public class ConfigBase
    {
        /// <summary>
        ///     内容の代入操作が行われ，値が変更された際に発行されます．
        /// </summary>
        public event EventHandler Substituted;

        /// <summary>
        ///     Substitutedイベントを発行します．
        /// </summary>
        public void RaiseSubstituted()
        {
            var s = Substituted;
            if (s != null)
                s(this, new EventArgs());
        }
    }
}