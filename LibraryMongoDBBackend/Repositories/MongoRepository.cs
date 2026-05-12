using LibraryMongoDBBackend.Repositories.Interfaces;
using MongoDB.Bson;
using MongoDB.Driver;

public class MongoRepository<T> : IRepository<T>
{
    private readonly IMongoCollection<T> _collection;

    public MongoRepository(MongoDbContext context, string collectionName)
    {
        _collection = context
            .GetType()
            .GetProperty(collectionName)
            ?.GetValue(context) as IMongoCollection<T>;
    }

    public async Task<T> CreateAsync(T entity)
    {
        await _collection.InsertOneAsync(entity);
        return entity;
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        return await _collection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _collection.Find(_ => true).ToListAsync();
    }

    public async Task<bool> UpdateAsync(int id, T entity)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);

        var result = await _collection.ReplaceOneAsync(filter, entity);

        return result.ModifiedCount > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var filter = Builders<T>.Filter.Eq("Id", id);
        var result = await _collection.DeleteOneAsync(filter);

        return result.DeletedCount > 0;
    }


}