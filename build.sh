#! /bin/bash

# Build & Test BackEnd

dotnet build
dotnet test

# Bundle Lambdas
dotnet tool install -g Amazon.Lambda.Tools
dotnet lambda package --project-location ./BackEnd/src/BackEnd.Api --output-package ./BackEnd/dist/backend-function.zip
dotnet lambda package --project-location ./Helpers/CodeSigner/src --output-package ./Helpers/CodeSigner/dist/codesigner-function.zip
dotnet lambda package --project-location ./Helpers/SlackNotifier/src --output-package ./Helpers/SlackNotifier/dist/slacknotifier-function.zip
dotnet lambda package --project-location ./Helpers/ViewsAggregator/src --output-package ./Helpers/ViewsAggregator/dist/viewsaggregator-function.zip

# Build FrontEnd

pushd ./FrontEnd/src

npm ci
npm run build
touch ./out/.nojekyll

if [ -d ../dist ]; then
	rm -rf ../dist
fi

mkdir ../dist
cp -r ./out/* ../dist

popd
