using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Lily.Core.Application;
using Lily.Core.Domain.Model;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace Lily.Core.Infrastructure.Persistence
{
    public class DocumentDbAggregateRepository<T> : IAggregateRepository<T> where T : class, IAggregate
    {
        private const string ENDPOINT_URI = "https://pixeldigitalbyra.documents.azure.com:443/";
        private const string PRIMARY_KEY = "Ji3YErhuh7joDb0t0xro4VD5AIpQKoLpSWrw04bBEhUcorpBKSgI4MvMb6CiC7GPdcA1kUPwiIlD23KBNRk5EQ==";
        private const string DATABASE_NAME = "LilyDB";
        private const string COLLECTION_NAME = "ShoppingListCollection";

        private DocumentClient _client;


        public async Task<IEnumerable<T>> GetAll()
        {
            await Initialize();
            return await Get(t => true);
        }

        public async Task<IEnumerable<T>> Get(Func<T, bool> predicate)
        {
            await Initialize();
            return await Task.FromResult(
                _client.CreateDocumentQuery<T>(CreateCollectionUri(),
                    $"SELECT * FROM c WHERE c.type = '{typeof(T).Name}'")
                    .Where(predicate));
        }

        public async Task<T> GetById(Guid id)
        {
            await Initialize();
            return await Task.FromResult(
                    (await Get(r => r.Id == id))
                        .SingleOrDefault());
        }

        public async Task AddOrUpdate(T aggregate)
        {
            await Initialize();
            var existingAggregate = await GetById(aggregate.Id);

            if (existingAggregate != null)
            {
                await _client.ReplaceDocumentAsync(CreateDocumentUri(aggregate.Id), aggregate);
            }
            else
            {
                await _client.CreateDocumentAsync(CreateCollectionUri(), aggregate);
            }
        }

        public async Task Delete(T aggregate)
        {
            await Initialize();
            await DeleteById(aggregate.Id);
        }

        public async Task DeleteById(Guid id)
        {
            try
            {
                await Initialize();
                await _client.DeleteDocumentAsync(CreateDocumentUri(id));
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
            await CreateDocumentCollectionIfNotExists(DATABASE_NAME, COLLECTION_NAME);
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
                    var collectionInfo = new DocumentCollection
                    {
                        Id = collectionName,
                        IndexingPolicy = new IndexingPolicy(new RangeIndex(DataType.String) { Precision = -1 })
                    };

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
            return UriFactory.CreateDocumentCollectionUri(DATABASE_NAME, COLLECTION_NAME);
        }

        private Uri CreateDocumentUri(Guid guid)
        {
            return UriFactory.CreateDocumentUri(DATABASE_NAME, COLLECTION_NAME, guid.ToString());
        }
    }
}