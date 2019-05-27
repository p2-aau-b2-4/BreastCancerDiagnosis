# BreastCancerDiagnosis
## Structure
This solution is a combination of the two C# libaries ImagePreprocessing and DimensionReduction, which performs relevant imageprocessing/loading and reduces dimensions of these images using Principal Component Analysis.

Furthermore there are two C# runnables, namely Training and WebApp. Also a modified version of LibSvmSharp is included in this git.

## How to run Training
To perform a training sequence, the following considerations needs to be made in the configuration file, found in the root of the solution.

Please note, that the training algorithm, depending on configuration and hardware, can be a while.

### Configuration
```sizeImageToAnalyze``` Specifies the quadratic size of the normalized image of the breast tumour to train the algorithm on.

```<name>CsvPath``` Specifies the location on the local filesystem, where to look for the datasets CSV-files. All four of these lines needs to point to proper csv files, located in the same folder, as the image folders of the dataset.

```ShouldCreateImages``` This tells the algorithm whether to load images from the datasets csv files, or to load the prepared image, from a previous run of the training algorithm. If this is 0, then make sure the following files exists and are valid before training:

```TrainReadyImage``` and ```TestReadyImage``` Specifies the path to save/load the already cropped and normalized images. Must be existing files, if ```ShouldCreateImages``` is configured to 0.

The following paths is the configuration of where to save/load files from:
```PcaModelLocation``` The trained PCA-model
```TestSetLocation``` The constructed SVM-test problem
```TrainSetLocation``` The constructed SVM-train problem
```ModelLocation```  The trained SVM-model

The configuration
```ShouldCreateSVMSetsWithPca``` should be 1 if one would like the PCA model to be loaded/trained, and generate the test and train problems from the normalized images. Should be 0, if the existing SVM-test and SVM-train problems should be used.

```nFold``` can be changed to specify the n-Fold-Crossvalidation used in SVM.

## How to run WebApp
To run the web application, one have to ensure the configurated path of pca model location and svm model location, relative to the WebApp.csproj, does exists, and are trained for the configuration currently in the configuration file. If this is not the case, the classification algorithm will result in an error.

If those files are properly configured, then run the WebApp by entering the folder WebApp in a terminal, and run ```dotnet run```. Then navigate your favorite browser to the URL: ```<ip/dns of server>:5000```, or ```localhost:5000```, if hosted locally. The used browser must have JavaScript enabled, and support HTML5.
