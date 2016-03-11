using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using BookProgress.Web.Models;

namespace TestWebProject.Models
{
    public class BookContext: DbContext
    {
        public BookContext()
            : base("name=DefaultConnection")
        {
        }

        public DbSet<Book> BookItems { get; set; }
    }
}