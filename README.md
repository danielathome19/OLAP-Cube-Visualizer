# About
This repository contains multiple variants of this project; the primary algorithm allows for data cube operations to be performed using lightweight functions with ease of usability to provide speed, efficiency, and assist in reducing data oversimplification as found in other OLAP cube viewers.
To find out more, check out the provided research papers:
  * "Optimizing Data Cube Visualization for Web Applications.pdf" (DOI: [10.48550/arxiv.2101.00171](https://doi.org/10.48550/arxiv.2101.00171))
  * "Visualization Techniques with Data Cubes.pdf" (DOI: [10.48550/arxiv.2101.00170](https://doi.org/10.48550/arxiv.2101.00170))

# Usage
The folders "csharpviewer", "cshoster", "golangviewer", and "interfaceviewer" contain unfinished projects; these are detailed in the paper "Optimizing Data Cube Visualization for Web Applications".

Working examples can be found in "csplotter*" and "pythonviewer".
The "csplotter" program has 3 variants:
  * csplotter
  * csplotter-timed (the same program but the functions display the runtime)
  * csplotter-timed-concurrent (the same program but the functions utilize concurrency and display the runtime)
In each of these three, "Program.cs" contains the executable code to be compiled.

The "pythonviewer" folder contains 3 variants as well:
  * "hoster.py", an unfinished Flask script meant to serve each OLAP cube function through HTTP requests to "interfaceviewer"
  * "plotter.py", the primary example used to compare between the Python and C# variants
  * "plotter-timed.py", the same as "plotter.py" but the functions display the runtime

The "main" function in both the C# and python variants will search for a file titled "data.csv" by default, but this my be modified to allow the user to enter any file name. Or for testing purposes, simply rename your dataset to "data.csv".

# Bugs/Features
Bugs are tracked using the GitHub Issue Tracker.

Please use the issue tracker for the following purpose:
  * To raise a bug request; do include specific details and label it appropriately.
  * To suggest any improvements in existing features.
  * To suggest new features or structures or applications.

# License
The code is licensed under Apache License 2.0.

# Citation
If you use this code for your research, please cite this project as either (**Optimizing Data Cube Visualization for Web Applications**):
```
@software{Szelogowski_OLAP-Cube-Visualizer_2020,
 author = {Szelogowski, Daniel},
 doi = {10.48550/arxiv.2101.00171},
 month = {Dec},
 title = {{OLAP-Cube-Visualizer}},
 license = {Apache-2.0},
 url = {https://github.com/danielathome19/OLAP-Cube-Visualizer},
 version = {1.0.0},
 year = {2020}
}
```
or (**Visualization Techniques with Data Cubes**):
```
@software{Szelogowski_OLAP-Cube-Visualizer_2020,
 author = {Szelogowski, Daniel},
 doi = {10.48550/arxiv.2101.00170},
 month = {Nov},
 title = {{OLAP-Cube-Visualizer}},
 license = {Apache-2.0},
 url = {https://github.com/danielathome19/OLAP-Cube-Visualizer},
 version = {1.0.0},
 year = {2020}
}
```
