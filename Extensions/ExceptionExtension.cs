using System;
using System.Text;

namespace Covid19.Client.Extensions
{
    public static class ExceptionExtension
    {
        public static string ToText(this Exception ex)
        {
            var sb = new StringBuilder(ex.ToString());

            sb.AppendLine();

            if (ex.InnerException?.Data?.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Inner Exception Data:");
                sb.AppendLine();
                sb.Append("Data = ").AppendLine(ex.InnerException.Data.Serialize(true));
                sb.AppendLine();
            }

            if (ex.Data.Count > 0)
            {
                sb.AppendLine();
                sb.Append("Data = ").AppendLine(ex.Data.Serialize(true));
            }

            return sb.ToString();
        }

        public static string ToHtml(this Exception ex)
        {
            var sb = new StringBuilder($"<p>{ex}</p>");

            sb.AppendLine("<br/>");

            if (ex.InnerException?.Data?.Count > 0)
            {
                sb.AppendLine("<br/>");
                sb.AppendLine("<p>Inner Exception Data:</p>");
                sb.AppendLine("<br/>");
                sb.Append("Data = ").Append(ex.InnerException.Data.Serialize(true)).AppendLine("</p>");
                sb.AppendLine("<br/>");
            }

            if (ex.Data.Count > 0)
            {
                sb.AppendLine("<br/>");
                sb.Append("Data = ").Append(ex.InnerException?.Data.Serialize(true)).AppendLine("</p>");
            }

            return sb.ToString();
        }
    }
}