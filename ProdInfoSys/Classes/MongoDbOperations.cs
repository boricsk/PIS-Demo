using MongoDB.Driver;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Classes
{
    /// <summary>
    /// Provides operations for adding and deleting documents in a MongoDB collection of the specified document type.
    /// </summary>
    /// <remarks>This class encapsulates basic MongoDB operations for a single collection. It is intended for
    /// use with a specific document type and requires an existing IMongoCollection<TDocument> instance. All operations
    /// are performed asynchronously.</remarks>
    /// <typeparam name="TDocument">The type of the documents stored in the MongoDB collection.</typeparam>
    public class MongoDbOperations<TDocument> // Add generic type parameter TDocument  
    {
        private readonly IMongoCollection<TDocument> _collection;
        public MongoDbOperations(IMongoCollection<TDocument> collection)
        {
            _collection = collection;
        }

        /// <summary>
        /// Asynchronously adds a new document to the collection.
        /// </summary>
        /// <param name="document">The document to add to the collection. Cannot be null.</param>
        /// <returns>A task that represents the asynchronous add operation.</returns>
        public async Task AddNewDocument(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        /// <summary>
        /// Deletes all documents from the collection asynchronously.
        /// </summary>
        /// <remarks>Use this method with caution, as it will remove every document in the collection.
        /// This operation cannot be undone.</remarks>
        /// <returns>A task that represents the asynchronous delete operation.</returns>
        public async Task DeleteAll()
        { 
            await _collection.DeleteManyAsync(FilterDefinition<TDocument>.Empty);
        }

    }
}
