using Microsoft.Maui.Controls;

namespace GestaoChamadosAI_MAUI.Behaviors;

/// <summary>
/// Comportamento que torna um Grid responsivo, alternando entre layout de colunas (desktop) 
/// e layout de linha única (mobile) baseado na largura da tela.
/// </summary>
public class ResponsiveGridBehavior : Behavior<Grid>
{
    private Grid? _grid;
    private int _originalColumns = 2;
    private const double MobileBreakpoint = 600;

    public static readonly BindableProperty ColumnsProperty =
        BindableProperty.Create(
            nameof(Columns),
            typeof(int),
            typeof(ResponsiveGridBehavior),
            2);

    /// <summary>
    /// Número de colunas no modo desktop (padrão: 2)
    /// </summary>
    public int Columns
    {
        get => (int)GetValue(ColumnsProperty);
        set => SetValue(ColumnsProperty, value);
    }

    protected override void OnAttachedTo(Grid bindable)
    {
        base.OnAttachedTo(bindable);
        _grid = bindable;
        _originalColumns = Columns;

        // Captura o evento de mudança de tamanho da página pai
        if (_grid.Parent is VisualElement parent)
        {
            parent.SizeChanged += OnParentSizeChanged;
            // Aplica layout inicial
            UpdateLayout(parent.Width);
        }
    }

    protected override void OnDetachingFrom(Grid bindable)
    {
        if (_grid?.Parent is VisualElement parent)
        {
            parent.SizeChanged -= OnParentSizeChanged;
        }
        _grid = null;
        base.OnDetachingFrom(bindable);
    }

    private void OnParentSizeChanged(object? sender, EventArgs e)
    {
        if (sender is VisualElement element)
        {
            UpdateLayout(element.Width);
        }
    }

    private void UpdateLayout(double width)
    {
        if (_grid == null) return;

        bool isMobile = width < MobileBreakpoint;

        if (isMobile)
        {
            // Mobile: 1 coluna, empilha verticalmente
            _grid.ColumnDefinitions.Clear();
            _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            // Reposiciona todos os children para coluna 0 e mantém a linha original
            foreach (var child in _grid.Children.Cast<View>())
            {
                var originalRow = Microsoft.Maui.Controls.Grid.GetRow(child);
                var originalColumn = Microsoft.Maui.Controls.Grid.GetColumn(child);
                
                // No mobile, empilha na coluna 0 e a "linha" é calculada como originalRow * originalColumns + originalColumn
                var newRow = originalRow * _originalColumns + originalColumn;
                
                Microsoft.Maui.Controls.Grid.SetColumn(child, 0);
                Microsoft.Maui.Controls.Grid.SetRow(child, newRow);
            }

            // Ajusta as definições de linha
            var maxRow = _grid.Children.Cast<View>().Max(c => Microsoft.Maui.Controls.Grid.GetRow(c));
            _grid.RowDefinitions.Clear();
            for (int i = 0; i <= maxRow; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
        }
        else
        {
            // Desktop: restaura layout original de colunas
            _grid.ColumnDefinitions.Clear();
            for (int i = 0; i < _originalColumns; i++)
            {
                _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
            }

            // Restaura posições originais baseado no índice
            var childrenList = _grid.Children.Cast<View>().ToList();
            for (int i = 0; i < childrenList.Count; i++)
            {
                var child = childrenList[i];
                var row = i / _originalColumns;
                var col = i % _originalColumns;
                
                Microsoft.Maui.Controls.Grid.SetRow(child, row);
                Microsoft.Maui.Controls.Grid.SetColumn(child, col);
            }

            // Ajusta as definições de linha
            var maxRow = _grid.Children.Cast<View>().Max(c => Microsoft.Maui.Controls.Grid.GetRow(c));
            _grid.RowDefinitions.Clear();
            for (int i = 0; i <= maxRow; i++)
            {
                _grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            }
        }
    }
}
