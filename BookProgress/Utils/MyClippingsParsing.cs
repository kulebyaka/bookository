using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using DAL;

namespace Utils
{
    public static class MyClippingsParsing
    {
        //          To (Stephen King)
        //          - Highlight on Page 451 | Loc. 6909  | Added on Monday, March 14, 2016, 09:15 AM

        //          klepů
        //          ==========
        //          To (Stephen King)
        //          - Highlight on Page 451 | Loc. 6913  | Added on Monday, March 14, 2016, 09:16 AM

        //          váhavě
        //          ==========

        public static Bookmark GetBookmark(string myClippings)
        {
            Regex regex = new Regex(@"(?<Title>[a-zA-Z0-9_ ]*)(?<Author>\([a-zA-Z0-9_ ]*\))(\n|\r|\r\n)(.+)(\n|\r|\r\n)(\n|\r|\r\n)(?<Bookmark>.+)");
            var matches = regex.Matches(myClippings);
            foreach (var match in matches)
            {
                var x = match;
            }
            //TODO: http://stackoverflow.com/questions/16947390/using-regex-to-parse-kindle-my-clippings-txt-file
            return null;
        }
    }
}
