using System;
using System.Web.Mvc;
using System.Web.UI;

namespace BookProgress.Web.Helpers
{
    public class BtPanel : IDisposable
    {
        private HtmlTextWriter _writer = null;

        public BtPanel(ViewContext viewContext, string type)
        {
            _writer = new HtmlTextWriter(viewContext.Writer);
            _writer.AddAttribute("class", "panel panel-" + type);
            _writer.RenderBeginTag("div");
        }

        public void Dispose()
        {
            _writer.RenderEndTag();
        }
    }
}