using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using DAL;

namespace Utils
{
    public static class MyClippingsParsing
    {
        public static List<Bookmark> GetBookmarks(FileStream myClippings)
        {
            Regex regex = new Regex(@"\d+");
            Match match = regex.Match("Dot 55 Perls");
            //TODO: http://stackoverflow.com/questions/16947390/using-regex-to-parse-kindle-my-clippings-txt-file
            return null;
        }
    }
}
