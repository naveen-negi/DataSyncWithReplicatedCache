#!/bin/bash

# Exit when any command fails
set -e

COMMAND=$1

start_apis() {
    echo "Starting all APIs..."

    dotnet run --project src/Sessions/Session.API &
    pid1=$!
    echo "Started Session.API with PID $pid1"

    dotnet run --project src/Payments/Payments.API &
    pid2=$!
    echo "Started Payments.API with PID $pid2"
    
    dotnet run --project src/ProductPricing/ProductPricing.API &
    pid3=$!
    echo "Started ProductPricing.API with PID $pid3"
    
    # Wait for all processes and capture exit codes
    wait $pid1
    exit1=$?

    wait $pid2
    exit2=$?

    wait $pid3
    exit3=$?

    # Check if any process exited with a non-zero status
    if [ $exit1 -ne 0 ] || [ $exit2 -ne 0 ] || [ $exit3 -ne 0 ]; then
        echo "An error occurred while starting the APIs"
        exit 1
    fi
}

kill_dotnet_processes() {
    echo "Killing all dotnet processes..."
    ps -ef | grep 'dotnet' | grep -v 'grep' | awk '{print $2}' | xargs kill
    echo "All dotnet processes killed."
}

case "$COMMAND" in
    start-apis)
        start_apis
        ;;
    kill-dotnet)
        kill_dotnet_processes
        ;;
    test)
        echo "Running tests..."
        # Add commands to run tests here
        ;;
    setup-db)
        echo "Setting up databases..."
        # Add commands to setup databases here
        ;;
    *)
        echo "Usage: ./go [command]"
        echo "Commands:"
        echo "  start-apis    Start all APIs"
        echo "  kill-dotnet   Kill all dotnet processes"
        echo "  test          Run tests"
        echo "  setup-db      Setup databases"
        ;;
esac
