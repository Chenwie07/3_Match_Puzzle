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
        Destroy(pieces[7, 4].gameObject);
        SpawnNewPiece(7, 4, PieceType.BOX);

        StartCoroutine(Fill());
    }


    // Update is called once per frame
    void Update()
    {

    }
    public IEnumerator Fill()
    {
        while (FillStep())
        {
            inverse = !inverse;
            yield return new WaitForSeconds(fillTime);
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

            int piece1X = piece1.X;
            int piece1Y = piece1.Y;

            piece1.MovableComponent.MovePiece(piece2.X, piece2.Y, fillTime);
            piece2.MovableComponent.MovePiece(piece1X, piece1Y, fillTime);

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

}
