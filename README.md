🚀 Shortlist – Smart Rental Decision Platform

A modern, AI-powered rental decision assistant that helps users find the best living areas based on real-world proximity data, personalized priorities, and intelligent insights.

🧠 Project Idea

Shortlist is not just another property search tool.

Instead of showing listings blindly, it answers:

👉 “Is this area actually good for MY lifestyle?”

Users select:

Budget
Travel distance
Lifestyle priorities (Gym, Parks, Grocery, etc.)
Location (via map)

And Shortlist intelligently evaluates nearby infrastructure using real-world OpenStreetMap data to give:

✔ Best areas
✔ Score-based insights
✔ AI explanations (Premium)
✔ Visual analytics

🎯 Problem We Solve

Most rental platforms:

Show listings ❌
Don’t evaluate area quality ❌
Ignore lifestyle fit ❌

Shortlist focuses on:

✅ Lifestyle-first decision making
✅ Data-driven area analysis
✅ Real-time proximity scoring
✅ AI-powered recommendations

🧩 Core Features
🔍 1. Smart Filters System
Select budget & max distance
Choose up to 3 priorities
Interactive map pin selection
Radius-based search (1–25 km)

👉 Fully session-based persistence
👉 State survives navigation

🗺️ 2. Real-Time Location Intelligence
Uses OpenStreetMap + Overpass API
Fetches nearby:
Grocery stores
Transit stops
Gyms
Parks
Parking
Laundry

👉 Live data → No static datasets

📊 3. Intelligent Scoring Engine

Each area is scored using:

📍 Density (number of matches)
📍 Distance (median, P80)
📍 Anchors (important facilities)
📍 Accessibility

Outputs:

Overall score
Strengths & weaknesses
Confidence level
📈 4. Analytics Dashboard
Match insights
Area strengths
Weakness breakdown
Live snapshot of nearby places
🤖 5. AI-Powered Recommendations (Premium)
Generates natural language explanations
Uses local AI models (Ollama)
Explains:
Why area is good
Trade-offs
Lifestyle fit
🔥 6. Heatmap Mode (Premium)
Visualize best areas dynamically
Shows density-based performance
💬 7. AI Assistant (Global Feature)
Available on all pages (except settings)
Answers:
How filters work
What scores mean
How to use features

👉 Context-aware assistant
👉 Lightweight API-based responses

💾 8. Save & Compare
Save searches
Compare different areas
Track decisions
🧾 9. AI Report Generation
Generate printable report
Clean UI optimized for PDF
Includes:
Score breakdown
Insights
AI summary
🎨 10. Modern UI/UX
Glassmorphism design
Fully responsive
Smooth transitions
Light/Dark theme support 🌙
⚙️ 11. Premium System
Free trial support
Session-based premium control
Features gated:
AI reports
Heatmap
Advanced insights
❓ 12. FAQ System (Settings)
Central help page
Styled for both themes
Covers all app features
🏗️ Tech Stack
💻 Frontend
Razor Views (ASP.NET MVC)
HTML5, CSS3
Vanilla JavaScript (no heavy frameworks)
Leaflet.js (maps)
⚙️ Backend
ASP.NET Core MVC (.NET 8)
C#
Session-based state management
🌐 APIs Used
OpenStreetMap (Overpass API) → location data
Ollama (Local AI) → AI summaries
🗃️ Data Handling
JSON-based session storage
Real-time API calls (no static DB dependency)
🧠 AI Integration
Local AI (Ollama)
Prompt-based recommendation engine
🧱 Architecture Overview
User → Filters → Session State → Results Page
           ↓
     Map + API Calls (Overpass)
           ↓
    Data Processing & Scoring
           ↓
     UI Rendering (Cards + Analytics)
           ↓
    (Optional) AI Layer (Ollama)
🔄 Key Implementation Highlights
✅ Session-Based Filter Persistence
Stores full filter state as JSON
Retrieved across controllers
Prevents data loss between pages
✅ Real-Time Data Fetching
POST https://overpass-api.de/api/interpreter
Dynamic query generation
Radius-based filtering
Multiple category support
✅ Scoring Logic
Weighted scoring system
Distance normalization
Density-based ranking
✅ AI Integration
Context-aware prompts
Structured input → natural output
✅ Assistant Widget
Injected globally via _Layout.cshtml
Pure JS (no Razor dependency)
API-driven responses
🧪 Testing Approach
Manual testing via:
Different locations
Different priorities
Edge cases (no results, max radius)
API validation:
Overpass response handling
Error fallback UI
⚡ Performance Considerations
Radius clamped (1–25 km)
API timeout handling
Minimal DOM re-renders
Efficient sorting logic
🔒 Security Considerations
No sensitive data stored
Session-based user state
API request validation
📸 Screenshots (Add These in GitHub)

👉 Filters Page
👉 Results Dashboard
👉 Heatmap View
👉 AI Report
👉 Assistant Widget

🚀 Future Improvements
🔹 Database integration (user history)
🔹 Machine learning scoring model
🔹 Real estate listing integration (API)
🔹 Chat memory for assistant
🔹 User profiles & preferences
🔹 Mobile app version
👨‍💻 About This Project

Built as a portfolio-level full-stack project focusing on:

Real-world problem solving
Clean architecture
UX-focused design
AI integration
Data-driven decision making
📬 Contact / Portfolio

Add your:

LinkedIn
Portfolio
GitHub
⭐ Why This Project Stands Out

✔ Not a CRUD app
✔ Real-world utility
✔ AI integration
✔ Map + geospatial logic
✔ Strong UX + visuals
✔ Full-stack complexity

🏁 Final Thought

Shortlist transforms rental searching from guesswork → intelligent decision making.
