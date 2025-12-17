using MongoDB.Driver;
using ProdInfoSys.Models.NonRelationalModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProdInfoSys.Classes
{
    public class MongoDbOperations<TDocument> // Add generic type parameter TDocument  
    {
        private readonly IMongoCollection<TDocument> _collection;
        public MongoDbOperations(IMongoCollection<TDocument> collection)
        {
            _collection = collection;
        }

        public async Task AddNewDocument(TDocument document)
        {
            await _collection.InsertOneAsync(document);
        }

        public async Task DeleteAll()
        { 
            await _collection.DeleteManyAsync(FilterDefinition<TDocument>.Empty);
        }

    }
}
