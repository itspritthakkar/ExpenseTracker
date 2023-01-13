using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardAPI.Models
{
    public class Transaction : BaseEntity
    {
        [Key]
        public int TransactionId { get; set; }

        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        [Required, Column(TypeName = "nvarchar(70)")]
        public string Title { get; set; } = string.Empty;

        [LimitValidation]
        public double Amount { get; set; }

        [Column(TypeName = "nvarchar(255)")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        public DateTime Date { get; set; } = DateTime.Now;

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.Type == "Expense") ? "- " : "+ ") + Amount.ToString("C0");
            }
        }
    }
}
