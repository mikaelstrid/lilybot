using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security;
using System.Threading.Tasks;
using Lily.Core.Application;
using Lily.Core.Domain.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Lily.Core.Infrastructure.Persistence
{
    public class DocumentDbAggregateRepository<T> : IAggregateRepository<T> where T : class, IAggregate
    {
        // ReSharper disable InconsistentNaming
        private readonly string ENDPOINT_URI = ConfigurationManager.AppSettings["DocumentDbEndpointUri"];
        private readonly string PRIMARY_KEY = ConfigurationManager.AppSettings["DocumentDbPrimaryKey"];
        private readonly string DATABASE_NAME = ConfigurationManager.AppSettings["DocumentDbDatabaseName"];

        private readonly string _collectionName;

        private DocumentClient _client;

        public DocumentDbAggregateRepository(string collectionName)
        {
            _collectionName = collectionName;
        }

        public async Task<IEnumerable<T>> GetAll(string username)
        {
            await Initialize();
            return await Get(username, t => true);
        }

        public async Task<IEnumerable<T>> Get(string username, Func<T, bool> predicate)
        {
            await Initialize();
            return _client.CreateDocumentQuery<T>(CreateCollectionUri(),
                $"SELECT * FROM c WHERE c.username = '{username}' AND c.type = '{typeof(T).Name}'")
                .Where(predicate);
        }

        protected async Task<IEnumerable<T>> Get(Func<T, bool> predicate)
        {
            await Initialize();
            return _client.CreateDocumentQuery<T>(CreateCollectionUri(),
                $"SELECT * FROM c WHERE c.type = '{typeof(T).Name}'")
                .Where(predicate);
        }

        public async Task<T> GetById(string username, Guid id)
        {
            await Initialize();
            var existingAggregate = (await Get(r => r.Id == id)).SingleOrDefault();

            if (existingAggregate != null)
            {
                if (existingAggregate.Username != username) throw new SecurityException("User not authorized to get entity");
                return existingAggregate;
            }
            return await Task.FromResult<T>(null);
        }

        public async Task AddOrUpdate(string username, T aggregate)
        {
            await Initialize();
            var existingAggregate = (await Get(r => r.Id == aggregate.Id)).SingleOrDefault();

            if (existingAggregate != null)
            {
                if (existingAggregate.Username != username) throw new SecurityException("User not authorized to update entity");
                await _client.ReplaceDocumentAsync(CreateDocumentUri(aggregate.Id), aggregate);
            }
            else
            {
                await _client.CreateDocumentAsync(CreateCollectionUri(), aggregate);
            }
        }

        public async Task Delete(string username, T aggregate)
        {
            await Initialize();
            await DeleteById(username, aggregate.Id);
        }

        public async Task DeleteById(string username, Guid id)
        {
            try
            {
                await Initialize();
                var document = await GetById(username, id);
                if (document == null) throw new SecurityException("User not authorized to remove entity.");
                await _client.DeleteDocumentAsync(CreateDocumentUri(document.Id));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode != HttpStatusCode.NotFound) // It is fine if the document didn't exist
                {
                    throw;
                }
            }
        }





        private async Task Initialize()
        {
            _client = new DocumentClient(new Uri(ENDPOINT_URI), PRIMARY_KEY);
            await CreateDatabaseIfNotExists(DATABASE_NAME);
            await CreateDocumentCollectionIfNotExists(DATABASE_NAME, _collectionName);
        }


        private async Task CreateDatabaseIfNotExists(string databaseName)
        {
            try
            {
                await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(databaseName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDatabaseAsync(new Database { Id = databaseName });
                }
                else
                {
                    throw;
                }
            }
        }

        private async Task CreateDocumentCollectionIfNotExists(string databaseName, string collectionName)
        {
            try
            {
                await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName));
            }
            catch (DocumentClientException de)
            {
                if (de.StatusCode == HttpStatusCode.NotFound)
                {
                    await _client.CreateDocumentCollectionAsync(
                        UriFactory.CreateDatabaseUri(databaseName),
                        new DocumentCollection { Id = collectionName },
                        new RequestOptions { OfferThroughput = 400 });
                }
                else
                {
                    throw;
                }
            }
        }

        private Uri CreateCollectionUri()
        {
            return UriFactory.CreateDocumentCollectionUri(DATABASE_NAME, _collectionName);
        }

        private Uri CreateDocumentUri(Guid guid)
        {
            return UriFactory.CreateDocumentUri(DATABASE_NAME, _collectionName, guid.ToString());
        }
    }
}