/**
 * Created by Jon on 2014-08-31.
 */
var approval = {};

approval.Approval = function(data){
    this.id = m.prop(data.id);
    this.status = m.prop('');
    this.pending = m.prop(false);
};

approval.ApprovalList = Array;

approval.controller = function(){
    this.list = new approval.ApprovalList();
    this.approver = m.prop("");
    this.currentuser = m.prop("");
    var self = this;

    this.request = function() {
        if (this.approver()) {
            m.request({method: "POST", url: "http://localhost:43563/command/approval/request", data: {"approver": this.approver()}});
        }
    }.bind(this);

    this.approveApproval = function(id) {
        if (this.currentuser()) {
            m.request({method: "POST", url: "http://localhost:43563/command/approval/approve", data: {id:id, "approver": this.currentuser()}});
        }
    }.bind(this);

    this.rejectApproval = function(id) {
        if (this.currentuser()) {
            m.request({method: "POST", url: "http://localhost:43563/command/approval/reject", data: {id:id, "approver": this.currentuser()}});
        }
    }.bind(this);

    this.get = function(){
        if (this.currentuser()) {
            self.list = new approval.ApprovalList();
            m.request({method: "GET", url: "http://localhost:43564/query/approver/" + this.currentuser()}).then(function(answers){
                answers.approvals.forEach(function(answer)
                {
                    self.list.push(new approval.Approval({id: answer}));
                    console.log(self.list);
                });
            });
        }
    }.bind(this);
};

approval.view = function(ctrl) {
    return m("html", [
        m("body", [
            m("span","Approver"),
            m("input", {onchange: m.withAttr("value", ctrl.approver), value: ctrl.approver()}),
            m("button", {onclick: ctrl.request}, "Send request"),
            m("span","Current User"),
            m("input", {onchange: m.withAttr("value", ctrl.currentuser), value: ctrl.currentuser()}),
            m("button", {onclick: ctrl.get}, "Get approvals"),
            m("table", [
                ctrl.list.map(function(item, index) {
                    return m("tr", [
                        m("td", [m("button", {onclick: function wrap() {ctrl.approveApproval(item.id())}}, "Approve")]),
                        m("td", [m("button", {onclick: function wrap() {ctrl.rejectApproval(item.id())}}, "Reject")])
                    ])
                })
            ])
        ])
    ]);
};

//initialize the application
m.module(document, approval);