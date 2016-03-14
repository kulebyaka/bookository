using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils.Metadata;

namespace Utils
{
    class Program
    {
        static void Main(string[] args)
        {
            MobiMetadata mobi = new MobiMetadata(@"C:\Users\Kirill\Downloads\Ellis_Pravila-seksa.222273.fb2.mobi");

            string fileName = mobi.PDBHeader.Name;
            uint fileVersion = mobi.MobiHeader.FileVersion;
            string authorName = mobi.MobiHeader.EXTHHeader.Author;
            string updatedTitle = mobi.MobiHeader.EXTHHeader.UpdatedTitle;

            Console.WriteLine(mobi.MobiHeader.EXTHHeader.fieldListNoBlankRows);
            Console.WriteLine(mobi.PalmDocHeader.ToString());
            Console.ReadLine();
        }
    }
}
