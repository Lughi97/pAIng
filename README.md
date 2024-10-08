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
  <li>The Ml-Agents version used is 3.0.0(git branch release_21)</li>
  <li>Python version used for the environment is: 3.10.12</li>
  <li>Torch version  is 1.13.1+ccu117 (cuda version 11.7)</li>
</ul>



<div align="center">

## Assignment analysis
</div>
In summary of the assignment, it can be identified:
<ul>
   <li>The disk needs to have a constant speed with no friction and when it bounces off a wall it conserves the energy but not the direction by adding to the outgoing direction a random value between [-5,5] degrees.</li>
   <li>The agent applied to the paddles must be trained using the Machine learning provided by Unity technologies in order to move the paddles up and down to keep the disk inside the game field.</li>
   <li>After completing the training, the neural network model needs to be applied to two paddles so that they can play against each other. </li>
</ul>

<div align="center">

## Assignment Solution
</div>

### Overview
The main key components to solve this assignment are:
<ul>
   <li> `PaddleAgent`, and `Disk` class scripts: Both scripts are important to setup the environment and the agent behavior. </li>
  <li>Two Unity Scene:
  <ul>
   <li> Training Scene: this scene is utilized to train the agent. It has multiple instances of the same game field, each has one paddle on either the left or right side and a wall on the opposite side. This setup gives the agent enough observation that helps the training process. By training both side simultaneously, the agent can get a better insight of the disk dynamics. 
</li>
    <li>Paing Scene: this scene contains only one game field instance that has two paddles and one disk. By applying the neural network model, developed during the training, to both paddles, they will be capable of playing against each other and keeping the disk inside the field. </li>
    </ul>
</li>
   <li> The Python API and  Python Trainer are essential components for training agents in the Unity environment.
    <ul>
      <li>Python API: This component connects to the Unity environment and receives the observation through the internal communicator of Unity </li>
      <li>Python Trainer: This component is responsible for executing the training  through the `mlagents-learn` command. It is based on the configuration file in YAML format, that has all the hyperparameters needed for the training process.  through the `mlagents-learn` based on the configuration file in YAML format, which contains all the hyperparameters needed for the training. It uses a deep reinforcement learning algorithm, the Proximal Policy Optimization (PPO), in order to process the observation and decide wich action to take and send them back to the Unity environment.</li>
   </ul></li>

 These key components work together to generate an effective training environment that will create a neural network model that will satisfy the assignment.
  


## Disk class documentation
This class is used to simulate a bouncing disk in the game field. The key components of this class are:
<ul>
  <li>Energy conservation: the disk needs to keep a constant speed of 15 m/s</li>
  <li>Direaction changes: the disk when it bounces off a wall, conserves the energy but not the direction by a adding a random amount between [-5,5] degrees. Instead when it bounces off the paddle it conserve both energy and direction</li>
</ul>
### Variables
<br>
  
    public float speed = 15;
    public Vector3 direction;
    private Rigidbody rb;
    public bool OutOfBounds;
    private Vector3 startPosition;
    


The variables are related to the disk object. All of them are self-explanatory

### Methods
- `Start()`: Sets up the random direction of the disk, saving the start position and applying the velocity to the rigid body.
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

           


- `OnCollisionEnter()`: It extrapolates the normal of the collision between the disk and the paddle or the wall by accessing their tags set up in the Unity scene.

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

 


- `BounceOffWall()`: Computes the new outgoing direction using the Vector3.reflect function that reflects a vector of the plane defined by a normal -- in this case, the normal of the collision. Afterward, a random value between the range of [-5 to 5] degrees is applied to the outgoing direction using the 3d rotation matrix and normalized. To calculate the new direction, the 3D matrix rotation angle:
  <br> (check how to put it better)
  R(θ) = | cos(θ)  0  -sin(θ) |
  <br>
         |   0     1     0    |
  <br>
         | sin(θ)  0   cos(θ)  |
  <br>
  Then the rb.velocity is updated with the new direction while mantaining the same speed. 

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

   

- `BounceOffPaddle()`: Similar to Bounce of wall  utilizes only the Vector3.Reflect and apply the new direction to the rgidbody velocity.

<br>

     //calculate the new outgoing direction based on the normal of the collision contact with the                 paddle.
    private void BounceOffPaddle(Vector3 normal)
    {
       
        direction = Vector3.Reflect(direction, normal).normalized;
        rb.velocity = direction * speed;
        SpeedCheck();

    }

  

- `SpeedCheck()`: It checks if the rb.velocity.magnitued has been affected, it reset the speed in the correct direction.

<br>

     private void SpeedCheck()
    {
        if (rb.velocity.magnitude < speed)
        {
            rb.velocity = direction * speed;
        }
    }

 
   
- `OnTriggerEnter()`: It detects when the disk is out of bounds. In case of an out-of-bound event then the environment will reset (see PaddleAi class for more detail).

<br>

     // trigger that checks if the disk is out of bounds.
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Bound" || other.gameObject.tag == "BoundPaddle")
        {
            OutOfBounds = true;
        }
    }

     

   - `ResetDisk()`: It resets the position of the disk to the start position and applies a new random direction for the disk to take. This is called when a new training episode begins.

<br>

     // Function called to reset the environment when the disk goes out of bounds.
      // is selected a new direction of the disk.
    public void RestDisk()
    {
        if (OutOfBounds)
            OutOfBounds = false;
        transform.position = startPosition;
        int startRandomDirection = Random.Range(0, 360);
        direction = new Vector3(Mathf.Cos(startRandomDirection * Mathf.Deg2Rad), 0f,                   
        Mathf.Sin(startRandomDirection * Mathf.Deg2Rad));
        rb.velocity = direction * speed;
    }

  


## Paddle class documentation
This class implements the agent's behavior  using  the Unity ML-Agents libraries (Sensors, Actuators). The agent receives positive or negative rewards based on what it is trying to do to keep the disk inside the game field.
A fundamental aspect of this script is the fine-tuning of the rewards values. After multiple training sessions, the rewards were given the best value to make the agent more responsive to the disk's movement making it capable of keeping the disk inside the field.

### Variables

<br>
  
    public Transform diskTransform;
    public float speed = 10f;
    public Vector3 startPosition;
    public bool resetFirstHit = true;
    public float proximityThreshold = 0.5f;
    


The variables are related to the disk object. Some of them are self-explanatory Only:
- `proximityThreshold`: A float that indicates how close the agent's z coordinates position must be to the disk's z coordinate position to receive a reward.

### Methods
- `Start()`: Save the startPosition as the current transform.position
<br>

        void Start()
        {
          startPosition = transform.position;
        }

             


- `OnEpisodeBegin()`: The ML-Agents method is used to set up the Agent instance at the beginning of each episode and  reset the disk by calling the `ResetDisk()` method.

<br>

    public override void OnEpisodeBegin()
    {
     
        transform.position = startPosition;
        RestDisk();
       
    }

  


- `CollectObservation()`: The Ml-Agents method collects the observation vectors of the agent for the step. This function describes the current environment from the agent's perspective.  In this context, the agent needs to keep track of:
  - The current z position (due to how the paddle is placed in the game field in the unity scene.
  - The current x and z position, and the velocity component of the disk.
By keeping track of these five floats the agent has enough information to take action in order to keep the disk inside the game field.
 It is important for the  training to know how many elements the agent is tracking. This number must match the dimension of the Vector Observation of the Behavior Parameters.

<br>


     /*get just the x and z position and velocity of the disk to match the paddle during training*/
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position.z);
        sensor.AddObservation(diskTransform.position.x);
        sensor.AddObservation(diskTransform.position.z);
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.x);
        sensor.AddObservation(diskTransform.GetComponent<Rigidbody>().velocity.z);

    }
  

- `OnActionRecived()`: The Ml-Agents method is used to specify an agent's behavior at every step, based on the provided action passed through the ActionBuffer parameter, which determines how many actions are needed to control the agent. In this particular case, since the paddle's movement is restricted to a single axis -- the z-axis (due to how the paddle is positioned in the game field) -- it only requires one continuous action with a single element in the array.
Continuous action was selected to provide the paddle with a more natural movement and for simplicity, as using the discrete action would have had 3 parameters: one to move up, one to move down, and one to stand still).
Also in this method is set the reward that incentivizes the agent to align its z-coordinate position with the disk's z-coordinate position, enabling it to keep up with the disk.



<br>

     public override void OnActionReceived(ActionBuffers actions)
    {
       ;
        //move in z direction
        float moveZ = actions.ContinuousActions[0];
        transform.position += new Vector3(0, 0, moveZ) * Time.deltaTime * speed;
       
        float zDistanceToDistk = Mathf.Abs(diskTransform.position.z- transform.position.z);
        if(zDistanceToDistk <= proximityThreshold && Mathf.Abs(diskTransform.position.x - transform.position.x)<5)
        {
            SetReward(0.1f);
        }
    }

 

- `Update()`: This method returns a negative reward to the paddle agent whenever the disk goes out of bounds, ending the current episode and starting a new one. 

 <br>

      private void Update()
    {
        if (diskTransform.GetComponent<Disk>().OutOfBounds|| Vector3.Distance(transform.position,   diskTransform.position) > 13f)
        {
          
            SetReward(-1f);
            RestDisk();
            EndEpisode();
        }
    
    }

  
   
- `OnCollisionEnter()`: This method returns a positive reward to the paddle agent each time it successfully hits the disk. This reward was implemented to let the agent know that this action yields a positive outcome, encouraging it to keep the disk inside the field.

<br>

       private void OnCollisionEnter(Collision collision)
      {
          if(collision.gameObject.tag == "Disk")
          {
              SetReward(1f);
          }
      }

   

- `ResetDisk()`: is a function that calls the RestDisk of the Disk class.
<br>

       private void RestDisk()
      {
        diskTransform.GetComponent<Disk>().RestDisk();
      }
  


During the development of this section of the project, the main obstacle was the fine-tuning and placement of the different rewards to have a satisfactory result.



## Behaviour Parameters & Request Decision class documentation
The Behaviour Parameters script and Decision scriptare attached to the object which has the `PaddleAgent` script, to connect with Python API in the Python environment.
<ul>
  <li> Behaviour Parameters: It defines the behavior and the brain properties of an agent within a learning environment. The important parameters are: </li>
      <ul> 
    <li>Behavior name: It is important to identify the behavior of the agent that needs to be the same as the behavior name of the configuration file YAML format (see section Configuration file) </li>
      <li>Behavior name: It is important to identify the behavior of the agent that needs to be the same as the behavior name of the configuration file YAML format (see section Configuration file) </li>
      <li>Vector Observation: is a method of collecting and representing environments from the perspective of the agent. It uses numerical arrays (vectors) to capture the relevant information. In order not to have a complication during the training the dimension of 5 numerical values that are defined in the  `CollectObservations()` of the `PaddleAgent` script 
        <li> Action: refers to the decision made by the agent based on its observation. Also, choose the type and quantity of action that is going to be used by our agent. In this case like mentioned in `OnActionRecived()` of the `PaddleAgent`  script, is used only one continuous action type.</li>
        <li>Model: is the reference where is going to be added the trained neural network model created</li>
  </ul>
  <li>Request Decision Script: it requests a decision every certain amount of time and then takes action.</li>
</ul>





<div align="center">

## Training specifications
</div>
--check this and read this part.
### Configuration file (PaddleAi.yamal)
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
          hidden_units: 256
          num_layers: 2
          vis_encode_type: simple
      reward_signals:
          extrinsic:
          gamma: 0.99
          strength: 1.0
    keep_checkpoints: 5
    max_steps: 3000000
    time_horizon: 1000
    summary_freq: 20000


This file contains all the hyperparameter information that are used to create a training session for the agent 
The behavior name in this file needs to be the same as the behavior chosen in the behavior parameter script.
Mlagenst has 2 different types of learning algorithms:  PPO (proximal Policy Optimization) and Soft Actor-Critic (SAC).  In this scenario, the PPO algorithm is used since is  more general-purpose and stable than many other reinforcement algorithms.

<ul>PPO 
  <li>Proximal Policy Optimization (PPO) is a deep reinforcement learning algorithm  that provides a more stable and efficient approach to policy optimization to balance exploration with exploitation. It is classified as an actor-critic method. (Actor: policy, Critic: value function).
    <ul>Key Features
    <li>Probability ratio: r_t(θ) = π_θ(a_t | s_t) / π_θ_old(a_t | s_t)
        It denotes the probability ratio between the current and the old policy. If this value > 1 then the action is more likely based on the current policy than the old policy. Vice versa if the value is between 0 and 1 the action is more likely based on the old policy than the current policy</li>
   
<li>Advantage Function, Generalized Advantage Estimation (GAE): Â_t = ∑_{l=0}^{∞} (γλ)^l * δ_{t+l} where δ_t = r_t + γ * V(s_{t+1}) - V(s_t)
  It estimates if an action is better or worse than the average return in a given state.</li>
 <li>Clipped Objective Funciton:L^{CLIP}(θ) = E_t[min(r_t(θ) * A_t, clip(r_t(θ), 1 - ε, 1 + ε) * A_t)]
 This objective function restricts the change of the policy during training. This helps to mantains the balance between improving the policy and not deviating too far from the old one. The clipping restricts the probability r_t(θ) between the range  1 - ε, 1 + ε to prevent large update that destabilize the learning process.</li>
 <li> <ul>Learing process: 
   <li>Experience collection: When the agent interacts with the environments it collects: states, action, rewards, and next states.</li>
     <li>Advantage calculation: after collecting the batch of experience, the agent uses the GAE:
         - The critic evaluates the value of the current state and the next state.
         - to compute the advantage function to assess if the action is a good one.  </li>
<li>Policy Update (actor): Using the advantage function, the actor (policy) is updated through the clipped objective function, ensuring stable updates.</li> 
<li>Value function Update (Critic): while updating the actor, the critic will be updated to improve its value function estimate. This involves minimizing the loss between predictive value and observers' returns. The critic is responsible for evaluating the action taken by the actor.
</li>
<li>Iteration over Episodes: this mentioned process is repeated over many episodes. Each iteration allows the agent to gradually refine the policy and value funciton.</li>
 </ul>
    </ul>
 </li></ul>
    
<ul>Hyperparametrs
<li>Batch size: the number of experiences used for one iteration of a gradient descend update. It should be a fraction of the buffer size.</li>
<li>Buffer size defines the number of experiences (agent observation, action, and reward obtained) before doing any learning or model updating.
</li>
<li>Learing rate: corresponds to the strength of each gradient descent update step. </li>
  <li>Beta: is the strength of the entropy regularization to make the policy more random for the agent to explore the action space during training. </li>
  <li> Epsilon: is the divergence threshold between the old and the new calculated policy during the gradient descent updating. Are the clipping parameters for the PPO policy</li> 
  <li>Lambda: the lambda used to calculate the Generalized Advantage Estimation. This is how much the agent relies on its current value estimate when calculating an updated value estimate. Low value the agent relies on its current value estimate, making him more biased. On the other hand, if it's a high value it relies more on the actual reward received in the environment. This parameter helps us to balance high bias and exploration. </li>
  <li>Number of epochs: is the number of passes through the experience bugger during gradient descent.</li>
  <li>learning_rate_schedule: defines how the learning rate changes during training. It will gradually decrease over time. In this case, the learning rate will decrease linearly from the initial value to the final value over a specified number of setups.</li
  <li>beta_schedule: refers to the beta value, in this case, is constant.</li
  <li>Epsilon Schedule: refers to the gradual adjustment of the epsilon parameter for the exploration strategies, like epsilon-greedy methods. The goal is to favor the exploration at the beginning of the training and decrease over time when the model has a more fleshed-out policy.</li>
   <li>Network Setting: specifies the structure of the neural network:
         - Normalize: normalization is applied to the vector observation inputs. Is helpful in complex continuous problems, like in this case, to improve training stability.
         - Hidden Units: the number of units in each fully connected neural network layer.
          - Number of Layers: the number of hidden layers.</li>
  <li> Reward Signals: the agent receives feedback from the environment. They help the agent to learn which action leads to positive outcomes. 
        - Gamma: is the discount factor for future rewards. It means how much the agent values future rewards compared to immediate rewards.
        - Strength: is a multiplier factor for the raw reward. </li>
</ul>
 
### Analysis of the training with this specific configuration file

#### Training Process
This segment of the assignment is the most time-consuming because it requires monitoring the changes in the mean reward and standard deviation of rewards to analyze in real time how effectively the agent is in training.
In these images (need to be added) are some snapshots that illustrate the current mean reward and standard deviation reward (str) at specific intervals determined by the `summary_freq` hyperparameter.
Analyzing this run time training information highlights a steady mean reward growth increasing each step summary, despite some fluctuation. This indicates that the agent is learning to take action that yields higher rewards over time.
Considering  the Standard Deviation of Rewards can yield a more detailed look at the agent's performance. In this case, the std keeps increasing as well over time and it's having some high fluctuations (between value-value) (add image to put correct data). These variations show that the agent is actively exploring different strategies to yield better results.
However, this is not enough information useful to the developer in order to understand the effectiveness of the training. 
To have more information it can be useful to  analyze the information given by the Tensorboard application.

#### Tensorboard results analysis
Tensorboard provides a complete insight into various aspects of the training process that allows us to understand if the model was trained effectively.
The graph here summarizes the training of the neural network model tuned with the parameters of the configuration used for this agent. 

<ul>Enviroment Statistic
    <li>Mean Cumulative Reward: Graph that represents the mean cumulative reward over all the agents and is the same information that is shown in the training process but in a visual format.</li>
<li>Episode Length: The graph represents the mean length of each episode for all the agents, providing insight into how their performance evolves. </li>
</ul>

<ul>Loss statistics
    <li>Policy loss graph: is the mean loss of policy loss function. It fluctuates with a tendency to decrease over time</li>
<li> Vlaue loss graph: the mean loss of the value function update. This represents how the model can predict the value of each state. First, it increases when the agent is learning and steadily decreases once it stabilizes.</li>
</ul>

<ul>Policy statistics
    <li>Entropy: it represents the unpredictability of the action taken by the model. A decrease in entropy during a successful training process means that the model tends to favor effective actions. </li>
<li>Learning rate: This parameter determines how large of a step the training algorithm takes as it searches for the optimal policy. In this case, since is set as linear in the hyperparameters, the graph is a straight line transitioning from  the max balance learning rate to the minum value when the training stops at the 3000000 step. </li>
  <li> Extrinsic reward: This statistic corresponds to the mean cumulative reward received from the environment per episode In our case there is a steady increase over time since the agent is correctly learning to keep the disk inside the field. </li>
    <li> Extrinsic Value Estimate: This statistic corresponds to the mean value estimate for all states visited by the agent. It increases when successful </li>
      <li> Epsilon: This parameter, set in the hyperparameters of the configuration YAML, and shows a steady decrease over time. </li>
   <li> Beta: This parameter, set in the hyperparameters of the configuration YAML, is a constant value.</li>
</ul>

Analyzing all these graphs it can be concluded that there is a steady increase in the mean reward and also the episode length accompanied by high variability. This suggests that the agent gets better and better at following its goal and exploring different strategies to keep the disk inside the game field. This conclusion is further supported by analysis of both the loss and policy statistics that demonstarte the expected increases and decreases. All of this information suggests that the agent had an effective training.

<div align="center">

## Result of the training

</div>

The result of this training is a neural network model (.oox file) that will be placed in the model variable of the Behavior parameters script to both paddles.
When running the scene, the final result fulfills the assignment's requirement: two paddles playing against each other, with the main objective of keeping the disk inside the game field. Occasionally, one of the paddles may fail to hit the disk causing it to go out of bounds and resetting the game. Is important to note that the scoring system is not implemented, since wasn't required in the assignment and  it would have meant more consideration over rewards and some changes in the agent behavior.


## Summary
    
</div>
All the classes, methods, and parameters, have been analyzed, is useful to recap the project, summing up how the requirements have been fulfilled.


<div align="center">
<table>
  <tr>
    <th>Requirement</th>
    <th>Solution</th>
  </tr>
  <tr>
    <td><span style="color:red">Game field setup</span></td>
    <td><span style="color:green">Prefab built accordingly</span></td>
  </tr>
  <tr>
    <td><span style="color:red">Disk must maintain a constant speed with no friction</span></td>
    <td><span style="color:green">Start() and OnCollisionEnter() to calculate the  and random direction and speed that are going to be applied to the rigidbody, without using forces.</span></td>
  </tr>
  <tr>
    <td><span style="color:red">When the disk bounces off a wall it conserves the energy but not the direction by adding to the outgoing direction a value in degrees between -5 and 5.</span></td>
    <td><span style="color:green">BounceOffWall() method to manage the collision with the wall and calculate the new outgoing direction. </span></td>
  </tr>
  <tr>
    <td><span style="color:red">When the disk bounces off the paddle it conserves the energy and direction</span></td>
    <td><span style="color:green">BounceOffPaddle() method handles the collision between disk and paddle.</span></td>
  </tr>
  <tr>
    <td><span style="color:red">When the disk goes out of the game filed a new game is stated.</span></td>
    <td><span style="color:green">The PaddleAgent and Disk handles the moment when the disk goes out of bounds</span></td>
  </tr>
  <tr>
    <td><span style="color:red">Agent must be trained using the Mlagents libraries</span></td>
    <td><span style="color:green">Downladed the necessary packages needed for creating the training environment in both Python and Unity Scene. In the Unity scene this is handled by the PaddleAgnet(), returning the reward for different actions. In the Python environment, through the configuration YAML and the mlagents-learn, start the Python environment training session connected to the specific unity Scene</span></td>
  </tr>
  <tr>
    <td><span style="color:red">Once trained, the same agent must be used on both paddle and play against itslef</span></td>
    <td><span style="color:green">The training generated the neural network model (.oox) that is going to be applied to both paddle to play against each other</span></td>
  </tr>
</table>

