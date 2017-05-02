using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;
using Aphrodite.MeCab.Core;
using Galateia.Baloon;
using Galateia.Ghost.Script;
using Galateia.Infra;
using Galateia.CSC;
using Galateia.Infra.Graphics;
using Galateia.Shell;
using Token = Galateia.Ghost.Script.Token;

namespace Galateia.Ghost
{
    public class Ghost : IDisposable, IRenderable
    {
        private bool _disposed = false;
        private readonly SerialDispatchQueue<string> _inputQueue = new SerialDispatchQueue<string>(ApartmentState.STA);
        private readonly SerialDispatchQueue<string> _outputQueue = new SerialDispatchQueue<string>(ApartmentState.STA);
        private readonly IIntelligence _intelligence;
        private readonly IParlance _parlance;
        private readonly IDemeanor _demeanor;
        private readonly BaloonWindow _baloon;
        private readonly ShellWindow _shell;

        public Ghost(IIntelligence intelligence, IParlance parlance, IDemeanor demeanor, BaloonWindow baloon, ShellWindow shell)
        {
            _intelligence = intelligence;
            _parlance = parlance;
            _demeanor = demeanor;
            _baloon = baloon;
            _shell = shell;

            _inputQueue.Dispatched += InputDispatched;
            _outputQueue.Dispatched += OutputDispatched;
            _intelligence.Output += IntelligenceOutput;
            _shell.ScreenChanged += ShellScreenChanged;
            _shell.Rendered += ShellRendered;

            _demeanor.Loaded(shell);
            _intelligence.Loaded();
        }

        private void ShellScreenChanged(object sender, ScreenChangedEventArgs e)
        {
            // var reply = ghost.Request(Message.OnScreenChange(e.Areas));
            // ((ISubsystemCallback) this).Request(reply);
        }

        ~Ghost()
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
                _intelligence.Disposing();
                
                _inputQueue.Dispose();
                _outputQueue.Dispose();

                _shell.Dispose();
                _demeanor.Disposing();

                _baloon.Close();

                _disposed = true;
            }
        }

        public void Render()
        {
            _shell.Render(_demeanor);
        }

        // おなじ _inputQueue を使うようにする？しない？
        // 文字入力の処理には時間がかかるかもしれないので，分けておいた方がよいかもしれない．
        // public void TouchInput() おさわり

        /// <summary>
        /// ユーザーからの文字列入力を受け付けます．
        /// </summary>
        /// <param name="text">入力された文字列を指定します．</param>
        public void TextInput(string text)
        {
            _inputQueue.Enqueue(text);
        }

        private void InputDispatched(object sender, EventArgs<string> eventArgs)
        {
            var text = eventArgs.Value;
            _intelligence.TextInput(text);
        }

        /// <summary>
        /// インテリジェンスからの出力を処理します．
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void IntelligenceOutput(object sender, EventArgs<string> eventArgs)
        {
            _outputQueue.Enqueue(eventArgs.Value);
        }

        private void OutputDispatched(object sender, EventArgs<string> eventArgs)
        {
            // 語調フィルタからの解析
            var script = _parlance.Filter(eventArgs.Value);

            // スクリプトの解析と再生
            foreach (var token in Token.Parse(script))
                PlayToken(token);
        }

        private void ShellRendered(object sender, EventArgs eventArgs)
        {
            Point headInScreen = _shell.BaloonReferencePoint;

            _baloon.Dispatcher.BeginInvoke((Action<Point>)(headInScr =>
            {
                // バルーンを左右どちらに表示するか決める
                Screen screen = Screen.FromPoint(headInScr);
                int left = headInScr.X - screen.WorkingArea.Left;
                int right = screen.WorkingArea.Right - headInScr.X;

                if (_shell.IsVisible)
                {
                    _baloon.Direction = (left > right)
                        ? BaloonWindow.Directions.Left
                        : BaloonWindow.Directions.Right;
                    _baloon.Top = headInScr.Y + _baloon.CurrentWindowConfig.VerticalScreenOffset -
                                  _baloon.ActualHeight;
                    _baloon.Left = (left > right)
                        ? headInScr.X -
                          _baloon.CurrentWindowConfig.HorizontalWorldOffset * _shell.GlobalConfig.PixelPerUnitLength -
                          _baloon.ActualWidth
                        : headInScr.X +
                          _baloon.CurrentWindowConfig.HorizontalWorldOffset * _shell.GlobalConfig.PixelPerUnitLength;
                }
                else
                {
                    _baloon.Direction = BaloonWindow.Directions.Left;
                    _baloon.Top = screen.WorkingArea.Bottom - _baloon.ActualHeight - 10;
                    _baloon.Left = screen.WorkingArea.Right - _baloon.ActualWidth - 10;
                }
            }), headInScreen);
        }

        private void PlayToken(Token token)
        {
            switch (token.Tag)
            {
                // 発言系
                case Tags.None:
                case Tags.Unknown:
                    _baloon.StopTimeoutCountdown();
                    _baloon.Show();
                    _baloon.Append(token.Surface);
                    break;
                case Tags.Clear:
                    _baloon.Clear();
                    break;
                case Tags.NewLine:
                    _baloon.NewLine();
                    break;
                case Tags.Anchor:
                    switch (token.Span)
                    {
                        case SpanModes.Enter:
                            _baloon.BeginAnchor(OnAnchorClick, token);
                            break;
                        case SpanModes.Exit:
                            _baloon.EndAnchor();
                            break;
                    }
                    break;
                case Tags.Image:
                    _baloon.AppendImage(token.Option);
                    break;
                // システム系
                case Tags.Wait:
                    {
                        double wait;
                        if (double.TryParse(token.Option, out wait) && wait > 0)
                            Thread.Sleep((int)(wait * 1000));
                    }
                    break;
                case Tags.Command:
                    OnCommand(token.Option);
                    break;
                case Tags.Demeanor:
                    OnDemeanor(token.Option);
                    break;
                // End of Script
                case Tags.EndOfScript:
                    _baloon.StartTimeoutCountdown();
                    SpinWait.SpinUntil(() => !_baloon.IsVisible);
                    break;
            }
        }

        private void OnCommand(string option)
        {
            switch (option)
            {
                case "hide":
                    _shell.Hide();
                    break;
                case "show":
                    _shell.Show();
                    break;
                case "quit":
                    _baloon.Dispatcher.BeginInvoke((Action)Dispose);
                    break;
                default:
                    _baloon.Append(@"\![" + option + "]");
                    break;
            }
        }

        private void OnDemeanor(string option)
        {
            _demeanor.Command(option);
        }

        private void OnAnchorClick(object sender, AnchorClickEventArgs e)
        {
            var token = (Token)e.Data;
            TextInput(token.Option);
            _baloon.StartTimeoutCountdown();
        }
    }
}
