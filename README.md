# 🚀 Shortlist — Smart Rental Decision Platform

> AI-powered rental decision assistant that helps users choose the best living area based on real-world proximity data, lifestyle priorities, and intelligent insights.

---


## 🧠 Project Overview

Shortlist is a modern rental decision platform that goes beyond listings.
Instead of just showing properties, it answers:
👉 *“Is this area suitable for my lifestyle?”*

Users define preferences, and the system analyzes real-world location data to generate:

- Smart recommendations  
- Area scores  
- AI insights  
- Visual analytics  

---

## 🎯 Problem Statement

Traditional rental platforms:

| Issue | Description |
|------|------------|
| ❌ Listing-focused | Only show properties |
| ❌ No lifestyle filtering | Ignore user priorities |
| ❌ No area analysis | No proximity insights |
| ❌ No intelligence | No scoring system |

---

## 💡 Solution

Shortlist introduces:

| Feature | Benefit |
|--------|--------|
| 🎯 Priority-based filtering | Personalized results |
| 🗺️ Map-based input | Accurate location targeting |
| 📊 Scoring system | Data-driven decisions |
| 🤖 AI recommendations | Human-like insights |
| 📈 Analytics dashboard | Clear understanding |

---

## ✨ Features

### 🔍 Smart Filters
- Budget selection
- Max distance
- Radius-based search
- Up to 3 priorities

**Available priorities:**
- Grocery  
- Transit  
- Gym/Fitness  
- Parks  
- Parking  
- Laundry  

---

### 🗺️ Real-Time Data (OpenStreetMap)
POST https://overpass-api.de/api/interpreter


Fetches nearby:
- Grocery stores
- Transit stops
- Gyms
- Parks
- Parking
- Laundry

---

### 📊 Results Dashboard

- Overall score
- Strengths & weaknesses
- Confidence level
- Live nearby data

---

### 🤖 AI Recommendation (Premium)

- Natural language explanations
- Personalized insights
- Powered by Ollama

---

### 🔥 Heatmap View (Premium)

- Visual density mapping
- Highlights best areas

---

### 💬 AI Assistant

- Available on all pages (except settings)
- Answers user questions
- Context-aware

---

### 🧾 AI Report

- Printable report
- Clean layout
- Includes insights & scores

---

### ⚙️ Premium System

| Feature | Free | Premium |
|--------|------|--------|
| Basic results | ✅ | ✅ |
| AI summary | ❌ | ✅ |
| Heatmap | ❌ | ✅ |
| AI report | ❌ | ✅ |

---

## 🧱 Architecture
User → Filters Page → Session Storage
↓
Results Controller
↓
Search API (Overpass)
↓
Data Processing & Scoring
↓
UI Rendering
↓
AI Layer (Optional)


---

## ⚙️ Tech Stack

### Backend
- ASP.NET Core MVC (.NET 8)
- C#
- Session State

### Frontend
- Razor Views
- HTML/CSS
- JavaScript
- Leaflet.js

### APIs
- OpenStreetMap (Overpass API)
- Ollama (AI)

---

## 🗂️ Data & State Design

### FilterState Model

```csharp
public class FilterState
{
    public decimal? Budget { get; set; }
    public int? MaxDistanceKm { get; set; }
    public List<string> Priorities { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
    public int RadiusKm { get; set; }
}
```

### 🔄 Application Flow
User selects filters
Filters saved to session
Results page loads
API call executed
Data processed
UI rendered

### 🧠 AI Integration
var response = await _httpClient.PostAsync(
    "http://localhost:11434/api/chat", content);

AI uses:

- Area name
- Score
- Priorities
- Match count
- Distance

- 
### 💬 Assistant Widget
fetch("/api/assistant/ask", {
    method: "POST",
    body: JSON.stringify({ message })
});

### Features:

- Floating UI
- Quick prompts
- Context-aware replies

- ### 📊 Scoring System

Factors:

Factor	Description
Density	Number of nearby places
Distance	Proximity
Anchors	Important facilities
Accessibility	Ease of access
Example Formula
Score = (Density * 0.4) + (Distance * 0.3) + (Anchors * 0.3)

### 🎨 UI/UX Design

- Glassmorphism design
- Smooth animations
- Clean layout
- Light/Dark mode support


### 🧪 Testing Strategy

- Manual testing (different locations)
- Edge cases (no results, large radius)
- API validation


### ⚡ Performance

- Radius limited (1–25 km)
- API timeout handling
- Efficient sorting
- Minimal re-rendering


### 🔒 Security

- No sensitive data stored
- Session-based state
- Input validation


## 👨‍💻 About

This project demonstrates:

- Full-stack development
- AI integration
- Real-world problem solving
-Data-driven decision making

⭐ Why This Project Stands Out
- Not a CRUD app
- AI-powered
- Real-world use case
- Advanced logic
- Strong UI/UX

## 🏁 Final Thought

Shortlist transforms rental searching from guesswork → intelligent decision making.
