/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-9-29
 * 时间: 12:28
 * 
 */
using System;

namespace update
{
	/// <summary>
	/// Description of Fileinfo.
	/// </summary>
	public class Fileinfo
	{
        public string name, md5;
        
        public Fileinfo(){
            name = "";
            md5 = "";
        }

        public Fileinfo(string name, string md5){
			this.name = name;
			this.md5 = md5;
		}
	}
}