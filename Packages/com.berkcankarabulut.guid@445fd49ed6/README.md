<body>
  <h3>Serializable (Serializable GUID Support)</h3>
    <p>Provides serialization support for Unity's GUID system.</p>
    <ul>
        <li><code>SerializableGuid</code> structure stores GUIDs as <code>uint</code> values.</li>
        <li>Includes helper methods like <code>ToHexString()</code>, <code>ToGuid()</code>, and <code>NewGuid()</code>.</li>
    </ul>
    <pre><code>SerializableGuid sGuid = SerializableGuid.NewGuid();
string hex = sGuid.ToHexString();
Guid normalGuid = sGuid.ToGuid();
    </code></pre>

  <h2>📦 Installation</h2>
    <p>1. Clone this repository:</p>
    <pre><code>git clone https://github.com/berkcankarabulut/PackageUtilities.git</code></pre>
    <p>2. Add it to your Unity project.</p>
    <p>3. Configure the necessary settings.</p>
  <h2>📄 License</h2>
    <p>This project is licensed under the MIT License. For more details, see the <a href="LICENSE">LICENSE</a> file.</p>
</body>
</html>
