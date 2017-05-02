using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Galateia.Infra.Windows;

namespace Galateia.Baloon
{
    /// <summary>
    ///     バルーンの一覧を管理します．
    /// </summary>
    public class BaloonManager
    {
        /// <summary>
        ///     ディレクトリと設定の連想辞書．
        /// </summary>
        private readonly Dictionary<Guid, BaloonConfig> configs = new Dictionary<Guid, BaloonConfig>();

        private readonly BaloonGlobalConfig globalConfig;
        private readonly ShellHookWindow shellHook;

        /// <summary>
        ///     バルーンを配置するディレクトリ．
        /// </summary>
        private string rootDir;

        /// <summary>
        ///     基本ディレクトリを指定してマネージャを初期化します．
        /// </summary>
        /// <param name="path">バルーンのある基本ディレクトリ．</param>
        public BaloonManager(string path, ShellHookWindow shellHook, BaloonGlobalConfig globalConfig)
        {
            rootDir = path;
            this.shellHook = shellHook;
            this.globalConfig = globalConfig;

            var serializer = new XmlSerializer(typeof (BaloonConfig));
            foreach (string dir in Directory.EnumerateDirectories(rootDir))
            {
                try
                {
                    using (var r = new StreamReader(dir + @"\description.xml"))
                    {
                        var config = (BaloonConfig) serializer.Deserialize(r);
                        config.BaseDirectory = dir;
                        configs.Add(config.Guid, config);
                    }
                }
                catch (FileNotFoundException)
                {
                } // ファイル見つからない
                catch (InvalidOperationException)
                {
                } // シリアライズ失敗
            }
        }

        /// <summary>
        ///     指定したGUIDのバルーンをインスタンス化します．
        /// </summary>
        /// <param name="guid">バルーンのGUID．</param>
        /// <returns>バルーンウィンドウのインスタンス．</returns>
        public BaloonWindow InstanciateBaloon(Guid guid)
        {
            return new BaloonWindow(configs[guid], shellHook, globalConfig);
        }

        public BaloonWindow InstanciateBaloon()
        {
            return new BaloonWindow(configs.First().Value, shellHook, globalConfig);
        }
    }
}