﻿using System;
using System.Collections.Generic;
using System.Linq;
using KLib;

namespace ExcelExport
{
    class Program
    {
        static void Main(string[] args)
        {
            CommandMode.exec(CommandParse.parse(args));
        }
    }
}
