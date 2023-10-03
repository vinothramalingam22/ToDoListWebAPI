using MongoDB.Driver;
using ToDoListMicroService.DataBaseConfig;
using ToDoListMicroService.Entities;

namespace ToDoListMicroService.Repository
{
    public class ToDoListCommandRepository: IToDoListCommandRepository
    {
        #region Private declarations
        private readonly IMongoCollection<ToDoList> _toDoListRepository;
        #endregion

        #region Public Methods
        public ToDoListCommandRepository(IToDoListDataBaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _toDoListRepository = database.GetCollection<ToDoList>(settings.CollectionName);

        }

        public async Task<ToDoList> Create(ToDoList toDoList)
        {
            await _toDoListRepository.InsertOneAsync(toDoList);
            return toDoList;
        }

        public async Task<string> Update(string id, string status)
        {
            var toDoTask =  _toDoListRepository.Find(x=> x.Id == id).FirstOrDefault();
            if(toDoTask != null)
            {
                toDoTask.Status = status;

                await _toDoListRepository.ReplaceOneAsync(x => x.Id == id, toDoTask);
                return status;
            }

            return status;
        }
        #endregion
    }
}
