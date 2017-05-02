using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Aphrodite.MeCab;
using Galateia.CSC;
using RPN = Aphrodite.ReversePolishNotation;

namespace Aphrodite
{
    public class Intelligence : IIntelligence
    {
        // ReSharper disable ConvertToConstant.Local
        private static readonly string UserDataDirectory =
            // ReSharper restore ConvertToConstant.Local
#if DEBUG
            @"..\..\..\Data";
#else
        @".";
        // Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Aphrodite");
#endif

        private System.Threading.Timer _timer;

        private readonly RPN.Calculator _calculator = new RPN.Calculator();
        private readonly MorphologicalAnalyzer _morphAnalyzer = new MorphologicalAnalyzer(Path.Combine(UserDataDirectory, "dic"));
        private int _numberOfMotions = 0;
        private int _currentMotion = 0;
        private readonly Random _rand = new Random();

        public void Loaded()
        {
            string modelpath = Path.Combine(Directory.EnumerateDirectories(Path.Combine(UserDataDirectory, "shells")).First(), "model.pmx");
            RaiseOutput(@"\?[load 0," + modelpath + "]");

            string motionDir = Path.Combine(UserDataDirectory, "motions");
            Directory.CreateDirectory(motionDir);
            foreach (var file in Directory.EnumerateFiles(motionDir, "*.vmd"))
            {
                RaiseOutput(@"\?[load " + _numberOfMotions + "," + file + "]");
                _numberOfMotions++;
            }

            RaiseOutput(@"\?[moveto 20]");
            if (_numberOfMotions > 0)
            {
                RaiseOutput(@"\?[play " + _currentMotion + ",repeat]");
            }
            RaiseOutput(@"\![show]こんにちは！\w[1]\n\nInsertを押して数式を入力すると\w[1]\n計算が実行されて\n結果がクリップボードにコピーされます\w[2]\n\n文章を打つと形態素解析します\w[2]\n\n「どいて」「出てきて」などと言うと，消えたり出てきたりします．\w[1]\n「右」「左」というと，少し移動します．\w[2]\n\nタスクトレイ アイコンのメニューから設定が変更できます\w[2]\n\n※ 物理演算入れるとすごく遅いので切ってあります");

            // 30秒に一回
            _timer = new System.Threading.Timer(TimerCallback, null, 1000, 1000);
        }

        public void Disposing()
        {
            _timer.Dispose();
            RaiseOutput(@"\![hide]");
        }

        private DateTime _lastRest = DateTime.Now;
        private DateTime _nextBlink = DateTime.Now.AddMinutes(1);
        private readonly Guid _restRestCommand = Guid.NewGuid();
        private bool _rested = false;

        private void TimerCallback(object state)
        {
            bool idling = Idle.Duration.TotalMinutes > 2.5;
            bool needRest = (DateTime.Now - _lastRest).TotalHours >= 1;

            if (!_rested)
                _rested = needRest && idling;

            if (idling)
                return;

            // アイドリング状態が解除された場合
            if (DateTime.Now >= _nextBlink)
            {
                RaiseOutput(
                    _rested ? string.Format(@"休憩してきた？\n\a[{0}]した\_a\n\a[]してない\_a", _restRestCommand)
                    : needRest
                    ? @"そろそろ休憩してきたら？" 
                    : @"目\w[0.5]・\w[0.5]・\w[0.5]・\w[0.5]乾くよ？");
                _nextBlink = DateTime.Now.AddMinutes(0.5 + _rand.NextDouble());
            }
        }

        public void TextInput(string text)
        {
            try
            {
                Guid guid;
                double result;
                if (Guid.TryParse(text, out guid))
                {
                    if (guid == _restRestCommand)
                    {
                        _rested = false;
                        _lastRest = DateTime.Now;
                    }
                }
                else if (_calculator.TryProcess(text, out result))
                {
                    Clipboard.SetText(result.ToString(), TextDataFormat.UnicodeText);
                    RaiseOutput(text + @"\n結果: " + result);
                }
                else if (text.StartsWith("\\"))
                {
                    RaiseOutput(text);
                }
                else
                {
                    string output = "";
                    foreach (var m in _morphAnalyzer.Parse(text))
                    {
                        output += m.Surface + "\t" + m.OriginalForm + @"\n";
                        switch (m.OriginalForm)
                        {
                            case "踊る":
                                if (++_currentMotion >= _numberOfMotions)
                                    _currentMotion = 0;
                                RaiseOutput(@"\?[play " + _currentMotion + ",repeat]");
                                break;
                            case "どく":
                            case "邪魔":
                                RaiseOutput(@"\![hide]");
                                break;
                            case "出る":
                            case "でる":
                                RaiseOutput(@"\![show]");
                                break;
                            case "右":
                                RaiseOutput(@"\?[move +5]");
                                break;
                            case "左":
                                RaiseOutput(@"\?[move -5]");
                                break;
                        }
                    }
                    RaiseOutput(output);
                }

            }
            catch (Exception ex)
            {
                RaiseOutput(ex.Message);
            }
        }

        public event EventHandler<EventArgs<string>> Output;

        private void RaiseOutput(string output)
        {
            var handler = Output;
            if (handler != null)
                handler(this, new EventArgs<string>(@"\c" + output + @"\e"));
        }
    }
}
