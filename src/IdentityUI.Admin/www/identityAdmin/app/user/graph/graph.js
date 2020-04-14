class registarionsGraph {
    constructor() {
        this.fromDate = moment.utc().startOf('month');
        this.toDate = moment.utc().add(1, 'M').startOf('month');

        $('#minus-one').click(() => {
            this.monthMinusOne();
        });

        $('#plus-one').click(() => {
            this.monthPlusOne();
        });

        this.initChart();

        this.getGraph();
    }

    initChart() {
        this.chart = new ApexCharts(
            document.querySelector("#chart-apex-area"),
            {
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
                series: [],
                xaxis: {
                    type: 'datetime',
                    tickPlacement: 'between',
                },
                yaxis: {
                    forceNiceScale: false,
                    decimalsInFloat: 2,
                    opposite: true
                },
                legend: {
                    horizontalAlign: 'left'
                }
            });

        this.chart.render();
    }

    monthMinusOne() {
        this.fromDate = this.fromDate.subtract(1, 'M');
        this.toDate = this.toDate.subtract(1, 'M');

        this.getGraph();
    }

    monthPlusOne() {
        this.fromDate = this.fromDate.add(1, 'M');
        this.toDate = this.toDate.add(1, 'M');

        this.getGraph();
    }

    getGraph() {
        Api.get(`/IdentityAdmin/GetRegistrationStatistics?from=${this.fromDate.toISOString()}&to=${this.toDate.toISOString()}`)
            .done((data) => {
                let series = data.map((e) => {
                    let r = { x: e.date, y: e.count }

                    return r;
                });

                this.chart.updateSeries([{
                    name: 'Registarions',
                    data: series
                }]);

                let selectedMonthString = this.fromDate.format('MMMM YYYY');
                $("#selected-month").text(selectedMonthString);
            })
            .fail((resp) => {
                console.log(resp);
            });
    }
}
