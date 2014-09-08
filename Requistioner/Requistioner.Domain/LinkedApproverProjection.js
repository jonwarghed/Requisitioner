var emitReadModel = function (s, e) {
    var streamId = "ApprovalFlatReadModel-" + e.streamId.replace("Approval-", "");
    var eventType = e.eventType + "_ApprovalFlatReadModel";
    emit(streamId, eventType, s);
};
fromCategory('Approval').foreachStream().when({
    $init: function () {
        return {
            status: '',
            approver: '',
            changedBy: ''
        };
    },
    "Requested": function (s, e) {
        s.status = "Requested";
        s.approver = e.data.value;
        emitReadModel(s, e);
    },
    "Approved": function (s, e) {
        s.status = "Approved";
        emitReadModel(s, e);
    },
    "Denied": function (s, e) {
        s.status = "Denied";
        emitReadModel(s, e);
    },
    "TimedOut": function (s, e) {
        s.status = "TimedOut";
        emitReadModel(s, e);
    }
});