namespace Farmer

type ResourceName =
    | ResourceName of string
    member this.Value =
        let (ResourceName path) = this
        path
type ConsistencyPolicy = Eventual | ConsistentPrefix | Session | BoundedStaleness of maxStaleness:int * maxIntervalSeconds : int | Strong
type FailoverPolicy = NoFailover | AutoFailover of secondaryLocation:string | MultiMaster of secondaryLocation:string
type CosmosDbIndexKind = Hash | Range
type CosmosDbIndexDataType = Number | String

namespace Farmer.Internal

open Farmer

/// A type of ARM resource e.g. Microsoft.Web/serverfarms
type ResourceType =
    | ResourceType of path:string
    member this.Value =
        let (ResourceType path) = this
        path

type WebAppExtensions = AppInsightsExtension
type AppInsights =
    { Name : ResourceName 
      Location : string
      LinkedWebsite: ResourceName }
type StorageAccount =
    { Name : ResourceName 
      Location : string
      Sku : string }
type WebApp =
    { Name : ResourceName 
      AppSettings : List<string * string>
      Extensions : WebAppExtensions Set
      Dependencies : ResourceName list }
type ServerFarm =
    { Name : ResourceName 
      Location : string
      Sku:string
      WebApps : WebApp list }
type CosmosDbContainer =
    { Name : ResourceName
      PartitionKey :
        {| Paths : string list
           Kind : CosmosDbIndexKind |}
      IndexingPolicy :
        {| IncludedPaths :
            {| Path : string
               Indexes :
                {| Kind : CosmosDbIndexKind
                   DataType : CosmosDbIndexDataType |} list
            |} list
           ExcludedPaths : string list
        |}
    }

type CosmosDbSql =
    { Name : ResourceName
      Dependencies : ResourceName list
      Throughput : string
      Containers : CosmosDbContainer list }
type CosmosDbServer =
    { Name : ResourceName
      Location : string
      ConsistencyPolicy : ConsistencyPolicy
      WriteModel : FailoverPolicy
      Databases : CosmosDbSql list }

module ResourceType =
    let ServerFarm = ResourceType "Microsoft.Web/serverfarms"
    let WebSite = ResourceType "Microsoft.Web/sites"
    let CosmosDb = ResourceType "Microsoft.DocumentDB/databaseAccounts"
    let CosmosDbSql = ResourceType "Microsoft.DocumentDB/databaseAccounts/apis/databases"
    let CosmosDbSqlContainer = ResourceType "Microsoft.DocumentDb/databaseAccounts/apis/databases/containers"
    let StorageAccount = ResourceType "Microsoft.Storage/storageAccounts"
    let AppInsights = ResourceType "Microsoft.Insights/components"

namespace Farmer

type ArmTemplate =
    { Parameters : string list
      Variables : (string * string) list
      Outputs : (string * string) list
      Resources : obj list }