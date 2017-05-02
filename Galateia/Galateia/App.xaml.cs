using System;
using System.IO;
using System.Threading;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;
using Aphrodite;
using Galateia.Baloon;
using Galateia.ConfigWindow;
using Galateia.CSC;
using Galateia.Ghost;
using Galateia.Ghost.Demeanor;
using Galateia.Infra;
using Galateia.Infra.Config;
using Galateia.Infra.Graphics;
using Galateia.Infra.Windows;
using Galateia.Shell;
using Galateia.UserInput;

namespace Galateia
{
    /// <summary>
    ///     App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {

        /// <summary>
        /// ユーザーデータを配置するデータディレクトリ
        /// </summary>
// ReSharper disable ConvertToConstant.Local
        private static readonly string UserDataDirectory = 
// ReSharper restore ConvertToConstant.Local
#if DEBUG
            @"..\..\..\Data";
#else
        @".";
         // Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Galateia");
#endif

        /// <summary>
        /// グローバル設定ファイル
        /// </summary>
        private static readonly string ConfigFile = Path.Combine(UserDataDirectory, "user.config");

        private GraphicDeviceManager _deviceManager;
        private ShellHookWindow _shellHookWindow;

        private readonly Renderer _renderer = new Renderer();
        public Renderer Renderer { get { return _renderer; } }

        /// <summary>
        ///     現在の<c>System.AppDomain</c>の<c>Galateia.App</c>オブジェクトを取得します．
        /// </summary>
        public new static App Current
        {
            get { return Application.Current as App; }
        }

        public GlobalConfig GlobalConfig { get; private set; }

        public UserInputWindow UserInputWindow { get; private set; }
        public BaloonManager BaloonManager { get; private set; }

        private Ghost.Ghost _ghost;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Directory.CreateDirectory(UserDataDirectory);

            // 設定の読み込み
            try
            {
                GlobalConfig = Deserialize<GlobalConfig>.AsXml(ConfigFile);
            }
            catch (Exception)
            {
                GlobalConfig = new GlobalConfig();
            }
            GlobalConfig.SystemConfig.Substituted += SystemConfig_Substituted;
            SystemConfig_Substituted(this, new EventArgs());

            // デバイスマネージャの初期化
            _deviceManager = new GraphicDeviceManager();
            _deviceManager.Load();

            // シェルフックの開始
            _shellHookWindow = new ShellHookWindow();

            // バルーン／シェルマネージャの初期化
            BaloonManager =
                new BaloonManager(Path.Combine(UserDataDirectory, "baloons"), _shellHookWindow, GlobalConfig.BaloonConfig);

            // ゴーストのインスタンス化
            _ghost = new Ghost.Ghost(
                new Intelligence(), new Parlance(), new DefaultDemeanor(),
                BaloonManager.InstanciateBaloon(),
                new ShellWindow(_deviceManager, _shellHookWindow, GlobalConfig.ShellConfig));
            _renderer.Add(_ghost);

            // ユーザー入力の受付を開始：最後にインスタンス化することで，一番前に出てくるようになる
            UserInputWindow =
                new UserInputWindow(Path.Combine(UserDataDirectory, "input.log"), _shellHookWindow, GlobalConfig.UserInputConfig);
            UserInputWindow.UserInput += UserInputWindow_UserInput;

            _renderer.Continue = true;
        }

        private void SystemConfig_Substituted(object sender, EventArgs e)
        {
            _renderer.FpsCounter.FpsLimit = GlobalConfig.SystemConfig.FpsLimit;
        }

        private void UserInputWindow_UserInput(object sender, EventArgs<string> e)
        {
            _ghost.TextInput(e.Value);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            // 設定の保存
            Serialize<GlobalConfig>.AsXml(ConfigFile, GlobalConfig);

            // レンダリングの停止
            _renderer.Continue = false;

            _renderer.Remove(_ghost);
            _ghost.Dispose();

            // 各システムのシャットダウン
            UserInputWindow.Dispose();
            _shellHookWindow.Dispose();
            _deviceManager.Dispose();
            _renderer.Dispose();
        }
    }
}