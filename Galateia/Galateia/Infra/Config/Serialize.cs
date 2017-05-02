using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

namespace Galateia.Infra.Config
{
    public class Serialize<T>
    {
        /// <summary>
        ///     ファイル名を指定して，オブジェクトをバイナリデータとしてシリアル化します．
        ///     ファイルが存在しない場合は，新しく作成されます．
        /// </summary>
        /// <param name="file">ファイルのパス．</param>
        /// <param name="obj">シリアル化するオブジェクト．</param>
        /// <remarks>オブジェクトの型<c>T</c>は<c>Serializable</c>としてマークされている必要があります．</remarks>
        public static void AsBinary(string file, T obj)
        {
            string fullPath = Path.GetFullPath(file);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            var formatter = new BinaryFormatter();
            using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, obj);
                stream.Close();
            }
        }

        /// <summary>
        ///     ファイル名を指定して，オブジェクトをXMLファイルとしてシリアル化します．
        ///     ファイルが存在しない場合は，新しく作成されます．
        /// </summary>
        /// <param name="file">ファイルのパス．</param>
        /// <param name="obj">シリアル化するオブジェクト．</param>
        public static void AsXml(string file, T obj)
        {
            string fullPath = Path.GetFullPath(file);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            var serializer = new XmlSerializer(typeof (T));
            using (var stream = new FileStream(fullPath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                serializer.Serialize(stream, obj);
                stream.Close();
            }
        }
    }
}