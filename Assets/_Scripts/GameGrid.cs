using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameGrid : MonoBehaviour
{
    public enum PieceType
    {
        EMPTY,
        NORMAL,
        BOX,
        ROW_CLEAR, 
        COLUMN_CLEAR,
        COUNT,
    };

    [System.Serializable]
    public struct PiecePrefab
    {
        public PieceType type;
        public GameObject prefab;
    };

    public int xDim;
    public int yDim;
    public float fillTime;

    public GameObject backgroundTilePrefab;
    public PiecePrefab[] piecePrefabs;

    // dictionaries can't be loaded on the inspector, so we load a structure that will get the KvP we need. 
    private Dictionary<PieceType, GameObject> piecePrefabDictionary;

    private GamePiece[,] pieces;

    private bool inverse = false;

    private GamePiece pressedPiece;
    private GamePiece enteredPiece;
    // Start is called before the first frame update
    void Start()
    {
        // then we use this start menu to populate our dictionary with what was passed through the inspector to the struture array. 
        piecePrefabDictionary = new Dictionary<PieceType, GameObject>();
        for (int i = 0; i < piecePrefabs.Length; i++)
        {
            if (!piecePrefabDictionary.ContainsKey(piecePrefabs[i].type))
            {
                piecePrefabDictionary.Add(piecePrefabs[i].type, piecePrefabs[i].prefab);
            }
        }

        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                GameObject background = Instantiate(backgroundTilePrefab,
                    GetWorldPosition(x, y), Quaternion.identity, transform);
            }
        }

        pieces = new GamePiece[xDim, yDim];
        for (int x = 0; x < xDim; x++)
        {
            for (int y = 0; y < yDim; y++)
            {
                #region Previous Method for future Ref
                /* GameObject newPiece = Instantiate(piecePrefabDictionary[PieceType.NORMAL],
                    Vector3.zero, Quaternion.identity, transform);
                newPiece.name = "Piece(" + x + "," + y + ")";
                //pieces[x, y].transform.parent = transform; 
                pieces[x, y] = newPiece.GetComponent<GamePiece>();
                pieces[x, y].Initialize(x, y, this, PieceType.NORMAL); 
                if (pieces[x, y].IsMovable())
                {
                    //x, y is where the 2-d array is at, which makes coordinates auto. 
                    pieces[x, y].MovableComponent.MovePiece(x, y); 
                }
                if (pieces[x, y].IsColored())
                {
                    pieces[x, y].ColorComponent.SetColor( 
                        // here we are casting an integer as an enum, because ofcos enums can 
                        // be converted as integers. 
                        (ColorPiece.ColorType)Random.Range(0, pieces[x, y].ColorComponent.NumColors)
                        );
                    pieces[x, y].GetComponent<SpriteRenderer>().sprite = pieces[x, y].ColorComponent.GetReferencedColorSprite(); 
                    // we could also pass a sprite variable by reference, and then change the value in ColorPiece function. 
                    // after, we use the sprite which have been changed by reference and use here. 
                    // or we could use the multiple function calls we did above. 
                } */
                #endregion
                SpawnNewPiece(x, y, PieceType.EMPTY);
            }
        }
        // testing 

        Destroy(pieces[4, 4].gameObject);
        SpawnNewPiece(4, 4, PieceType.BOX);

        Destroy(pieces[1, 4].gameObject);
        SpawnNewPiece(1, 4, PieceType.BOX);
        Destroy(pieces[2, 4].gameObject);
        SpawnNewPiece(2, 4, PieceType.BOX);
        Destroy(pieces[3, 4].gameObject);
        SpawnNewPiece(3, 4, PieceType.BOX);
        Destroy(pieces[5, 4].gameObject);
        SpawnNewPiece(5, 4, PieceType.BOX);
        Destroy(pieces[6, 4].gameObject);
        SpawnNewPiece(6, 4, PieceType.BOX);

        StartCoroutine(Fill());
    }
    public IEnumerator Fill()
    {
        bool needsRefill = true;
        while (needsRefill)
        {
            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(fillTime);
            }
            needsRefill = ClearAllValidMatches(); 
        }
    }
    public bool FillStep()
    {
        bool movedPiece = false;
        for (int y = yDim - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < xDim; loopX++)
            {
                int x = loopX;
                if (inverse)
                {
                    x = xDim - 1 - loopX;
                }
                GamePiece piece = pieces[x, y];
                if (piece.IsMovable())
                {
                    GamePiece pieceBelow = pieces[x, y + 1];
                    if (pieceBelow.Type == PieceType.EMPTY)
                    {
                        Destroy(pieceBelow.gameObject);
                        piece.MovableComponent.MovePiece(x, y + 1, fillTime);
                        pieces[x, y + 1] = piece;
                        SpawnNewPiece(x, y, PieceType.EMPTY);
                        movedPiece = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;
                                if (inverse)
                                {
                                    diagX = x - diag;
                                }
                                if (diagX >= 0 && diagX < xDim)
                                {
                                    GamePiece diagonalPiece = pieces[diagX, y + 1];
                                    if (diagonalPiece.Type == PieceType.EMPTY)
                                    {
                                        bool hasPieceAbove = true;
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            GamePiece pieceAbove = pieces[diagX, aboveY];
                                            if (pieceAbove.IsMovable())
                                            {
                                                break;
                                            }
                                            else if (!pieceAbove.IsMovable() && pieceAbove.Type != PieceType.EMPTY)
                                            {
                                                hasPieceAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasPieceAbove)
                                        {
                                            Destroy(diagonalPiece.gameObject);
                                            piece.MovableComponent.MovePiece(diagX, y + 1, fillTime);
                                            pieces[diagX, y + 1] = piece;
                                            SpawnNewPiece(x, y, PieceType.EMPTY);
                                            movedPiece = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    } // diagonal movement n stuff. 
                }
            }
        }

        // for the top row. 
        for (int x = 0; x < xDim; x++)
        {
            GamePiece pieceBelow = pieces[x, 0];
            if (pieceBelow.Type == PieceType.EMPTY)
            {
                Destroy(pieceBelow.gameObject);
                GameObject newPiece = Instantiate(piecePrefabDictionary[PieceType.NORMAL],
                    GetWorldPosition(x, -1), Quaternion.identity, transform);

                pieces[x, 0] = newPiece.GetComponent<GamePiece>();
                pieces[x, 0].Initialize(x, -1, this, PieceType.NORMAL);
                pieces[x, 0].MovableComponent.MovePiece(x, 0, fillTime);
                pieces[x, 0].ColorComponent.SetColor((ColorPiece.ColorType)Random.Range(0, pieces[x, 0].ColorComponent.NumColors));
                pieces[x, 0].GetComponent<SpriteRenderer>().sprite = pieces[x, 0].ColorComponent.GetReferencedColorSprite();
                movedPiece = true;
            }
        }
        return movedPiece;
    }
    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2(transform.position.x - xDim / 2.0f + x, transform.position.y + yDim / 2.0f - y);
    }
    public GamePiece SpawnNewPiece(int x, int y, PieceType type)
    {
        GameObject newPiece = Instantiate(piecePrefabDictionary[type], GetWorldPosition(x, y), Quaternion.identity, transform);
        pieces[x, y] = newPiece.GetComponent<GamePiece>();
        pieces[x, y].Initialize(x, y, this, type);

        return pieces[x, y];
    }

    public bool IsAdjacent(GamePiece piece1, GamePiece piece2)
    {
        return (piece1.X == piece2.X && (int)Mathf.Abs(piece1.Y - piece2.Y) == 1
            || (piece1.Y == piece2.Y && (int)Mathf.Abs(piece1.X - piece2.X) == 1));
    }

    public void SwapPieces(GamePiece piece1, GamePiece piece2)
    {
        if (piece1.IsMovable() && piece2.IsMovable())
        {
            pieces[piece1.X, piece1.Y] = piece2;
            pieces[piece2.X, piece2.Y] = piece1;

            //we only swap when there's a match. 
            if (GetMatch(piece1, piece2.X, piece2.Y) != null || GetMatch(piece2, piece1.X, piece1.Y) != null)
            {
                int piece1X = piece1.X;
                int piece1Y = piece1.Y;

                piece1.MovableComponent.MovePiece(piece2.X, piece2.Y, fillTime);
                piece2.MovableComponent.MovePiece(piece1X, piece1Y, fillTime);

                ClearAllValidMatches();
                if (piece1.Type == PieceType.ROW_CLEAR || piece1.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece1.X, piece1.Y); 
                }

                if (piece2.Type == PieceType.ROW_CLEAR || piece2.Type == PieceType.COLUMN_CLEAR)
                {
                    ClearPiece(piece2.X, piece2.Y);
                }
                pressedPiece = null;
                enteredPiece = null; 

                StartCoroutine(Fill());
            }
            else
            {
                pieces[piece1.X, piece1.Y] = piece1;
                pieces[piece2.X, piece2.Y] = piece2;
            }
        }
    }
    // one line function. 
    public void PressPiece(GamePiece piece) => pressedPiece = piece;
    public void EnterPiece(GamePiece piece) => enteredPiece = piece;
    public void ReleasePiece()
    {
        if (IsAdjacent(pressedPiece, enteredPiece))
        {
            SwapPieces(pressedPiece, enteredPiece);
        }
    }
    public List<GamePiece> GetMatch(GamePiece piece, int newX, int newY)
    {
        if (piece.IsColored())
        {
            ColorPiece.ColorType color = piece.ColorComponent.Color;
            List<GamePiece> horizontalPieces = new List<GamePiece>();
            List<GamePiece> verticalPieces = new List<GamePiece>();
            List<GamePiece> matchingPieces = new List<GamePiece>();

            // If the future we face matching and swapping problems with regards to that 
            // check the codes here. 
            #region Checking Horizontal
            // first check horizontal. 
            horizontalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < xDim; xOffset++)
                {
                    int x;
                    if (dir == 0)
                    {
                        // Going left
                        x = newX - xOffset;
                    }
                    else
                    {
                        // Right
                        x = newX + xOffset;
                    }
                    if (x < 0 || x >= xDim)
                    {
                        // if x goes outside of bounds, we break out of the loop. 
                        break;
                    }
                    if (pieces[x, newY].IsColored() && pieces[x, newY].ColorComponent.Color == color)
                    {
                        horizontalPieces.Add(pieces[x, newY]);
                    }
                    else
                    {
                        break; // stop traversing in this direction. 
                    }
                }
            }

            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    matchingPieces.Add(horizontalPieces[i]);
                }
            }
            // if we find a horizontal match for 3, we need to check for vertical in the same vein. 
            // that is traversing vertically to look for an L or T match.
            if (horizontalPieces.Count >= 3)
            {
                for (int i = 0; i < horizontalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < yDim; yOffset++)
                        {
                            int y;
                            if (dir == 0)
                            {
                                // traverse up. 
                                y = newY - yOffset;
                            }
                            else
                            {
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= yDim)
                            {
                                break; // outside dimensions. 
                            }
                            if (pieces[horizontalPieces[i].X, y].IsColored() && pieces[horizontalPieces[i].X, y].ColorComponent.Color == color)
                            {
                                verticalPieces.Add(pieces[horizontalPieces[i].X, y]);
                            }
                            else { break; }
                        }
                    }
                    if (verticalPieces.Count < 2)
                    {
                        verticalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticalPieces.Count; j++)
                        {
                            matchingPieces.Add(verticalPieces[j]);
                        }
                        break;
                    }
                }
            }

            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
            #endregion
            #region Vertical
            // Didn't find anything Horizontal, check vertical. 
            // first clear the vertical and horizontal arrays to get ready to start again. 
            horizontalPieces.Clear();
            verticalPieces.Clear();
            verticalPieces.Add(piece);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < yDim; yOffset++)
                {
                    int y;
                    if (dir == 0)
                    {
                        // Traversing Up
                        y = newY - yOffset;
                    }
                    else
                    {
                        // Traversing down
                        y = newY + yOffset;
                    }
                    if (y < 0 || y >= xDim)
                    {
                        break;
                    }
                    if (pieces[newX, y].IsColored() && pieces[newX, y].ColorComponent.Color == color)
                    {
                        verticalPieces.Add(pieces[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    matchingPieces.Add(verticalPieces[i]);
                }
            }
            // if we find a verticle match for 3, we need to check for vertical in the same vein. 
            // that is traversing horizontally to look for an L or T match.
            if (verticalPieces.Count >= 3)
            {
                for (int i = 0; i < verticalPieces.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < xDim; xOffset++)
                        {
                            int x;
                            if (dir == 0)
                            {
                                // traverse left. 
                                x = newX - xOffset;
                            }
                            else
                            {// traverse right
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= yDim)
                            {
                                break; // outside dimensions. 
                            }
                            if (pieces[x, verticalPieces[i].Y].IsColored() && pieces[x, verticalPieces[i].Y].ColorComponent.Color == color)
                            {
                                //verticalPieces.Add(pieces[x, verticalPieces[i].Y]);
                                horizontalPieces.Add(pieces[x, verticalPieces[i].Y]);
                            }
                            else { break; }
                        }
                    }
                    if (horizontalPieces.Count < 2)
                    {
                        horizontalPieces.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalPieces.Count; j++)
                        {
                            matchingPieces.Add(horizontalPieces[j]);
                        }
                        break;
                    }
                }
            }
            if (matchingPieces.Count >= 3)
            {
                return matchingPieces;
            }
        }
        #endregion
        return null;
    }
    public bool ClearAllValidMatches()
    {
        // checking full board after a Clear for any resulting matches that can were made. 
        // we can use this to make sure our board is randomly shuffled if they're not matches left. 
        bool needsRefill = false;
        for (int y = 0; y < yDim; y++)
        {
            for (int x = 0; x < xDim; x++)
            {
                if (pieces[x, y].IsClearable())
                {
                    List<GamePiece> match = GetMatch(pieces[x, y], x, y);
                    if (match != null)
                    {
                        PieceType specialPieceType = PieceType.COUNT;
                        GamePiece randomPiece = match[Random.Range(0, match.Count)];
                        int specialPieceX = randomPiece.X; 
                        int specialPieceY = randomPiece.Y; 

                        if (match.Count == 4)
                        {
                            if (pressedPiece == null || enteredPiece == null)
                            {
                                specialPieceType = (PieceType)Random.Range((int)PieceType.ROW_CLEAR, (int)PieceType.COLUMN_CLEAR); 
                            }
                            else if (pressedPiece.Y == enteredPiece.Y)
                            {
                                specialPieceType = PieceType.ROW_CLEAR; 
                            }else
                            {
                                specialPieceType = PieceType.COLUMN_CLEAR; 
                            }
                        }

                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearPiece(match[i].X, match[i].Y))
                            {
                                needsRefill = true;

                                if (match[i] == pressedPiece || match[i] == enteredPiece)
                                {
                                    specialPieceX = match[i].X; 
                                    specialPieceY = match[i].Y;
                                }
                            }
                        }
                        if (specialPieceType != PieceType.COUNT)
                        {
                            Destroy(pieces[specialPieceX, specialPieceY]);
                            GamePiece newPiece = SpawnNewPiece(specialPieceX, specialPieceY, specialPieceType); 
                        
                            if ((specialPieceType == PieceType.ROW_CLEAR || specialPieceType == PieceType.COLUMN_CLEAR)
                                && newPiece.IsColored() && match[0].IsColored())
                            {
                                newPiece.ColorComponent.SetColor(match[0].ColorComponent.Color);
                                newPiece.GetComponent<SpriteRenderer>().sprite = newPiece.ColorComponent.GetReferencedColorSprite(); 
                            }
                        }
                    }
                }
            }
        }
        return needsRefill;
    }
    public bool ClearPiece(int x, int y) // takes position on the grid to clear. 
    {
        if (pieces[x, y].IsClearable() && !pieces[x, y].ClearableComponent.IsBeingCleared)
        {
            pieces[x, y].ClearableComponent.ClearPiece(); // clear the piece
            SpawnNewPiece(x, y, PieceType.EMPTY);

            ClearObstacles(x, y);

            return true;
        }
        return false;
    }

    public void ClearObstacles(int x, int y)
    {
        for (int adjacentX = x - 1; adjacentX <= x + 1; adjacentX++)
        {
            if (adjacentX != x && adjacentX >= 0 && adjacentX < xDim)
            {
                if (pieces[adjacentX, y].Type == PieceType.BOX && pieces[adjacentX, y].IsClearable())
                {
                    pieces[adjacentX, y].ClearableComponent.ClearPiece();
                    SpawnNewPiece(adjacentX, y, PieceType.EMPTY);

                }
            }
        }
        for (int adjacentY = y - 1; adjacentY <= y + 1; adjacentY++)
        {
            if (adjacentY != y && adjacentY >= 0 && adjacentY < yDim)
            {
                if (pieces[x, adjacentY].Type == PieceType.BOX && pieces[x, adjacentY].IsClearable())
                {
                    pieces[x, adjacentY].ClearableComponent.ClearPiece();
                    SpawnNewPiece(x, adjacentY, PieceType.EMPTY);
                }
            }
        }
    }

    public void ClearRow(int row)
    {
        for (int x = 0; x < xDim; x++)
        {
            ClearPiece(x, row); 
        }
    }

    public void ClearColumn(int column)
    {
        for (int y = 0; y < yDim; y++)
        {
            ClearPiece(column, y); 
        }
    }
}
