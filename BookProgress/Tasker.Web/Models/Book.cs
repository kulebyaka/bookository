using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Tasker.Web.Models
{
    public class Book
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public Status Status;
    }

    public enum Status
    {
        Reeding = 0,
        Complite = 1,
        Planned = 2,
    }
}