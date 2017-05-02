using Galateia.Baloon;
using Galateia.Infra.Config;
using Galateia.Shell;
using Galateia.UserInput;

namespace Galateia.ConfigWindow
{
    public class GlobalConfig : ConfigBase
    {
        public GlobalConfig()
        {
            SystemConfig = new SystemGlobalConfig();
            BaloonConfig = new BaloonGlobalConfig();
            ShellConfig = new ShellGlobalConfig();
            UserInputConfig = new UserInputConfig();
        }

        /// <summary>
        ///     インスタンスのディープ コピーとして新しいインスタンスを作成します．
        /// </summary>
        public GlobalConfig(GlobalConfig src)
        {
            SystemConfig = ConfigTools.CreateCopyOf(src.SystemConfig);
            BaloonConfig = ConfigTools.CreateCopyOf(src.BaloonConfig);
            ShellConfig = ConfigTools.CreateCopyOf(src.ShellConfig);
            UserInputConfig = ConfigTools.CreateCopyOf(src.UserInputConfig);
        }

        public SystemGlobalConfig SystemConfig { get; set; }
        public BaloonGlobalConfig BaloonConfig { get; set; }
        public ShellGlobalConfig ShellConfig { get; set; }
        public UserInputConfig UserInputConfig { get; set; }

        /// <summary>
        ///     インスタンスに値を代入します．
        /// </summary>
        public void Substitute(GlobalConfig conf)
        {
            ConfigTools.Substitute(conf.SystemConfig, SystemConfig);
            ConfigTools.Substitute(conf.BaloonConfig, BaloonConfig);
            ConfigTools.Substitute(conf.ShellConfig, ShellConfig);
            ConfigTools.Substitute(conf.UserInputConfig, UserInputConfig);
        }

        /// <summary>
        ///     インスタンス同士が等しいかどうかを判定します．
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is GlobalConfig))
                return false;

            var conf = (GlobalConfig) obj;
            return
                ConfigTools.IsEquivalent(SystemConfig, conf.SystemConfig) &&
                ConfigTools.IsEquivalent(BaloonConfig, conf.BaloonConfig) &&
                ConfigTools.IsEquivalent(ShellConfig, conf.ShellConfig) &&
                ConfigTools.IsEquivalent(UserInputConfig, conf.UserInputConfig);
        }
    }
}