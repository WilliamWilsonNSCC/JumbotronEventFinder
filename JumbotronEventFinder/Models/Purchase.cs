using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JumbotronEventFinder.Models
{
    public class Purchase
    {
        //primary Key
        public int PurchaseId { get; set; }

        public int TicketSales { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PaymentType { get; set; } = string.Empty;

        public int CardNumber { get; set; }

        [NotMapped]
        public string ExpirationDate { get; set; } = string.Empty;

        public int CCV { get; set; }

        //Foreign Key
        [Display(Name = "Show")]
        public int ShowId { get; set; }

        //Navigation property
        public Show? Show { get; set; } //nullable

        [NotMapped]
        [Display(Name = "Purchases")]
        public IFormFile? FormFile { get; set; } //Nullable
    }
}
