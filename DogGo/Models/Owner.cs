﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DogGo.Models
{
    public class Owner
    {
        //These are validation attributes. They are pretty straightforward with what they do.
        public int Id { get; set; }
        [EmailAddress]
        [Required]
        public string Email { get; set; }
        [Required(ErrorMessage = "Hmmm... You should really add a Name...")]
        [MaxLength(35)]
        public string Name { get; set; }
        [Required]
        [StringLength(55, MinimumLength = 5)]
        public string Address { get; set; }
        [Required]
        public int NeighborhoodId { get; set; }
        [Phone]
        public string Phone { get; set; }
        [Required]
        [DisplayName("Neighborhood")]
        public Neighborhood Neighborhood { get; set; }
        public List<Dog> Dogs { get; set; }

    }
}
