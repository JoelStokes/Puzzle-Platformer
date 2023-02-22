using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTile : MonoBehaviour
{
    public string levelScene;
    public Route[] routes = new Route[4];  //North, East, South, West
    private Vector3[,] routePositions = new Vector3[4,4];  //Final values applied here to prevent reversal overwriting original
    public bool[] reversed = new bool[4];

    void Start(){
        SetRoutePositions();
    }

    private void SetRoutePositions(){
        for (int i=0; i<4; i++){
            if (routes[i]){
                for (int j = 0; j<4; j++)
                {
                    if (routes[i].controlPoints[j]){
                        Vector3 newPos = new Vector3(routes[i].controlPoints[j].position.x, routes[i].controlPoints[j].position.y, routes[i].controlPoints[j].position.z);
                        if (reversed[i]){
                            routePositions[i,Mathf.Abs(j-3)] = newPos;
                        } else {
                            routePositions[i,j] = newPos;
                        }
                    } else {
                        routePositions[i,j] = Vector3.zero;
                    }
                }
            }
        }
    }

    public Vector3[,] GetPositions(){
        return routePositions;
    }

    //private Route[] routes = new Route[4];  

    /*void Start(){
        //Add check later to see if level has been unlocked!!!!!
        if (levelN != "" && levelN != null){
            routes[0] = FindRoute("N", 0, levelN);
        }

        if (levelE != "" && levelE != null){
            routes[1] = FindRoute("E", 1, levelE);
        }

        if (levelS != "" && levelS != null){
            routes[2] = FindRoute("S", 2, levelS);
        }

        if (levelW != "" && levelW != null){
            routes[3] = FindRoute("W", 3, levelW);
        }
    }

    private Route FindRoute(string dir, int dirInt, string name){
        var childObj = transform.Find(dir);
        Debug.Log("childObj: " + childObj + ", dir: " + dir + ", dirInt: " + dirInt + ", name: " + name);
        if (childObj != null)
        {
            return childObj.GetComponent<Route>();
        } else {    //Route is from a previous level, reverse other level's route
            Route otherRoute = GameObject.Find("/Levels/" + name).GetComponent<LevelTile>().GetRouteByName(transform.name);

            if (otherRoute != null){
                List<Transform> points = new List<Transform>();
                for (int i=0; i<4; i++){
                    Debug.Log(otherRoute.controlPoints[i]);
                    points.Insert(0, otherRoute.controlPoints[i]);
                }

                for (int i=0; i<4; i++){
                    otherRoute.controlPoints[i] = points[i];
                }

                return otherRoute;
            } else {
                Debug.Log("ERROR: Null Route returned from " + name + " getComponent call during GetRouteByName(" + transform.name + ")");

                return otherRoute;
            }

        }
    }

    public Route GetRouteByName(string name){
        if (name == levelN){
            return routes[0];
        } else if (name == levelE){
            return routes[1];
        } else if (name == levelS){
            return routes[2];
        } else if (name == levelW){
            return routes[3];
        }

        Debug.Log(levelN + ", " + levelE + ", " + levelS + ", " + levelW + ", ||| Looking for: " + name);
        Debug.Log("ERROR! No matching route found for " + name + " in level tile " + transform.name);
        return routes[0];
    }*/
}
