/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-10-5
 * 时间: 8:30
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Configuration;

namespace update
{
	/// <summary>
	/// Description of Config.
	/// </summary>
	public class Config
	{
		/// <summary>下载线程数</summary>
		public static int ThreadNum = 0x10;
		/// <summary>工作目录</summary>
		public static string workPath;
		/// <summary>信息目录</summary>
		public static string infoPath;
		/// <summary>版本</summary>
		public static string versionFile;
		/// <summary>新版本</summary>
		public static string newVersionFile;
		/// <summary>删除列表</summary>
		public static string deleteFile;
		/// <summary>重命名列表</summary>
		public static string renameFile;
		/// <summary>文件列表</summary>
		public static string filelistFile;
		/// <summary>错误列表</summary>
		public static string errorFile;
		
		/// <summary>下载网址</summary>
		public static string url_home="http://localhost/";
		/// <summary>版本下载网址</summary>
		public static string url_version;
		/// <summary>删除下载网址</summary>
		public static string url_delete;
		/// <summary>文件列表下载网址</summary>
		public static string url_filelist;
		/// <summary>重命名下载网址</summary>
		public static string url_rename;
		
		/// <summary>使用代理</summary>
		public static bool useProxy = false;
		/// <summary>代理IP</summary>
		public static string proxyIP = "127.0.0.1";
		/// <summary>代理端口</summary>
		public static int proxyPort = 80;
		
		public static string[] ignores;

        public static string GetUrl(string name) => url_home + name;

        public static string GetPath(string name) => Path.Combine(path1: workPath, path2: name.Replace(oldChar: '/', newChar: Path.DirectorySeparatorChar));

        public static void SetWorkPath(string workPath, string url_home){
			string tmp;
			if(string.IsNullOrEmpty(value: workPath)){
                //当前目录
                Config.workPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                tmp = (new DirectoryInfo(path: Config.workPath).Parent != null)? new DirectoryInfo(path: Config.workPath).Parent.FullName:"";
				if(!string.IsNullOrEmpty(value: tmp))//上一级目录不为空
                    Config.workPath = tmp;
			}else
                Config.workPath = workPath;
			if(string.IsNullOrEmpty(value: url_home))
                Config.url_home = ConfigurationManager.AppSettings[name: "url"];
			else
                Config.url_home = url_home;
            url_version = $"{Config.url_home}update/version.txt";
            url_delete = $"{Config.url_home}update/delete.txt";
            url_filelist = $"{Config.url_home}update/filelist.txt";
            url_rename = $"{Config.url_home}update/rename.txt";

            infoPath = Path.Combine(path1: Config.workPath, path2: "update");
			if(!Directory.Exists(path: infoPath))
				Directory.CreateDirectory(path: infoPath);
			versionFile = Path.Combine(path1: infoPath, path2: "version.txt");
			newVersionFile = Path.Combine(infoPath, "version_new.txt");
			deleteFile = Path.Combine(path1: infoPath, path2: "delete.txt");
			renameFile = Path.Combine(path1: infoPath, path2: "rename.txt");
			filelistFile = Path.Combine(path1: infoPath, path2: "filelist.txt");
			errorFile = Path.Combine(path1: infoPath, path2: "error.txt");
		}

		public static void Init(string workPath, string url_home){
            SetWorkPath(workPath: workPath, url_home: url_home);
            //忽略列表
            List<string> iglist = new List<string>();

            for (int i = 1;  !string.IsNullOrEmpty(value: ConfigurationManager.AppSettings[name: $"ignore{i}"]); i++)
				iglist.Add(item: ConfigurationManager.AppSettings[name: $"ignore{i}"].Replace(oldValue: "*", newValue: "[^/]*?"));
            ignores = iglist.ToArray();
			//代理设置
			bool useProxy = ("true".Equals(ConfigurationManager.AppSettings[name: "useproxy"], StringComparison.OrdinalIgnoreCase))? true:false;
			if(useProxy){
                proxyIP = ConfigurationManager.AppSettings[name: "proxy"].Split(separator: ':')[0];
				int.TryParse(s: ConfigurationManager.AppSettings[name: "proxy"].Split(separator: ':')[1], result: out proxyPort);
			}
		}	
	}
}