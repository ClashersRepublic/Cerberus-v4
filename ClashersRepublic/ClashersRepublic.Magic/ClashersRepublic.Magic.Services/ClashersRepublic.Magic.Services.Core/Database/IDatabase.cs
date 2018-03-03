namespace ClashersRepublic.Magic.Services.Core.Database
{
    public interface IDatabase
    {
        /// <summary>
        ///     Gets the database id.
        /// </summary>
        int GetDatabaseId();

        /// <summary>
        ///     Gets the higher document id.
        /// </summary>
        int GetHigherId();

        /// <summary>
        ///     Inserts the specified document.
        /// </summary>
        void InsertDocument(long id, string json);

        /// <summary>
        ///     Updates the specified document.
        /// </summary>
        void UpdateDocument(long id, string json);

        /// <summary>
        ///     Gets the specified document.
        /// </summary>
        string GetDocument(long id);
    }
}