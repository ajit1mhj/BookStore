using System.ComponentModel.DataAnnotations;

namespace BookStore.Models.DTOS
{
    public class GenreDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(50)]
        public string GenreName { get; set; } 
    }
}
