using System;
using System.IO;

namespace WebBackend.Util
{
    /// <summary>
    /// 文件读取器（基于项目根目录从本地读取或根据URL远程读取）
    /// </summary>
    public class FileReader
    {
        /// <summary>
        /// 读取一个文件的每一行
        /// </summary>
        /// <param name="filepath">文件绝对路径</param>
        /// <returns>String[]数组，每个元素代表文件的一行</returns>
        /// <exception cref="Exception">读取文件时抛出</exception>
        public String[] ReadAllLines(string filepath)
        {
            return File.ReadAllLines(filepath);
        }
    }
}
