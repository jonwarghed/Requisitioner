module Host

module Approval.Nancy.Web
 
open System
open Nancy
open Nancy.Hosting.Self
open Nancy.ModelBinding

let (?) (parameters:obj) param =
    (parameters :?> Nancy.DynamicDictionary).[param]


type QueryApprovalModule() as self = 
    inherit NancyModule()
    do
        self.Get.["/"] <-
            fun parameter -> 
                View["index"]

        self.Get.["/Approver/{user}"] <-
            fun parameter -> 
                let id = parameter?user.ToString()
                let approver = ReadModelGetters.getApproverFlatReadModel(id) 
                match approver with
                | Some item -> item :> obj
                | None -> HttpStatusCode.NotFound :> obj
            
type Bootstrapper() =
        inherit DefaultNancyBootstrapper()
        override d.RequestStartup(container,pipelines,context) =
            pipelines.AfterRequest.AddItemToEndOfPipeline(fun ctx ->
                            ctx.Response.WithHeader("Access-Control-Allow-Origin", "*").WithHeader("Access-Control-Allow-Methods", "POST,GET").WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type") |> ignore)    
 
[<EntryPoint>]
let main args = 
    let config = new HostConfiguration(UrlReservations =new UrlReservations( CreateAutomatically = true) )
    try
        let nancyHost = new NancyHost(new Bootstrapper(),config,new Uri("http://localhost:43564/Query/"))
        nancyHost.Start()
        printfn "ready..."
        Console.ReadKey() |> ignore
        nancyHost.Stop()
     with
         ex -> 
            printfn "Exception: %s" (ex.ToString());
            printfn "Inner Exception: %s" (ex.InnerException.ToString());
            Console.ReadKey() |> ignore
    0
