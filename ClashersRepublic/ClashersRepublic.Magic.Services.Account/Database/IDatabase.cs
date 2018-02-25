namespace ClashersRepublic.Magic.Services.Account.Database
{
    using System.Threading.Tasks;
    using ClashersRepublic.Magic.Services.Account.Game;
    using Couchbase;

    internal interface IDatabase
    {
        int GetHigherId();

        bool InsertDocument(long id, Account account);
        Task<IDocumentResult<Account>> InsertDocumentAsync(long id, Account account);

        Account GetDocument(long id);
        Task<IOperationResult<Account>> GetDocumentAsync(long id);

        void UpdateDocument(long id, Account account);
        void UpdateDocumentAsync(long id, Account account);
    }
}