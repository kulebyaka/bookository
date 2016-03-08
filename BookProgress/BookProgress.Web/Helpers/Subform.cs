using System;
using System.Web.Mvc;
using System.Web.UI;

namespace BookProgress.Web.Helpers
{
    public class Subform : IDisposable
    {
        private HtmlTextWriter _writer = null;

        public Subform(ViewContext viewContext, string url, string tagName)
        {
            _writer = new HtmlTextWriter(viewContext.Writer);
            _writer.AddAttribute("class", "subform");
            _writer.AddAttribute("data-subform-action", url);
            _writer.RenderBeginTag(tagName);
        }

        public void Dispose()
        {
            _writer.RenderEndTag();
        }
    }
}