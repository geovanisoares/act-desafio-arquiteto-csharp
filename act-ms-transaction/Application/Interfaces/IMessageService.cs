namespace act_ms_transaction.Application.Interfaces
{
    public interface IMessageService
    {
        Task PublishAsync(string message);
    }
}
