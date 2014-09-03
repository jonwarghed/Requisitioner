[<RequireQualifiedAccess>]
module Approval

type ApprovalState = {
    pending : bool;
    approver: string
}
with static member Zero = {pending = true; approver = ""}

type ApprovalCommand = 
    | Request of approver: string
    | Approve of approver: string * message: string
    | Reject of approver: string * message: string
    | TimeOut of approver: string

type ApprovalEvent = 
    | Requested of string
    | Approved of  approver: string * message: string
    | Rejected of  approver: string * message: string
    | TimedOut of string

open Validator

module private Assert = 
    let possibleToChange state  = validator (fun approval -> approval.pending = true ) ["This approval has already been determined"] state 
    let validateName name = notNull ["The name must not be null."] name <* notEmptyString ["The name must not be empty"] name
    let validateApprover (state,name) = validator (fun approval -> approval.approver = name) ["Insufficient rights"] state

let exec state =
    function
    | Request name                  -> Assert.validateName name  <?> Requested(name)
    | Approve (approver,message)    -> Assert.possibleToChange state <* Assert.validateApprover (state,approver) <?> Approved(approver,message)
    | Reject (approver,message)     -> Assert.possibleToChange state <?> Rejected(approver,message)
    | TimeOut  approver             -> Assert.possibleToChange state <?> TimedOut(approver)

let apply approval = function
    | Requested req -> {approval with approver = req}
    | Approved _ -> { approval with pending = false; }
    | Rejected _ -> { approval with pending = false;}
    | TimedOut _ -> { approval with pending = false;}

module CommandHandler = 
    let Handle = Aggregate.makeHandler 
                    { zero = ApprovalState.Zero; apply = apply; exec = exec }
                        (EventStore.makeRepository EventStore.Global.EventStoreConnection.Value "Approval" Serialization.serializer)