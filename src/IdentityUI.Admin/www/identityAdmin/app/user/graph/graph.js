

var selectedMonthString = moment().startOf('month').format('MMMM YYYY');
document.getElementById("selected-month").innerHTML = selectedMonthString;

var fromDate = moment().startOf('month').format();
var toDate = moment().add(1, 'M').startOf('month').format();

$(document).ready(function () {
    getGraph(fromDate, toDate);
});

$('#minus-one').click(() => {
    monthMinusOne();
});

$('#plus-one').click(() => {
    monthPlusOne();
});

function monthMinusOne() {
    var newFromDate = moment(fromDate).subtract(1, 'M').format()
    var newToDate = moment(toDate).subtract(1, 'M').format();
    selectedMonthString = moment(newFromDate).format('MMMM YYYY');
    document.getElementById("selected-month").innerHTML = selectedMonthString;
    fromDate = newFromDate;
    toDate = newToDate;
    updateGraph(fromDate, toDate)
}

function monthPlusOne() {
    var newFromDate = moment(fromDate).add(1, 'M').format()
    var newToDate = moment(toDate).add(1, 'M').format();
    selectedMonthString = moment(newFromDate).format('MMMM YYYY');
    document.getElementById("selected-month").innerHTML = selectedMonthString;
    fromDate = newFromDate;
    toDate = newToDate;
    updateGraph(fromDate, toDate)
}
