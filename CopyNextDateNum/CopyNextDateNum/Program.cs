using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace CopyNextDateNum
{
    class Program
    {
        static void Main(string[] args)
        {
            Regex reg_datenum = new Regex(@"20[0-3]\d((0[1-9])|(1[0-2]))((0[1-9])|([1-2]\d)|(3[0-1]))_\d+");
            if (args.Length > 0)
            {
                foreach (string fileName in args.Where(x => File.Exists(x)))
                {
                    Environment.CurrentDirectory = Path.GetDirectoryName(Path.GetFullPath(fileName));
                    string tempFileName = Path.GetFileName(fileName);
                    if (reg_datenum.IsMatch(tempFileName))
                    {
                        string tempDateNum = reg_datenum.Matches(tempFileName)[0].ToString();
                        string tempNum = tempDateNum.Substring(9);
                        string newFileName = "";
                        for (int i = 1; i < int.Parse(new string('9', tempNum.Length)); i++)
                        {
                            string format = "{0}_{1:" + new string('0', tempNum.Length) + "}";
                            string newDateNum = string.Format(format,
                                DateTime.Now.ToString("yyyyMMdd"),
                                (int.Parse(tempNum)) + i);
                            newFileName = tempFileName.Replace(tempDateNum, newDateNum);
                            if (!File.Exists(newFileName)) { break; }
                        }
                        if (newFileName != "")
                        {
                            File.Copy(fileName, newFileName);
                        }
                    }
                }
            }
        }
    }
}
