using AuthenticationMicroService.Models;
using MassTransit;

namespace ToDoListMicroService.Consumer
{
    public class UserConsumer: IConsumer<UserMessageQueue>
    {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task Consume(ConsumeContext<UserMessageQueue> context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            var data = context.Message;
           /// await _service.Delete(data.CompanyCode);
            Console.Write(data.UserId);
        }
    }
}
