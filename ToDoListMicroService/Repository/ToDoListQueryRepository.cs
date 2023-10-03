using MongoDB.Driver;
using ToDoListMicroService.DataBaseConfig;
using ToDoListMicroService.Entities;

namespace ToDoListMicroService.Repository
{    
    public class ToDoListQueryRepository: IToDoListQueryRepository
    {
        #region private variable
        private readonly IMongoCollection<ToDoList> _toDoListRepository;
        #endregion

        #region Public Methods
        public ToDoListQueryRepository(IToDoListDataBaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _toDoListRepository = database.GetCollection<ToDoList>(settings.CollectionName);

        }

        public IQueryable<ToDoList> Get(string userId)
        {
            var response =  _toDoListRepository.AsQueryable().Where(x => x.UserId.ToLower() == userId.ToLower());

            return response;
        }

        public async Task<ToDoList> GetByName(string taskName, string userId)
        {
            var toDos = await _toDoListRepository.FindAsync(x => x.UserId.ToLower() == userId.ToLower() && x.Name.ToLower() == taskName.ToLower());
            return toDos.FirstOrDefault();
        }
        #endregion
    }
}
