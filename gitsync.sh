#!/bin/sh

if [ "$1" == "" ]; then
	echo "Provide a commit message, please";
	exit 1;
fi

git add --all
git commit -m "$1"
git push
git pull
