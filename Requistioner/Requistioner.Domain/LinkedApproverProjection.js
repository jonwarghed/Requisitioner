var emitReadModel = function (s, e) {
    var streamId = "ApproverFlatReadModel-" + e.data.value;;
    var eventType = e.eventType + "_ApproverFlatReadModel";
    emit(streamId, eventType, s);
};
fromCategory('Approval').foreachStream().whenAny(function(state, ev) {
    linkTo('ApproverTest-' + ev.data.value)
});