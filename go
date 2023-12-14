#!/bin/bash

# Exit when any command fails
set -euo pipefail
trap "echo 'error: Script failed: see failed command above'" ERR

COMMAND=$1

start_valid_session() {
    echo "Starting a new session with a valid UserId..."
    curl --location 'http://localhost:5056/api/sessions' \
    --header 'Content-Type: application/json' \
    --data '{
        "locationId": "1234",
        "UserId": "1"
    }'
    echo "Session started."
}


start_invalid_session() {
    echo "Starting a new session with invalid userId..."
    curl --location 'http://localhost:5056/api/sessions' \
    --header 'Content-Type: application/json' \
    --data '{
        "locationId": "1234",
        "UserId": "9999"
    }'
    echo "Session started."
}

stop_session() {
    local sessionId=${1:-"default_session_id"}
    if [ -z "$sessionId" ] || [ "$sessionId" = "default_session_id" ]; then
        echo "Error: No session ID provided."
        return 1
    fi
    
    echo "Stopping session with ID ${sessionId}..."
    local response=$(curl --location --request POST "http://localhost:9012/api/sessions/${sessionId}/processPayment" \
    --header 'Content-Type: application/json')
    echo "Response from server:"
    echo "$response"
}

jaeger() {
    local url="http://localhost:16686/search"
    open "$url"
}

case "$COMMAND" in
    start-valid-session)
        start_valid_session
        ;;
    start-invalid-session)
        start_invalid_session
        ;;
    stop-session)
        stop_session $2
        ;;
    open-jaeger)
        jaeger
        ;;
    *)
        echo "Usage: ./go [command]"
        echo "Commands:"
        echo "  start-valid-session    Start a session that can be Priced and Payed"
        echo "  stop-session    Stop a session"
        echo "  start-invalid-session    Start a session that can NOT be Priced and Payed"
        echo "  jaeger    Open jaeger in browser"
        ;;
esac
