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
        public virtual TrailerType TransportType { get; set; }

        [Required]
        [DisplayName("Длина объекта")]
        public double ObjectLength { get; set; }

        [Required]
        [DisplayName("Ширина объекта")]
        public double ObjectWidth { get; set; }

        [Required]
        [DisplayName("Высота объекта")]
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

        [DisplayName("Автомобиль для перевозки")]
        public int? AutoparkId { get; set; }
        [ForeignKey("AutoparkId")]
        public virtual Autopark Autopark { get; set; }

        [DisplayName("Примечание")]
        public string Comment { get; set; }

        public bool IsCancelled { get; set; }

        public double Capacity
        {
            get { return ObjectWidth * ObjectHeight * ObjectLength; }
        }

        public bool IsCompleted
        {
            get { return (FactInDate != null) && (FactOutDate != null); }
        }

        public bool HasAuto
        {
            get { return AutoparkId != null; }
        }

        public bool IsOutOfDate
        {
            get { return ProspectiveOutDate < DateTime.Now; }
        }

        public string HtmlClassForRow {
            get
            {
                if (IsCancelled) return "info";
                if (IsCompleted) return "success";
                if (IsOutOfDate) return "danger";
                if (!HasAuto) return "warning";
                return "default";
            }
        }
    }
}