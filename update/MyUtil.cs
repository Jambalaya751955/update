/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-30
 * 时间: 15:34
 * 
 */
using System.IO;
using System.Text;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace update
{
    /// <summary>
    /// Description of MyUtil.
    /// </summary>
    public class MyUtil
	{
        #region 获取网址内容
        public static string GetHtmlContentByUrl(string requestUriString) {
            string htmlContent = string.Empty;
            try {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString: requestUriString);
                httpWebRequest.Timeout = 30000;
                httpWebRequest.UserAgent = httpWebRequest.UserAgent = "Mozilla/5.0 (Windows Phone 10.0; Android 6.0.1; Microsoft; Lumia 640 LTE) AppleWebKit/537.36 (KHTML, like Gecko) "
                    + "Chrome/51.0.2704.79 Mobile Safari/537.36 Edge/14.14390";
                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse()) {
                    using (Stream stream = httpWebResponse.GetResponseStream()) {
                        using (StreamReader streamReader = new StreamReader(stream: stream, encoding: Encoding.UTF8)) {
                            htmlContent = streamReader.ReadToEnd();
                            streamReader.Close();
                        }
                    }
                }
                return htmlContent;
            }
            catch {

            }
            return "";
        }
        #endregion

        #region MD5校验
        /// <summary>
        /// 计算文件的MD5校验
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string MD5_File(string fileName){
			if(!File.Exists(path: fileName))
				return "";
			long filesize = 0;
			try {
                byte[] retVal;
                using (FileStream file = new FileStream(path: fileName, mode: FileMode.Open)){
                    filesize = file.Length;
                    System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                    retVal = md5.ComputeHash(inputStream: file);
                }
                
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                    sb.Append(value: retVal[i].ToString(format: "x2"));
                return sb.ToString();
            }
			catch {
                //(Exception ex)
                //throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
			return filesize.ToString();
		}
		#endregion
		
		public static void CreateDir(string filename){
            string path = filename.Substring(startIndex: 0, length: filename.LastIndexOf(value: Path.DirectorySeparatorChar));
			if(!Directory.Exists(path: path))
				Directory.CreateDirectory(path: path);
		}

		public static bool CheckList(string[] iglist, string name){
			if(iglist == null)
				return false;
			foreach(string tmp in iglist){
				if(Regex.IsMatch(input: name, pattern: $"^{tmp}$", options: RegexOptions.IgnoreCase))
					return true;
			}
			return false;
		}

		public static void SaveText(string filename, string contents){
			if(File.Exists(path: filename))
				File.Delete(path: filename);
			else
				MyUtil.CreateDir(filename: filename);
			File.WriteAllText(path: filename, contents: contents, encoding: Encoding.UTF8);
		}

		public static void SaveList(string filename, Fileinfo[] fileinfos){
			if(File.Exists(path: filename))
				File.Delete(path: filename);
			else
				MyUtil.CreateDir(filename: filename);
			using (FileStream fs = new FileStream(path: filename, mode: FileMode.Create, access: FileAccess.Write)){
                using (StreamWriter sw = new StreamWriter(stream: fs, encoding: Encoding.UTF8)){
                    if (fileinfos != null){
                        foreach (Fileinfo ff in fileinfos)
                            sw.WriteLine(value: $"{ff.name}\t{ff.md5}");
                    }
                }
			}
		}
		
		public static Fileinfo[] ReadList(string file){
            List<Fileinfo> list = new List<Fileinfo>();
            if (File.Exists(path: file)){
				using (FileStream fs = new FileStream(path: file, mode: FileMode.Open, access: FileAccess.Read)){
                    using (StreamReader sr = new StreamReader(stream: fs, encoding: Encoding.UTF8)){
                        string line;
                        while ((line = sr.ReadLine()) != null){
                            if (!line.StartsWith(value: "#")){
                                string[] w = line.Split(separator: '\t');
                                if (w.Length >= 2)
                                    list.Add(item: new Fileinfo(w[0], w[1]));
                            }
                        }
					}
				}
			}
			return list.ToArray();
		}
	}
}