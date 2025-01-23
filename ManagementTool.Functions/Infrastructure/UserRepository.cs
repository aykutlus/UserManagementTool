using ManagementTool.Functions.Domain;
using ManagementTool.Functions.Infrastructure;
using Microsoft.Azure.Cosmos;
using System.Net;

public class UserRepository : IUserRepository
{
    private readonly Container _container;

    public UserRepository(string connectionString, string databaseName, string containerName)
    {
        var client = new CosmosClient(connectionString);
        _container = client.GetContainer(databaseName, containerName);
    }

    public async Task<List<DomainUser>> GetAllAsync()
    {
        var query = new QueryDefinition("SELECT * FROM c");
        var users = new List<DomainUser>();

        using var resultSet = _container.GetItemQueryIterator<CosmosUser>(query);
        while (resultSet.HasMoreResults)
        {
            var response = await resultSet.ReadNextAsync();
            users.AddRange(response.Select(cosmosUser => cosmosUser.ToDomain()));
        }
        return users;
    }

    public async Task<DomainUser> GetByIdAsync(string id)
    {
        try
        {
            var response = await _container.ReadItemAsync<CosmosUser>(id, new PartitionKey(id));
            return response.Resource.ToDomain();
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }
    }

    public async Task AddAsync(DomainUser domainUser)
    {
        var cosmosUser = new CosmosUser(domainUser);
        await _container.CreateItemAsync(cosmosUser, new PartitionKey(cosmosUser.Id));
    }

    public async Task UpdateAsync(DomainUser domainUser)
    {
        var cosmosUser = new CosmosUser(domainUser);
        await _container.UpsertItemAsync(cosmosUser, new PartitionKey(cosmosUser.Id));
    }

    public async Task<bool> DeleteAsync(string id)
    {
        try
        {
            await _container.DeleteItemAsync<CosmosUser>(id, new PartitionKey(id));
            return true; 
        }
        catch (CosmosException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
        {
            return false; 
        }
        catch
        {
            throw; 
        }
    }

}
