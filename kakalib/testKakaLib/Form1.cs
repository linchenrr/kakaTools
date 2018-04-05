using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using KLib.data;
using KLib.tools;
using KLib.interfaces;
using KLib.events;
using KLib.net.protocol;
using KLib.utils;

namespace testKakaLib
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            return;

            /*
            DateTime date = DateTime.Now;

            KDataPackager packager = new KDataPackager();

            packager.writeDate(date);
            packager.resetPosition();
            DateTime date2 = packager.readDate();

            Byte[][] bytesList = new Byte[1][];
            bytesList[0] = Encoding.UTF8.GetBytes("ddc45");

            packager.AddEventListener("ss", ssHandler);
            packager.DispatchEvent(new Event("ss", new Point(5, 8)));
            */
            //FileUtil.writeFile(new String[] { @"F:\1\1.txt" }, bytesList, writeHandler);
            //FileUtil.writeFile(@"F:\1\1.txt" , bytesList[0]);
            //bytesList = FileUtil.readFile(new String[] { @"F:\1\1.txt" });

            //bytesList = ZlibUtil.compress(bytesList);
            //bytesList = ZlibUtil.uncompress(bytesList);
            //ZlibUtil.compress(new String[] { @"J:\1.txt" }, "_c" );
            //ZlibUtil.uncompress(new String[] { @"J:\1_c.txt" }, "_u");

            /*
            ICompresser compresser = new ZlibCompresser();
            compresser = new LZMACompresser();

            FileStream input = File.Open(@"J:\1.txt", FileMode.Open);
            FileStream output = File.Create(@"J:\2.txt");

            compresser.compress(input, output);

            input.Close();
            output.Close();

            input = File.Open(@"J:\2.txt", FileMode.Open);
            output = File.Create(@"J:\3.txt");

            compresser.uncompress(input, output);

            input.Close();
            output.Close();
            */
        }

        private void ssHandler(Event evt)
        {
            textBox1.Text = evt.Type + evt.Data;
        }

        private void writeHandler(DoWorkResult result)
        {



        }

    }
}
