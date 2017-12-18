using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoMovie
{
    public class Cmd
    {
        private Process proc = null;

        /// <summary>
        /// 构造方法
        /// </summary>
        public Cmd()
        {
            proc = new Process();
        }

        /// <summary>
        /// 执行CMD语句
        /// </summary>
        /// <param name="cmd">要执行的CMD命令</param>
        public void RunCmd(string cmd)
        {
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardError = false;
            proc.StartInfo.RedirectStandardInput = true;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.Start();
            proc.StandardInput.WriteLine(cmd+"&exit");
            proc.WaitForExit();
            proc.Close();
        }
    }
}
