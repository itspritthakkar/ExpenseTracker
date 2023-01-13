using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DashboardAPI.Models
{
    public class Category : BaseEntity
    {
        [Key]
        public int CategoryId { get; set; }

        [Required, Column(TypeName = "nvarchar(50)")]
        public string? Name { get; set; }

        [Column(TypeName = "nvarchar(150)")]
        public string Icon { get; set; } = string.Empty;

        [NotMapped]
        [DisplayName("Icon")]
        public IFormFile? IconFile { get; set; }

        [Column(TypeName = "nvarchar(50)")]
        public string Type { get; set; } = "Expense";

        public byte Bookmark { get; set; } = 0;

        public double Limit { get; set; }
    }

    public class BaseEntity
    {
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
