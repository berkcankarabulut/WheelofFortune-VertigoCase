<h1>🎰 Wheel of Fortune - Unity Gambling Game</h1>

<p>A modern, risk-based wheel spinning game built with <b>Unity 2021 LTS</b> featuring zone progression, safe mechanics, and exciting rewards.</p>

https://github.com/user-attachments/assets/a5deebc1-d3ef-4583-9ffb-34c5426c6e08

<img width="1301" height="976" alt="43 (1)" src="https://github.com/user-attachments/assets/9fdb09b3-6ee9-473c-86d9-555497068afc" />
<img width="1309" height="982" alt="43-death (1)" src="https://github.com/user-attachments/assets/25f7b92d-332f-4a80-91f4-101a5962eb75" />

<hr/>

<h2>🎮 Game Overview</h2>
<p><b>Wheel of Fortune</b> is an adrenaline-pumping gambling game where players spin wheels to collect rewards while risking everything on the dreaded bomb.<br/>
Test your luck and strategy as you progress through increasingly challenging zones with better rewards.</p>

<hr/>

<h2>🎯 Core Mechanics</h2>
<ul>
<li>🎡 <b>Wheel Spinning</b>: Spin the wheel to collect various rewards</li>
<li>💣 <b>Bomb Risk</b>: Every spin risks hitting the bomb and losing all progress</li>
<li>🛡️ <b>Safe Zones</b>: Every 5th zone offers risk-free silver spins</li>
<li>⭐ <b>Super Zones</b>: Every 30th zone provides golden spins with special rewards</li>
<li>🚪 <b>Strategic Exit</b>: Leave safely at any non-spinning moment in safe/super zones</li>
<li>📈 <b>Progressive Rewards</b>: Rewards improve with each zone advancement</li>
</ul>

<hr/>

<h2>✨ Key Features</h2>

<h3>🏗️ Technical Excellence</h3>
<ul>
<li><b>SOLID Architecture</b>: Built with OOP principles and design patterns</li>
<li><b>UniRx Integration</b>: Reactive programming for smooth event handling</li>
<li><b>DOTween Animations</b>: Polished visual transitions and effects</li>
<li><b>Zenject DI</b>: Dependency injection for maintainable code</li>
<li><b>ScriptableObject System</b>: Data-driven configuration for wheels and rewards</li>
</ul>

<h3>🎨 Visual Design</h3>
<ul>
<li>Responsive UI: Supports <b>20:9, 16:9, and 4:3</b> aspect ratios</li>
<li><b>TextMeshPro</b>: Crisp, professional text rendering</li>
<li><b>Addressable Sprite Atlas</b>: Optimized texture streaming and loading</li>
<li>Smooth animations for wheel spins and reward collection</li>
</ul>

<hr/>

<h2>🛠️ Technical Stack</h2>
<ul>
<li><b>Engine</b>: Unity 2021 LTS</li>
<li><b>Programming</b>: C# with SOLID principles</li>
<li><b>Reactive Programming</b>: UniRx</li>
<li><b>Animation</b>: DOTween</li>
<li><b>Dependency Injection</b>: Zenject</li>
<li><b>UI Framework</b>: Unity UI + TextMeshPro</li>
<li><b>Data Management</b>: ScriptableObjects</li>
<li><b>Asset Management</b>: Unity Addressables</li>
</ul>

<hr/>

<h2>📱 Platform Support</h2>
<ul>
<li><b>Android</b>: Optimized APK builds</li>
<li><b>Multiple Aspect Ratios</b>: 20:9, 16:9, 4:3</li>
<li><b>Performance</b>: 60 FPS target with optimizations</li>
</ul>

<hr/>

<h2>🎲 Game Rules</h2>

<h3>Zone Types</h3>
<ul>
<li>🥉 <b>Bronze Zones (Regular)</b>: Standard spins with bomb risk</li>
<li>🥈 <b>Silver Zones (Every 5th)</b>: Safe spins without bombs</li>
<li>🥇 <b>Golden Zones (Every 30th)</b>: Premium rewards without bombs</li>
</ul>

<h3>Risk vs Reward</h3>
<ul>
<li><b>Continue</b>: Spin again for better rewards but risk losing everything</li>
<li><b>Cash Out</b>: Leave safely with current rewards (only in safe/super zones)</li>
<li><b>Bomb Hit</b>: Lose all progress and restart</li>
</ul>

<h3>Progression System</h3>
<ul>
<li><b>Zone Advancement</b>: Each successful spin advances one zone</li>
<li><b>Multiplier Growth</b>: Rewards increase with progression</li>
<li><b>Strategic Decision</b>: Balance greed vs safety</li>
</ul>

<hr/>

<h2>🗂️ Project Structure</h2>
<pre>
📦 Assets/_Project/Scripts
├── Config/                  # Game configuration
├── Core                     # DI, Storage       
├── Data/                    # Reward, Wheel
├── Event/                   # Event system (UniRx)
├── Interfaces/              
├── Runtime/                 # Game, Manager, Player, Wheel, Zone
├── Service/                 
├── UI/                      # Currency, Fail, General, Zone, Storage UI
└── Utils/                   # Utilities & Addressable loaders
</pre>

<hr/>

<h2>🚀 Setup & Installation</h2>
<h3>Prerequisites</h3>
<ul>
<li>Unity <b>2021.3 LTS</b> or higher</li>
<li>Android SDK (for mobile builds)</li>
<li>Addressables Package (via Package Manager)</li>
</ul>

<h3>Dependencies</h3>
<ul>
<li>UniRx</li>
<li>DOTween</li>
<li>Zenject</li>
<li>TextMeshPro</li>
<li>Addressables</li>
</ul>

<h3>Build Instructions</h3>
<ol>
<li>Clone the repository</li>
<li>Open in Unity 2021 LTS</li>
<li>Import required packages via Package Manager</li>
<li>Configure Addressables groups for sprite atlases</li>
<li>Build Addressables content</li>
<li>Build for Android platform (APK generated in Releases)</li>
</ol>

<hr/>

<h2>🎯 Code Quality Features</h2>
<ul>
<li><b>Dependency Injection</b> (Zenject)</li>
<li><b>Event-Driven Design</b> (UniRx)</li>
<li><b>Strategy / Observer / Factory Patterns</b></li>
<li><b>Clean Code with SOLID Principles</b></li>
<li><b>Performance Optimized</b> (Pooling, Addressables, Draw Call reduction)</li>
</ul>

<hr/>

<h2>📊 Performance Optimizations</h2>
<ul>
<li>Addressables + Sprite Atlas streaming</li>
<li>Object Pooling for particles & wheels</li>
<li>Efficient UniRx event subscriptions</li>
<li>Optimized UI hierarchy with canvas batching</li>
</ul>

<hr/>

<h2>🎨 UI/UX Excellence</h2>
<ul>
<li><b>Responsive Design</b> with safe area support</li>
<li><b>Smooth Animations</b> (DOTween)</li>
<li><b>Dynamic Theming</b> (zone-based visuals)</li>
<li><b>High-quality assets, effects, and haptics</b></li>
</ul>

<hr/> 

<h2>📄 License</h2>
<p>This project is developed as a <b>technical demonstration</b> showcasing Unity development skills, architecture patterns, and mobile game optimization techniques.</p>

<hr/> 

<hr/>

<p>Built with ❤️ using Unity 2021 LTS, UniRx, DOTween, Zenject, and modern game development practices.</p>
