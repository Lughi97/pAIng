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
To solve the constraints put on the isk the Disk class is used to simulate a bouncing disk ina field that when it bounces only off the wall will conserve the energy but not the direction.
### Variables
<details>
<summary>Click here to show/hide the code for variables...</summary>
    <br>
  
    public float speed = 15;
    public Vector3 direction;
    private Rigidbody rb;
    public bool OutOfBounds;
    private Vector3 startPosition;
    
</details>

The variables are related to the disk object. All of them are self-explanatory

### Methods
- `Start()`: Here set up the random direction of the disk, saving the start position and apply the velocity to the rigidbody.
<details>
        <summary>Click here to show/hide the <code>Start()</code> method...</summary>
            <br>

          void Start()
          {
              rb = GetComponent<Rigidbody>();
              int startRandomDirection = Random.Range(0, 360);
              direction = new Vector3(Mathf.Cos(startRandomDirection * Mathf.Deg2Rad), 0f,           
              Mathf.Sin(startRandomDirection * Mathf.Deg2Rad));
              startPosition = transform.position;
              rb.velocity = rb.velocity.normalized * speed;
         }

  </details>                 


- `OncollisionEnter()`: it extrapolate the normal of the collision between the disk and the paddle or the wall by accessing their tags (set-up) in the Unity scene.

<details>
 <summary>Click here to show/hide the <code>OncollisionEnter()</code> method...</summary>
            <br>

     private void OnCollisionEnter(Collision collision)
        {
          if (collision.gameObject.tag == "Wall")
          {
              BounceOffWall(collision.contacts[0].normal);
          }
          if (collision.gameObject.tag == "Paddle")
          {
              BounceOffPaddle(collision.contacts[0].normal);
          }
        }

   </details>    


- `BounceOffWall()`: computes the new outgoing direction using the Vector3.reflect function that reflects a vector of the plane defined by a normal, in this case the normal of the collision. Afterward using the 2d rotation matrix, the random value between the range of [-5 to 5] degrees and normalize it. To calculate the new direction, the 3D matrix rotation angle:
  <br> (check how to put it better)
  R(θ) = | cos(θ)  0  -sin(θ) |
  <br>
         |   0     1     0    |
  <br>
         | sin(θ)  0   cos(θ)  |
  <br>
  is used to calculate the new outgoing direction.
  Then the rb.velocity is updated with the new direction and maintain the same speed. 

<details>
 <summary>Click here to show/hide the <code>BounceOffWall()</code> method...</summary>
            <br>

     //Calculate the new outgoing direction based on the normal of the collision contact with the wall.
    private void BounceOffWall(Vector3 normal)
    {
        direction = Vector3.Reflect(direction, normal);

        // Calculate the new outgoing direction of the disk.
        float randomDeflectAngle = Random.Range(-5f, 5f) * Mathf.Deg2Rad;
        // use the 2d rotation matrix.
        float directionX = direction.x * Mathf.Cos(randomDeflectAngle) - direction.z *           
        Mathf.Sin(randomDeflectAngle);
        float directionZ = direction.x * Mathf.Sin(randomDeflectAngle) + direction.z * 
         Mathf.Cos(randomDeflectAngle);

        direction = new Vector3(directionX, 0f, directionZ).normalized;
        //apply the new direction with the same speed in order keep energy.
        rb.velocity = direction * speed;
        SpeedCheck();
    }

   </details>    

- `BounceOffPaddle()`: Similar to Bounce of wall  utilizes only the Vector3.Reflect and apply the new direction to the rgidbody velocity.

<details>
 <summary>Click here to show/hide the <code>BounceOffPaddle()</code> method...</summary>
            <br>

     //calculate the new outgoing direction based on the normal of the collision contact with the                 paddle.
    private void BounceOffPaddle(Vector3 normal)
    {
       
        direction = Vector3.Reflect(direction, normal).normalized;
        rb.velocity = direction * speed;
        SpeedCheck();

    }

   </details>    

- `SpeedCheck()`: it checks if the rb.velocity.speed has been badly influenced, it only reset the speed in the correct direction.

<details>
 <summary>Click here to show/hide the <code>SpeedCheck()</code> method...</summary>
            <br>

     private void SpeedCheck()
    {
        if (rb.velocity.magnitude < speed)
        {
            rb.velocity = direction * speed;
        }
    }

   </details>  
   
- `OnTriggerEnter()`: it resets the position of the disk to the start position and applies a new random direction for the disk to take. This is called when a new training episode begins.

<details>
 <summary>Click here to show/hide the <code>OnTriggerEnter()</code> method...</summary>
            <br>

     // trigger that checks if the disk is out of bounds.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bound" || other.gameObject.tag == "BoundPaddle")
        {
            OutOfBounds = true;
        }
    }

   </details>    

   - `ResetDisk()`: it detects when the disk is out of bounds. In case of an out-of-bound event then the environment will reset (see PaddleAi class for more detail).

<details>
 <summary>Click here to show/hide the <code>ResetDisk()</code> method...</summary>
            <br>

   // Function called to reset the environment when the disk goes out of bounds.
    // is selected a new direction of the disk.
    public void RestDisk()
    {
        if (OutOfBounds)
            OutOfBounds = false;
        transform.position = startPosition;
        int startRandomDirection = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(startRandomDirection * Mathf.Deg2Rad), 0f, Mathf.Sin(startRandomDirection * Mathf.Deg2Rad));
        rb.velocity = direction * speed;
    }

   </details>    


## Paddle class documentation
### Variables
- `Start()`:
### Methods
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
Buffer size defines the number of experiences (agent observation, action, and reward obtained) before doing any learning or model updating.
</li>
<li>Learing rate: corresponds to the strength of each gradient descent update step. </li>
  <li>Beta: is the strength of the entropy regularization to make the policy more random for the agent to explore the action space during training. </li>
  <li> Epsilon: is the threshold of divergence between the old and the new calculated policy during the gradient descent updating</li> </li>
  <li>Lambda: the lambda used to calculate the Generalized Advantage Estimation. This is how much the agent relies on its current value estimate when calculating an updated value estimate. Low value the agent relies on its current value estimate, making him more biased. On the other hand, if it's a high value it relies more on the actual reward received in the environment. This parameter helps us to balance high bias and exploration. </li>
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
Analyzing this run time training information highlights a steady mean reward growth increasing each step summary, apart from some fluctuation always in the positive values. This means the agent is learning to take action that yields higher rewards over time.
Considering  the Standard Deviation of Rewards can highlight a more detailed look at the agent's performance. In this case, the std keeps increasing as well over time and it's having some high fluctuations (between value-value) (add image to put correct data). This shows that the agent is exploring different strategies more actively to yield better results.
However, this is not enough information useful to the developer to understand if the training was effective. 
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


<div align="center">
## Result of the training
</div>

The result of this training is a Neural network model (.oox file) that is going to be placed in the variable model of the Behavior parameters script to both paddles.
The final result when running the scene fulfills the requirement of the assignment, which is 2 paddles playing against each other, with the main objective to keep the disk inside the filed. On rare occasions, one of the paddles is not able to hit the disk in time and it goes out of bounds resetting the game. The scoring system is not implemented since it will mean more consideration over rewards and substantial changes in the environment setup




