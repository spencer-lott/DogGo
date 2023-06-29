using System.ComponentModel.DataAnnotations;

namespace DogGo.Models
{
    public class Walks
    {
        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM dd, yyyy}")]
        public DateTime Date { get; set; }
        public int Duration { get; set; }
        public int WalkerId { get; set; }
        public int DogId { get; set; }
        public Owner Owner { get; set; }
        public Dog Dog { get; set; }

    }
}
