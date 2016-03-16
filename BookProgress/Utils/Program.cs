using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL;
using Utils.Metadata;

namespace Utils
{
    class Program
    {
        const string EndOfBookmark = "==========";

        static void Main(string[] args)
        {
            Console.WriteLine(GetMetadata());
            //Console.WriteLine(GetHighlights());
            Console.ReadLine();

            object o = Encoding.Unicode.CodePage; 
            Encoding enc = Encoding.GetEncoding(int.Parse(o.ToString()));
        }

        private static string GetMetadata()
        {
            MobiMetadata mobi = new MobiMetadata(@"C:\Users\Kirill\Downloads\Dikson_Plenennaya-Vselennaya-avtorskiy-sbornik-.392223.fb2.mobi");

            string fileName = mobi.PDBHeader.Name;
            uint fileVersion = mobi.MobiHeader.FileVersion;
            string fullname = mobi.MobiHeader.FullName;
            string authorName = mobi.MobiHeader.EXTHHeader.Author;
            string updatedTitle = mobi.MobiHeader.EXTHHeader.UpdatedTitle;
            var dontKnow = mobi.MobiHeader.EXTHHeader.fieldListNoBlankRows;
            var anotherField = mobi.PalmDocHeader.ToString();
            return String.Format("Title = {0}; Author = {1}", fullname, authorName);
        }

        private static string GetHighlights()
        {
            var bookmarks = new List<Bookmark>();
            StringBuilder clipping = new StringBuilder();
            foreach (var line in File.ReadLines(@"C:\Users\Kirill\Downloads\My Clippings.txt"))
            {
                if (string.Equals(EndOfBookmark, line, StringComparison.CurrentCultureIgnoreCase))
                {
                    bookmarks.Add(MyClippingsParsing.GetBookmark(clipping.ToString()));
                    clipping.Clear();
                }
                else
                {
                    clipping.AppendLine(line);
                }


            }

            var returnedString = new StringBuilder();
            foreach (var bookmark in bookmarks)
            {
                returnedString.AppendLine(String.Format("Book = {0}; Bookmark = {1}", bookmark.BookTitle, bookmark.Value));
            }
            return returnedString.ToString();
        }
    }
}
