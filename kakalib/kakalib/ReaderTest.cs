using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using KLib;

public class ReaderTest
{
    static public void Start()
    {
        var count = 1000000;

        var str = "10080006";
        var a = 0;
        var st = new Stopwatch();
        var ms = new MemoryStream();
        var writer = new EndianBinaryWriter(Endian.BigEndian, ms);
        var reader = new EndianBinaryReader(Endian.BigEndian, ms);

        var ms2 = new MemoryStream();
        var writer2 = new EndianBinaryWriter(Endian.BigEndian, ms2);
        var reader2 = new EndianBinaryReader(Endian.BigEndian, ms2);

        var ms3 = new MemoryStream();
        var writer3 = new ProtocolBinaryWriter(ms3);
        var reader3 = new ProtocolBinaryReader(ms3);

        for (int i = 0; i < count; i++)
        {
            writer.Write(i);
            writer2.WriteUTF(i.ToString());
            writer3.Write(i);
        }
        ms.Position = 0;
        ms2.Position = 0;
        ms3.Position = 0;
        Thread.Sleep(10);

        st.Start();
        for (int i = 0; i < count; i++)
        {
            //a = Convert.ToInt32(str);
            a = reader.ReadInt32();
        }
        st.Stop();
        Console.WriteLine(a);
        Console.WriteLine(st.ElapsedMilliseconds);

        Thread.Sleep(10);

        st.Restart();
        for (int i = 0; i < count; i++)
        {
            a = Convert.ToInt32(reader2.ReadUTF());
            //a = reader.ReadInt32();
        }
        st.Stop();
        Console.WriteLine(a);
        Console.WriteLine(st.ElapsedMilliseconds);

        Thread.Sleep(10);

        st.Restart();
        for (int i = 0; i < count; i++)
        {
            //a = Convert.ToInt32(str);
            a = reader3.ReadInt32();
        }
        st.Stop();
        Console.WriteLine(a);
        Console.WriteLine(st.ElapsedMilliseconds);

        Console.ReadLine();
    }
}

