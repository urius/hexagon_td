using UnityEngine;

public interface IGridViewProvider
{
    GridView GridView { get; }
}

public interface ICellSizeProvider
{
    Vector2 CellSize { get; }
}

public class GridViewProvider : IGridViewProvider, ICellSizeProvider
{
    public GridView GridView { get; private set; }

    public Vector2 CellSize { get; private set; }

    public void SetGridView(GridView gridView)
    {
        GridView = gridView;
        CellSize = new Vector2(gridView.CellSize.x, gridView.CellSize.y);
    }
}
