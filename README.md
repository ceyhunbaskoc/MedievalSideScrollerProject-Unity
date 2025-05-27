# 🌙 Nightclaw (Still Developing) 
[![Unity Version](https://img.shields.io/badge/Unity-6+-black.svg?logo=unity)]
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://img.shields.io/badge/License-MIT-yellow.svg)]

A pixel art style, side-scrolling kingdom management game. Build, defend, and expand your village using only one resource: **money**!  

## 🎮 About the Game  
In **Mediavel Side Scroller Project**, your goal is to survive nightly enemy attacks while strategically expanding your village. You can only use **money** to build, upgrade, and manage your kingdom. 

### 🌟 Key Features  
- **Unique Economy System:** Manage your kingdom using only money.  
- **Dynamic Building Mechanic:** Throw money onto objects to unlock new structures and upgrades.  
- **Day and Night Cycle:**  
  - **Day:** Earn money through hunters, builders, and traders.  
  - **Night:** Defend your village against enemy raids.  
- **Village Expansion:** Upgrade your main building to increase buildable areas.  
- **Command and Leadership:** Recruit commanders and artisans to empower your archers and builders.  
- **Procedural Events:** Waves of enemies and resource opportunities that grow in number and variety every night.  

---

## 📸 Screenshots  

(The game is not finished yet, this is a demo version!!!):**

**In-game footage (2x Accelerated) :**

![1](Assets/GameplayVideos/video1.gif)          ![2](Assets/GameplayVideos/video2.gif)

![2](Assets/GameplayVideos/video2.gif)

![3](Assets/GameplayVideos/video3.gif)

![4](Assets/GameplayVideos/video4.gif)


---

## 🛠️ Gameplay Mechanics  

### 💰 Money Sources  
1. **Archers:** Earn money by killing animals during the day.  
2. **Builders:** Complete construction projects to receive rewards.  
3. **Traders:** Return every morning with money after being sent out at night.  

### 🏰 Village Structures  
- **Main Building:** Expand your kingdom and unlock new build areas.  
- **Archer's Hut:** Store bows for new archers.  
- **Builder's Workshop:** Store tools for new builders.  
- **Market:** The place where the trader rests and upgrades are made.  
- **Defense Structures:**  
  - **Walls:** Basic protection against enemies.  
  - **Archer Towers:** High ground for archers to defend the village.  
  - **Traps:** Deal damage to enemies approaching the village.  
- **Special Buildings:**  
  - **Commander's Headquarters:** Boosts archer skills.  
  - **Artisan's Guild:** Increases builder efficiency.  

---

## 🔄 How to Play  
1. Start your journey with a small village and limited money.  
2. Use the **money throw mechanic** to interact with objects (The player will learn these mechanics by playing!!!):  
   - Throw money at stones to build **archer towers**.  
   - Throw money at pile of dirt to construct **walls**.  
3. Strategically recruit villagers to become **archers** or **builders**.  
4. Expand your kingdom by upgrading the main building.  
5. Defend against nightly enemy waves and gather enough resources to thrive.  

---

## 🗺️ Village Management  
- **Balance Economy and Defense:** Manage your funds wisely to maintain a strong defense while growing your village.  
- **Recruit and Equip:** Use your money to recruit villagers and equip them with bows or tools.  
- **Build Strategically:** Choose where to place towers and walls based on enemy attack patterns.

---

## 🛠️ Building Construction Logic
- **Our main character arrives at an object that can be turned into a building (stone, mound of earth, bush, etc.) and fills the money slots.**
- **The building enters the construction process and the nearest builders rush to the construction site.**
- **The more builders there are in a construction site, the faster the construction will be finished.**
- **When the building is completed, a sum of money is deducted according to the value of the building and the builder receives this money.**
- **Builder gives it to us when he is next to our character.**

