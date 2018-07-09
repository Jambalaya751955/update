/*
 * 由SharpDevelop创建。
 * 用户： Acer
 * 日期: 2014-10-5
 * 时间: 8:04
 * 
 */

namespace update
{
    /// <summary>
    /// Description of IMyHttpListener.
    /// </summary>
    public interface IMyHttpListener
	{
		void OnStart(string name, string file);
		void OnEnd(Fileinfo ff, bool isOK);
	}
}
