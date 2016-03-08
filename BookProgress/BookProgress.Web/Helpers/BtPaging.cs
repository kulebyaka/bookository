using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace BookProgress.Web.Helpers
{
    public class BtPaging : IDisposable
    {
        const int PagingItemsPerPage = 20;
        private HtmlHelper _html;
        private HtmlTextWriter _writer = null;

        public BtPaging(HtmlHelper html)
        {
            _html = html;
            _writer = new HtmlTextWriter(html.ViewContext.Writer);

            _writer.AddAttribute(HtmlTextWriterAttribute.Class, "paging-container hidden");
            _writer.RenderBeginTag(HtmlTextWriterTag.Div);
        }

        public BtPagingToolbar Toolbar(long count)
        {
            return new BtPagingToolbar(count);
        }

        public string ItemClass 
        { 
            get { return "top-level"; }
        }

        public void Dispose()
        {
            _writer.RenderEndTag();
        }

        public class BtPagingToolbar : IHtmlString
        {
            private HtmlTextWriter _writer = new HtmlTextWriter(new StringWriter());

            public BtPagingToolbar(long count)
            {
                _writer.AddAttribute("data-items-per-page", PagingItemsPerPage.ToString());
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "paging-toolbar");
                _writer.RenderBeginTag("div");
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "pull-right");
                _writer.RenderBeginTag("nav");
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "pagination");
                _writer.AddAttribute("data-active-page", "1");
                _writer.RenderBeginTag("ul");

                this.RenderLink("previous", "Previous", "&laquo;");

                //page
                int pages = (int)(count - 1) / PagingItemsPerPage + 1;

                for (var i = 1; i <= pages; i++)
                {
                    this.RenderPageLink(i, i == 1);
                }

                this.RenderLink("next", "Next", "&raquo;");

                _writer.RenderEndTag();//ul
                _writer.RenderEndTag();//nav
                _writer.RenderEndTag();//div
            }

            private void RenderLink(string cssClass, string label, string symbol)
            {
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass + " always-show");
                _writer.RenderBeginTag("li");
                _writer.AddAttribute("aria-label", label);
                _writer.AddAttribute("href", "javascript:;");
                _writer.RenderBeginTag("a");
                _writer.AddAttribute("aria-hidden", "true");
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "unselectable");
                _writer.RenderBeginTag("span");
                _writer.Write(symbol);
                _writer.RenderEndTag();//span
                _writer.RenderEndTag();//a
                _writer.RenderEndTag();//li
            }

            private void RenderPageLink(int page, bool active)
            {
                string cssClass = "page";

                if (active)
                {
                    cssClass += " active";
                }

                if (page % 10 == 0 || page == 1)
                {
                    cssClass += " always-show";
                }
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
                _writer.AddAttribute("data-page", page.ToString());
                _writer.RenderBeginTag("li");
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, "unselectable");
                _writer.AddAttribute("href", "javascript:;");
                _writer.RenderBeginTag("a");
                _writer.Write(page);
                _writer.RenderEndTag();//a
                _writer.RenderEndTag();//li
            }

            public string ToHtmlString()
            {
                return _writer.InnerWriter.ToString();
            }
        }
    }
}