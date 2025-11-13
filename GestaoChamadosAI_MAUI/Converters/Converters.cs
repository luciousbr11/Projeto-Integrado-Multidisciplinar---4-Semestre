using System.Globalization;

namespace GestaoChamadosAI_MAUI.Converters
{
    public class StringToBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class InvertedBoolConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return true;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
                return !boolValue;
            return false;
        }
    }

    public class ChatAlignmentConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int messageUserId && parameter is int currentUserId)
            {
                return messageUserId == currentUserId ? LayoutOptions.End : LayoutOptions.Start;
            }
            return LayoutOptions.Start;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class ChatColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int messageUserId && parameter is int currentUserId)
            {
                return messageUserId == currentUserId 
                    ? Color.FromArgb("#512BD4") // Primary
                    : Color.FromArgb("#6E6E6E"); // Gray500
            }
            return Color.FromArgb("#6E6E6E");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToIconConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Aberto" => "🕐",
                    "Em Andamento" => "⚙️",
                    "Concluído" => "✅",
                    "Solucionado por IA" => "🤖",
                    _ => "📋"
                };
            }
            return "📋";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Aberto" => Color.FromArgb("#4dabf7"), // Info Blue
                    "Em Andamento" => Color.FromArgb("#ffd43b"), // Warning Yellow
                    "Concluído" => Color.FromArgb("#51cf66"), // Success Green
                    "Solucionado por IA" => Color.FromArgb("#01acac"), // Primary Teal
                    _ => Color.FromArgb("#6E6E6E") // Gray
                };
            }
            return Color.FromArgb("#6E6E6E");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StatusToTextConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string status)
            {
                return status switch
                {
                    "Aberto" => "⏳ Aguardando",
                    "Em Andamento" => "👤 Em Atendimento",
                    "Concluído" => "✅ Concluído",
                    "Solucionado por IA" => "🤖 IA Resolveu",
                    _ => status
                };
            }
            return value?.ToString() ?? "";
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class PrioridadeToColorConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is string prioridade)
            {
                return prioridade switch
                {
                    "Alta" => Color.FromArgb("#EF4444"), // Red
                    "Média" => Color.FromArgb("#F59E0B"), // Orange
                    "Baixa" => Color.FromArgb("#10B981"), // Green
                    _ => Color.FromArgb("#6B7280") // Gray
                };
            }
            return Color.FromArgb("#6B7280");
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    // Novos converters para o chat
    public class BoolToColorConverter : IValueConverter
    {
        public string TrueColor { get; set; } = "#EFEFEF";
        public string FalseColor { get; set; } = "#512BD4";

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return Color.FromArgb(boolValue ? TrueColor : FalseColor);
            }
            return Color.FromArgb(FalseColor);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToLayoutOptionsConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "Start";
        public string FalseValue { get; set; } = "End";

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var result = boolValue ? TrueValue : FalseValue;
                return result == "Start" ? LayoutOptions.Start :
                       result == "End" ? LayoutOptions.End :
                       result == "Center" ? LayoutOptions.Center :
                       LayoutOptions.Fill;
            }
            return LayoutOptions.Start;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToMarginConverter : IValueConverter
    {
        public string TrueValue { get; set; } = "8,2,80,2";
        public string FalseValue { get; set; } = "80,2,8,2";

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                var marginString = boolValue ? TrueValue : FalseValue;
                var parts = marginString.Split(',');
                if (parts.Length == 4)
                {
                    return new Thickness(
                        double.Parse(parts[0]),
                        double.Parse(parts[1]),
                        double.Parse(parts[2]),
                        double.Parse(parts[3])
                    );
                }
            }
            return new Thickness(0);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BoolToDoubleConverter : IValueConverter
    {
        public double TrueValue { get; set; } = 1.0;
        public double FalseValue { get; set; } = 0.5;

        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is bool boolValue)
            {
                return boolValue ? TrueValue : FalseValue;
            }
            return FalseValue;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class StringNotEmptyConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return !string.IsNullOrWhiteSpace(value as string);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CountToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value is int count)
            {
                return count > 0;
            }
            return false;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
