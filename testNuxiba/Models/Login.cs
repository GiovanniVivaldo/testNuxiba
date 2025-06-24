    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    namespace testNuxiba.Models
    {
        [Table("ccLogLogin")]
        public class Login
        {
            //se agrega ID ya que es necesario tener un ID unico en EF
            [Key]
            public int Id { get; set; }

            [Required]
            public int User_id { get; set; }
        
            public int? Extension {  get; set; }
        
            [Required]
            [Range(0,1)]
            public int TipoMov {  get; set; }

            [Required]
            public DateTime Fecha { get; set; }
        
            [ForeignKey("User_id")]
            public User? User { get; set; }

        }
    }
