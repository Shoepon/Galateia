using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphrodite.MeCab
{
    /// <summary>
    /// NMeCab 0.06.4 のコードをそのまま移植
    /// </summary>
    public class MorphologicalAnalyzer : IDisposable
    {
        private bool _disposed = false;
        private readonly MeCabTagger _tagger;

        public MorphologicalAnalyzer(string dictionaryPath)
        {
            _tagger = MeCabTagger.Create(new MeCabParam
            {
                DicDir = dictionaryPath,
                UserDic = new string[0],
                OutputFormatType = "lattice",
                LatticeLevel = MeCabLatticeLevel.One,
                AllMorphs = false
            });
        }

        ~MorphologicalAnalyzer()
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
                _tagger.Dispose();

                _disposed = true;
            }
        }

        public IEnumerable<IMorpheme> Parse(string text)
        {
            for (var node = _tagger.ParseToNode(text); node != null; node = node.Next)
            {
                if (node.Stat == MeCabNodeStat.Nor && !string.IsNullOrEmpty(node.Surface))
                    yield return new Morpheme(node);
            }
        }
    }
}
