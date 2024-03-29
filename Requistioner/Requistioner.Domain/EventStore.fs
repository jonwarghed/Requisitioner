﻿/// Integration with EventStore.
[<RequireQualifiedAccess>]
module EventStore

open System
open System.Net
open EventStore.ClientAPI

/// Creates and opens an EventStore connection.
let conn endPoint =   
    let conn = EventStoreConnection.Create(endPoint)
    conn.Connect()
    conn

/// Creates event store based repository.
let makeRepository (conn:IEventStoreConnection) category (serialize:obj -> string * byte array, deserialize: Type * string * byte array -> obj) =

    let streamId (id:Guid) = category + "-" + id.ToString("N").ToLower()

    let load (t,id) = async {
        let streamId = streamId id
        let! eventsSlice = conn.ReadStreamEventsForwardAsync(streamId, 0, Int32.MaxValue, false) |> Async.AwaitTask
        return eventsSlice.Events |> Seq.map (fun e -> deserialize(t, e.Event.EventType, e.Event.Data))
    }

    let commit (id,expectedVersion) e = async {
        let streamId = streamId id
        let eventType,data = serialize e
        let metaData = [||] : byte array
        let eventData = new EventData(Guid.NewGuid(), eventType, true, data, metaData)
        //Currently we do not look at what version of the event we are expecting
        return! conn.AppendToStreamAsync(streamId, ExpectedVersion.Any, eventData) |> Async.AwaitIAsyncResult |> Async.Ignore
    }

    load,commit

/// Creates a function that returns a read model from the last event of a stream.
let makeReadModelGetter (conn:IEventStoreConnection) (deserialize:byte array -> _) =
    fun streamId -> async {
        let! eventsSlice = conn.ReadStreamEventsBackwardAsync(streamId, -1, 1, false) |> Async.AwaitTask
        if eventsSlice.Status <> SliceReadStatus.Success then return None
        elif eventsSlice.Events.Length = 0 then return None
        else 
            let lastEvent = eventsSlice.Events.[0]
            //Not sure if this row is a good idea
//            if lastEvent.Event.EventNumber = 0 then return None
//            else 
            return Some(deserialize(lastEvent.Event.Data))    
    }

module Global =
    let EventStoreConnection = 
        lazy(let endPoint = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 1113)
        conn endPoint)
