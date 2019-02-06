using LoadRunnerClient;
using LoadRunnerClient.MapAndModel;
using System.Windows;
using System.Windows.Media;

/// <summary>
/// Cursor Class for Editor Bindings
/// </summary>
public class BlockCursor : ObservableViewModelBase
{
    private int _x;
    private int _y;
    private Tile _currentTile;

    /// <summary>
    /// Border Color in Editor
    /// </summary>
    private SolidColorBrush _color;

    private Brush _texture;

    public BlockCursor(string color)
    {
        _x = 0;
        _y = 0;
        _color = Application.Current.Resources["COLOR_" + color] as SolidColorBrush;
    }

    public BlockCursor(string color, int x, int y)
    {
        _x = x;
        _y = y;
        _color = Application.Current.Resources["COLOR_" + color] as SolidColorBrush;
    }

    public BlockCursor(string color, int x, int y, Tile tile)
    {
        _currentTile = tile;
        _color = Application.Current.Resources["COLOR_" + color] as SolidColorBrush;
        _x = x;
        _y = y;
    }

    public int X
    {
        get => _x;
        set
        {
            _x = value;
            OnPropertyChanged("X");
        }
    }

    public int Y
    {
        get => _y;
        set
        {
            _y = value;
            OnPropertyChanged("Y");
        }
    }

    public Tile currentTile
    {
        get => _currentTile;
        set => _currentTile = value;
    }

    public SolidColorBrush Color
    {
        get => _color;
        set => _color = value;
    }

    public Brush Texture
    {
        get => _texture;
        set
        {
            _texture = value;
            OnPropertyChanged("Texture");
        }
    }
}