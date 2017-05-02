using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Galateia.Infra.Config
{
    public class Deserialize<T>
    {
        /// <summary>
        ///     ファイル名を指定して，オブジェクトをバイナリデータとして逆シリアル化します．
        /// </summary>
        /// <param name="file">ファイルのパス．</param>
        /// <returns>逆シリアル化されたオブジェクト．</returns>
        public static T AsBinary(string file)
        {
            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                return (T) formatter.Deserialize(stream);
        }

        /// <summary>
        ///     ファイル名を指定して，オブジェクトをXMLファイルとして逆シリアル化します．
        /// </summary>
        /// <param name="file">ファイルのパス．</param>
        /// <returns>逆シリアル化されたオブジェクト．</returns>
        public static T AsXml(string file)
        {
            var serializer = new XmlSerializer(typeof (T));
            using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read))
                return (T) serializer.Deserialize(stream);
        }
    }
}