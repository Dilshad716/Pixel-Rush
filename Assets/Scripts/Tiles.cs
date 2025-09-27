using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    [SerializeField] public int Row;
    [SerializeField] public int Column;
    [SerializeField] public int ScoreAmount;
    [SerializeField] Animator animator;
    public enum TileState
    {
        Normal,
        HighLight,
        Red
    }
    SpriteRenderer rend;
    [SerializeField] public TileState State;

    private void Awake()
    {
        rend = GetComponent<SpriteRenderer>();
        SetTile();
        animator = GameObject.FindWithTag("MainCamera").GetComponent<Animator>();
    }

    public void SetTile()
    {
        switch (State)
        {
            case TileState.Normal:
               NormalTile();
                break;
            case TileState.HighLight:
                HighLightTile();
                break;
            case TileState.Red:
                RedTile();
                break;
        }
    }

    public void NormalTile()
    {
        State = TileState.Normal;
        rend.color = BoardManager.instance.NormalColor;

    }

    public void HighLightTile()
    {
        State = TileState.HighLight;
        rend.color = BoardManager.instance.HighlightColor;
    }
    public void RedTile()
    {
        State = TileState.Red;
        rend.color = BoardManager.instance.RedColor;
    }

    private void OnMouseDown()
    {
        switch (State)
        {
            case TileState.Normal:
                break;
            case TileState.HighLight:
                BoardManager.instance.RemoveHighLightTileLocation(Row, Column);
                BoardManager.instance.HighLightedTilesNo--;
                ExplosionBegin();
                animator.SetTrigger("ShakeIt");
                NormalTile();
                BoardManager.instance.AddScore(ScoreAmount);
                if (BoardManager.instance.HighLightedTilesNo <= 0)
                { 
                  BoardManager.instance.GenerateHighLightRandomTiles(BoardManager.instance.HighLightedTilesNoStored);
                  BoardManager.instance.HighLightedTilesNo = BoardManager.instance.HighLightedTilesNoStored;
                  BoardManager.instance.CurrentTime = BoardManager.instance.StartTimeInSec;
                  if(BoardManager.instance.CurrentTime >= 2)
                    {

                        BoardManager.instance.AddTimerScore();
                    }
                }
                break;
            case TileState.Red:
                print("Game Over");
                Time.timeScale = 0f;
                BoardManager.instance.GameOverManu.SetActive(true);
                BoardManager.instance.GameOverScore.text = BoardManager.instance.Score.ToString();
                break;
        }
    }

    void ExplosionBegin()
    {
        GameObject Explosion = BoardManager.instance.GetPooledObject();
        if (Explosion != null)
        {
            Explosion.transform.position = transform.position;
        }
        Explosion.SetActive(true);
        StartCoroutine(DisableAfterDelay(Explosion, 1f));

    }
    IEnumerator DisableAfterDelay(GameObject Obj, float Delay)
    { 
      yield return new WaitForSeconds(Delay);
      Obj.SetActive(false);
    }

}
