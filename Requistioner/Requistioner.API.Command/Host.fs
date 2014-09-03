module Approval.Nancy.Web
 
open System
open Nancy
open Nancy.Hosting.Self
open Nancy.ModelBinding

//Input Command
[<CLIMutable>]
type RequestApproval = { 
    approver : string;
}

//Response class
[<CLIMutable>]
type RequestApprovalResponse = {
    id : String;  
}

//Input Command
[<CLIMutable>]
type ApproveCommand = {
    id : Guid;
    approver: String;
    message: String;
}



let handleCommand' =
        Approval.CommandHandler.Handle

let handleCommand (id,v) c = handleCommand' (id,v) c |> Async.RunSynchronously

type ApprovalModule() as self = 
    inherit NancyModule()
    do
        self.Post.["/Approval/Request"] <-
            fun parameter ->
                let id = Guid.NewGuid()
                let command = self.Bind<RequestApproval>()
                //Create approval request command and send it down the pipe
                Approval.Request(command.approver) |> handleCommand (id,0) |> ignore
                //Nancy expects an object to be returned
                {RequestApprovalResponse.id = id.ToString("N")} :> obj
        
        self.Post.["/Approval/Approve"] <-
            fun parameter ->
                let command = self.Bind<ApproveCommand>()
                Approval.Approve(command.approver,command.message) |> handleCommand (command.id,1) :> obj

        self.Post.["/Approval/Reject"] <-
            fun parameter ->
                let command = self.Bind<ApproveCommand>()
                Approval.Reject(command.approver,command.message) |> handleCommand (command.id,1) :> obj
            
type Bootstrapper() =
        inherit DefaultNancyBootstrapper()
        override d.RequestStartup(container,pipelines,context) =
            pipelines.AfterRequest.AddItemToEndOfPipeline(fun ctx ->
                            ctx.Response.WithHeader("Access-Control-Allow-Origin", "*").WithHeader("Access-Control-Allow-Methods", "POST,GET").WithHeader("Access-Control-Allow-Headers", "Accept, Origin, Content-type") |> ignore)    
 
[<EntryPoint>]
let main args = 
    let config = new HostConfiguration(UrlReservations =new UrlReservations( CreateAutomatically = true) )
    try
        let nancyHost = new NancyHost(new Bootstrapper(),config,new Uri("http://localhost:43563/Command/"))

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
