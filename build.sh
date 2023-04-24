#! /bin/bash

# Build & Test BackEnd

dotnet build
dotnet test

# Bundle Lambdas

dotnet lambda package --project-location ./BackEnd/src --output-package ./BackEnd/dist/backend-function.zip
dotnet lambda package --project-location ./CodeSigner/src --output-package ./CodeSigner/dist/codesigner-function.zip

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
