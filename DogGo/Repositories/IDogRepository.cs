using DogGo.Models;

namespace DogGo.Repositories
{
    public interface IDogRepository
    {
        List<Dog> GetAllDogs();
        List<Dog> GetDogsByOwnerId(int ownerId);
        Dog GetDogById(int id);
        void AddDog(Dog dog);
        void UpdateDog(Dog dog);
        void DeleteDog(int dogId);

    }
}
