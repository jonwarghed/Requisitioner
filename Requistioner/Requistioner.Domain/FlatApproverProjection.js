var emitReadModel = function (s, e, eventtype) {
    var streamId = "ApproverFlatReadModel-" + e.data.value;    
    emit(streamId, eventType, e);
};
fromCategory('Approval').foreachStream().when({
    $init: function (s,e) {
        return {
        };
    },
    "Requested": function (s, e) {
        var eventType = "ApprovedAdded";
        emitReadModel(s, e);
    }
});