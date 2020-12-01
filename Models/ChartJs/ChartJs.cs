using Covid19.Client.Extensions;

namespace Covid19.Client.Models
{
    public class ChartJs
    {
        public ChartJs()
        {
        }

        public ChartJs(ChartJsType type, ChartJsData data, ChartJsOptions options)
        {
            Type = type.ToString().ToCamelCase();
            Data = data;
            Options = options;
        }

        public string Type { get; set; } = default!;
        public ChartJsData Data { get; set; } = default!;
        public ChartJsOptions Options { get; set; } = default!;
    }
}