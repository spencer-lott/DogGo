namespace DogGo.Models.ViewModels
{
    public class WalkerFormViewModel
    {
        public Walker Walker { get; set; }
        public List<Walks> Walks { get; set; }
        public List<Dog> Dogs { get; set; }
        public List<Walker> Walkers { get; set; }
        //The list of DogIds is essential because it's how the user selects multiple dogs to take on a walk. The Id of the dog is stored here then can be access by a foreach loop
        public List<int > DogIds { get; set; } = new List<int>();

        public Walks Walk { get; set; }
    }
}
