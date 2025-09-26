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
        public DateTime CreateDate { get; set; }

        //Foreign Key
        public int CategoryId { get; set; }

        //Navigation property
        public Category? Category { get; set; }
    }
}
