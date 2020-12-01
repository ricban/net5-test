import { Chart, ChartTooltipItem, ChartData } from "chart.js";
import "../../node_modules/chartjs-chart-radial-gauge/build/Chart.RadialGauge.cjs";

export function createChart(id: string, configuration: string): Chart {

    if (!configuration) {
        throw `[${id}] Chart configuration is required.`;
    }

    const chartConfiguration = JSON.parse(configuration);
    const options = chartConfiguration.options;

    switch (chartConfiguration.type) {
        case "bar": {

            const tooltips = options.tooltips || {};
            const scales = options.scales || {};

            tooltips.callbacks = {
                title: function (): string | string[] {
                    return "";
                },
                label: function (tooltipItem: ChartTooltipItem, data: ChartData): string | string[] {
                    const dataset = data.datasets[tooltipItem.datasetIndex];
                    return ` ${dataset.label}: ${dataset.data[tooltipItem.index].toLocaleString()}`;
                }
            };

            scales.yAxes = [{
                ticks: {
                    callback: function (value: number | string) {
                        return value.toLocaleString();
                    }
                }
            }];

            chartConfiguration.options.tooltips = tooltips;
            chartConfiguration.options.scales = scales;

            break;
        }
        case "horizontalBar": {

            const tooltips = options.tooltips || {};
            const scales = options.scales || {};

            tooltips.callbacks = {
                label: function (tooltipItem: ChartTooltipItem, data: ChartData): string | string[] {
                    const dataset = data.datasets[tooltipItem.datasetIndex];
                    return ` ${dataset.label}: ${dataset.data[tooltipItem.index].toLocaleString()}`;
                }
            };

            scales.xAxes = [{
                ticks: {
                    callback: function (value: number | string) {
                        return value.toLocaleString();
                    }
                }
            }];

            chartConfiguration.options.tooltips = tooltips;
            chartConfiguration.options.scales = scales;

            break;
        }
        case "line": {

            const tooltips = options.tooltips || {};
            const scales = options.scales || {};

            tooltips.callbacks = {
                title: function (item: ChartTooltipItem[]): string | string[] {
                    return item[0].label || "";
                },
                label: function (tooltipItem: ChartTooltipItem, data: ChartData): string | string[] {
                    const dataset = data.datasets[tooltipItem.datasetIndex];
                    return ` ${dataset.label}: ${dataset.data[tooltipItem.index].toLocaleString()}`;
                }
            };

            scales.yAxes = [{
                ticks: {
                    callback: function (value: number | string) {
                        return value.toLocaleString();
                    }
                }
            }];

            chartConfiguration.options.tooltips = tooltips;
            chartConfiguration.options.scales = scales;

            break;
        }
        case "radialGauge": {

            const centerArea = options.centerArea || {};

            centerArea.text = function (value: number) {
                return value + "%";
            }

            chartConfiguration.options.centerArea = centerArea;

            break;
        }
        default: {
            throw `Chart of type '${chartConfiguration.type}' is not supported`;
        }
    }

    return new Chart(id, chartConfiguration);
}

export function updateChart(chart: Chart): void {
    chart.update();
}