/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-30
 * 时间: 15:31
 * 
 */
using System;

namespace update
{
	class Program
	{
		public static void Main(string[] args)
		{
			Console.Title="AUTOUPDATE";
			Config.Init(null, null);

			if(args.Length > 0){
				switch(args[0]){
					case "-m":UpdateList(args: args); break;
					case "-ci":UpdateList(args: args); return;
					case "--ignore-sound":Download(workPath: null, url_home: null, ignore_sound: true); break;
                    case "-d":
					if(args.Length == 2)
						Download(workPath: args[1], url_home: null, ignore_sound: false);
					else{
						if(args[2] == "--ignore-sound")
							Download(workPath: args[1], url_home: null, ignore_sound: true);
						else if(args[3]=="--ignore-sound")
							Download(workPath: args[1], url_home: args[2], ignore_sound: true);
					}
					break;
				}
			}else
				Download(workPath: null, url_home: null, ignore_sound: false);
			Console.WriteLine(value: "Press Any Key to continue ... ... ");
			Console.ReadKey(intercept: true);
		}

		private static void UpdateList(string[] args){
			if(args.Length >= 2)
				Config.SetWorkPath(workPath: args[1], url_home: null);
			bool ci_run = false;
			if(args[0] == "-ci")
				ci_run = true;
            new Server().Run(ci_run: ci_run);//更新文件列表
		}

		private static void Download(string workPath, string url_home, bool ignore_sound){
			//线程数
			MyHttp.Init(max: Config.ThreadNum);
            MyHttp.SetListner(listiner: new Client(workPath: workPath, url_home: url_home));
            new Client(workPath: workPath, url_home: url_home).Run(ignore_sound: ignore_sound);//开始更新
		}
	}
}