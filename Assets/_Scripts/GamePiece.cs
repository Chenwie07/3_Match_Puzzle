using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GamePiece : MonoBehaviour
{
    // private int x
    //public int X
    //{
    //    get { return x; }
    //    set
    //    {
    //        if (IsMovable())
    //            x = value;
    //    }
    //}
    public int X { get; set; }
    public int Y { get; set; }
    // private int y; 
    //public int Y {
    //    get { return y; }
    //    set
    //    {
    //        if (IsMovable())
    //            y = value;
    //    }
    //}

    public GameGrid.PieceType Type { get; set; }

    public MovablePiece MovableComponent { get; set; }
    public ColorPiece ColorComponent { get; set; }
    public ClearablePiece ClearableComponent { get; set; }
    public GameGrid GridRef { get; set; }

    public int score;

    private void Awake()
    {
        ColorComponent = GetComponent<ColorPiece>();
        MovableComponent = GetComponent<MovablePiece>();
        ClearableComponent = GetComponent<ClearablePiece>();
    }

    public void Initialize(int _x, int _y, GameGrid _Grid, GameGrid.PieceType _type)
    {
        X = _x;
        Y = _y;
        GridRef = _Grid;
        Type = _type;
    }
   
    public bool IsMovable()
    {
        return !(MovableComponent == null);
    }

    public bool IsColored()
    {
        return ColorComponent != null;
    }
    public bool IsClearable()
    {
        return ClearableComponent != null;
    }

    private void OnMouseEnter()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        GridRef.EnterPiece(this);
    }
    private void OnMouseDown()
    {
        // if we are pressing through a UI (we will create a transparent panel to block out touches, then just return). 
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        GridRef.PressPiece(this);
    }
    private void OnMouseUp()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;
        GridRef.ReleasePiece();
    }
}
