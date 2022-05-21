using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMaze : MonoBehaviour
{

    public GameObject _wallPrefab;
    public GameObject _mazePrefab;
    public int _gridXSize;
    public int _gridZSize;
    public int[] entrance = new int[2];
    public int[] exit = new int[2];
    public int[,] grid = new int[0,0];


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Init()
    {
        for (int i = 0; i < _gridXSize; i++)
        {
            for (int j = 0; j < _gridZSize; j++)
            {
                if (grid[i, j] == 1) 
                {
                    Vector3 position = new Vector3(i, 0.5f, j) + _mazePrefab.transform.position;
                    GameObject wall = Instantiate(_wallPrefab, position, Quaternion.identity);
                    wall.name = i + " " + j;
                    wall.transform.parent = _mazePrefab.transform;
                }
            }
        }
    }

    public int[,] PrimMaze() 
    {
        grid = new int[_gridXSize, _gridZSize];
        List<int[]> wallList = new List<int[]>();
        // Start with grid full of walls
        for (int i = 0; i < _gridXSize; i++)
        {
            for (int j = 0; j < _gridZSize; j++)
            {
                grid[i, j] = 1; // 1 for wall
            }
        }
        // Pick a random cell and mark it as part of the maze
        int[] startCell = new int[] {Random.Range(0, _gridXSize-1), Random.Range(0, _gridZSize-1)};
        grid[startCell[0], startCell[1]] = 0; // 0 for passage
        // Add neighbours to WallList
        List<int[]> neighbours = Neighbours(startCell);
        foreach (int[] cell in neighbours)
        {
            if (!wallList.Contains(cell) )
            {
                wallList.Add(cell);
            }
        }
        // While there are walls in list
        while (wallList.Count > 0)
        {
            // Pick random wall from list
            int index = Random.Range(0, wallList.Count);
            int[] cell = wallList[index];
            // Count neighbours that are passages
            neighbours = Neighbours(cell);
            int count = 0;
            foreach (int[] n in neighbours)
            {
                if (grid[n[0], n[1]] == 0)
                {
                    count++;
                }
            }
            if (count == 1) // Only one neighbour is a passage
            {
                // Mark as passage
                grid[cell[0], cell[1]] = 0;
                // Add cell's neighbouring walls to list
                foreach (int[] n in neighbours)
                {
                    if (!wallList.Contains(n) )
                    {
                        wallList.Add(n);
                    }
                }
            }
            // Remove wall from list
            wallList.Remove(cell);   
        }

        // Add entrance and exit
        for (int i = 0; i < _gridZSize; i++)
        {
            if (grid[1, i] == 0) {
                grid[0, i] = 0;
                entrance = new int[] { 0, i };
                break;
            }
        }
        for (int i = _gridZSize - 1; i > 0; i--)
        {
            if (grid[_gridXSize - 2, i] == 0) {
                grid[_gridXSize - 1, i] = 0;
                exit = new int[] {_gridXSize - 1, i};
                break;
            }
        }

        return grid;
    }

    /**
      * Return a list of valid neighbours to the cell
      * int[] cell: array length 2 of grid coordinates 
      */
    List<int[]> Neighbours(int[] cell) {
        List<int[]> neighbours = new List<int[]>();
        int[] n;
        if (cell[0] - 1 > 0) {
            n = new int[] { cell[0] - 1, cell[1]};
            neighbours.Add(n);
        }
        if (cell[1] - 1 > 0) {
            n = new int[] { cell[0], cell[1] - 1 };
            neighbours.Add(n);
        }
        if (cell[0] + 1 < _gridXSize - 1) {
            n = new int[] { cell[0] + 1, cell[1] };
            neighbours.Add(n);
        }
        if (cell[1] + 1 < _gridZSize - 1) {
            n = new int[] { cell[0], cell[1] + 1 };
            neighbours.Add(n);
        }

        return neighbours;
    }

    public int[,] Grid() 
    {
        return grid;
    }
}
