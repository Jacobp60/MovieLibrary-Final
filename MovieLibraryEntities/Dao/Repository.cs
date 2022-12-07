using Castle.Core.Internal;
using Microsoft.EntityFrameworkCore;
using MovieLibraryEntities.Context;
using MovieLibraryEntities.Models;

namespace MovieLibraryEntities.Dao
{
    public class Repository : IRepository, IDisposable
    {
        private readonly IDbContextFactory<MovieContext> _contextFactory;
        private readonly MovieContext _context;

        public Repository(IDbContextFactory<MovieContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _context = _contextFactory.CreateDbContext();
        }

        public void Add()
        {
            Console.WriteLine("Enter the movie Title");
            var movTitle = Console.ReadLine();
            var movie = new Movie();

            if (string.IsNullOrEmpty(movTitle))
            {
                Console.WriteLine("Invalid Input");
            }
            else
            {
                movie.Title = movTitle;
                Console.WriteLine("Enter the release date");
                var movieRelease = Console.ReadLine();
                if (string.IsNullOrEmpty(movieRelease))
                {
                    Console.WriteLine("Invald Input");
                }
                else
                {
                    try
                    {
                        var movieReleaseDate = DateTime.Parse(movieRelease);
                        movie.ReleaseDate = movieReleaseDate;
                    }catch(FormatException)
                    {
                        Console.WriteLine("Invalid Date input");
                    }
                }
                
            }

            _context.Add(movie);
            _context.SaveChanges();
        }

        public void AddUser()
        {
                User newuser = new User();
                var addChoice = "";
                do
                {
                    var userAge = 0;
                    Console.WriteLine("Enter age of new user: ");
                    var age = Console.ReadLine();
                    while (!int.TryParse(age, out userAge))
                    {
                        Console.WriteLine("Must be a integer!\nEnter valid age: ");
                        age = Console.ReadLine();
                    }

                    if (userAge < 1 || userAge > 100)
                    {
                        Console.WriteLine("Not a valid age! Age must between the ages of 1-100");
                        userAge = Int32.Parse(Console.ReadLine());
                    }

                    newuser.Age = userAge;
                    Console.WriteLine("Enter user gender (M/F): ");
                    var userGender = Console.ReadLine().Substring(0, 1).ToUpper();
                    if (userGender != "M" || userGender != "F")
                    {
                        Console.WriteLine("Please Enter a valid Gender input");
                        Console.WriteLine("Enter user gender (M/F): ");
                        userGender = Console.ReadLine().Substring(0, 1).ToUpper();
                    }   

                    newuser.Gender = userGender;

                    Console.WriteLine("Enter User ZipCode: ");
                    var zipCode = Console.ReadLine();
                    while (zipCode.Length != 5)
                    {
                        Console.WriteLine("Please enter a 5 digit ZipCode");
                        zipCode = Console.ReadLine();
                    }

                    newuser.ZipCode = zipCode;
                    Console.WriteLine("Enter occupation Id, here are possible occupations: ");

                    foreach (var o in _context.Occupations)
                    {
                        Console.WriteLine($"Id: {o.Id}) {o.Name}");
                    }

                    var occId = Console.ReadLine();
                    var oID = 0;
                    while (!int.TryParse(occId, out oID))
                    {
                        Console.WriteLine("Must be a number!  Enter valid Occupation Id: ");
                        occId = Console.ReadLine();
                    }

                    while (oID < 0 || oID > 21)
                    {
                        Console.WriteLine("Not a valid Occupation Id! Enter a number from 1 - 21: ");
                    }

                    var occQuery = (from o in _context.Occupations where o.Id == oID select o).FirstOrDefault();
                    if (occQuery != null)
                    {
                        newuser.Occupation = occQuery;
                    }


                    Console.WriteLine($"You've entered:\nUser Age of: {newuser.Age}\n" +
                                      $"User Gender of: {newuser.Gender}\n" +
                                      $"User ZipCode of: {newuser.ZipCode}\n" +
                                      $"User Occupation of: {newuser.Occupation.Name}\n" +
                                      $"Add user to database? (y/n)");

                    addChoice = Console.ReadLine().ToLower().Substring(0, 1);


                } while (addChoice != "y");

                _context.Users.Add(newuser);
                _context.SaveChanges();
            }

        public void Delete()
        {
            try {
                Console.WriteLine($"Here is a list of all Movies:\n");
                foreach (Movie movie in _context.Movies)
                {
                    Console.WriteLine($"Movie ID: {movie.Id}, {movie.Title}");
                }
                Console.WriteLine("\nPlease enter the Movie Id to Delete: ");
                var movieIdSearchDelete = Console.ReadLine();
                if (string.IsNullOrEmpty(movieIdSearchDelete))
                    Console.WriteLine("\nInvalid Input");
                else
                {
                    var userDecision = "";
                    var IntMovieIdDelete = Convert.ToInt64(movieIdSearchDelete);
                    var movieToDelete = _context.Movies.Where(x => x.Id == IntMovieIdDelete).FirstOrDefault();
                    Console.WriteLine($"Are you sure you want to delete {movieToDelete.Title}. (y/n)");
                    userDecision = Console.ReadLine().ToLower();
                    if (userDecision != "y" && userDecision != "n")
                    {
                        Console.WriteLine("Please Enter a valid input");
                        Console.WriteLine($"Are you sure you want to delete {movieToDelete.Title}. (y/n)");
                        userDecision = Console.ReadLine().Substring(0, 1).ToLower();
                    }
                    else if(userDecision == "y")
                    {
                        Console.WriteLine($"Movie Deleted {movieToDelete.Title} with ID {movieToDelete.Id}");
                        _context.Movies.Remove(movieToDelete);
                        _context.SaveChanges();
                    }
                    else if(userDecision == "n")
                    {
                        Console.WriteLine("Going back to M  enu\n");
                    }
                }
            }

            catch (ArgumentNullException)
            {
                Console.WriteLine("Invalid movie. Make sure to input entire movie Id. ");
            }
            catch (NullReferenceException)
            {
                Console.WriteLine("Invalid movie. Please Try Again");
            }
        }

        public void DisplayAllUsers()
        {
            foreach(User user in _context.Users)
            {
                Console.WriteLine($"User Id: {user.Id} , Gender: {user.Gender} , Age: {user.Age}");
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public IEnumerable<Movie> GetAll()
        {
            return _context.Movies.ToList();
        }

        public Movie GetById(int id)
        {
            return _context.Movies.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Movie> Search(string searchString)
        {
            var allMovies = _context.Movies;
            var listOfMovies = allMovies.ToList();
            var temp = listOfMovies.Where(x => x.Title.Contains(searchString, StringComparison.CurrentCultureIgnoreCase));

            return temp;
        }
        public void Update()
        {
            try
            {
                Console.WriteLine("Enter the MovieId to update: ");
                var movieIdSearch = Convert.ToInt32(Console.ReadLine());
                var movieToUpdate = _context.Movies.FirstOrDefault(x => x.Id == movieIdSearch);

                Console.WriteLine($"Here is your movie: {movieToUpdate.Title}");
                Console.WriteLine("What would you want to change title to?:");
                var newTitle = Console.ReadLine();
                if (string.IsNullOrEmpty(newTitle))
                {
                    Console.WriteLine("Please Enter a valid input!");
                }
                else
                {
                    movieToUpdate.Title = newTitle;
                    Console.WriteLine("Please enter a new release date: ");
                    var newreleaseDate = DateTime.Parse(Console.ReadLine());
                    movieToUpdate.ReleaseDate = newreleaseDate.ToUniversalTime();
                    _context.SaveChanges();
                }

            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid Format");
            }
            catch(NullReferenceException)
            {
                Console.WriteLine("Please Enter a Valid Input");
            }
        }

        public void UserRateMovie()
        {
            UserMovie userMovie = new UserMovie();

                Console.WriteLine("Enter title of movie to rate: ");
                var searchString = Console.ReadLine();
                var movieQuery = _context.Movies.Where(x => x.Title == searchString).FirstOrDefault();
            if (movieQuery != null)
            {
               Console.WriteLine($"Do you want to rate {movieQuery.Title}? (y/n)");
               var choice = Console.ReadLine().ToLower();
               if (choice == "y")
               {
                    userMovie.Movie = movieQuery;
                    Console.WriteLine("Enter user Id of user who is rating movie: ");
                    var searchId = Console.ReadLine();
                    var id = 0;
                    while (!int.TryParse(searchId, out id))
                    {
                        Console.WriteLine("Not a number! Enter a valid userID: ");
                        searchId = Console.ReadLine();
                    }
                    var intSearchId = Convert.ToInt64(searchId);

                    var userQuery = _context.Users.Where(x => x.Id == intSearchId).FirstOrDefault();
                    if (userQuery != null)
                    {
                        userMovie.User = userQuery;
                        Console.WriteLine($"What do you want to rate {movieQuery.Title}? 1-5");
                        var rating = Console.ReadLine();
                        var userRating = 0;
                        if (!int.TryParse(rating, out userRating))
                        {
                            Console.WriteLine("Not a number!  Enter valid rating: ");
                        }
                        else if (userRating < 1 || userRating > 5)
                        {
                            Console.WriteLine("Not a valid rating! Rating must be from 1 to 5! Enter valid rating");
                            userRating = Int32.Parse(Console.ReadLine());
                        }

                        userMovie.Rating = userRating;
                        userMovie.RatedAt = DateTime.Now;

                        _context.UserMovies.Add(userMovie);
                        _context.SaveChanges();
                        Console.WriteLine($"User {userQuery.Id} rated {movieQuery.Title} a {userMovie.Rating} at {userMovie.RatedAt}");
                    }
                    else
                    {
                        Console.WriteLine("User doesn't exist in database");
                    }
                }
            }
            else 
            {
                Console.WriteLine("Movie not found in database");
                
            }           
        }
    }
}
