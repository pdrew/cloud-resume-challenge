#!/bin/bash

stackname=$1

if [ -z "$stackname" ]; then
	echo "Must enter a stackname"
	exit 1
fi

echo "Deleting stack ${stackname}..."

aws cloudformation delete-stack --stack-name ${stackname}

while true; do
    stackstatus=$(aws cloudformation describe-stacks --stack-name ${stackname} --query "Stacks[0].StackStatus" --output text 2>/dev/null)
    if [ $? -ne 0 ]; then
        echo "Stack ${stackname} has been successfully deleted"
        break
    elif [ "$stackstatus" = "DELETE_FAILED" ]; then
        echo "Stack ${stackname} deletion failed"
        exit 1
    else
        echo "Waiting for stack ${stackname} to be deleted... (current status: ${stackstatus})"
        sleep 10
    fi
done
