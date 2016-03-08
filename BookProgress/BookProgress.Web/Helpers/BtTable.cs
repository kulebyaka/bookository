using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BookProgress.Web.Helpers
{
    public class BtTable : IDisposable
    {
        

        private HtmlHelper _html;
        private HtmlTextWriter _writer = null;
        private int _totalRecords = -1;

        public BtTable(HtmlHelper html)
        {
            _html = html;
            _writer = new HtmlTextWriter(html.ViewContext.Writer);

            _writer.AddAttribute(HtmlTextWriterAttribute.Class, "table table-hover");
            _writer.RenderBeginTag(HtmlTextWriterTag.Table);
        }

        public BtTable TotalRecords(int totalRecords)
        {
            _totalRecords = totalRecords;
            return this;
        }

        public BtTableHeader Header(string content)
        {
            return new BtTableHeader(content);
        }        

        public TableContextMenu ContextMenu()
        {
            return new TableContextMenu(_html);
        }

        public void Dispose()
        {
            _writer.RenderEndTag();
        }

        public string DisabledRow(bool active)
        {
            return active ? "" : "disabled";
        }

        public class TableContextMenu : IHtmlString
        {
            private HtmlTextWriter _writer = new HtmlTextWriter(new StringWriter());
            private HtmlHelper _html;
            private UrlHelper _url;

            public TableContextMenu(HtmlHelper html)
            {
                _html = html;
                _url = new UrlHelper(_html.ViewContext.RequestContext);

                _writer.AddStyleAttribute(HtmlTextWriterStyle.Display, "none");
                _writer.RenderBeginTag("td");

                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "context-menu");
                _writer.RenderBeginTag("ul");
            }

            public TableContextMenu MenuItem(string content, string faIcon, string action = null,
                string controller = null, object routeValues = null, object htmlAttributes = null)
            {
                foreach (var attribute in HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes))
                {
                    _writer.AddAttribute(attribute.Key, attribute.Value.ToString());
                }
                _writer.RenderBeginTag("li");

                string href;
                if (action != null)
                    href = _url.Action(action, controller, routeValues);
                else
                    href = "javascript://nop/";

                _writer.AddAttribute(HtmlTextWriterAttribute.Href, href);
                _writer.RenderBeginTag("a");

                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "fa fa-" + faIcon);
                _writer.RenderBeginTag("i");
                _writer.RenderEndTag();

                _writer.Write(content);
                _writer.RenderEndTag(); // a
                _writer.RenderEndTag(); // li

                

                return this;
            }

            public string ToHtmlString()
            {
                _writer.RenderEndTag();
                _writer.RenderEndTag();
                return _writer.InnerWriter.ToString();
            }
        }

        public class BtTableHeader : IHtmlString
        {
            private HtmlTextWriter _writer = new HtmlTextWriter(new StringWriter());
            private string _content;

            public BtTableHeader(string content)
            {
                _content = content;
            }

            public BtTableHeader Width(string width)
            {
                _writer.AddStyleAttribute(HtmlTextWriterStyle.Width, width);
                return this;
            }

            public BtTableHeader Sort(string sortedValueSelector = null)
            {
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "sort");
                if (sortedValueSelector != null) 
                {
                    _writer.AddAttribute("data-sorted-value-selector", sortedValueSelector);
                }
                return this;
            }

            public string ToHtmlString()
            {
                _writer.RenderBeginTag("th");
                _writer.Write(_content);
                _writer.RenderEndTag();
                return _writer.InnerWriter.ToString();
            }
        }
    }
}