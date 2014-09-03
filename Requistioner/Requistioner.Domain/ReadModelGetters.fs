module ReadModelGetters

open System
open Newtonsoft.Json

let EventStoreConnection = lazy(
        let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
        EventStore.conn endPoint)

let getApprovalFlatReadModel = 
    let get = EventStore.makeReadModelGetter EventStoreConnection.Value (fun data -> JsonConvert.DeserializeObject<ReadModels.ApprovalFlatReadModel>(System.Text.Encoding.ASCII.GetString(data)))
    fun (id:Guid) -> get ("ApprovalFlatReadModel-" + id.ToString("N")) |> Async.RunSynchronously

let getApproverFlatReadModel = 
    let get = EventStore.makeReadModelGetter EventStoreConnection.Value (fun data -> JsonConvert.DeserializeObject<ReadModels.ApproverFlatReadModel>(System.Text.Encoding.ASCII.GetString(data)))
    fun (id:String) -> get ("ApproverFlatReadModel-" + id) |> Async.RunSynchronously