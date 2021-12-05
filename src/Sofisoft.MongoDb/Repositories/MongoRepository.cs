using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using MongoDB.Driver;
using Sofisoft.Abstractions.Models;
using Sofisoft.MongoDb.Attributes;

namespace Sofisoft.MongoDb.Repositories
{
    public class MongoRepository<TDocument> : IMongoRepository<TDocument>
        where TDocument : IEntity
    {
        #region Members

        private readonly ISofisoftMongoDbContext _ctx;

        #endregion

        #region Constructors

        public MongoRepository(ISofisoftMongoDbContext context)
        {
            _ctx = context ?? throw new ArgumentNullException(nameof(context));
        }

        #endregion

        #region Implementations

        public virtual Task<IEnumerable<TResult>> AggregateAsync<TResult>(
            PipelineDefinition<TDocument, TResult> pipeline,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return Task.Run(() =>
                    GetCollection()
                        .Aggregate(
                            _ctx.GetCurrentSession(),
                            pipeline,
                            cancellationToken: cancellationToken)
                        .ToEnumerable(cancellationToken)
                );
            }
            
            return Task.Run(() =>
                GetCollection()
                    .Aggregate(
                        pipeline,
                        cancellationToken: cancellationToken)
                    .ToEnumerable(cancellationToken)
            );
        }

        public virtual Task<long> CountAsync(
            Expression<Func<TDocument, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            if (_ctx.HasActiveTransaction)
            {
                return GetCollection().CountDocumentsAsync(
                    _ctx.GetCurrentSession(),
                    filterExpression ?? FilterDefinition<TDocument>.Empty,
                    cancellationToken: cancellationToken);
            }

            return GetCollection().CountDocumentsAsync(
                filterExpression ?? FilterDefinition<TDocument>.Empty,
                cancellationToken: cancellationToken
            );
        }

        public virtual Task DeleteByIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>
                .Filter.Eq(doc => doc.Id, id);

            if(_ctx.HasActiveTransaction)
            {
                return GetCollection().FindOneAndDeleteAsync(
                    _ctx.GetCurrentSession(),
                    filter,
                    cancellationToken: cancellationToken
                );
            }

            return GetCollection().FindOneAndDeleteAsync(
                filter,
                cancellationToken: cancellationToken
            );
        }

        public virtual Task<IEnumerable<TDocument>> FilterByAsync(
            Expression<Func<TDocument, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return Task.Run(() => 
                    GetCollection().Find(
                        _ctx.GetCurrentSession(),
                        filterExpression ?? FilterDefinition<TDocument>.Empty)
                        .ToEnumerable(cancellationToken)
                );
            }

            return Task.Run(() => 
                GetCollection().Find(
                    filterExpression ?? FilterDefinition<TDocument>.Empty)
                    .ToEnumerable(cancellationToken)
            );
        }

        [ExcludeFromCodeCoverage]
        public virtual Task<IEnumerable<TProjected>> FilterByAsync<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return Task.Run(() =>
                    GetCollection().Find(
                        _ctx.GetCurrentSession(),
                        filterExpression ?? FilterDefinition<TDocument>.Empty)
                        .Project(projectionExpression)
                        .ToEnumerable(cancellationToken)
                );
            }

            return Task.Run(() =>
                GetCollection().Find(
                    filterExpression ?? FilterDefinition<TDocument>.Empty)
                    .Project(projectionExpression)
                    .ToEnumerable(cancellationToken)
            );
        }

        [ExcludeFromCodeCoverage]
        public virtual Task<TDocument> FindByIdAsync(
            string id,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, id);

            if(_ctx.HasActiveTransaction)
            {
                return GetCollection()
                    .Find(
                        _ctx.GetCurrentSession(),
                        filter)
                    .SingleOrDefaultAsync(cancellationToken);
            }

            return GetCollection()
                .Find(filter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        [ExcludeFromCodeCoverage]
        public virtual Task<TDocument> FindOneAsync(
            Expression<Func<TDocument, bool>> filterExpression,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return GetCollection().Find(
                    _ctx.GetCurrentSession(),
                    filterExpression ?? FilterDefinition<TDocument>.Empty)
                    .Limit(1)
                    .SingleOrDefaultAsync(cancellationToken);
            }

            return Task.Run(() =>
                GetCollection().Find(
                    filterExpression ?? FilterDefinition<TDocument>.Empty)
                    .Limit(1)
                    .SingleOrDefaultAsync(cancellationToken)
            );
        }

        [ExcludeFromCodeCoverage]
        public virtual Task<TProjected> FindOneAsync<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return GetCollection().Find(
                        _ctx.GetCurrentSession(),
                        filterExpression ?? FilterDefinition<TDocument>.Empty)
                    .Project(projectionExpression)
                    .Limit(1)
                    .SingleOrDefaultAsync(cancellationToken);
            }

            return GetCollection().Find(
                    filterExpression ?? FilterDefinition<TDocument>.Empty)
                .Project(projectionExpression)
                .Limit(1)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public virtual Task InsertOneAsync(
            TDocument entity,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return GetCollection()
                    .InsertOneAsync(
                        _ctx.GetCurrentSession(),
                        entity,
                        cancellationToken: cancellationToken);
            }

            return GetCollection()
                    .InsertOneAsync(
                        entity,
                        cancellationToken: cancellationToken);
        }

        public virtual Task InsertManyAsync(
            ICollection<TDocument> entities,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return GetCollection()
                    .InsertManyAsync(
                        _ctx.GetCurrentSession(),
                        entities,
                        cancellationToken: cancellationToken);
            }

            return GetCollection()
                .InsertManyAsync(
                    entities,
                    cancellationToken: cancellationToken);
        }

        [ExcludeFromCodeCoverage]
        public virtual Task<IEnumerable<TDocument>> PaginatedAsync(
            Expression<Func<TDocument, bool>> filterExpression,
            string sort,
            int pageSize,
            int start,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return Task.Run(() => 
                    GetCollection().Find(
                            _ctx.GetCurrentSession(),
                            filterExpression ?? FilterDefinition<TDocument>.Empty)
                        .Sort(sort)
                        .Skip(start)
                        .Limit(pageSize)
                        .ToEnumerable(cancellationToken)
                );
            }

            return Task.Run(() => 
                GetCollection().Find(
                        filterExpression ?? FilterDefinition<TDocument>.Empty)
                    .Sort(sort)
                    .Skip(start)
                    .Limit(pageSize)
                    .ToEnumerable(cancellationToken)
            );
        }

        [ExcludeFromCodeCoverage]
        public virtual Task<IEnumerable<TProjected>> PaginatedAsync<TProjected>(
            Expression<Func<TDocument, bool>> filterExpression,
            Expression<Func<TDocument, TProjected>> projectionExpression,
            string sort,
            int pageSize,
            int start,
            CancellationToken cancellationToken = default)
        {
            if(_ctx.HasActiveTransaction)
            {
                return Task.Run(() => 
                    GetCollection().Find(
                            _ctx.GetCurrentSession(),
                            filterExpression ?? FilterDefinition<TDocument>.Empty)
                        .Project(projectionExpression)
                        .Sort(sort)
                        .Skip(start)
                        .Limit(pageSize)
                        .ToEnumerable(cancellationToken)
                );
            }

            return Task.Run(() => 
                GetCollection().Find(
                        filterExpression ?? FilterDefinition<TDocument>.Empty)
                    .Project(projectionExpression)
                    .Sort(sort)
                    .Skip(start)
                    .Limit(pageSize)
                    .ToEnumerable(cancellationToken)
            );
        }

        public virtual Task<long> UpdateOneAsync(
            TDocument entity,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<TDocument>.Filter.Eq(doc => doc.Id, entity.Id);
            var update = GetUpdateDefinition(entity);
            var options = new UpdateOptions { IsUpsert = false };

            if(_ctx.HasActiveTransaction)
            {
                return Task.Run(() =>
                {
                    var updateResult = GetCollection()
                        .UpdateOne(
                            _ctx.GetCurrentSession(),
                            filter,
                            update,
                            options, cancellationToken);

                    return updateResult.ModifiedCount;
                }, cancellationToken);
            }

            return Task.Run(() =>
            {
                var updateResult = GetCollection()
                    .UpdateOne(
                        filter,
                        update,
                        options,
                        cancellationToken);

                    return updateResult.ModifiedCount;
                }, cancellationToken);
        }

        #endregion

        #region  Publics Methods

        public IMongoCollection<TDocument> GetCollection()
        {
            return _ctx.Database
                .GetCollection<TDocument>(
                    GetCollectionName(typeof(TDocument)));
        }

        #endregion

        #region  Privates Methods

        private static string GetCollectionName(Type documentType)
        {
            var attribute = (BsonCollectionAttribute?) documentType.GetCustomAttributes(
                    typeof(BsonCollectionAttribute),
                    true)
                .FirstOrDefault();

            if (attribute is null)
            {
                throw new KeyNotFoundException(nameof(attribute));
            }

            return attribute.CollectionName;
        }

        private static UpdateDefinition<TDocument> GetUpdateDefinition(TDocument document)
        {
            var builder = new UpdateDefinitionBuilder<TDocument>();
            var updates = document.GetType().GetProperties()
                    .Where(x => !new string[] { "Id", "CreatedAt", "CreatedBy" }.Contains(x.Name)
                        && (x.GetValue(document) is not null || x.Name == "ModifiedAt"))
                    .Select(x => x.Name == "ModifiedAt"
                        ? builder.Set(x.Name, DateTime.UtcNow)
                        : builder.Set(x.Name, x.GetValue(document)));
            
            return builder.Combine(updates);
        }

        #endregion

    }
}