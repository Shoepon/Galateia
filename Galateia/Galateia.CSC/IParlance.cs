using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Galateia.CSC
{
    /// <summary>
    /// 口調を調整する部分のインタフェースです．
    /// </summary>
    public interface IParlance
    {
        /// <summary>
        /// IIntelligenceを実装したクラスのオブジェクトによって出力されたスクリプトを調整します．
        /// </summary>
        /// <param name="script">スクリプトを指定します．</param>
        /// <returns>調整されたスクリプトを返します．</returns>
        string Filter(string script);
    }
}
