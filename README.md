<div align="center">

# pAing (not final version)

</div>

This project is the course "Ai for Videogames" assignment at the University of Milan.
<div align="center">

| Assignment date | Deadline | Delivery date |
| :---: | :---: | :---: |
| 10/08/2024 | 03/09/2024 | 03/09/2024 |
</div>

<div align="center">

## Assignment text
</div>

### Goal
The goal of this project is to create an agent to play the PONG game using machine learning
### Setup
The field is a flat surface of 10 by 5 meters closed by walls on the long sides (up and down) and open on the short ones (left and right). On the left and right sides there are 2 paddles (one on each side) moving along up and down. Paddles measure 1 meter along the dimension facing the field. Inside the field there is a disk with diameter 0.3 meters bouncing back and forth. The speed of the disk is constant at 15 m/s. At the beginning, the disk starts from the center and is set to a random direction. It is not required to keep track of the score. When the disk goes out of the field a new game is started. The system has no friction and the disk, when bouncing on the walls, will conserve its energy but not the direction. At every bounce on walls, the outgoing direction will change by a random value in the range [-5, 5] degrees. E.g., an incoming angle of 30 degrees will make the disk bounce with an outgoing angle between 25 and 35 degrees. The random change is not applied when the disk bounces on the paddles.
### Constraints
You must train, using ML, an agent to move the paddle. Of course, the goal of the paddle is to keep the disk inside the field. Once trained, the same agent must be used on both paddles and play against itself
<ul>
  <li>The Mlagents version used is 3.0.0(git branch release_21)</li>
  <li>Python version used for the environment is: 3.10.12</li>
  <li>Torch version  is 1.13.1+ccu117 (cuda version 11.7)</li>
</ul>



<div align="center">

## Problem analysis
</div>
Different problems need to be considered:
<ul>
   <li>The disk has constant speed and has no friction and only when it bounces off a wall is needed to conserve its energy, but not its direction by applying to the outgoing direction a random value change in the range of [-5,5] degrees. When bouncing off a paddle it keeps both energy and direction</li>
   <li>The agent applied to the paddles must be trained by using the Machine learning provided by Unity technologies to move the paddles up and down to keep the disk inside the field.</li>
   <li>After the training is completed we need to apply the same model agents to both the paddles to play against each other.</li>
</ul>

<div align="center">

## Assignment Resolution
</div>

### Overview
The assignment needs two major components: the Paddle and Disk class script to set up the game environment and the agent with mlagents library compoenet (mlagents version 3.0.0). And the mlagents packages, the configuration file (.yamal), and the neural network trained model (.oox) that are needed to start the training.

<div align="center">
## Game Environment
</div>
In this project, we have two different scenes: 
<ul></ul>
 <li>>In one we have the training scene where there are multiple instances of the same game field with only one paddle with a wall that closes the other side. Inside the scene, there are alternating paddle positions, where in one there is the paddle on the left side of the field and another one is on the right side. This is done to give the agent more observation and the possibility to train simultaneously on both sides in order not to have a final model that only works for one specific paddle due to its position during training.</li>
 <li>The second scene is the one where we have a single game where we have two paddles that are going to play using the same model obtained in the training to play against each other and not to let the disk go out of bounds. </li>

## Disk class documentation
<details>
<summary>Click here to show/hide the code for variables...</summary>
    <br>
  
    public float speed = 15;
    public Vector3 direction;
    private Rigidbody rb;
    public bool OutOfBounds;
    private Vector3 startPosition;
    
</details>

## Paddle class documentation
 --need to add code and explanation
## Additional Script for the Paddle class
--need to add code and explanation
### Behaviour Parameters & Request Decision class documentation

<div align="center">

## Training specifications
</div>
--check this and read this part.
### Configuration file (PaddleAi.yamal)
<details>
<summary>Click here to show/hide the hyperparameters used </summary>
    <br>

    behaviors:
      PaddleAi:
      trainer_type: ppo
      hyperparameters:
         batch_size: 512
         buffer_size: 20000
         learning_rate: 0.0003
         beta: 0.001
         epsilon: 0.2
         lambd: 0.99
         num_epoch: 3
         learning_rate_schedule: linear
         beta_schedule: constant
         epsilon_schedule: linear
      network_settings:
         normalize: true
         hidden_units: 128
         num_layers: 2
      reward_signals:
         extrinsic:
            gamma: 0.99
            strength: 1.0
         network_settings:
            normalize: true
            hidden_units: 128
            num_layers: 2
        max_steps: 2000000
        time_horizon: 1000
        summary_freq: 20000    
</details>

This file contains all the hyperparameter information that are used to create a training session for the agent 
The behavior name in this file needs to be the same as the behavior chosen in the behavior parameter script.
Mlagenst has 2 different types of learning algorithms:  PPO (proximal Policy Optimization) and Soft Actor-Critic (SAC).  In this scenario, the PPO algorithm is used since is  more general-purpose and stable than many other reinforcement algorithms.

<ul>PPO
  <li>Proximal Policy Optimization algorithm is a deep reinforcement learning algorithm. It is a policy gradient method that aims to improve the training stability of deep reinforcement learning...
  ...The main challenge of the PPO and also at large for this project is to fine-tune the Hyperparameters to get the desired outcome of the model.</li><ul>
    
<ul>Hyperparametrs
<li>Batch size: the number of experiences used for one iteration of a gradient descend update. It should be a fraction of the buffer size.
Buffer size: it defines the number of experiences (agent observation, action, and reward obtained) before doing any learning or model updating.
</li>
<li>Learing rate: corresponds to the strength of each gradient descent update step. </li>
  <li>Beta: is the strength of the entropy regularization to make the policy more random for the agent to explore the action space during training. </li>
  <li> Epsilon: is the threshold of divergence between the old and the new calculated policy during the gradient descent updating</li> </li>
  <li>Lambda: is the lambda used for calculating the Generalized Advantage Estimation. This is how much the agent relies on its current value estimate when calculating an updated value estimate. Low value the agent relies on its current value estimate, making him more biased. On the other hand, if it's a high value it relies more on the actual reward received in the environment. This parameter helps us to balance between the high bias and exploration. </li>
  <li>Number of epochs: is the number of passes through the experience bugger during gradient descent.</li>
  <li>learning_rate_schedule: defines how the learning rate changes during training. It will gradually decrease over time. In this case, the learning rate will decrease linearly form the initial value to the final value over a specified number of setups.</li
  <li>beta_schedule: refers to the beta value, in this case, is constant.</li
  <li>Epsilon Schedule: refers to the gradual adjustment of the epsilon parameter for the exploration strategies, like epsilon-greedy methods. The goal is to favor the exploration at the beginning of the training and decrease over time when the model has a more fleshed-out policy.</li>
</ul>
  --- continue
### Analysis of the training with this specific configuration file

#### Training Process
This session of the Assignment is the most time-consuming because it requires monitoring the changes in the mean reward and standard deviation of rewards to analyze in real-time how effectively the agent in training.
In these images (need to be added) are some moments that represent the current mean reward and str (standard deviation reward) in precise steps, in which the interval is set by the hyperparameter summary_freq.
Analyzing this run time training information it's highlighted a steady mean reward growth increasing each step summary, apart from some fluctuation that is always in the positive value. This means that the agent is learning to take action that yields higher rewards over time.
Taking also into consideration the Standard Deviation of Rewards because can highlight a more detailed look on the agent's performance. In this case, the std keeps increasing as well over time and it's having some high fluctuations (between value-value) (add image to put correct data). This shows that the agent is exploring different strategies more actively to yield better results.
This, however, is not enough information that is useful to the developer to understand if the training was effective. 
To have more information is advised to analyze also the information given by the tensorboard application.

#### Tensorboard results analysis
Tensorbaord provides all the visual information of different aspects of the training that can inform the developer if the model was trained correctly.
In summary, all of these graphs can sum up the training of the neural network model tuned with these specific hyperparameters and can help us understand highlights a positive trend of the mean reward. 

<ul>Enviroment Statistic
    <li>The first graph represents  the mean cumulative reward over all the agents and is the same information that is shown in the training process but put in graph</li>
<li>Episode length graph represents the mean length of each episode for all the agents </li>
</ul>

<ul>Loss statistics
    <li>Policy loss graph: is the mean loss of policy loss function, it fluctuates with a tendency to decrease over time</li>
<li> Vlaue loss graph: the mean loss of the value function update. This represents how the model can predict the value of each state. First, it increases when the agent is learning and steadily decreases once it stabilizes.</li>
</ul>

<ul>Policy statistics
    <li>Entropy: it represents how random the model is. This decreases during successful training process</li>
<li>Learning rate:  is how large a step the training algorithm takes as it searches for the optimal policy. In this case, since is set as linear in the hyperparameters, the graph is a straight line between the max balance learning rate and min value when the training stops at the 2000000 step. </li>
  <li> Extrinsic reward: it corresponds to the mean cumulative reward received from the environment per episode In our case there is a steady increase over time since the agent is correctly learning to keep the disk inside the field. </li>
</ul>

Analyzing all these graphs it can be concluded that from the begging is a steady increase in the mean reward and also the episode length with high variability since the agent gets better and better in following its goal and exploring different ways to keep the disk inside the filed. This idea is refornced by observing also the loss statistic and policy statistic that specific value mentioned increase and decrease correctly for each useful training session made by the agent.


div align="center">
## Result of the training
</div>

The result of this training is a Neural network model (.oox file) that is going to be placed in the variable model of the Behavior parameters script to both paddles.
The final result when running the scene fulfills the requirement of the assignment, which is 2 paddles playing against each other, with the main objective to keep the disk inside the filed. On rare occasions, one of the paddles is not able to hit the disk in time and it goes out of bounds resetting the game. The scoring system is not implemented since it will mean more consideration over rewards and substantial changes in the environment setup




