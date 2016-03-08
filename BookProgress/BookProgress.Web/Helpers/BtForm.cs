using System;
using System.Web.Mvc;
using System.Web.UI;

namespace BookProgress.Web.Helpers
{
    public class BtForm : IDisposable
    {
        private HtmlTextWriter _writer = null;

        public BtForm(ViewContext viewContext)
        {
            _writer = new HtmlTextWriter(viewContext.Writer);

            _writer.AddAttribute(HtmlTextWriterAttribute.Class, "form-horizontal");
            _writer.AddAttribute("method", "post");
            _writer.RenderBeginTag(HtmlTextWriterTag.Form);
        }

        public void Dispose()
        {
            _writer.RenderEndTag();
        }
    }
}