using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMF.DeviceManager;
using MMF.Model;
using MMF.Model.PMX;

namespace Galateia.CSC
{
    public interface IShell
    {
        WorldSpace WorldSpace { get; }
        void LoadModel(PMXModel model);
    }
}
