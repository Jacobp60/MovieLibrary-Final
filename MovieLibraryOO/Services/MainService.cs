using ConsoleTables;
using Microsoft.Extensions.Logging;
using MovieLibraryEntities.Dao;
using MovieLibraryEntities.Models;
using MovieLibraryOO.Dao;
using MovieLibraryOO.Dto;
using MovieLibraryOO.Mappers;
using System;
using System.Reflection;

namespace MovieLibraryOO.Services
{
    public class MainService : IMainService
    {
        private readonly ILogger<MainService> _logger;
        private readonly IMovieMapper _movieMapper;
        private readonly IRepository _repository;
        private readonly IFileService _fileService;

        public MainService(ILogger<MainService> logger, IMovieMapper movieMapper, IRepository repository, IFileService fileService)
        {
            _logger = logger;
            _movieMapper = movieMapper;
            _repository = repository;
            _fileService = fileService;
        }

        public void Invoke()
        {
            var menu = new Menu();

            Menu.MenuOptions menuChoice;
            do
            {
                menuChoice = menu.ChooseAction();

                switch (menuChoice)
                {
                    case Menu.MenuOptions.ListFromDb:
                        _logger.LogInformation("Listing movies from database");
                        var allMovies = _repository.GetAll();
                        var movies = _movieMapper.Map(allMovies);
                        ConsoleTable.From<MovieDto>(movies).Write();
                        break;
                    case Menu.MenuOptions.ListFromFile:
                        _fileService.Read();
                        _fileService.Display();
                        break;
                    case Menu.MenuOptions.Add:
                        _logger.LogInformation("Adding a new movie");
                        _repository.Add();
                        break;
                    case Menu.MenuOptions.Update:
                        _logger.LogInformation("Updating an existing movie");

                        //Search to know what movie user wants to update
                        var userSearchTermforUpdate = menu.GetUserResponse("Enter your", "search string:", "green");
                        var searchedMoviesforUpdate = _repository.Search(userSearchTermforUpdate);
                        movies = _movieMapper.Map(searchedMoviesforUpdate);
                        ConsoleTable.From<MovieDto>(movies).Write();

                        _repository.Update();
                        break;
                    case Menu.MenuOptions.Delete:

                        _logger.LogInformation("Deleting a movie");
                        _repository.Delete();

                        break;
                    case Menu.MenuOptions.Search:
                        _logger.LogInformation("Searching for a movie");
                        var userSearchTerm = menu.GetUserResponse("Enter your", "search string:", "green");
                        var searchedMovies = _repository.Search(userSearchTerm);
                        movies = _movieMapper.Map(searchedMovies);
                        ConsoleTable.From<MovieDto>(movies).Write();
                        break;
                    case Menu.MenuOptions.AddUser:
                        _logger.LogInformation("Adding a User");
                        _repository.AddUser();
                        
                        break;
                    case Menu.MenuOptions.RateMovie:
                        _logger.LogInformation("User Rating Movie");
                        _repository.UserRateMovie();
                        break;
                    case Menu.MenuOptions.DisplayUsers:
                        _logger.LogInformation("Display all Users");
                        _repository.DisplayAllUsers();
                        break;
                }
            }
            while (menuChoice != Menu.MenuOptions.Exit);

            menu.Exit();


            Console.WriteLine("\nThanks for using the Movie Library!");

        }
    }
}