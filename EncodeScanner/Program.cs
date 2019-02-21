using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace EncodeScanner
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"E:\rule-remote\rule_center\airule-parent\src\main\java";
            Scanner scanner = new Scanner(Encoding.UTF8);
            scanner.DoWork(path,
                t => { return !t.ToLower().Contains("impl.java"); },
                t => { return !t.ToLower().Contains("const.java"); },
                t => { return !t.ToLower().Contains(".bo"); });
            Console.ReadKey();
        }
    }

    static class ScannerExtension
    {
        private static char unknown_char = Convert.ToChar(65533);
        public static bool ContainsUnknown(this string str) => str.Contains(unknown_char);
    }

    class Scanner
    {
        static Scanner()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        private static string[] SupportEncoding = { Encoding.UTF8.BodyName, "GB2312", Encoding.Unicode.EncodingName, Encoding.ASCII.EncodingName, };


        private readonly Encoding TargetEncoding;

        public Scanner(Encoding TargetEncoding)
        {
            this.TargetEncoding = TargetEncoding;
        }

        public void DoWork(string target_dir_path, params Func<string, bool>[] path_fliters)
        {
            foreach (var path in Directory.EnumerateFiles(target_dir_path))
            {
                var continue_flag = false;
                foreach (var fliter in path_fliters)
                    if (!fliter.Invoke(path))
                    {
                        continue_flag = true;
                        break;
                    }
                if (continue_flag) continue;

                var file_encoding = GetEncoding(path);
                if (file_encoding != null && file_encoding != TargetEncoding)
                    ChangeEncode(path, file_encoding);
            }
            foreach (var dir in Directory.EnumerateDirectories(target_dir_path))
                DoWork(dir, path_fliters);
        }



        /// <summary>
        /// return true if encoding equals RightEncode
        /// </summary>
        /// <param name="path"></param>
        /// <returns>file encoding</returns>
        public static Encoding GetEncoding(string path) => GetEncoding(path, 0);

        private static Encoding GetEncoding(string path, int ic)
        {
            if (ic == SupportEncoding.Length) return Encoding.Default;
            Encoding encoding = Encoding.GetEncoding(SupportEncoding[ic]);
            string str;
            using (var fs = File.Open(path, FileMode.Open))
            using (var sr = new StreamReader(fs, encoding))
                str = sr.ReadToEnd();
            if (str.ContainsUnknown())
                return GetEncoding(path, ++ic);
            else
                return encoding;

        }

        private void ChangeEncode(string path, Encoding encoding)
        {
            using (var fs = File.Open(path, FileMode.Open, FileAccess.ReadWrite))
            {
                string str = new StreamReader(fs, encoding).ReadToEnd();
                if (!str.ContainsUnknown())
                {
                    Console.WriteLine(path + "<<<" + encoding.EncodingName + "<<<" + TargetEncoding.EncodingName);
                    fs.Position = 0;
                    fs.Write(TargetEncoding.GetBytes(str));
                }
            }
        }
    }
}
