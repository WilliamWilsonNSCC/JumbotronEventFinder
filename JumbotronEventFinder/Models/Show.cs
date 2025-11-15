using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JumbotronEventFinder.Models
{
    public class Show
    {
        //internal string CategoryTitle;

        public int ShowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Creator { get; set; } = string.Empty;
        public string Filename { get; set; } = string.Empty;
        public DateTime Date { get; set; }

        [Display(Name = "Created")]
        public DateTime CreateDate { get; set; }

        //Foreign Key
        [Display(Name="Category")]
        public int CategoryId { get; set; }

        //Navigation property
        public Category? Category { get; set; }
        public List<Purchase>? Purchases { get; set; }

        [NotMapped]
        [Display(Name = "Shows")]
        public IFormFile? FormFile { get; set; } //Nullable
    }
}
