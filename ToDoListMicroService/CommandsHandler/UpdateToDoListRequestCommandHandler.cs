using MediatR;
using ToDoListMicroService.Commands;
using ToDoListMicroService.Services;

namespace ToDoListMicroService.CommandsHandler
{
    public class UpdateToDoListRequestCommandHandler : IRequestHandler<UpdateToDoListRequest, string?>
    {
        #region Private declarations    
        private readonly IToDoListService _service;
        #endregion
        
        #region Public Methods, constructors 
        public UpdateToDoListRequestCommandHandler(IToDoListService toDoListService)
        {
            _service = toDoListService;
        }
        
        public async Task<string?> Handle(UpdateToDoListRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.Update(request.Id, request.Status);
            return result;
        }
        #endregion
    }
}
