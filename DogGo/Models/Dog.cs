using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
namespace DogGo.Models
{
    public class Dog
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Really? You didn't name your dog? You're a terrible person. Name it right now in order to continue.")]
        [MaxLength(35)]
        public string Name { get; set; }
        [Required]
        [DisplayName("Id of Owner")]
        public int OwnerId { get; set; }
        public string Breed { get; set; }
        public string Notes { get; set; }
        public string ImageUrl { get; set; }

    }
}
