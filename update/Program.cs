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
    internal class Program
	{
		public static void Main(string[] args){
			Console.Title="AUTOUPDATE";
			
			if(args.Length > 0){
				switch(args[0]){
					case "-m":
                        if (args.Length >= 2)
                            Config.Init(workPath: args[1], url_home: null);
                        else
                            Config.Init(workPath: null, url_home: null);
                        new Server().Run();//更新文件列表
                        break;
					case "-ci":
                        if (args.Length >= 2)
                            Config.Init(workPath: args[1], url_home: null);
                        else
                            Config.Init(workPath: null, url_home: null);
                        Server.ci_run = true;
                        new Server().Run();//更新文件列表
                        return;
					case "--ignore-sound":
                        Config.Init(workPath: null, url_home: null);
                        Config.ignore_sound = true;
                        Download();
                        break;
                    case "-d":
                        if (args.Length == 2){
                            Config.Init(workPath: args[1], url_home: null);
                            Download();
                        }else {
                            if (args[2] == "--ignore-sound"){
                                Config.Init(workPath: args[1], url_home: null);
                                Config.ignore_sound = true;
                                Download();
                            } else if (args[3] == "--ignore-sound"){
                                Config.Init(workPath: args[1], url_home: args[2]);
                                Config.ignore_sound = true;
                                Download();
                            }
                        }
                        break;
                    default :
                        Console.WriteLine("You typed undefined argument. (-m, -ci, --ignore-sound, -d) are available.");
                        break;
                }
            }else {
                Config.Init(workPath: null, url_home: null);
                Download();
            }
            Console.WriteLine(value: "Press Any Key to continue ... ... ");
			Console.ReadKey(intercept: true);
		}

		private static void Download(){
			//线程数
			MyHttp.Init(max: Config.ThreadNum);
            MyHttp.SetListner(listner: new Client());
            new Client().Run();//开始更新
		}
	}
}