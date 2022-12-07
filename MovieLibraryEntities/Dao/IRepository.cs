using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public interface IRepository
    {
        void Add(); 
        IEnumerable<Movie> GetAll();
        IEnumerable<Movie> Search(string searchString);
        Movie GetById(int id);
        void Update();
        void Delete();
        void AddUser();
        void UserRateMovie();
        void DisplayAllUsers();
    }
}
