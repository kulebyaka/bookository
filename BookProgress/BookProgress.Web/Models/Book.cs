using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace BookProgress.Web.Models
{
    public class Book
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "Title")]
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P2}")]
        [Range(0, 100)]
        public decimal Percentage;
        public List<Tag> Tags;
    }

    public enum Status
    {
        Reeding = 0,
        Complete = 1,
        Planned = 2,
    }
}