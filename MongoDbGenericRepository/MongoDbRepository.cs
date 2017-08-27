﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
using MongoDbGenericRepository.Models;
using System.Linq;

namespace MongoDbGenericRepository
{
    public interface IBaseMongoRepository
    {
        #region Read

        /// <summary>
        /// Asynchronously returns one document given its id.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="id">The Id of the document you want to get.</param>
        Task<TDocument> GetById<TDocument>(Guid id) where TDocument : IDocument;

        /// <summary>
        /// Asynchronously returns one document given an expression filter.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        Task<TDocument> GetOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Returns one document given an expression filter.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        TDocument GetOne<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Returns a collection cursor.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        IFindFluent<TDocument, TDocument> GetCursor<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Asynchronously returns true if any of the document of the collection matches the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        Task<bool> AnyAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Returns true if any of the document of the collection matches the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        bool Any<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Asynchronously returns a list of the documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        Task<List<TDocument>> GetAllAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Returns a list of the documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        List<TDocument> GetAll<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Asynchronously counts how many documents match the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        Task<long> CountAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        /// <summary>
        /// Counts how many documents match the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        long Count<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument;

        #endregion Get

        #region Create

        /// <summary>
        /// Asynchronously adds a document to the collection.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        Task AddOneAsync<TDocument>(TDocument document) where TDocument : IDocument;

        /// <summary>
        /// Adds a document to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        void AddOne<TDocument>(TDocument document) where TDocument : IDocument;

        /// <summary>
        /// Asynchronously adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        Task AddManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument;

        /// <summary>
        /// Adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        void AddMany<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument;

        #endregion
    }

    public abstract class BaseMongoRepository : IBaseMongoRepository
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }

        /// <summary>
        /// The base constructor
        /// </summary>
        /// <param name="connectionString">The connection string of the MongoDb server.</param>
        /// <param name="databaseName">The name of the database against which you want to perform operations.</param>
        protected BaseMongoRepository(string connectionString, string databaseName)
        {
            _mongoDbContext = new MongoDbContext(connectionString, databaseName);
        }

        protected IMongoDbContext _mongoDbContext = null;

        #region Read

        /// <summary>
        /// Asynchronously returns one document given its id.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="id">The Id of the document you want to get.</param>
        public async Task<TDocument> GetById<TDocument>(Guid id) where TDocument : IDocument
        {
            var filter = Builders<TDocument>.Filter.Eq("Id", id);
            return await GetCollection<TDocument>().Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Asynchronously returns one document given an expression filter.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public async Task<TDocument> GetOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return await GetCollection<TDocument>().Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Returns one document given an expression filter.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public TDocument GetOne<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return GetCollection<TDocument>().Find(filter).FirstOrDefault();
        }

        /// <summary>
        /// Returns a collection cursor.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public IFindFluent<TDocument, TDocument> GetCursor<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return GetCollection<TDocument>().Find(filter);
        }

        /// <summary>
        /// Returns true if any of the document of the collection matches the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public async Task<bool> AnyAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            var count = await GetCollection<TDocument>().CountAsync(filter);
            return (count > 0);
        }

        /// <summary>
        /// Returns true if any of the document of the collection matches the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public bool Any<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            var count = GetCollection<TDocument>().Count(filter);
            return (count > 0);
        }

        /// <summary>
        /// Asynchronously returns a list of the documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public async Task<List<TDocument>> GetAllAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return await GetCollection<TDocument>().Find(filter).ToListAsync();
        }

        /// <summary>
        /// Returns a list of the documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public List<TDocument> GetAll<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return GetCollection<TDocument>().Find(filter).ToList();
        }

        /// <summary>
        /// Asynchronously counts how many documents match the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public async Task<long> CountAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return await GetCollection<TDocument>().CountAsync(filter);
        }

        /// <summary>
        /// Counts how many documents match the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter">A LINQ expression filter.</param>
        public long Count<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return GetCollection<TDocument>().Find(filter).Count();
        }

        /// <summary>
        /// Asynchronously returns a paginated list of the documents matching the filter condition.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <param name="skipNumber">The number of documents you want to skip. Default value is 0.</param>
        /// <param name="takeNumber">The number of documents you want to take. Default value is 50.</param>
        public async Task<List<TDocument>> GetPaginatedAsync<TDocument>(Expression<Func<TDocument, bool>> filter, int skipNumber = 0, int takeNumber = 50) where TDocument : IDocument
        {
            return await GetCollection<TDocument>().Find(filter).Skip(skipNumber).Limit(takeNumber).ToListAsync();
        }


        /// <summary>
        /// Returns a list of projected objects
        /// </summary>
        /// <typeparam name="TDocument">T is a DbEntity</typeparam>
        /// <typeparam name="TProjection"></typeparam>
        /// <returns></returns>
        public async Task<TProjection> ProjectBy<TDocument, TProjection>(Expression<Func<TDocument, bool>> filter, Expression<Func<TDocument, TProjection>> projection)
            where TDocument : IDocument
            where TProjection : class, new()
        {
            return await GetCollection<TDocument>().Find(Builders<TDocument>.Filter.Where(filter))
                                                   .Project(projection)
                                                   .FirstOrDefaultAsync();
        }

        #endregion Get

        #region Create


        private void FormatDocument<TDocument>(TDocument document) where TDocument : IDocument
        {
            if (document.Id == default(Guid))
            {
                document.Id = Guid.NewGuid();
            }

            if (document.AddedAtUtc == default(DateTime))
            {
                document.AddedAtUtc = DateTime.UtcNow;
            }
        }

        /// <summary>
        /// Asynchronously adds a document to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        public async Task AddOneAsync<TDocument>(TDocument document) where TDocument : IDocument
        {
            FormatDocument(document);
            await GetCollection<TDocument>().InsertOneAsync(document);
        }

        /// <summary>
        /// Adds a document to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        public void AddOne<TDocument>(TDocument document) where TDocument : IDocument
        {
            FormatDocument(document);
            GetCollection<TDocument>().InsertOne(document);
        }

        /// <summary>
        /// Asynchronously adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        public async Task AddManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument
        {
            foreach (var document in documents)
            {
                FormatDocument(document);
            }
            await GetCollection<TDocument>().InsertManyAsync(documents);
        }

        /// <summary>
        /// Adds a list of documents to the collection.
        /// Populates the Id and AddedAtUtc fields if necessary.
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="document">The document you want to add.</param>
        public void AddMany<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument
        {
            foreach (var document in documents)
            {
                FormatDocument(document);
            }
            GetCollection<TDocument>().InsertMany(documents);
        }

        #endregion Create

        #region Delete

        /// <summary>
        /// A generic delete one method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<long> DeleteOneAsync<TDocument>(TDocument document) where TDocument : IDocument
        {
            return await DeleteOneAsync<TDocument>(x => x.Id == document.Id);
        }

        /// <summary>
        /// A generic delete one method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public long DeleteOne<TDocument>(TDocument document) where TDocument : IDocument
        {
            return DeleteOne<TDocument>(x => x.Id == document.Id);
        }

        /// <summary>
        /// A generic delete one method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public long DeleteOne<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return GetCollection<TDocument>().DeleteOne(filter).DeletedCount;
        }

        /// <summary>
        /// A generic delete one method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<long> DeleteOneAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            return (await GetCollection<TDocument>().DeleteOneAsync(filter)).DeletedCount;
        }

        /// <summary>
        /// A generic delete many method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<long> DeleteManyAsync<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            var deleteRes = await GetCollection<TDocument>().DeleteManyAsync(filter);
            return deleteRes.DeletedCount;
        }

        /// <summary>
        /// A generic delete many method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<long> DeleteManyAsync<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument
        {
            if (!documents.Any())
            {
                return 0;
            }
            var idsTodelete = documents.Select(e => e.Id).ToArray();
            var deleteRes = await GetCollection<TDocument>().DeleteManyAsync(x => idsTodelete.Contains(x.Id));
            return deleteRes.DeletedCount;
        }

        /// <summary>
        /// A generic delete many method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public long DeleteMany<TDocument>(IEnumerable<TDocument> documents) where TDocument : IDocument
        {
            if (!documents.Any())
            {
                return 0;
            }
            var idsTodelete = documents.Select(e => e.Id).ToArray();
            var deleteRes = GetCollection<TDocument>().DeleteMany(x => idsTodelete.Contains(x.Id));
            return deleteRes.DeletedCount;
        }

        /// <summary>
        /// A generic delete many method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <returns></returns>
        public long DeleteMany<TDocument>(Expression<Func<TDocument, bool>> filter) where TDocument : IDocument
        {
            var deleteRes = GetCollection<TDocument>().DeleteMany(filter);
            return deleteRes.DeletedCount;
        }
        #endregion Delete

        #region Update

        /// <summary>
        /// Updates a document
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateOneAsync<TDocument>(TDocument entity) where TDocument : IDocument
        {
            var updateRes = await GetCollection<TDocument>().ReplaceOneAsync(x => x.Id == entity.Id, entity);
            return updateRes.ModifiedCount < 1;
        }

        /// <summary>
        /// UpdateOne with filter
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        private async Task<long> UpdateOne<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update) where TDocument : IDocument
        {
            var updateRes = await GetCollection<TDocument>().UpdateOneAsync(filter, update);
            return updateRes.ModifiedCount;
        }

        /// <summary>
        /// UpdateMany with filter
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        public async Task<long> UpdateMany<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update) where TDocument : IDocument
        {
            var collection = GetCollection<TDocument>();
            var updateRes = await collection.UpdateManyAsync(filter, update);
            return updateRes.ModifiedCount;
        }
        #endregion Update

        #region Find And Update

        /// <summary>
        /// GetAndUpdateOne with filter
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="filter"></param>
        /// <param name="update"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task<TDocument> GetAndUpdateOne<TDocument>(FilterDefinition<TDocument> filter, UpdateDefinition<TDocument> update, FindOneAndUpdateOptions<TDocument, TDocument> options) where TDocument : IDocument
        {
            return await GetCollection<TDocument>().FindOneAndUpdateAsync(filter, update, options);
        }

        #endregion Find And Update

        /// <summary>
        /// The private GetCollection method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        private IMongoCollection<TDocument> GetCollection<TDocument>(TDocument document) where TDocument : IDocument
        {
            return _mongoDbContext.GetCollection<TDocument>(document);
        }

        /// <summary>
        /// The private GetCollection method
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        private IMongoCollection<TDocument> GetCollection<TDocument>() where TDocument : IDocument
        {
            return _mongoDbContext.GetCollection<TDocument>();
        }
    }
}