using MediatR;
using ToDoListMicroService.Commands;
using ToDoListMicroService.Models;
using ToDoListMicroService.Services;

namespace ToDoListMicroService.CommandsHandler
{
    public class CreateToDoListRequestCommandHandler : IRequestHandler<CreateToDoListRequest, ToDoListRequestModel>
    {
        #region Private declarations
        private readonly IToDoListService _service;
        #endregion

        #region Public Methods, constructors 
        public CreateToDoListRequestCommandHandler(IToDoListService toDoListService)
        {
            _service = toDoListService;
        }

        public async Task<ToDoListRequestModel> Handle(CreateToDoListRequest request, CancellationToken cancellationToken)
        {
            var result = await _service.Create(request, request.UserId);
            return result;
        }
        #endregion
    }
}
