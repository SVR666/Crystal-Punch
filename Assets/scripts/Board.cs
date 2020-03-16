using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move
}

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    public GameState currentState = GameState.move;
    public int width;
    public int height;
    public int offSet;
    public GameObject tilePrefab;
    public GameObject[] dots;
    private BackgroundTile[,] allTiles;
    public GameObject[,] allDots;

    private FindMatches findMatches;

    //transaction  variables
    public static int specialMultiplier = 0;
    public static int healMultiplier = 0;
    public static int coinmultiplayer = 0;
    private healthbar healthbar;



    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        allTiles = new BackgroundTile[width, height];
        allDots = new GameObject[width, height]; 
        SetUp();
        healthbar = FindObjectOfType<healthbar>();
    }

    private void SetUp()
    {
        for (int i=0; i<width; i++)
        {
            for (int j=0; j<height; j++)
            {
                Vector2 tempPosition = new Vector2(i, j + offSet);
                GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                backgroundTile.transform.parent = this.transform;
                backgroundTile.name = "(" + i + "," + j + ")";
                int dotToUse = Random.Range(0, dots.Length);

                int maxIterations = 0;
                while(MatchesAt(i,j, dots[dotToUse]) && maxIterations < 100)
                {
                    dotToUse = Random.Range(0, dots.Length);
                    maxIterations++;
                }
                maxIterations = 0;

                GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                dot.GetComponent<Dot>().row = j;
                dot.GetComponent<Dot>().column = i;
                dot.transform.parent = this.transform;
                dot.name = "(" + i + "," + j + ")";

                allDots[i, j] = dot;  
            }
        }
    }

    private bool MatchesAt(int column, int row, GameObject crystal)
    {
        if(column > 1 && row > 1)
        {

            if(allDots[column-1,row].tag == crystal.tag && allDots[column-2,row].tag == crystal.tag)
            {
                return true;
            }
            //rows
            if (allDots[column, row-1].tag == crystal.tag && allDots[column, row-2].tag == crystal.tag)
            {
                return true;
            }
        }
        else if (column <=1 || row <= 1)
        {
            if (row > 1)
            {
                if (allDots[column, row - 1].tag == crystal.tag && allDots[column, row - 2].tag == crystal.tag)
                {
                    return true;
                }
            }

            if (column > 1)
            {
                if (allDots[column-1, row].tag == crystal.tag && allDots[column-2, row].tag == crystal.tag)
                {
                        return true;
                }
            }
            
        }

        return false;
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched == true)
        {
            findMatches.currentMatches.Remove(allDots[column, row]);
            Destroy(allDots[column, row]);


            //transcations

            GameObject currentDot = allDots[column, row];
            
            if (currentDot.tag == "Sword")
            {
                specialMultiplier += 1;
                
                
            }
            else if (currentDot.tag == "Heart")
            {
                healMultiplier += 1;
                
            }
            else if (currentDot.tag == "coin")
            {
                coinmultiplayer += 1;

            }
            else
            {
                Debug.Log("Error");
            }
            allDots[column, row] = null;
        }
    }

    public void DestroyMatches()
    {
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i,j);
                }
            }
        }
        /*Debug.Log("special attack =" + specialMultiplier);
        Debug.Log("shielding =" + shieldMultiplier);
        Debug.Log("healing =" + healMultiplier);
        Debug.Log("normal attack =" + attackMultiplier);*/
        if (specialMultiplier > 0)
        {
            if (specialMultiplier == 3)
            {
                healthbar.attackzz(8);
            }
            else if (specialMultiplier == 4)
            {
                healthbar.attackzz(9);
            }
            else
            {
                healthbar.attackzz(10);
            }
        }

        else if(healMultiplier > 0)
        {
            if (healMultiplier == 3)
            {
                healthbar.healzz(8);
            }
            else if (healMultiplier == 4)
            {
                healthbar.healzz(9);
            }
            else
            {
                healthbar.healzz(10);
            }
        }
        else if(coinmultiplayer > 0)
        {
            healthbar.UpdateCoin(coinmultiplayer);
        }
        specialMultiplier = 0;
        healMultiplier = 0;
        coinmultiplayer = 0;  
        StartCoroutine(DecreaseRowCo());
    } 

    //coroutine to collapse columns
    private IEnumerator DecreaseRowCo()
    {
        int nullCount = 0;
        //looking for null spaces in the array
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if(allDots[i,j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }
            nullCount = 0;
        }
        yield return new WaitForSeconds(.4f);
        StartCoroutine(FillBoardCo());
    }

    //refilling board

    private void RefillBoard()
    {
       for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotToUSe = Random.Range(0, dots.Length);
                    GameObject piece = Instantiate(dots[dotToUSe], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesOnBoard()
    {
        for(int i = 0;i<width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    if (allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        RefillBoard();
        yield return new WaitForSeconds(.6f);

        while (MatchesOnBoard())
        {
            yield return new WaitForSeconds(.6f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(.5f);
        currentState = GameState.move;

    }
}
