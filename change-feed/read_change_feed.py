import azure.cosmos.documents as documents
import azure.cosmos.exceptions as exceptions
import azure.cosmos.cosmos_client as cosmos_client
from azure.identity import DefaultAzureCredential

import time
import config

ENDPOINT = config.settings['endpoint']
DATABASE_ID = config.settings['database_id']
CONTAINER_ID = config.settings['container_id']


def read_change_feed_with_all_versions_and_delete_mode(container):
    print('\nReading Change Feed with all versions and deletes mode\n')

    # This initial call stores a continuation token from now
    response_iterator = container.query_items_change_feed(mode="AllVersionsAndDeletes")
    for doc in response_iterator:
        print(doc)
    continuation_token = container.client_connection.last_response_headers['etag']

    while(True):
        print('\nPolling for changes...\n')
        
        # Read all change feed with 'AllVersionsAndDeletes' mode after create items from a continuation
        response_iterator = container.query_items_change_feed(mode="AllVersionsAndDeletes", continuation=continuation_token)

        for doc in response_iterator:
            # Business logic to process incoming change
            operationType = doc['metadata']['operationType']
            if(operationType == 'delete'):
                docId = doc['metadata']['id']
                userId = doc['metadata']['partitionKey']['userId']
            else:
                docId = doc['current']['id']
                userId = doc['current']['userId']

            print('Found {0} for game from user {1} with id: {2}'.format(operationType, userId, docId))

            if(operationType == 'replace'):
                score = doc['current']['score']
                clicks = doc['current']['clicks']
                print('\tNew score: {0} \t Total clicks: {1}'.format(score, clicks))

        # Store continuation token to resume from in next iteration
        continuation_token = container.client_connection.last_response_headers['etag']

        # Wait 5 seconds in between change feed polls
        time.sleep(5)


def run_app():
    # Initialize CosmosClient
    credential = DefaultAzureCredential()
    client = cosmos_client.CosmosClient(ENDPOINT, credential=credential)

    try:
        # Connect to database and container
        db = client.get_database_client(DATABASE_ID)
        container = db.get_container_client(CONTAINER_ID)

        # Read change feed
        read_change_feed_with_all_versions_and_delete_mode(container)

    except exceptions.CosmosHttpResponseError as e:
        print('\nrun_sample has caught an error. {0}'.format(e.message))

    finally:
        print("\nrun_sample done")


if __name__ == '__main__':
    run_app()