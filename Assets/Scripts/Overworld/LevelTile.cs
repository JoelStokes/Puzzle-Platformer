using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    public int levelNumber; //Pulls level data from saved data
    public Vector2 exit1Pos;
    public Vector2 exit2Pos;
    public LineRenderer exit1Line;
    public LineRenderer exit2Line;

    //Level Data
    class pathData{
        int exitNumber;
        bool exitCleared;
        
    }

    private string levelName;
    private bool exit1Clear = false;
    private GameObject exit1Level;
    private bool exit2Clear = false;
    private GameObject exit2Level;

    void Start()
    {
        exit1Line = GetComponent<LineRenderer>();

        GetLevelData();
        SetPaths();
    }

    void Update()
    {
        
    }

    private void GetLevelData(){
        

        exit1Pos = exit1Level.transform.position;
        exit2Pos = exit2Level.transform.position;
    }

    private void SetPaths(){
        if (exit1Clear){
            
        } else if (exit2Clear){

        }
    }

    public void PassPaths(){

    }
}
