namespace JumbotronEventFinder.Models
{
    public class Category
    {
       public int CategoryId { get; set; }
       public string Title { get; set; } = string.Empty;

        public List<Event>? Events { get; set; } 
    }
}
