# Shortlist – Smart Location Decision Platform

Shortlist is a web application that helps users evaluate locations based on proximity to essential amenities such as grocery stores, transit stops, parks, gyms, safety services, and more.
The system combines **live OpenStreetMap data**, **custom scoring logic**, and **user-defined priorities** to help users make smarter location decisions.
Users can search an area, analyze nearby amenities, compare locations, and save searches for future reference.

---

# Project Overview

Shortlist is designed as a **decision support platform** rather than a simple map search tool.

The application provides:
• Smart scoring based on proximity and density of amenities  
• Interactive map visualization  
• Saved searches with shareable links  
• Location comparison tools  
• Personalized search preferences  

This project demonstrates real-world development concepts including:
- ASP.NET MVC architecture
- Entity Framework Core
- REST APIs
- Session state management
- Map integrations
- Data-driven scoring algorithms
- Responsive UI design

---

# Key Features

## Live Location Analysis
Users can search a location and radius to analyze nearby amenities using OpenStreetMap data.

The system evaluates categories such as:
- Grocery stores
- Public transit
- Parks
- Fitness facilities
- Parking
- Laundry
- Safety services
- Quiet spaces

Each category receives a **match score** based on density, proximity, and presence of anchor locations.

---

## Interactive Map
The application includes an interactive map powered by **Leaflet.js**.

Features include:
- Live map markers
- Radius visualization
- Location pin
- Popup details
- Automatic zoom and bounds fitting

---

## Smart Scoring Algorithm
Each category is scored using three metrics:

| Metric | Weight |
|------|------|
| Density (number of places nearby) | 50% |
| Anchor locations (major facilities) | 30% |
| Distance accessibility | 20% |

The system calculates:
- Category score
- Overall match score
- Strengths and weaknesses
- Confidence level based on available data

---

## Compare Locations
Users can add locations to a **Compare list**.
The compare feature allows users to evaluate multiple locations side-by-side to determine which area best matches their needs.

---

## Saved Searches
Users can save search configurations.

Each saved search stores:
- Radius
- Priorities
- Location coordinates
- Search name
- Creation timestamp

Saved searches allow users to:
- Reload filters instantly
- Share searches using a unique link
- Regenerate share tokens
- Delete searches

---

## Settings and Preferences
The Settings page allows users to configure default preferences:
- Default search radius
- Default priorities
- Default location label

Additional controls include:
- Clear session filters
- Clear comparison list
- Delete saved searches
- Logout

---

## Shareable Search Links
Saved searches generate unique tokens that can be shared.
Anyone with the link can open the search and see the same filters applied.

---

# Technology Stack

## Backend
- ASP.NET Core MVC
- Entity Framework Core
- SQL Server / SQLite (depending on configuration)
- REST API endpoints

## Frontend
- Razor Views
- HTML5
- CSS3
- JavaScript

## Map and Data
- Leaflet.js
- OpenStreetMap
- Overpass API

---

# Project Architecture

The project follows the **MVC (Model-View-Controller)** architecture.


Controllers
Handles user requests, business logic, and API calls.

Models
Represents application data such as users, saved searches, and filter states.

Views
Razor pages that render the user interface.

Data
Entity Framework DbContext used for database operations.

Helpers
Utility classes for session and JSON operations.


---

# Core Components

## AppDbContext

Entity Framework database context responsible for managing:
- Users
- Saved Searches
- User Settings

Includes relationships and indexes for performance.

---

## SessionJsonExtensions

Helper class that allows storing complex objects in session using JSON serialization.

Example usage:
HttpContext.Session.SetJson("FilterState", filterState);

This enables storing filter preferences between pages.

---

# Database Schema

## Users

| Column | Description |
|------|------|
| Id | Primary key |
| Email | User email |
| Password | Local login password |

---

## SavedSearches

| Column | Description |
|------|------|
| Id | Primary key |
| Name | Search name |
| UserId | Owner |
| ShareToken | Unique share identifier |
| CreatedAtUtc | Creation timestamp |

---

## UserSettings

| Column | Description |
|------|------|
| UserId | Foreign key |
| DefaultRadiusKm | Default search radius |
| DefaultPrioritiesCsv | Stored priority categories |
| DefaultLocationLabel | Optional location label |

---

# Application Workflow

1. User selects location and radius in Filters page
2. Results page retrieves nearby places using OpenStreetMap API
3. Scoring algorithm evaluates each category
4. Results displayed with map and analytics
5. User can save the search or compare locations
6. Settings allow customization of default preferences

---

# API Endpoints

## Search API
GET /api/search


Parameters:
lat
lng
radiusKm

Returns nearby amenities within the selected radius.

---

## Compare API

POST /Compare/Add
GET /Compare/Count


Handles compare list operations.

---

## Saved Searches API

POST /SavedSearches/SaveCurrent
POST /SavedSearches/Delete
POST /SavedSearches/RegenerateShareLink


Handles saved search management.

---

# UI Design

The interface is designed using a modern card-based layout with:
- consistent spacing
- subtle shadows
- soft color palette
- responsive layout

The UI emphasizes clarity and usability.

---

# Security Considerations

- Authorization enforced for user-specific actions
- Anti-forgery tokens used for form submissions
- Session data used for temporary state management
- Share tokens generated securely for saved searches

---

# Installation

Clone the repository:


git clone https://github.com/yourusername/shortlist.git

Navigate to project directory:
cd shortlist

Restore dependencies:
dotnet restore

Run the application:
dotnet run


Open in browser:
https://localhost:5001


---

# Future Improvements

Potential enhancements include:

- advanced location scoring models
- machine learning recommendations
- mobile app integration
- user authentication with OAuth providers
- additional amenity categories
- improved comparison visualizations

---

# Learning Outcomes

This project demonstrates skills in:

- ASP.NET MVC development
- API integration
- geospatial data handling
- UI/UX design
- database modeling
- full-stack application development

---

# License

This project is intended for educational and portfolio use.
