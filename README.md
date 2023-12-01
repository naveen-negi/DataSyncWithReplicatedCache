# Project Overview: Parking Facility Management System

## Introduction

Our project is dedicated to developing a system for efficiently managing parking facilities in a town. This system streamlines the handling of parking sessions, encompassing the initiation and ending of sessions, calculating parking fees, and processing payments.

## Workflow Overview

- **Parking Session Initiation:**
    - A vehicle owner enters a parking facility and starts a parking session.
    - The Session Management Service records the session details like the spot number and entry time.

- **Parking Session Termination:**
    - The session ends when the vehicle exits the facility.
    - The Pricing Service calculates the parking fee based on the total duration of the parking.

- **Payment Processing:**
    - The vehicle owner is charged upon session termination.

## Initial Implementation Strategy

We are starting with a straightforward approach:
- **Synchronous Communication:** Using RESTful API calls for inter-service communication.
- **Atomic Consistency:** Ensuring transactions are processed as complete units.
- **Choreography-based Coordination:** Allowing services to interact without centralized control.

## Core Services

The backend is composed of three main services:

1. **Session Management Service:**
    - Manages the start and end of parking sessions.
    - Tracks the specific details of each vehicle's parking.

2. **Pricing Service:**
    - Calculates parking fees based on session duration and spot location.

3. **Payment Service:**
    - Handles transactions after parking sessions.
    - Supports various payment methods including credit cards and digital wallets.

## Features of the First Version

- **Communication:** Synchronous, for real-time service interactions.
- **Consistency:** Atomic, to ensure transaction integrity.
- **Coordination:** Choreography-based, promoting decentralized service interactions.

## Prerequisites
- Docker and Docker Compose installed on your machine.

## Steps to Run

1. **Clone the Repository:**
    - Ensure you have the project repository cloned or downloaded on your machine.

2. **Navigate to the Project Directory:**
    - Change directory to the root of the project where the `docker-compose.yml` file is located.

3. **Run Docker Compose:**
    - Execute the following command to start the services defined in the Docker Compose file:
      ```
      docker-compose up
      ```

## Services Configuration

The `docker-compose.yml` file defines the following services:

- **Sessions API (`sessions-api`):**
    - Built from the Dockerfile in `src/Sessions/Sessions.API`.
    - Accessible on port `5056`.

- **Product Pricing API (`productpricing-api`):**
    - Built from the Dockerfile in `src/ProductPricing/ProductPricing.API`.
    - Accessible on port `5055`.

- **Payments API (`payments-api`):**
    - Built from the Dockerfile in `src/Payments/Payments.API`.
    - Accessible on port `9011`.

- **Database Services:**
    - Three PostgreSQL instances for `sessions-db`, `payments-db`, and `productpricing-db`.
    - Accessible on ports `5436`, `5435`, and `5437` respectively.

- **Jaeger Tracing (`jaeger`):**
    - Used for tracing and monitoring.
    - UI accessible on port `16686`.

## Environment Configuration

Each service is configured with specific environment variables for database connections and inter-service communication. These are set in the `environment` section of each service in the `docker-compose.yml` file.


Run the following command:

```docker-compose up
```

---
Starting a Successful Session
Start another session with a different user ID using the following endpoint:

POST http://localhost:5056/api/sessions

Request body:
```json 
{
"locationId": "1234",
"UserId": "1"
}
```

Response body:
```json 
{
    "status": "Started",
    "sessionId": "some-unique-session-id",
    "userId": "1",
    "locationId": "1234",
    "startDate": "2023-12-01T15:45:00.000Z",
    "endDate": null
}
```
Stopping the Successful Session
Stop the session using the following endpoint:

POST http://localhost:5056/api/sessions/some-unique-session-id/end

No request body required.

Expected Response
```json 
{
    "status": "Ended",
    "sessionId": "some-unique-session-id",
    "userId": "1",
    "locationId": "1234",
    "startDate": "2023-12-01T15:45:00.000Z",
    "endDate": "2023-12-01T16:45:00.000Z"
}
```

---
Starting a Failed Session (Rollback)
Start a session with a user ID that is not in the database using the following endpoint:

POST http://localhost:5056/api/sessions
Request body:
```json 
{
    "locationId": "1234",
    "UserId": "9999"
}
```
Response body:
```json 
{
    "status": "Started",
    "sessionId": "ae5166b6-c8cc-4821-aef0-52a8014d59d6",
    "userId": "9999",
    "locationId": "1234",
    "startDate": "2023-12-01T15:57:13.1573133Z",
    "endDate": null
}
```

Stopping the Failed Session (Rollback)
Stop the session using the following endpoint:
POST http://localhost:5056/api/sessions/ae5166b6-c8cc-4821-aef0-52a8014d59d6/end

Response: 500 Internal Server Error
```json 
{
    "message": "An error occurred while processing your request. Please try again."
}
``` 





