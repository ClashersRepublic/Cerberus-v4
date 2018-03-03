namespace ClashersRepublic.Magic.Services.Account.Database
{
    internal interface IDatabase
    {
        int GetHigherId();

        void InsertDocument(long id, string json);
        void UpdateDocument(long id, string json);
        string GetDocument(long id);
    }
}