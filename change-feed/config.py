import os

settings = {
    'endpoint': os.environ.get('ACCOUNT_ENDPOINT', '<Your Cosmos DB Endpoint>'),
    'database_id': os.environ.get('DATABASE_ID', 'GameDB'),
    'container_id': os.environ.get('CONTAINER_ID', 'games'),
}