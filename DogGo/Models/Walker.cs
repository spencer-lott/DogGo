using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DogGo.Models
{
    public class Walker
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(100)]
        [MinLength(1)]
        public string Name { get; set; }
        [Required]
        public int NeighborhoodId { get; set; }
        public string ImageUrl { get; set; }
        public Neighborhood Neighborhood { get; set; }
        public List<Walks> Walks { get; set; }
        public Owner Owner { get; set; }
    }
}