using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MMF.Model.PMX;

namespace Galateia.Ghost.Demeanor
{
    public class ModelDataCollection : IEnumerable<ModelData>
    {
        private string _currentModelName;
        private readonly Dictionary<string, ModelData> _models = new Dictionary<string, ModelData>();

        public ModelData this[string name]
        {
            get { return _models[name]; }
        }

        public ModelData CurrentModelData
        {
            get
            {
                try
                {
                    return _models[_currentModelName];
                }
                catch (ArgumentNullException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void SetCurrent(string name)
        {
            _currentModelName = name;
        }

        public void Add(ModelData modelData)
        {
            _models.Add(modelData.Name, modelData);
        }

        public IEnumerator<ModelData> GetEnumerator()
        {
            return _models.Select(pair => pair.Value).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
