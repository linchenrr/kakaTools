﻿using KLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace ResourceInfoExporter
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic = CommandParse.parse(args);
            if (dic.ContainsKey("input") && dic.ContainsKey("output"))
            {
#if !DEBUG
                try
#endif
                {
                    FileInfoMaker.makeCfg(dic["input"], dic["output"]);
#if DEBUG
                    Console.ReadLine();
#endif
                }
#if !DEBUG
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.ReadLine();
                }
#endif
            }
            else
            {
                Console.WriteLine("-input 需要导出信息的svn目录");
                Console.WriteLine("-output 导出目录");
                Console.ReadLine();
            }
        }
    }
}
