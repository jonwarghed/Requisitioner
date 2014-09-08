var emitReadModel = function (s, e) {
    var streamId = "ApproverFlatReadModel-" + e.data.value;;
    var eventType = e.eventType + "_ApproverFlatReadModel";
    emit(streamId, eventType, s);
};
fromCategory('Approval').foreachStream().when({
    $init: function (s, e) {
        return {
            approvals: []
        };
    },
    "Requested": function (s, e) {
        s.approvals.push(e.streamId.replace("Approval-", ""));
        emitReadModel(s, e);
    }
});