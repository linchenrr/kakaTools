using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileExtract
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 3)
            {
                Console.WriteLine("Usage:   FileExtract.exe inputFile outputFile startPos [endPos]");
                Console.ReadLine();
                return;
            }

            try
            {
                FileInfo input = new FileInfo(args[0]);
                FileInfo output = new FileInfo(args[1]);
                FileStream f_input = new FileStream(input.FullName, FileMode.Open);

                int startPos = Convert.ToInt32(args[2]);
                int endPos;
                if (args.Length < 4)
                    endPos = Convert.ToInt32(f_input.Length);
                else
                    endPos = Convert.ToInt32(args[3]);
                Byte[] bytes = new Byte[endPos - startPos];
                f_input.Position = startPos;
                f_input.Read(bytes, 0, bytes.Length);
                f_input.Close();

                FileStream f_output = new FileStream(output.FullName, FileMode.Create);
                f_output.Write(bytes, 0, bytes.Length);
                f_output.Close();

                Console.WriteLine("已生成" + output.FullName);
                Console.ReadLine();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
