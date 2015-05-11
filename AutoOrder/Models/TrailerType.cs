using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoOrder.Models
{
    public class TrailerType
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Тип кузова")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Тип перевозки")]
        public string TransportName { get; set; }
    }
}