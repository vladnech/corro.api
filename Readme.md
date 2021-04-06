# Correspondence.Api + CosmosDB PoC
## Getting started
Install Azure CosmosDB emulator for the development purpose: 
https://docs.microsoft.com/en-us/azure/cosmos-db/local-emulator?tabs=cli%2Cssl-netstd21.

Once it is installed - copy URL from Emulator Explorer into COSMOSDB_ENDPOINT and Primary key in COSMOSDB_MASTER_KEY to _SetupDatabaseAndData_ (local.settings.json)

To start the project run _SetupDatabaseAndData_. It will setup DB (_correspondence-db_) and collection (_investor-docs_) in Azure CosmosDB.


## Correspondence.Api description
The API has two methods:
1. List with paging all Investor documents
2. Insert/Update a document for Investor 

## Data structure
It is proposed to have nested structure of investor correspondence:

```json
{
    "InvestorId": 6,
    "id": "4d381c4d-dd8d-4c0b-b77c-d60feb5958fd",
    "Corro":[
        {
            "RefId": "60001",
            "Type": "Maturity completeness",
            "AccountNo": "65580001",
            "ProductCode": "",
            "ProductName": "Fixed Term Annuity",
            "Date": "2018-02-01T00:00:00",
            "Link": null
        },
        {
            "RefId": "60002",
            "Type": "Maturity completeness",
            "AccountNo": "68910002",
            "ProductCode": "",
            "ProductName": "Liquid Lifetime Annuity",
            "Date": "2018-03-01T00:00:00",
            "Link": null
        }] 
}		
```			

Compared with the flat data structure it will cause more RUs (Request Units):

```
{
    "InvestorId": 8,
    "Type": "Change details",
    "RefId": "80001",
    "AccountNo": "85840097",
    "ProductCode": "283",
    "ProductName": "Liquid Lifetime Annuity",
    "Date": "2021-02-01T00:00:00",
    "Link": "111112"
}
```