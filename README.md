Shortlist
Smart Rental Search & Neighborhood Discovery Platform

Shortlist is a web-based housing discovery platform designed to help users find ideal rental areas by combining location-based filtering, amenity proximity analysis, and real-time geographic data.

Instead of listing individual apartments, the platform helps users identify the best neighborhoods based on personal priorities such as transit access, grocery stores, parks, gyms, and other daily amenities.

The system integrates OpenStreetMap data through the Overpass API, allowing real-time discovery of nearby points of interest and enabling users to evaluate locations based on real-world accessibility.

Key Features
Authentication System

Shortlist supports both traditional and modern authentication methods.

Email / Password Authentication

Users can create accounts and securely log in using stored credentials.

Google OAuth Authentication

Users may also authenticate using their Google account through OpenID Connect integration, providing a secure and seamless login experience.

Authentication uses:

ASP.NET Core Cookie Authentication

Google OpenID Connect

Session-based user tracking

Interactive Map-Based Filtering

The platform includes an interactive map that allows users to define their search area.

Users can:

Select a location directly on the map

Set a search radius

Visualize nearby amenities

Map functionality allows the user to define their geographic area of interest instead of relying on fixed neighborhoods.

Smart Filter System

Users can refine searches using multiple filters:

Budget Filter

Define a preferred rental budget.

Distance Filter

Specify how far amenities should be located from the chosen area.

Priority Amenities

Users can prioritize nearby amenities such as:

Parking

Gym / Fitness

Parks

Grocery Stores

Transit

The platform retrieves these points using OpenStreetMap real-time geographic data.

Real-Time Results Engine

When filters are applied, the platform queries the Overpass API to retrieve nearby amenities.

Results are calculated dynamically and displayed based on:

Distance from the selected location

Category relevance

User priorities

This ensures that results reflect live geographic data rather than static datasets.

Results Visualization

Results are displayed through:

Interactive Map Markers

All discovered amenities are visualized directly on the map.

Category Breakdown

Amenities are grouped by category, allowing users to easily see what is available nearby.

Accessibility Insight

Users can quickly understand how accessible a location is based on available services and facilities.

Saved Searches

Users can save filter configurations for future use.

Each saved search stores:

Selected location

Radius

Budget

Priority amenities

Saved searches allow users to quickly reload previously explored configurations and compare different neighborhoods efficiently.

Features

Save current search state

Automatically generate search names

Reload filters and results instantly

Delete saved searches

Saved search data is stored in SQLite using Entity Framework Core.

Session-Based User State

The application uses session storage to temporarily store filter selections and location data.

This enables:

Smooth page transitions

Persistent filter states

Fast result reloading

System Architecture

The platform follows a Model-View-Controller (MVC) architecture.

Controllers

Handle application logic and request routing.

Examples:

FiltersController

ResultsController

SavedSearchesController

AccountController

Models

Represent application data structures.

Examples:

UserProfile

FilterState

SavedSearch

ResultItem

Views

Razor views responsible for UI rendering.

Examples:

Filters page

Results page

Saved searches page

Login / Registration pages

Database

The application uses SQLite as the persistence layer.

Entity Framework Core manages the database schema and data access.

Stored Data

Users
Saved searches
Session-based filter states

Technologies Used
Backend

ASP.NET Core MVC
C#
Entity Framework Core
SQLite

Authentication

ASP.NET Cookie Authentication
Google OpenID Connect (OAuth 2.0)

Frontend

Razor Views
HTML5
CSS3
JavaScript

Data Sources

OpenStreetMap
Overpass API

Current Pages
Home

Landing page providing entry into the platform.

Filters

Allows users to define location and search preferences.

Results

Displays discovered amenities and accessibility insights based on applied filters.

Saved Searches

Allows users to save, reload, and manage previously defined search configurations.

Login / Register

User authentication pages supporting both manual login and Google OAuth.

Project Goals

This project demonstrates:

Full-stack web development

Secure authentication systems

Real-time geographic data integration

User-centric search interfaces

Persistent user data storage

The platform is designed as a practical decision-support tool for people searching for rental-friendly neighborhoods.