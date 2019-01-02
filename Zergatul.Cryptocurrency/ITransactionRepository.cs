namespace Zergatul.Cryptocurrency
{
    public interface ITransactionRepository<out T>
        where T : Base.TransactionBase
    {
        T GetTransaction(byte[] id);
        T GetTransaction(string id);
    }
}