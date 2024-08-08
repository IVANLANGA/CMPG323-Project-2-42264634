#Telemetry Management System - CMPG 323 Project 2
Project Overview
The Telemetry Management System is designed to help NWU Tech Trends measure and report the time and cost savings achieved by various automation processes. Every time an automation runs, telemetry data is recorded, capturing essential information about the automation's performance. This data is then used to calculate cumulative time and cost savings, which can be filtered by project or client.

The project is implemented as a CRUD RESTful API using .NET 8, which allows easy integration and interaction with the telemetry data stored in a SQL Server database. The API supports various operations, including creating, reading, updating, and deleting telemetry records, as well as specialized endpoints for calculating savings.

Features
CRUD Operations: Manage telemetry records with standard CRUD operations.
Savings Calculation: Calculate time and cost savings based on telemetry data, filterable by project or client.
Secure API: Authentication is implemented to ensure that only authorized users can access the API.
Cloud Hosting: The API is hosted on Azure, making it accessible over the internet.
Getting Started
Prerequisites
Before you can run or use this API, ensure you have the following:

Visual Studio 2022 Community Edition with .NET 8 installed.
Access to the NWU Azure.
SQL Server set up with the necessary tables (use the efundi provided SQL script).
Installation and Setup
Clone the Repository:


git clone https://github.com/IVANLANGA/CMPG-323-Project-2.git
cd CMPG-323-Project-2
Configure the Database:

Ensure your SQL Server instance is running.
Update the appsettings.json file with your SQL Server connection string.
Run the provided SQL script to create the necessary tables in the database.
Run the API Locally:

Open the project in Visual Studio.
Build the solution to restore dependencies.
Run the project. The API should now be accessible at https://localhost:7008/swagger.
API Usage
Endpoints Overview
GET /api/telemetry: Retrieve all telemetry records.
GET /api/telemetry/{id}: Retrieve a specific telemetry record by ID.
POST /api/telemetry: Create a new telemetry record.
PATCH /api/telemetry/{id}: Update an existing telemetry record by ID.
DELETE /api/telemetry/{id}: Delete a telemetry record by ID.
GET /api/savings/project: Calculate and retrieve cumulative time and cost savings for a specific project within a date range.
GET /api/savings/client: Calculate and retrieve cumulative time and cost savings for a specific client within a date range.
Example Requests
Retrieve All Telemetry Records:


GET https://localhost:7008/api/telemetry
Response:

json
Copy code
[
  {
    "id": 1,
    "processId": 101,
    "jobId": 2001,
    "queueId": 301,
    ...
  },
  ...
]
Calculate Savings by Project:

bash
Copy code
GET https://localhost:7008/api/savings/project?projectId=123&startDate=2024-08-01&endDate=2024-08-09
Response:


{
  "totalTimeSaved": "50 hours",
  "totalCostSaved": "$5000"
}
Authentication
The API is secured, meaning you must be authenticated to access most endpoints. Ensure you have the necessary credentials and include them in your requests.