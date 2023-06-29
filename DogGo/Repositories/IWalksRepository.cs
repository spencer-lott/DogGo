using DogGo.Models;

namespace DogGo.Repositories
{
    public interface IWalksRepository
    {
        List<Walks> GetAllWalks();
        Walks GetWalksById(int id);
        List<Walks> GetWalksByWalkerId(int walkerId);
        void AddWalks (Walks walks);

    }
}
