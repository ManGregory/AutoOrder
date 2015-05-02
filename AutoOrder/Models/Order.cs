using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AutoOrder.Models
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        [Required]
        [DisplayName("Телефон")]
        public string Phone { get; set; }

        [Required]
        [DisplayName("Тип перевозимого объекта")]
        public string TransportedObjectType { get; set; }

        [Required]
        [DisplayName("Тип перевозки")]
        public int TransportTypeId { get; set; }
        [ForeignKey("TransportTypeId")]
        public TrailerType TransportType { get; set; }

        [Required]
        [DisplayName("Длина прицепа")]
        public double ObjectLength { get; set; }

        [Required]
        [DisplayName("Ширина прицепа")]
        public double ObjectWidth { get; set; }

        [Required]
        [DisplayName("Высота прицепа")]
        public double ObjectHeight { get; set; }

        [Required]
        [DisplayName("Перспективная дата погрузки")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString="{0:yyyy-MM-dd}")]
        public DateTime ProspectiveInDate { get; set; }

        [Required]
        [DisplayName("Перспективная дата разгрузки")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ProspectiveOutDate { get; set; }

        [Required]
        [DisplayName("Адрес погрузки")]
        public string AddressFrom { get; set; }

        [Required]
        [DisplayName("Адрес разгрузки")]
        public string AddressTo { get; set; }

        [DisplayName("Фактическая дата погрузки")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FactInDate { get; set; }

        [DisplayName("Фактическая дата разгрузки")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? FactOutDate { get; set; }

        public int? AutoparkId { get; set; }
        [ForeignKey("AutoparkId")]
        public Autopark Autopark { get; set; }

        public bool IsCancelled { get; set; }
    }
}