$(function() {
  $('.close-sidebar-btn').click(function() {
    var classToSwitch = $(this).attr('data-class');
    var containerElement = '.app-container';
    $(containerElement).toggleClass(classToSwitch);

    var closeBtn = $(this);

    if (closeBtn.hasClass('is-active')) {
      closeBtn.removeClass('is-active');
    } else {
      closeBtn.addClass('is-active');
    }
  });
});





var options = {};
var chart
var dates = [];
var values = [];
var xx = 0;
function getGraph(fromDate, toDate) {
    var newFromDate = formatDate(fromDate);
    var newToDate = formatDate(toDate);

    $.ajax({
        //from=2019.10.1Z&to=2019.11.1Z
        url: "/IdentityAdmin/GetRegistrationStatistics?from=" + newFromDate + "&to=" + newToDate,
        async: true,
        cache: false,
        success: function (result) {
            for (i in result) {
                dates.push(result[i].date);
                values.push(result[i].count);
            }

            options = {
                chart: {
                    height: 350,
                    type: 'area',
                    zoom: {
                        enabled: false
                    }
                },
                dataLabels: {
                    enabled: false
                },
                stroke: {
                    curve: 'straight'
                },
                series: [{
                    name: "Number of registrations",
                    data: values
                }],
                labels: dates,
                xaxis: {
                    type: 'date',
                    labels: {
                        show: true,
                        rotate: -90
                    }
                },
                yaxis: {
                    forceNiceScale: false,
                    decimalsInFloat: 2,
                    opposite: true
                },
                legend: {
                    horizontalAlign: 'left'
                }
            };

            chart = new ApexCharts(
                document.querySelector("#chart-apex-area"),
                options
            );
            chart.render();
        }
    });
}

function updateGraph(fromDate, toDate) {
    var newFromDate = formatDate(fromDate);
    var newToDate = formatDate(toDate);
    dates = [];
    values = [];

    $.ajax({
        //2019.10.1Z&to=2019.11.1Z
        url: "/IdentityAdmin/GetRegistrationStatistics?from=" + newFromDate + "&to=" + newToDate,
        async: true,
        cache: false,
        success: function (result) {
            for (i in result) {
                dates.push(result[i].date);
                values.push(result[i].count);
            }

            chart.updateOptions({
                series: [{
                    name: "Number of registrations",
                    data: values
                }],
                labels: dates,
                xaxis: {
                    type: 'date',
                    categories: dates,
                    labels: {
                        show: true,
                        rotate: -90,
                    }
                },
                yaxis: {
                    forceNiceScale: false,
                    decimalsInFloat: 1,
                    opposite: true
                }
            })
        }
    });
}

function formatDate(date) {
    var d = new Date(date),
        month = '' + (d.getMonth() + 1),
        day = '' + d.getDate(),
        year = d.getFullYear();

    if (month.length < 2)
        month = '0' + month;
    if (day.length < 2)
        day = '0' + day;

    return [year, month, '1Z'].join('.');
}
