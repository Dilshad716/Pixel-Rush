using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Tiles;
using TMPro;
using UnityEngine.SceneManagement;

public sealed class BoardManager : MonoBehaviour
{
    [SerializeField] private GameObject[] RowArray;
    private Tiles[,] BoardTiles;
    [SerializeField] float WaitTime;
    [SerializeField] public int HighLightedTilesNo;
    [NonSerialized]  public int HighLightedTilesNoStored;
    public int Score = 0;
    [SerializeField] public TextMeshProUGUI GameOverScore;
    [SerializeField] public int Health;
    [SerializeField] TextMeshProUGUI HealthText;
    [SerializeField] int DifficultyLevel;
    [SerializeField] public GameObject PauseManu;
    [SerializeField] public GameObject GameOverManu;
    [SerializeField] TextMeshProUGUI ScoreText;
    [SerializeField] public int StartTimeInSec;
    [NonSerialized] public int CurrentTime;
    [SerializeField] TextMeshProUGUI TimeLeftText;
    List<(int Row, int Col)> HighLightedTilesLocations;
    // [ColorUsage(true, true)]
    [SerializeField] public Color NormalColor;
  //  [ColorUsage(true, true)]
    [SerializeField] public Color HighlightColor;
   // [ColorUsage(true, true)]
    [SerializeField] public Color RedColor;

    [SerializeField] Transform ObjectPool;
    List<GameObject> pooledObjects = new List<GameObject>();
    int amountToPool = 5;
    [SerializeField] GameObject Explosion;


    internal static BoardManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        GenerateArray();
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Rolling());
        StartCoroutine(StartCountdown());
        StartCoroutine(GameDifficultyLevels(DifficultyLevel));
        HighLightedTilesLocations = new List<(int Row, int Col)>();
        GenerateHighLightRandomTiles(HighLightedTilesNo);
        HighLightedTilesNoStored = HighLightedTilesNo;
        ScoreText.text = Score.ToString();
        HealthText.text = Health.ToString();

        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(Explosion,ObjectPool.transform);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
    }

    // Update is called once per frame
    void Update()
    {

       
        
    }

    public void AddScore(int Amount)
    {

        Score += Amount;
        ScoreText.text = Score.ToString();
    }

    public void AddTimerScore()
    {

        Score += 500;
        ScoreText.text = Score.ToString();
    }

    void GenerateArray()
    {
        BoardTiles = new Tiles[14,10];
        for (int i = 0; i < 14; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                RowArray[i].transform.GetChild(j).GetComponent<Tiles>().Row = i;
                RowArray[i].transform.GetChild(j).GetComponent<Tiles>().Column = j;
                BoardTiles[i, j] = RowArray[i].transform.GetChild(j).GetComponent<Tiles>();
            }
        }
    }

    Tiles GetTile(int row, int col)
    { 
        try{
            return BoardTiles[row, col];
           }
        catch (Exception)
           {
            Debug.LogError("Invalid row or column.");
            return null;
           }

    }

    public void GenerateHighLightRandomTiles(int Count)
    {
        if (HighLightedTilesLocations.Count > 0)
        { NormalRandomTileAgain(HighLightedTilesLocations.Count); }
            
        HighLightedTilesLocations.Clear();
        for (int i = 0; i < Count; i++)
        {
            int RandomRowNo = UnityEngine.Random.Range(0, 14);
            int RandomColNo = UnityEngine.Random.Range(0, 10);

            HighLightedTilesLocations.Add((RandomRowNo, RandomColNo));

            GetTile(RandomRowNo, RandomColNo).HighLightTile();
        }
        
    }

    void HighLightRandomTileAgain(int Count)
    {

        if (HighLightedTilesLocations.Count > 0)
        {
            for (int i = 0; i < Count; i++)
            {
                if (GetTile(HighLightedTilesLocations[i].Row, HighLightedTilesLocations[i].Col).State != TileState.Red)
                {
                    GetTile(HighLightedTilesLocations[i].Row, HighLightedTilesLocations[i].Col).HighLightTile();
                }
            }
        }

    }

    void NormalRandomTileAgain(int Count)
    {

        for (int i = 0; i < Count; i++)
        {
            if (GetTile(HighLightedTilesLocations[i].Row, HighLightedTilesLocations[i].Col).State != TileState.Red)
            {
                GetTile(HighLightedTilesLocations[i].Row, HighLightedTilesLocations[i].Col).NormalTile();
            }
        }

    }

    public void RemoveHighLightTileLocation(int row, int col)
    {
        HighLightedTilesLocations.Remove((row, col));

    }    
    void MakeRowNormal(int row)
    {
        
        for (int i = 0; i < 10; i++)
        {
            GetTile(row, i).NormalTile();
        }
    }
    void MakeRowRed(int row)
    {
        for (int i = 0; i < 10; i++)
        {
            
                GetTile(row, i).RedTile();
            
            
        }
    }

    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        return null;
    }

    IEnumerator Rolling()
    {

        while (true)
        {
            int StartRow = 13;
            int EndRow = 12;


            for (int i = 0; i < 12; i++)
            {
                yield return new WaitForSeconds(WaitTime);
                EndRow--;
                MakeRowRed(EndRow);
                MakeRowNormal(StartRow);
                HighLightRandomTileAgain(HighLightedTilesNo);
                StartRow--;
            }

            for (int i = 12; i > 0; i--)
            {
                yield return new WaitForSeconds(WaitTime);
                StartRow++;
                MakeRowRed(StartRow);
                MakeRowNormal(EndRow);
                HighLightRandomTileAgain(HighLightedTilesNo);
                EndRow++;
            }

        }
        
    }

    void UpdateDisplayTime()
    {
        TimeLeftText.text = CurrentTime.ToString();
    }

    IEnumerator StartCountdown()
    {
        CurrentTime = StartTimeInSec;
        UpdateDisplayTime();
        while (true)
        {
            while (CurrentTime > 0)
            {
                yield return new WaitForSeconds(1f);
                CurrentTime--;
                UpdateDisplayTime();
            }
            if (HighLightedTilesNo > 0)
            {
                Health--;
                HealthText.text = Health.ToString();
            }

            if (Health <= 0)
            {
                Time.timeScale = 0;
                GameOverManu.SetActive(true);
                GameOverScore.text = Score.ToString();
            }
            HighLightedTilesNo = HighLightedTilesNoStored;
            CurrentTime = StartTimeInSec;
            UpdateDisplayTime();
            GenerateHighLightRandomTiles(HighLightedTilesNoStored);
        }
    }

    IEnumerator GameDifficultyLevels(int difficultyLevel)
    {
        int timer = 0;
        int changetimer = 0;
        while (true)
        {
            if (timer >= (changetimer + difficultyLevel) && WaitTime > 0.02f)
            {
                WaitTime -= 0.005f;
                WaitTime = Mathf.Round(WaitTime * 1000f) / 1000f;
                changetimer = timer;
            }
            yield return new WaitForSeconds(1f);
            timer++;
        }

       
    }
}
