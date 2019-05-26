# BreastCancerDiagnosis
## Structure
This solution is a combination of the two C# libaries ImagePreprocessing and DimensionReduction, which performs relevant imageprocessing/loading and reduces dimensions of these images using Principal Component Analysis
Furthermore there is two C# runnables, namely Training and WebApp. Also a modified version of LibSvmSharp is included in this git.

## How to run Training
To perform a training sequence, the following considerations needs to be made in the configuration file, found in the root of the solution.

.. list whatever consideration to be made

## How to run WebApp
To run the web application, one have to ensure the configurated path of pca model location and svm model location, relative to the WebApp.csproj, does exists, and are trained for the configuration currently in the configuration file. If this is not the case, the classification algorithm will result in an error.
If those files are properly configured, then run the WebApp by entering the folder WebApp in a terminal, and run "dotnet run". Then navigate your favorite browser to the URL: <ip/dns of server>:5000, or localhost:5000, if hosted locally. The used browser must have JavaScript enabled, and support HTML5.
