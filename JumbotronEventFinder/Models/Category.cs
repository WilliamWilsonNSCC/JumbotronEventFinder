namespace JumbotronEventFinder.Models
{
    public class Category
    {
       public int CategoryId { get; set; }
       public string Title { get; set; } = string.Empty;

        public List<Show>? Shows { get; set; } 
    }
}
