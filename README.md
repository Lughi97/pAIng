
# PAIng 

 A 3D Pong game where both paddles are controlled by an AI agent trained from scratch using Deep Reinforcement Learning — watch it play against itself.

Developed as a solo project for the **AI for Video Games** course at the **University of Milan**.

---

## 🎮 About the Project

pAIng is a Unity-based Pong prototype built to train and deploy a **reinforcement learning agent** using **Unity ML-Agents** and the **Proximal Policy Optimization (PPO)** algorithm. The agent learns entirely through trial and error — no hard-coded rules, no manual scripting of movement logic. Once trained, the same neural network model is applied to both paddles, making them play against each other autonomously.

The project was developed as a university assignment to demonstrate a complete ML training pipeline: environment setup, observation design, reward shaping, training configuration, and model deployment.

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Engine | Unity 2022.3.10f1 (LTS) |
| Language | C# |
| ML Framework | Unity ML-Agents 3.0.0 (release 21) |
| RL Algorithm | Proximal Policy Optimization (PPO) |
| Training Runtime | Python 3.10.12, PyTorch 1.13.1 (CUDA 11.7) |
| Training Monitoring | TensorBoard |
| Model Format | ONNX (`.onnx`) |

---

## Architecture Highlights

### Reinforcement Learning Agent — `PaddleAgent.cs`
The paddle is implemented as a **Unity ML-Agents `Agent`** subclass. The training loop is built around three core overrides:

**Observations** (`CollectObservations`) — 6 values fed to the neural network each step:
- Paddle Z position
- Disk X and Z positions
- Disk X and Z velocity
- Current paddle speed

**Actions** (`OnActionReceived`) — 2 continuous outputs:
- `moveZ`: direction and magnitude of paddle movement along the Z axis
- `speed`: clamped between `minSpeed (5)` and `maxSpeed (25)` m/s

**Reward Shaping** — designed to encourage effective play:
| Condition | Reward |
|---|---|
| Paddle close to disk (Z distance ≤ threshold) | +0.1 |
| Paddle speed close to disk speed | +0.5 |
| Paddle hits the disk | +1.0 |
| Disk goes out of bounds | -1.0 (episode ends) |

---

### Disk Physics — `Disk.cs`
The disk maintains a **constant speed of 15 m/s** using `Rigidbody.velocity` directly — no Unity physics forces — to avoid acceleration or deceleration. Gravity, drag, and rotation are all frozen on the Rigidbody.

Wall bounces apply `Vector3.Reflect` plus a **random deflection of ±5°** using a 3D rotation matrix:

```
outgoingDirection = [cos(θ)  0  -sin(θ)] [x]
                    [  0     1     0   ] [y]
                    [sin(θ)  0   cos(θ)] [z]
```

Paddle bounces reflect direction without the random deflection, preserving predictability for the agent.

---

### Training Environment
Two Unity scenes were used:

| Scene | Purpose |
|---|---|
| `TrainScene` | 64 simultaneous game instances — one paddle + one wall each side — for parallelised training |
| `Paing` | Single game field with two trained paddles playing against each other |

Training both left and right paddle positions simultaneously gave the agent a fuller understanding of disk dynamics. The full training session ran for **~26 minutes** across **4,000,000 steps**.

---

### PPO Algorithm
The agent is trained using **Proximal Policy Optimization (PPO)**, a policy gradient method that uses a **Clipped Surrogate Objective** to prevent large destabilising policy updates:

```
L_CLIP(θ) = E[min(r_t(θ) * Â_t, clip(r_t(θ), 1-ε, 1+ε) * Â_t)]
```

Advantage estimation uses **Generalised Advantage Estimation (GAE)**:

```
A_GAE(γ,λ) = Σ (γλ)^l * δ^V_{t+l}
```

The combined objective optimises policy, value function, and entropy bonus simultaneously:

```
L(θ) = L_CLIP(θ) - c1 * L_VF(θ) + c2 * S[π_θ](s_t)
```

Key training hyperparameters were tuned through monitoring TensorBoard graphs — mean cumulative reward, episode length, policy loss, value loss, entropy, and epsilon decay.

---

### TensorBoard Training Results [To see the graph result -> PDF documentation of this project ](https://github.com/Lughi97/pAIng/blob/main/PAingDocumentation.pdf)
Training converged successfully over 4M steps:
- **Mean Cumulative Reward**: steady upward trend showing the agent learning effective strategies
- **Episode Length**: increased over time, meaning the agent kept the disk in play longer each episode
- **Value Loss**: stabilised around 3M steps, indicating the agent's value estimates became reliable
- **Entropy**: decreased over training, showing the agent exploiting learned strategies rather than exploring randomly
- **Learning Rate**: linear decay schedule from max to min at 4M steps

---

## Running the Project

To run this project visit my [portfolio website](https://lughi97.github.io/LucaPortfolio/) in which in the PAing section you can run the simulation of this project.

---

## References
- [PDF documentation of this project ](https://github.com/Lughi97/pAIng/blob/main/PAingDocumentation.pdf)
- [Unity ML-Agents Documentation](https://docs.unity3d.com/Packages/com.unity.ml-agents@3.0/manual/index.html)
- [PPO Paper — Schulman et al. 2017](https://arxiv.org/abs/1707.06347)
- [GAE Paper — Schulman et al. 2015](https://arxiv.org/abs/1506.02438)
- [TensorBoard with ML-Agents](https://unity-technologies.github.io/ml-agents/Using-Tensorboard/)
