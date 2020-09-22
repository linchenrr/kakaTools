using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Interop;
using System.Xml;
using System.Xml.Serialization;



namespace Unity3dPacth.Lib
{
    [XmlRoot(ElementName = "root", Namespace = "", IsNullable = false)]
    public class UnityLicense
    {
        [XmlElement] public StringValue TimeStamp = new StringValue { Value = "jWj8PXAeZMPzUw==" };
        [XmlElement] public StringValue TimeStamp2 = new StringValue { Value = "cn/lkLOZ3vFvbQ==" };
        [XmlElement] public LicenseData License;
        [XmlElement] public SignatureElement Signature = new SignatureElement();
        public UnityLicense() { }
        public UnityLicense(params string[] Serial)
        {
            License = new LicenseData(Serial);
        }
        private static string[] CreateSerial()
        {
            char[] buffer = Guid.NewGuid().ToString("N").ToUpperInvariant().ToCharArray();
            char[][] charss = { new char[4], new char[4], new char[4], new char[4], new char[4] };
            int index = 0;
            for (int i = 0; i < 5; i++)
                for (int j = 0; j < 4; j++)
                    charss[i][j] = buffer[index++];
            return charss.Select(p => new string(p)).ToArray();
        }
        public static string GetLicenseDocument =>
            UtilityToolsCollect.UtilityToolsCollect.SerializeToolUtility.XmlSerialize.XmlSerializerToString(new UnityLicense(CreateSerial()), false, true, true, true).Replace("xmlnsX", "xmlns");
        public static bool SaveLicenseDocument(Window owner)
        {
            DirectoryInfo di = new DirectoryInfo(@"C:\ProgramData\Unity");
            FileInfo fi = new FileInfo(Path.Combine(di.FullName, "Unity_lic.ulf"));
            if (!di.Exists)
                di.Create();
            if (fi.Exists)
            {
                if (MessageBox.Show(owner, "证书文件以存在，是否重写新的证书文件？", "写入证书", MessageBoxButton.OKCancel, MessageBoxImage.Question, MessageBoxResult.No) == MessageBoxResult.OK)
                {
                    fi.MoveTo($"{fi.FullName}.{new Random().Next(1000, 10000)}.bak");
                    File.WriteAllText(Path.Combine(di.FullName, "Unity_lic.ulf"), GetLicenseDocument);
                    return true;
                }
                return false;
            }
            File.WriteAllText(Path.Combine(di.FullName, "Unity_lic.ulf"), GetLicenseDocument);
            return true;
        }
    }
    public class LicenseData
    {
        [XmlAttribute] public string id = "Terms";
        [XmlElement] public StringValue ClientProvidedVersion = new StringValue { Value = string.Empty };
        [XmlElement] public StringValue DeveloperData;
        [XmlArray] public Feature[] Features = LicHeader.ReadAll();
        [XmlElement] public StringValue LicenseVersion = new StringValue { Value = "6.x" };
        [XmlElement] public object MachineBindings = new object();
        [XmlElement] public StringValue MachineID = new StringValue { Value = string.Empty };
        [XmlElement] public StringValue SerialHash = new StringValue { Value = string.Empty };
        [XmlElement] public StringValue SerialMasked;
        [XmlElement] public StringValue StartDate = new StringValue { Value = $"{DateTime.Now.AddDays(-1.0):yyyy-MM-dd}T00:00:00" };
        [XmlElement] public StringValue StopDate = new StringValue { Value = string.Empty };
        [XmlElement] public StringValue UpdateDate = new StringValue { Value = $"{DateTime.Now.AddYears(10):yyyy-MM-dd}T00:00:00" };
        public LicenseData() { }
        public LicenseData(params string[] Serial)
        {
            SerialMasked = new StringValue { Value = $"U3-{Serial[0]}-{Serial[1]}-{Serial[2]}-{Serial[3]}-{Serial[4]}" };
            DeveloperData = new StringValue { Value = Convert.ToBase64String(new byte[4] { 1, 0, 0, 0 }.Concat(Encoding.ASCII.GetBytes(SerialMasked.Value)).ToArray()) };
        }

    }
    public class SignatureElement
    {
        [XmlAttribute] public string xmlnsX = "http://www.w3.org/2000/09/xmldsig#";
        [XmlElement] public SignedInfoElement SignedInfo = new SignedInfoElement();
        [XmlElement]
        public string SignatureValue =
            "WuzMPTi0Ko1vffk9gf9ds/iU0b0K8UHaLpi4kWgm6q1am5MPTYYnzH1InaSWuzYo" +
            "EpJThKspOZdO0JISeEolNdJVf3JpsY55OsD8UaruvhwZn4r9pLeNSC7SzQ1rvAWP" +
            "h77XaHizhVVs15w6NYevP27LTxbZaem5L8Zs+34VKXQFeG4g0dEI/Jhl70TqE0CS" +
            "YNF+D0zqEtyMNHsh0Rq/vPLSzPXUN12jfPLZ3dO9B+9/mG7Ljd6emZjjLZUVuSKQ" +
            "uKxN5jlHZsm2kRMudijICV6YOWMPT+oZePlCg+BJQg5/xcN5aYVBDZhNeuNwQL1H" +
            "MPT/GJPxVuETgd9k8c4uDg==";
    }
    public class SignedInfoElement
    {
        [XmlElement] public AlgorithmElement CanonicalizationMethod = new AlgorithmElement { Algorithm = "http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments" };
        [XmlElement] public AlgorithmElement SignatureMethod = new AlgorithmElement { Algorithm = "http://www.w3.org/2000/09/xmldsig#rsa-sha1" };
        [XmlElement] public ReferenceElement Reference = new ReferenceElement();
    }
    public class ReferenceElement
    {
        [XmlAttribute] public string URI = "#Terms";
        [XmlArray] public Transform[] Transforms = new Transform[] { new Transform { Algorithm = "http://www.w3.org/2000/09/xmldsig#enveloped-signature" } };
        [XmlElement] public AlgorithmElement DigestMethod = new AlgorithmElement { Algorithm = "http://www.w3.org/2000/09/xmldsig#sha1" };
        [XmlElement] public string DigestValue = "oeMc1KScgy617DHMPTxbYhqNjIM=";
    }
    public struct StringValue : IEquatable<StringValue>, System.Collections.IEqualityComparer, IEqualityComparer<StringValue>
    {
        [XmlAttribute] public string Value;
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) => Value.Equals(((StringValue)obj).Value, StringComparison.Ordinal);
        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals([AllowNull] StringValue other) => Value.Equals(other.Value, StringComparison.Ordinal);

        bool System.Collections.IEqualityComparer.Equals(object x, object y) => ((StringValue)x).Equals(((StringValue)y));

        public int GetHashCode(object obj) => ((StringValue)obj).GetHashCode();

        public bool Equals([AllowNull] StringValue x, [AllowNull] StringValue y) => x.Equals(y);

        public int GetHashCode([DisallowNull] StringValue obj) => obj.GetHashCode();
    }
    public struct Feature : IEquatable<Feature>, System.Collections.IEqualityComparer, IEqualityComparer<Feature>
    {
        [XmlAttribute] public string Value;
        public override string ToString() => Value.ToString();
        public override bool Equals(object obj) => Value.Equals(((Feature)obj).Value, StringComparison.Ordinal);
        public override int GetHashCode() => Value.GetHashCode();

        public bool Equals([AllowNull] Feature other) => Value.Equals(other.Value, StringComparison.Ordinal);

        bool System.Collections.IEqualityComparer.Equals(object x, object y) => ((Feature)x).Equals(((Feature)y));

        public int GetHashCode(object obj) => ((Feature)obj).GetHashCode();

        public bool Equals([AllowNull] Feature x, [AllowNull] Feature y) => x.Equals(y);

        public int GetHashCode([DisallowNull] Feature obj) => obj.GetHashCode();
    }
    public struct AlgorithmElement {[XmlAttribute] public string Algorithm; }
    public struct Transform {[XmlAttribute] public string Algorithm; }
    public class LicHeader
    {
        public class LicSettings { public int Android, Blackberry, Flash, IPhone, SamsungTv, Tizen, WinStore, Type; public bool Educt, NRelease, Team, Nin, PlayStation, Wii, Xbox; }
        public static LicSettings PropLicSettings { get; set; } = new LicSettings { Type = 1, Nin = true, PlayStation = true, Wii = true, Xbox = true };
        public static Feature[] ReadAll()
        {
            List<Feature> list = new List<Feature>();
            void Add(int value) { list.Add(new Feature { Value = value.ToString() }); }
            switch (PropLicSettings.Type) { case 0: Add(0); Add(1); Add(16); break; case 1: Add(0); Add(1); break; case 2: Add(62); break; }
            if (PropLicSettings.Team) { Add(2); }
            switch (PropLicSettings.IPhone) { case 0: Add(3); Add(4); Add(9); break; case 1: Add(3); Add(9); break; }
            if (PropLicSettings.Xbox) { Add(5); Add(33); Add(11); }
            if (PropLicSettings.PlayStation) { Add(6); Add(10); Add(30); Add(31); Add(32); }
            if (PropLicSettings.Wii) { Add(23); Add(36); }
            if (PropLicSettings.Nin) { Add(39); Add(35); }
            if (PropLicSettings.NRelease) { Add(61); }
            if (PropLicSettings.Educt) { Add(63); }
            switch (PropLicSettings.Android) { case 0: Add(12); Add(13); break; case 1: Add(12); break; }
            switch (PropLicSettings.Flash) { case 0: Add(14); Add(15); break; case 1: Add(14); break; }
            switch (PropLicSettings.WinStore) { case 0: Add(19); Add(20); Add(21); Add(26); break; case 1: Add(19); break; }
            switch (PropLicSettings.SamsungTv) { case 0: Add(24); Add(25); Add(34); break; case 1: Add(24); Add(34); break; }
            switch (PropLicSettings.Blackberry) { case 0: Add(17); Add(18); Add(28); break; case 1: Add(17); Add(28); break; }
            switch (PropLicSettings.Tizen) { case 0: Add(33); Add(34); Add(29); break; case 1: Add(33); Add(29); break; }

            return list.Distinct().OrderBy(p => int.Parse(p.Value)).ToArray();
        }

    }
}


/* 原始代码
 
  public class wlf
    {
		//public bool WriteLicenseToFile(string appDir, bool spfold, int version)
		public bool WriteLicenseToFile()
		{
			string text = string.Format("{0}-{1}-{2}-{3}-{4}-{5}", "U3", "XXXX", "XXXX", "XXXX", "XXXX", "XXXX");
			
			List<byte> list = new List<byte>();
            list.AddRange(new byte[4] { 1, 0, 0, 0 });
            list.AddRange(System.Text.Encoding.ASCII.GetBytes($"{text}"));
			List<string> list2 = new List<string>
			{
				"<root>",
				"  <TimeStamp2 Value=\"cn/lkLOZ3vFvbQ==\"/>",
				"  <TimeStamp Value=\"jWj8PXAeZMPzUw==\"/>",
				"  <License id=\"Terms\">",
				"    <ClientProvidedVersion Value=\"\"/>",
				$"    <DeveloperData Value=\"{Convert.ToBase64String(list.ToArray())}\"/>",
				"    <Features>"
			};
			int[] array = LicHeader.ReadAll();
			foreach (int num in array)
			{
				list2.Add($"      <Feature Value=\"{num}\"/>");
			}
			list2.Add("    </Features>");
			list2.Add("    <LicenseVersion Value=\"6.x\"/>");
			list2.Add("    <MachineBindings>");
			list2.Add("    </MachineBindings>");
			list2.Add("    <MachineID Value=\"\"/>");
			list2.Add("    <SerialHash Value=\"\"/>");
			list2.Add($"    <SerialMasked Value=\"{text}-XXXX\"/>");
			DateTime now = DateTime.Now;
			list2.Add(string.Format("    <StartDate Value=\"{0}T00:00:00\"/>", now.AddDays(-1.0).ToString("yyyy-MM-dd")));
			list2.Add("    <StopDate Value=\"\"/>");
			list2.Add(string.Format("    <UpdateDate Value=\"{0}T00:00:00\"/>", now.AddYears(10).ToString("yyyy-MM-dd")));
			list2.Add("  </License>");
			list2.Add("");
			list2.Add("<Signature xmlns=\"http://www.w3.org/2000/09/xmldsig#\">");
			list2.Add("<SignedInfo>");
			list2.Add("<CanonicalizationMethod Algorithm=\"http://www.w3.org/TR/2001/REC-xml-c14n-20010315#WithComments\"/>");
			list2.Add("<SignatureMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#rsa-sha1\"/>");
			list2.Add("<Reference URI=\"#Terms\">");
			list2.Add("<Transforms>");
			list2.Add("<Transform Algorithm=\"http://www.w3.org/2000/09/xmldsig#enveloped-signature\"/>");
			list2.Add("</Transforms>");
			list2.Add("<DigestMethod Algorithm=\"http://www.w3.org/2000/09/xmldsig#sha1\"/>");
			list2.Add("<DigestValue>oeMc1KScgy617DHMPTxbYhqNjIM=</DigestValue>");
			list2.Add("</Reference>");
			list2.Add("</SignedInfo>");
			list2.Add("<SignatureValue>WuzMPTi0Ko1vffk9gf9ds/iU0b0K8UHaLpi4kWgm6q1am5MPTYYnzH1InaSWuzYo");
			list2.Add("EpJThKspOZdO0JISeEolNdJVf3JpsY55OsD8UaruvhwZn4r9pLeNSC7SzQ1rvAWP");
			list2.Add("h77XaHizhVVs15w6NYevP27LTxbZaem5L8Zs+34VKXQFeG4g0dEI/Jhl70TqE0CS");
			list2.Add("YNF+D0zqEtyMNHsh0Rq/vPLSzPXUN12jfPLZ3dO9B+9/mG7Ljd6emZjjLZUVuSKQ");
			list2.Add("uKxN5jlHZsm2kRMudijICV6YOWMPT+oZePlCg+BJQg5/xcN5aYVBDZhNeuNwQL1H");
			list2.Add("MPT/GJPxVuETgd9k8c4uDg==</SignatureValue>");
			list2.Add("</Signature>");
			list2.Add("</root>");

			StringBuilder sb = new StringBuilder();
			list2.ForEach(p => sb.AppendLine(p));
			Console.WriteLine(sb.ToString());
			return true;
			
			string text2 = string.Empty;
			if (spfold)
			{
				text2 = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData) + "\\Unity";
				if (!Directory.Exists(text2))
				{
					try
					{
						Directory.CreateDirectory(text2);
					}
					catch (Exception ex)
					{
						spfold = false;
						MessageBox.Show(ex.Message, string.Empty, MessageBoxButtons.OK);
					}
				}
				else
				{
					spfold = true;
				}
			}
			string text3 = text2 + "\\Unity_lic.ulf";
			if (spfold)
			{
				if (File.Exists(text3))
				{
					File.Delete(text3);
				}
				if (spfold)
				{
					try
					{
						if (text3 == text2 + "\\Unity_lic.ulf")
						{
							using (FileStream fileStream = new FileStream(text3, FileMode.Append))
							{
								foreach (string item in list2)
								{
									byte[] bytes = System.Text.Encoding.ASCII.GetBytes(item + "\r");
									fileStream.Write(bytes, 0, bytes.Length);
								}
								fileStream.Flush();
								fileStream.Close();
							}
						}
						else
						{
							File.WriteAllLines(text3, list2);
						}
					}
					catch (Exception ex2)
					{
						spfold = false;
						MessageBox.Show(ex2.Message, string.Empty, MessageBoxButtons.OK);
					}
				}
			}
			if (!spfold)
			{
				if (File.Exists(text3))
				{
					File.Delete(text3);
				}
				try
				{
					if (text3 == text2 + "\\Unity_lic.ulf")
					{
						using (FileStream fileStream2 = new FileStream(text3, FileMode.Append))
						{
							foreach (string item2 in list2)
							{
								byte[] bytes2 = System.Text.Encoding.ASCII.GetBytes(item2 + "\r");
								fileStream2.Write(bytes2, 0, bytes2.Length);
							}
							fileStream2.Flush();
							fileStream2.Close();
						}
					}
					else
					{
						File.WriteAllLines(text3, list2);
					}
				}
				catch (Exception ex3)
				{
					list2.Clear();
					MessageBox.Show(ex3.Message, string.Empty, MessageBoxButtons.OK);
					return false;
				}
			}
			list2.Clear();
			return true;
			
		}
	}

	public class LicHeader
{
	public class LicSettings
	{
		public int Android;

		public int Blackberry;

		public bool Educt;

		public int Flash;

		public int IPhone;

		public bool Nin = true;

		public bool NRelease;

		public bool PlayStation = true;

		public int SamsungTv;

		public bool Team = true;

		public int Tizen;

		public int Type = 1;

		public bool Wii = true;

		public int WinStore;

		public bool Xbox = true;
	}

	public static LicSettings PropLicSettings { get; set; } = new LicSettings();


	public static int[] ReadAll()
	{
		List<int> list = new List<int>();
		switch (PropLicSettings.Type)
		{
			case 0:
				list.Add(0);
				list.Add(1);
				list.Add(16);
				break;
			case 1:
				list.Add(0);
				list.Add(1);
				break;
			case 2:
				list.Add(62);
				break;
		}
		if (PropLicSettings.Team)
		{
			list.Add(2);
		}
		switch (PropLicSettings.IPhone)
		{
			case 0:
				list.Add(3);
				list.Add(4);
				list.Add(9);
				break;
			case 1:
				list.Add(3);
				list.Add(9);
				break;
		}
		if (PropLicSettings.Xbox)
		{
			list.Add(5);
			list.Add(33);
			list.Add(11);
		}
		if (PropLicSettings.PlayStation)
		{
			list.Add(6);
			list.Add(10);
			list.Add(30);
			list.Add(31);
			list.Add(32);
		}
		if (PropLicSettings.Wii)
		{
			list.Add(23);
			list.Add(36);
		}
		if (PropLicSettings.Nin)
		{
			list.Add(39);
			list.Add(35);
		}
		if (PropLicSettings.NRelease)
		{
			list.Add(61);
		}
		if (PropLicSettings.Educt)
		{
			list.Add(63);
		}
		switch (PropLicSettings.Android)
		{
			case 0:
				list.Add(12);
				list.Add(13);
				break;
			case 1:
				list.Add(12);
				break;
		}
		switch (PropLicSettings.Flash)
		{
			case 0:
				list.Add(14);
				list.Add(15);
				break;
			case 1:
				list.Add(14);
				break;
		}
		switch (PropLicSettings.WinStore)
		{
			case 0:
				list.Add(19);
				list.Add(20);
				list.Add(21);
				list.Add(26);
				break;
			case 1:
				list.Add(19);
				break;
		}
		switch (PropLicSettings.SamsungTv)
		{
			case 0:
				list.Add(24);
				list.Add(25);
				list.Add(34);
				break;
			case 1:
				list.Add(24);
				list.Add(34);
				break;
		}
		switch (PropLicSettings.Blackberry)
		{
			case 0:
				list.Add(17);
				list.Add(18);
				list.Add(28);
				break;
			case 1:
				list.Add(17);
				list.Add(28);
				break;
		}
		switch (PropLicSettings.Tizen)
		{
			case 0:
				list.Add(33);
				list.Add(34);
				list.Add(29);
				break;
			case 1:
				list.Add(33);
				list.Add(29);
				break;
		}
		list.Sort();
		return list.Distinct().ToArray();
	}
}

*/