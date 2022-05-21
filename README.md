# RL Maze Runner

This project using reinforcement learning to teach an agent how to solve a randomly generated maze. The agent uses the Sarsa method to find the most efficient path through the maze. Mazes are generated using Prim's algorithm.

The number of episodes the agent should train for can be modified by inspecting the Controller GameObject. The height and width of the maze can be modified in a similar way. The agent has modifiable learning parameters:
- alpha: the step size
- gamma: the discount rate
- epsilon: the exploration rate

Please note that large mazes and number of episodes will significantly increase the training time.

Press Play to instantiate the maze and agent, and begin the agent's training. Once the scene is loaded, inspect the Controller GameObject and set Move Agent to True.
