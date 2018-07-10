/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-30
 * 时间: 15:45
 * 
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace update
{
    /// <summary>
    /// Description of Download.
    /// </summary>
    public class Client : IMyHttpListener
	{
        private List<Fileinfo> errorlist = new List<Fileinfo>();
        public int Num, All_num;

        public void OnStart(string name, string file){
            //Console.WriteLine("开始下载："+name);
            //Console.WriteLine("保存到："+file);
        }

        public Client(){
			//代理设置
			if(Config.useProxy)
				Console.WriteLine(value: $"USE PROXY:{Config.proxyIP}:{Config.proxyPort}");
		}

        private void Delete(){
			if(!MyHttp.DownLoad(url: Config.url_delete, filename: Config.deleteFile))
				return;
            foreach (string line in File.ReadAllLines(path: Config.deleteFile, encoding: Encoding.UTF8)){
                if (!line.StartsWith(value: "#")){
					if(File.Exists(path: Config.GetPath(name: line))){
						Console.WriteLine(value: $"DELETE FILE:{line}");
						File.Delete(path: Config.GetPath(name: line));
					}
				}
			}
		}

        private void Rename(){
			if(!MyHttp.DownLoad(url: Config.url_rename, filename: Config.renameFile))
				return;
            foreach (string line in File.ReadAllLines(path: Config.renameFile, encoding: Encoding.UTF8)){
				if(!line.StartsWith(value: "#")){
                    if (line.Split(separator: '\t').Length >= 2){
                        Console.WriteLine(value: $"RENAME:{line.Split(separator: '\t')[0]}=>{line.Split(separator: '\t')[1]}");
                        File.Move(sourceFileName: Config.GetPath(name: line.Split('\t')[0]), destFileName: Config.GetPath(name: line.Split('\t')[1]));
                    }
                }
			}
		}

        private void ShowProcess(int Num, int All_num) => Console.Title = $"PROGRESS: {Num}/{All_num}";

        public bool Download(string name, string md5){
            if (Config.ignore_sound && (name.EndsWith(value: ".mp3", comparisonType: StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(value: ".ogg", comparisonType: StringComparison.OrdinalIgnoreCase)
                || name.EndsWith(value: ".wav", comparisonType: StringComparison.OrdinalIgnoreCase))){
                //ignores sound
                Console.WriteLine(value: $"SOUND IGNORED:{name}");
                ShowProcess(Num: Num++, All_num: All_num);
                return true;
            }

            if (File.Exists(path: Config.GetPath(name: name))){
                if (md5 == MyUtil.MD5_File(fileName: Config.GetPath(name: name))){
                    //一致
                    Console.WriteLine(value: $"SKIPPED:{name}");
                    ShowProcess(Num: Num++, All_num: All_num);
                    return true;
                }else if (MyUtil.CheckList(iglist: Config.ignores, name: name)){
                    //忽略更新
                    Console.WriteLine(value: $"IGNORED:{name}");
                    ShowProcess(Num: Num++, All_num: All_num);
                    return true;
                }
            }
            //线程已满
            while (MyHttp.NUM >= MyHttp.MAX_NUM){
                //System.Threading.Thread.Sleep(100);
            }
            //下载文件
            new MyHttp(url: Config.GetUrl(name: name), filename: Config.GetPath(name: name), ff: new Fileinfo(name: name, md5: md5)).Start();
            return true;
            //return MyHttp.DownLoad(url_download+name,file);
        }

        private void Update(){
			if(!File.Exists(Config.errorFile)){//上一次下载是否失败
				Console.WriteLine(value: "Downloading Filelist... ...");
				if(!MyHttp.DownLoad(url: Config.url_filelist, filename: Config.filelistFile))
					return;
				Console.WriteLine(value: "Starting Update... ...");
			}else {
				File.Delete(path: Config.filelistFile);
				File.Move(sourceFileName: Config.errorFile, destFileName: Config.filelistFile);
				Console.WriteLine(value: "Continuing Update... ...");
			}

			if(Config.ignore_sound)
				Console.WriteLine(value: "The sound files will be ignored.");

            All_num = File.ReadAllLines(path: Config.filelistFile, encoding: Encoding.UTF8).Length;
			Num = 0;
			ShowProcess(Num: Num++, All_num: All_num);

			foreach(string line in File.ReadAllLines(path: Config.filelistFile, encoding: Encoding.UTF8)){
				if(!line.StartsWith(value: "#")){
                    if (line.Split(separator: '\t').Length >= 2)
                        Download(name: line.Split(separator: '\t')[0], md5: line.Split(separator: '\t')[1]);
				}
			}

			while(!MyHttp.IsOK()){
                // Do not start next process until finish task.
			}

			if(errorlist.Count > 0){
				Console.WriteLine(value: "Some of files failed to update... ...");
				MyUtil.SaveList(filename: Config.errorFile, fileinfos: errorlist.ToArray());
			}
		}

        public void Run(){
			Console.WriteLine(value: $"UPDATE FROM:{Config.url_home}");
			Console.WriteLine(value: $"DOWNLOAD TO:{Config.workPath}");
			Console.WriteLine(value: $"CONFIG FILE:{Assembly.GetExecutingAssembly().Location}.config");

			if(!File.Exists(Config.errorFile)){
				Console.WriteLine(value: "Getting New Version ... ...");
				//version
				MyHttp.DownLoad(url: Config.url_version, filename: Config.newVersionFile);
				//版本号一致
				string md5_1 = MyUtil.MD5_File(fileName: Config.versionFile);
				string md5_2 = MyUtil.MD5_File(fileName: Config.newVersionFile);
				if(md5_1 == md5_2 && md5_1.Length > 0){
					Console.WriteLine(value: "Your files are already up-to-date.");
					return;
				}
				Console.WriteLine(value: "New Version Discovered... ...");
				//删除旧文件
				Delete();
				//重命名文件
				Rename();
			}

			Console.Clear();
			//filelist
			Update();
			if(File.Exists(path: Config.newVersionFile)){
				File.Delete(path: Config.versionFile);
				File.Move(sourceFileName: Config.newVersionFile, destFileName: Config.versionFile);
			}
			Console.WriteLine(value: "UPDATE COMPLETE!! You can safely close this window, press any key to quit.");
		}

        public void OnEnd(Fileinfo ff, bool isOK){
            if (All_num > 0)
                ShowProcess(Num: Num++, All_num: All_num);

            if (!isOK){
                if (ff != null){
                    Console.WriteLine(value: $"DOWNLOAD FAILED:{Config.GetUrl(ff.name)}");
                    errorlist.Add(ff);
                }else
                    Console.WriteLine(value: "DOWNLOAD FAILED");
            }else if (ff != null){
                    Console.WriteLine(value: $"DOWNLOAD COMPLETE:{ff.name}");
            }
        }
    }
}