using LoadRunnerClient;
using LoadRunnerClient.MapAndModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static System.Net.Mime.MediaTypeNames;

public class BlockCursorView : ObservableViewModelBase
{

    EditorViewModel _editorViewModel;
    BlockCursor _model;

    public BlockCursorView(BlockCursor model, EditorViewModel editorViewModel)
    {
        this._model = model;
        this._editorViewModel = editorViewModel;

        model.PropertyChanged += CursorChanged;
        editorViewModel.PropertyChanged += OffsetChanged;
    }

    private void CursorChanged(object sender, PropertyChangedEventArgs e)
    {
        OnPropertyChanged(e.PropertyName);
    }

    private void OffsetChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "offset")
        {
            OnPropertyChanged("X");
        }
    }

    public int X
    {
        get => _model.X - _editorViewModel.offset;
    }

    public int Y
    {
        get => _model.Y;
    }

    public Tile currentTile
    {
        get => _model.currentTile;     
    }

    public SolidColorBrush Color
    {
        get => _model.Color;       
    }

    public Brush Texture
    {
        get => _model.Texture;     
    }

}

