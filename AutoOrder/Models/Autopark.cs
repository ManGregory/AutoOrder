using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoOrder.Models
{
    public class Autopark
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [DisplayName("Название машины")]
        public string Name { get; set; }

        [Required]
        [DisplayName("Прицеп")]
        public string Trailer { get; set; }

        [Required]
        [DisplayName("Количество прицепов")]
        public int TrailerCount { get; set; }

        [Required]
        [DisplayName("Тип кузова")]
        public int TrailerTypeId { get; set; }
        [DisplayName("Тип кузова")]
        [ForeignKey("TrailerTypeId")]
        public virtual TrailerType TrailerType { get; set; }

        [Required]
        [DisplayName("Длина прицепа")]
        public double TrailerLength { get; set; }

        [Required]
        [DisplayName("Ширина прицепа")]
        public double TrailerWidth { get; set; }

        [Required]
        [DisplayName("Высота прицепа")]
        public double TrailerHeight { get; set; }

        public double Capacity {
            get { return TrailerWidth*TrailerHeight*TrailerLength; }
        }
    }
}