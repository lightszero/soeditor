using System;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var stream = System.IO.File.OpenRead("sample\\libmain.so"))
            {
                soeditor.elfformat.ElfDocument doc = new soeditor.elfformat.ElfDocument();
                doc.Read(stream);
                doc.Dump((log) =>
                {
                    Console.WriteLine(log);
                });


                soeditor.elfparse.ParsedDocument docP = new soeditor.elfparse.ParsedDocument();
                docP.Parse(doc);
                docP.Dump((log) =>
                {
                    Console.WriteLine(log);
                });
                Console.ReadLine();
            }
        }
    }
}