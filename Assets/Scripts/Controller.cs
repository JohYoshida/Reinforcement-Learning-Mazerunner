using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    

    public int n_episodes = 1000;    
    public float step = 0.02f;
    public bool moveAgent = false;
    public GameObject _mazePrefab;
    public GameObject _agentPrefab;
    public GameObject _agentCamera;

    private GameObject agent;
    private GameObject maze;
    private GenerateMaze _mazeScript;
    private Agent _agentScript;
    private FollowAgent _cameraScript;
    private int[,] grid = new int[0,0];
    private int state_index = 0;


    // Start is called before the first frame update
    void Start()
    {
        // Setup maze
        maze = Instantiate(_mazePrefab, _mazePrefab.transform.position, Quaternion.identity);
        _mazeScript = maze.GetComponent<GenerateMaze>();
        grid = _mazeScript.PrimMaze();
        _mazeScript.grid = grid;
        _mazeScript.Init();

        // Setup RL Environment
        Env env = new Env();
        env.grid = grid;
        int[] start = _mazeScript.entrance;
        env.goal = _mazeScript.exit;

        // Setup agent
        agent = Instantiate(_agentPrefab, _mazePrefab.transform.position, Quaternion.identity);
        _agentScript = agent.GetComponent<Agent>();
        _agentScript.maze_position = maze.transform.position;
        Debug.Log(agent.transform.position[0]);
        _cameraScript = _agentCamera.GetComponent<FollowAgent>();
        _cameraScript.agent = agent;
        
        // Train agent
        List<float> returns = new List<float>();
        for (int i = 0; i < n_episodes; i++)
        {
            float G = 0;
            env.Reset(start);
            int action = _agentScript.Reset(start);
            bool terminal = false;
            while (!terminal)
            {
                StepReturn sr = env.Step(action);
                G += sr.Reward;
                action = _agentScript.NextAction(sr.Reward, sr.NextState);
                terminal = sr.Terminal;
            }
            _agentScript.FinishEpisode();
            returns.Add(G);
        }

    }

    // Update is called once per frame
    void Update()
    {   
        // _cameraScript.agent = agent.transform;
        if (moveAgent)
        {
            if (state_index < _agentScript.action_history.Count)
            {   
                Vector3 offset = agent.transform.position - _agentScript.maze_position;
                
                int[] state = _agentScript.state_history[state_index];
                float dx = state[0] - offset[0];
                float dz = state[1] - offset[2];
                if (Mathf.Abs(dx) <= 0.1f && Mathf.Abs(dz) <= 0.1f) 
                {
                    state_index++;
                } else {
                    Vector3 move = new Vector3(dx * step, 0f, dz * step);
                    agent.transform.position += move;
                }

            }
        }
    }


}

class Env {
    public float base_reward = -0.01f;
    public float collision_reward = -5f;
    public float goal_reward = 10f;
    public float stagnation_penalty = -0.1f;

    public int[,] grid = new int[0,0];
    public int[] start = new int[2];
    public int[] goal = new int[2];

    public List<int[]> state_history = new List<int[]>();
    private List<int> action_history = new List<int>();
    private List<float> reward_history = new List<float>();
    
    public StepReturn Step(int action)
    {
        bool terminal = false;
        int[] current_state = state_history[state_history.Count-1];
        int[] next_state = TransitionDynamics(current_state, action);
        float reward = 0f;
        if (isCollision(next_state)) 
        {
            reward = collision_reward;
            // terminal = true;
            next_state = current_state;
            terminal = false;
        } 
        else if (next_state[0] == goal[0] && next_state[1] == goal[1])
        {
            reward = goal_reward;
            terminal = true;
        }
        else if (current_state[0] == next_state[0] && current_state[1] == next_state[1])
        {
            reward = stagnation_penalty;
            terminal = false;
        }
        else 
        {
            reward = base_reward;
            terminal = false;
        }

        state_history.Add(next_state);
        reward_history.Add(reward);
        action_history.Add(action);

        StepReturn sr = new StepReturn(next_state, reward, terminal);
        return sr;
    }

    int[] TransitionDynamics(int[] state, int action)
    {
        int[] next_state = (int[]) state.Clone();
        switch (action)
        {
            case 0: 
                next_state[0] += 1;
                break;
            case 1: 
                next_state[1] += 1;
                break;
            case 2: 
                next_state[0] -= 1;
                break;
            case 3: 
                next_state[1] -= 1;
                break;
            default:
                Debug.Log("Invalid action: Must be ints beween 0 and 3 inclusive.");
                break;
        }
        return next_state;
    }

    bool isCollision(int[] state)
    {
        // Out of grid range
        if (
            state[0] >= grid.GetLength(0) ||
            state[0] < 0 ||
            state[1] >= grid.GetLength(1) || 
            state[1] < 0 
        ) {
            return true;
        }
        // Hit a wall
        if (grid[state[0], state[1]] == 1) {
            return true;
        }
        // No collision
        return false;
    }

    public void Reset(int[] init_state)
    {
        state_history.Clear();
        action_history.Clear();
        reward_history.Clear();
        state_history.Add(init_state);
    }
}

class StepReturn {
    
    public int[] NextState;
    public float Reward;
    public bool Terminal;
    
    public StepReturn (int[] state, float reward, bool terminal){
        NextState = state;
        Reward = reward;
        Terminal = terminal;        
    }
}
