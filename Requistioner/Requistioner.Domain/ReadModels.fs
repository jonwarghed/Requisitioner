module ReadModels

[<CLIMutable>]
type ApprovalFlatReadModel = {
    status : string;
    approver : string;
    approvedBy : string;
}
open System

[<CLIMutable>]
type ApproverFlatReadModel = {
    approvals : Guid list;
}