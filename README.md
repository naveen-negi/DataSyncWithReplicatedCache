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

- ** SessionOrchestrator:**
  - Orchestrator executes the whole flow for session start and stop.

## Initial Implementation Strategy

We are starting with a straightforward approach:
- **Synchronous Communication:** Using RESTful API calls for inter-service communication.
- **Eventual Consistency:** api calls immediately return and offload task to a background task/event.
- **Orchestration-based Coordination:** Centralized control for whole flow.

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

- **session Orchestration API (`orchestrator-api`):**
    - Built from the Dockerfile in `src/SessionOrchestrator`.
    - Accessible on port `9012`.

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

```
docker-compose up
```

---
### Starting a Successful Session
``` bash
./go start-valid-session
# above command will return a sessionId
```
### Stopping the Successful Session
Stop the session using the following endpoint:

``` bash
./go stop-session {sessionId}
# SessionId is received in start-valid-session command
```



