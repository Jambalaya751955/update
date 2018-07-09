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

namespace update
{
    /// <summary>
    /// Description of Server.
    /// </summary>
    public class Server
	{
        private List<Fileinfo> list;    //文件信息列表
        private bool ci_run;
        public Server() => list = new List<Fileinfo>();

        public void Run(bool ci_run){
			this.ci_run = ci_run;
			if (this.ci_run)
                Console.WriteLine(value: "Updating Filelist on CI... ...");				
			else
                Console.WriteLine(value: "Updating Filelist... ...");
			list.Clear();
			AddDir(dir: Config.workPath);//当前目录所有文件
			//版本
			MyUtil.SaveText(filename: Config.versionFile, contents: DateTime.Now.ToString());
			//重命名列表
			MyUtil.SaveText(filename: Config.renameFile, contents: $"# Rename List (Codepage is UTF-8, please use TAB to seperate entries, " +
                $"Use Relative Address.){Environment.NewLine}# An example of renaming a file from 123456.jpg to 456789.jpg" +
                $"{Environment.NewLine}# pics/123456.jpg	pics/456789.jpg");
			//删除列表
			MyUtil.SaveText(filename: Config.deleteFile, contents: "# Delete List (Codepage is UTF-8, please use TAB to seperate entries, Use Relative Address.)");
			//文件列表
			MyUtil.SaveList(filename: Config.filelistFile, fileinfos: list.ToArray());
			Console.WriteLine(value: "FILELIST UPDATED!!");
		}

        private void AddDir(string dir){
			//所有文件
			string[] files = Directory.GetFiles(path: dir);
			foreach(string file in files)
				AddFile(file: file);
            //获取所有子目录
            foreach (string dirs in Directory.GetDirectories(path: dir)){
                if (!dirs.EndsWith(value: Path.DirectorySeparatorChar + ".git"
                               , comparisonType: StringComparison.OrdinalIgnoreCase))
                    AddDir(dir: dirs); //添加子目录的所有文件
            }
        }

        private void AddFile(string file){
			string filename = Path.GetFileName(path: file);
			string exename = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
			if(filename.EndsWith(value: "Thumbs.db", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: ".gitignore", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "LICENSE", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "appveyor.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: ".travis.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "circle.yml", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "README.md", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "web.config", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "update-push.bat", comparisonType: StringComparison.OrdinalIgnoreCase)
               || filename.EndsWith(value: "update-server.bat", comparisonType: StringComparison.OrdinalIgnoreCase)
               || filename.EndsWith(value: "desktop.ini", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "start.htm", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || filename.EndsWith(value: "update.exe.config", comparisonType: StringComparison.OrdinalIgnoreCase)
			   || file == exename
			   || file == $"{exename}.config"
              )
				return;
			//处理名字
			string name = file.Replace(oldValue: Config.workPath, newValue: "").Replace(oldChar: Path.DirectorySeparatorChar, newChar: '/');
			
			if(name.IndexOf(value: '/') == 0)
				name = name.Substring(startIndex: 1);
			
			if(MyUtil.CheckList(iglist: Config.ignores, name: name))
				return;
			string md5 = MyUtil.MD5_File(fileName: file);

			if(!ci_run) {
				Console.WriteLine(value: $"FILE:	{name}");
				Console.WriteLine(value: $"MD5:	{md5}");
			}
			list.Add(new Fileinfo(name, md5));
		}
	}
}