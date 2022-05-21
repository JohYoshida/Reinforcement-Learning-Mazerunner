using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    private GenerateMaze _mazeScript;
    public Vector3 maze_position;

    // RL parameters
    public float alpha = 0.1f;
    public float gamma = 1f;
    public float epsilon = 0.005f;

    private Dictionary<string, float> action_values = new Dictionary<string, float>();
    public List<int[]> state_history = new List<int[]>();
    public List<int> action_history = new List<int>();
    private List<float> reward_history = new List<float>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /**
     * Called at the start of each episode
     */
    public int Reset(int[] init_state)
    {
        Vector3 offset = new Vector3(init_state[0], 1, init_state[1]);
        transform.position = maze_position + offset;
        // Debug.Log("offset: " + offset[0] + " " + offset[1] + " " + offset[2]);

        //Choose action
        int action = ChooseAction(init_state);

        // Initialize histories
        state_history.Clear();
        action_history.Clear();
        reward_history.Clear();
        state_history.Add(init_state);
        action_history.Add(action);
        

        return action;
    }

    /**
     * Choose action from state using e-greedy policy derived from action-values
     */
    int ChooseAction(int[] state)
    {
        // Get list of values for each action from this state
        List<ActionValue> values = new List<ActionValue>();
        for (int a = 0; a < 4; a++)
        {
            string key = state[0] + "" + state[1] + ":" + a;
            if (!action_values.ContainsKey(key))
            {
                // Ensure that all of this state's actions have initialized action-values
                action_values.Add(key, 0f);
            }
            ActionValue av = new ActionValue(a, action_values[key]); 
            values.Add(av);
        }
        // Select action with max value
        float max_value = values[0].Value;
        int action = values[0].Action;
        foreach (var av in values)
        {   
            if (av.Value > max_value)
            {
                max_value = av.Value;
                action = av.Action;
            }
        }
        // Use epsilon-greedy strategy to potentially change the action
        float chance = Random.Range(0f, 1f);
        if (chance <= epsilon)
        {
            action = Random.Range(0,4);
        }

        return action;
    }

    /**
     * Called during each time step of a reinforcement learning episode
     */
    public int NextAction(float reward, int[] state)
    {
        // Choose action according to policy
        int action = ChooseAction(state);
        // Update action-vaue of previous state-action pair
        int[] prev_state = state_history[state_history.Count-1];
        int prev_action = action_history[action_history.Count-1];
        string prev_key = prev_state[0] + "" + prev_state[1] + ":" + prev_action;
        string key = state[0] + "" + state[1] + ":" + action;
        float Q = action_values[prev_key];
        float Q_prime = action_values[key];
        action_values[prev_key] = Q + alpha * (reward + (gamma * Q_prime) - Q);

        // Record history
        state_history.Add(state);
        action_history.Add(action);
        reward_history.Add(reward);

        return action;
    }

    /**
     * Called at the end of each episode
     */
    public void FinishEpisode()
    {
        return;
    }
}

class ActionValue {
    
    public int Action;
    public float Value;
    
    public ActionValue (int action, float value){
        Action = action;
        Value = value;        
    }
}