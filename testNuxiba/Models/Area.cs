using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testNuxiba.Models
{
    [Table("ccRIACat_Areas")]
    public class Area
    {
        [Key]
        public int IDArea { get; set; }

        [Required]
        public string AreaName { get; set; }= string.Empty;

        public int StatusArea { get; set; }
        public DateTime? CreateDate { get; set; }
    }
}
