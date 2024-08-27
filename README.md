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
In summary of the assignment, it is possible to find the following key points:
<ul>
   <li>The disk has constant speed and no friction and only when it bounces off a wall is needed to conserve its energy, but not its direction by applying a random value change in the range of [-5,5] degrees. When bouncing off a paddle it keeps both energy and direction</li>
   <li>The agent applied to the paddles must be trained using the Machine learning provided by Unity technologies to move the paddles up and down to keep the disk inside the game field.</li>
   <li>After completing the training, the neural network model needs to be applied to two paddles so that they can play against each other. </li>
</ul>

<div align="center">

## Assignment Solution
</div>

### Overview
The main key components to solve this assignment are:
<ul>
   <li> `PaddleAgent`, and `Disk` class script to set up the agent's behavior, rewards, and  the game environment. </li>
  <li>Two Unity Scene:
  <ul>
   <li> Training Scene: this is the scene where the agent's training takes place. There are multiple instances of the same game field, each side with only one paddle and a  wall that closes the opposite side. The paddle placement alternates, with some instances where the paddle is on the left side of the field and other on the right side. This setup allows the agent more observation and trains simultaneously on both sides, ensuring that the final neural network model works on both sides, not just one specific position.
</li>
    <li>Paing Scene: a single game instance with two paddles, each using the same neural network model from the training, playing against each other to not letting the disk go out of bounds.</li>
    </ul>
</li>
   <li> The Python API and  Python Trainer are essential to train agents in the Unity environment.
    <ul>
      <li>Python API: connects to the Unity environment through the communicator and gets the observation from the agents </li>
      <li>Python Trainer: execute the training through the `mlagents-learn` based on the  configuration file in YAML format, which contains all the hyperparameters needed for the training session. Implements deep reinforcement learning algorithm, like PPO to  process the observations, decide on actions to take, and send them back to the Unity environment.</li>
   </ul></li>
  
  


## Disk class documentation
To solve the constraints put on the disk, the Disk class is used to simulate a bouncing disk in a field where the disk conserves the energy but not the direction when it bounces off the walls and conserves both when it bounces off the paddle.

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

 


- `BounceOffWall()`: Computes the new outgoing direction using the Vector3.reflect function that reflects a vector of the plane defined by a normal -- in this case, the normal of the collision. Afterward, a random value between the range of [-5 to 5] degreesù is applied to the outgoing direction  using the 3d rotation matrix and normalized. To calculate the new direction, the 3D matrix rotation angle:
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

 
   
- `OnTriggerEnter()`:It detects when the disk is out of bounds. In case of an out-of-bound event then the environment will reset (see PaddleAi class for more detail).

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
This class implements the agent's behavior  using  the Unity ML-Agents libraries (Sensors, Actuators). Positive or negative rewards are added for the paddle to learn to keep the disk inside the field.  In this section, all the reward values are fine-tuned after multiple training instances and the ones that make the agents more reactive and responsive to the disk's movement.
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

  


- `CollectObservation()`: The Ml-Agents method collects the observation vectors of the agent for the step. This function describes the current environment from the agent's perspective for achieving the goal.  In this case, the agent needs to keep track of its current z position (due to how the paddle is placed in the game field in the unity scene), the current x and z position, and the velocity component of the disk. This information allows the agent to observe and attempt to hit the disk to keep it inisde the game field. It is important for the system and the training to  know how many elements the agent is tracking, as it needs to match the dimension of the Vector Observation of the Behavior Parameters. In this particular case, the agent is observing five floats.

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
  

- `OnActionRecived()`: The Ml-Agents method is used to specify an agent's behavior at every step, based on the provided action passed through the ActionBuffer parameter, which specifies how many actions are needed to control the agent. In this particular case, since the only movement the paddle needs to perform is along a single axis -- the z-axis (due to how the paddle is positioned in the game field) -- it only requires one continuous action with a single element in the array.
Continuous action was selected to provide the paddle with a more natural movement and for simplicity, as using the discrete action would have had 3 parameters: one to move up, one to move down, and one to stand still).
It also sets the reward that incentivizes the agent to align its z coordinates position to the disk's z-coordinate position, enabling it to keep up with the disk.



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

 

- `Update()`: This method returns a negative reward to the paddle agent whenever the disk goes out of bounds, ending the current episode and start  a new one. 

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


This file contains all the hyperparameter information that are used to create a training session for the agent 
The behavior name in this file needs to be the same as the behavior chosen in the behavior parameter script.
Mlagenst has 2 different types of learning algorithms:  PPO (proximal Policy Optimization) and Soft Actor-Critic (SAC).  In this scenario, the PPO algorithm is used since is  more general-purpose and stable than many other reinforcement algorithms.

<ul>PPO (NEEDS TO BE UPDATED THIS IS NOT FINAL)
  <li>Proximal Policy Optimization (PPO) algorithm deep reinforcement learning algorithm that provides a more stable and efficient approach to policy optimization to balance exploration with exploitation. 
    <ul>Key Features
    <li>Probability ratio: r_t(θ) = π_θ(a_t | s_t) / π_θ_old(a_t | s_t)
        It denotes the probability ratio between the current and the old policy. If this value > 1 then the action is more likely based on .the current policy than the old policy. Vice versa if the value is between 0 and 1</li>
   
<li>Advantage Function, Generalized Advantage Estimation (GAE): Â_t = ∑_{l=0}^{∞} (γλ)^l * δ_{t+l} where δ_t = r_t + γ * V(s_{t+1}) - V(s_t)
  it estimates an action is better or worse than the average return in a given state.</li>
 <li>Clipped Objective Funciton:L^{CLIP}(θ) = E_t[min(r_t(θ) * A_t, clip(r_t(θ), 1 - ε, 1 + ε) * A_t)]
 This objective function restricts the change of the policy during training. This helps to mantains the balance between improving the policy and not deviating too far from the old one. </li>
    </ul>
 </li><ul>
    
<ul>Hyperparametrs
<li>Batch size: the number of experiences used for one iteration of a gradient descend update. It should be a fraction of the buffer size.
Buffer size defines the number of experiences (agent observation, action, and reward obtained) before doing any learning or model updating.
</li>
<li>Learing rate: corresponds to the strength of each gradient descent update step. </li>
  <li>Beta: is the strength of the entropy regularization to make the policy more random for the agent to explore the action space during training. </li>
  <li> Epsilon: is the threshold of divergence between the old and the new calculated policy during the gradient descent updating</li> </li>
  <li>Lambda: the lambda used to calculate the Generalized Advantage Estimation. This is how much the agent relies on its current value estimate when calculating an updated value estimate. Low value the agent relies on its current value estimate, making him more biased. On the other hand, if it's a high value it relies more on the actual reward received in the environment. This parameter helps us to balance high bias and exploration. </li>
  <li>Number of epochs: is the number of passes through the experience bugger during gradient descent.</li>
  <li>learning_rate_schedule: defines how the learning rate changes during training. It will gradually decrease over time. In this case, the learning rate will decrease linearly from the initial value to the final value over a specified number of setups.</li
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




