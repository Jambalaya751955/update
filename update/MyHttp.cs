/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-25
 * 时间: 21:33
 * 
 */

using System;
using System.IO;
using System.Net;
using System.Threading;

namespace update
{
    /// <summary>
    /// Description of MyHttp.
    /// </summary>
    public class MyHttp
	{
		public static int NUM = 0;
		public static int MAX_NUM = 0x10;
        private readonly string url;
        private readonly string filename;
        private static int NTASK = 0;
		private static int TASK = 0;
		private readonly Fileinfo ff;
		private static IMyHttpListener imyhttplistiner = null;
		
		public MyHttp(string url, string filename, Fileinfo ff){
			this.url = url;
			this.filename = filename;
			this.ff = ff;
		}
		
		public void Start(){
            Thread thread = new Thread(start: Download){
                IsBackground = true
            };
            thread.Start();
		}
		
		public void Download(){
			if(NUM >= MAX_NUM)
				return;
			NUM++;
			TASK++;
            DownLoad(url: url, filename: filename, ff: ff);
			NTASK++;
			NUM--;
		}

        public static void SetListner(IMyHttpListener listner) => imyhttplistiner = listner;

        public static void Init(int max){
			ServicePointManager.DefaultConnectionLimit = 255;
			MAX_NUM = max;
		}
		
		public static bool IsOK() => (NTASK == TASK);

        public static int GetTask() => TASK - NTASK;

        public static bool DownLoad(string url, string filename) => DownLoad(url: url, filename: filename, ff: null);

        public static bool DownLoad(string url, string filename, Fileinfo ff){
			if(imyhttplistiner != null)
				imyhttplistiner.OnStart(name: url, file: filename);

            bool isOK = false;

            try {
                if (File.Exists(filename))
                    File.Delete(filename);
                else
                    MyUtil.CreateDir(filename);

                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(requestUriString: url);
                httpWebRequest.Timeout = 30000;
                //Myrq.UserAgent="Mozilla/5.0 (Windows NT 6.2; WOW64) "
                //	+"AppleWebKit/537.36 (KHTML, like Gecko) "
                //	+"Chrome/27.0.1453.94 Safari/537.36";
                if (Config.useProxy)
                    httpWebRequest.Proxy = new WebProxy(Host: Config.proxyIP, Port: Config.proxyPort);

                using (HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse()){
                    using (Stream st = httpWebResponse.GetResponseStream()){
                        using (Stream so = new FileStream(path: $"{filename}.tmp", mode: FileMode.Create)){
                            long totalDownloadedByte = 0;
                            byte[] by = new byte[2048];
                            int osize = st.Read(buffer: by, offset: 0, count: by.Length);
                            while (osize > 0){
                                totalDownloadedByte = osize + totalDownloadedByte;
                                so.Write(buffer: by, offset: 0, count: osize);
                                osize = st.Read(buffer: by, offset: 0, count: by.Length);
                            }
                        }
                    }
                }
                File.Delete(path: filename);
                File.Move(sourceFileName: $"{filename}.tmp", destFileName: filename);
            }
            catch (Exception){
                Console.WriteLine(value: "The webrequest or webresponse threw the Exception.");
            }
            isOK = File.Exists(path: filename);
			if(imyhttplistiner != null)
				imyhttplistiner.OnEnd(ff: ff, isOK: isOK);
			return isOK;
		}
	}
}