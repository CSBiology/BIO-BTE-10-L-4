(**
---
title: Automated analysis of bioreactor growth curves
category: projects
categoryindex: 2
index: 1
---
*)


(**
# Automated analysis of bioreactor growth curves


#### Table of contents

- [Introduction](#Introduction) 
    - [Input](#Input)
    - [Output](#Output)
- [Coding clues](#Coding-clues)
- [References](#References)
- [Additional information](#Additional-information)


## Introduction

  - The bioreactor in the department of Biotechnology and Systems Biology contains eight individual growth cells that contain the green algae _Chlamydomonas reinhardtii_. Every minute a measurement of the 
  optical density is taken (680 and 750 nm). The reactor operates in turbidostatic mode, indicating that the cells density should be kept constant. If the cells grow, the optical density increases and the 
  reactor has to dilute the growth medium to restore a constant OD750. Please note, that for photosynthetic active organisms you should use OD750, because OD650 lies within the chlorophyll autofluorescence and therefore
  may be biased.

  - The manual process of analysing the data is quite cumbersome. The exported data set constists of (i) time, (ii) measured OD, and (iii) the activity of the dilution pump. To analyse the growth, the slope of the growth
  phase OD is required. Therefore, the data is loaded in MS-Excel, the growth phases are manually selected and individually fitted with a straight regression line. The slope of these lines are then used to get an 
  estimate of the average growth rate (inverse slope).

  
![](../img/odData_1.png)
_Fig. 1: Schematic view of the data. The saw tooth signals has to be separated into distinct growth-phases. The slope of these lines can be used to estimate the growth rate. The growth changes during the experiment as the slope is lower in the center three growth phases (e.g. the light was turned off and the cells grow slower)._


- The aim of this project is to automate this process. A possible solution to this problem is the following:
  - input: CSV, TXT, or TSV file in which column 1 and 2 contains time data, column 3 the OD, and column 4 the pump data
  - output: Plotly figure that contains all necessary information to (i) perform a visual quality check of the analysis performace, and (2) further determine growth.


 
### Input:


  |time(h)|time(absolute)|OD750|pumping volumen|misc|
  |-------|--------------|-----|---------------|----|
  |0.00000|2023/10/23 12:45:25|0.657|0.0|...|
  |0.01667|2023/10/23 12:46:25|0.660|0.0|...|
  |0.03333|2023/10/23 12:47:25|0.656|0.0|...|
  |0.05000|2023/10/23 12:48:25|0.650|0.0|...|
  |0.06667|2023/10/23 12:49:25|0.661|0.0|...|
  |0.08333|2023/10/23 12:50:25|0.657|0.0|...|
  |0.10000|2023/10/23 12:51:25|0.658|0.0|...|
  |0.11667|2023/10/23 12:52:25|0.662|0.0|...|
  |0.13333|2023/10/23 12:53:25|0.661|0.1|...|
  
  
### Output:

![](../img/odData_2.png)
_Fig. 2: A possible visual solution_



## Coding clues

  - start by developing in a jupyter notebook or fsx script. The automatic analysis can be done afterwards by creating a skript, that takes a predefined data set that is stored e.g. at the same location as the script.
  The output plot can be stored in the same folder with the name containing the current date. In the end you can create a file named "analysis.cmd" and that just contains "dotnet fsi analysisScriptName.fsx". By double clicking
  this file, you start the complete analysis workflow without the need to open any script or do any programming at all.
  
  - Import the data and dissect it by using the pumping volume data. Whenever this data is constant you can start collecting OD data for the current growth phase. When the pump starts diluting again, stop collecting data for the 
  current growth data

  - There are various options to fit a line to the data:

    - General usage: https://fslab.org/FSharp.Stats/Fitting.html
  
    - (i) Use simple linear regression where the squared distances from the fit to the original data points (residuals) are minimized (OLS regression). 
    The drawback is, that outlier values have a huge impact to the fitting line. `LinearRegression.fit(testDataX, testDataY, FittingMethod = Method.SimpleLinear)`

    - (ii) You can use outlier insensitive slope determination methods (e.g. Theil-Sen estimator). 
    The drawback is, that outlier values have a huge impact to the fitting line. `LinearRegression.fit(testDataX, testDataY, FittingMethod = Method.Robust RobustEstimator.TheilSen)`

    - (iii) Use simple linear regression but remove outliers after the initial fit and fit again ignoring the outliers. 
    e.g. use [cooks distance](https://fslab.org/FSharp.Stats/GoodnessOfFit.html#Cook-s-distance) for outlier detection

  - Use Plotly.NET for visualization
    - For the BoxPlot you can use `Chart.BoxPlot(Y = [2.;3.4], Name = "slopes",Jitter=0.2,BoxPoints=StyleParam.BoxPoints.All,BoxMean=StyleParam.BoxMean.True)` to additionally view all data points and the data mean.


The following plotting example contains much of the functionality you'll need, but not necessarily in the correct position. I would suggest for each fitting strategy (OLS, robust, ...) to generate an individual output plot.


*)

(******)

#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: Plotly.NET, 4.2.0"
            
open FSharp.Stats
open Plotly.NET
open FSharp.Stats.Fitting

let xs = vector [|1. .. 10.|]
let ys = vector [|4.;10.;9.;7.;13.;17.;16.;23.;15.;30.|]

let fitA = Fitting.LinearRegression.fit(xs,ys,FittingMethod=Fitting.Method.SimpleLinear)
let fitB = Fitting.LinearRegression.fit(xs,ys,FittingMethod=Fitting.Method.Robust RobustEstimator.TheilSen)



let description = 
    // some styling for a html table
    let style = "<style>table {font-family: arial, sans-serif;border-collapse: collapse;width: 75%;}td, th {border: 1px solid #dddddd;text-align: left;padding: 8px;}tr:nth-child(even) {background-color: #dddddd;}</style>"
    
    // header row of the table
    let header = "<tr><th>Phase ID</th><th>Time span start (h)</th><th>Time span end (h)</th><th>Slope (t<sup>-1</sup></th></tr>"
    
    // table rows
    let rows = 
        [fitA;fitB]
        |> List.mapi (fun i x -> 
            // create a table row with phase id, the start and end of the treatment formatted as a float with one significant figure (defined by %.1f) and the slope with four significant figures.
            // the slope is stored within the fit coefficients as [intersect;slope]
            $"<tr><td>{i + 1}</td><td>%.1f{Seq.head xs}</td><td>%.1f{Seq.last xs}</td><td>%.4f{x.Coefficients.[1]}</td></tr>"
        )

    // constructed table
    let table = $"{style}<table>{header}{rows}</table>"

    // convert the table string to a giraffe node element to be compatible with Plotly.NET
    Giraffe.ViewEngine.HtmlElements.rawText table



let chart = 
    [
        [
            Chart.Point(x=xs,y=ys,Name="raw data",MarkerColor=Color.fromHex "#1e1e1e")
            Chart.Line(x=[1.;10.],y=[fitA.Predict 1.;fitA.Predict 10.;],Name="OLS fit",LineColor=Color.fromHex "#1f77b4")
        ]
        |> Chart.combine

        [
            Chart.Point(x=xs,y=ys,Name="raw data",MarkerColor=Color.fromHex "#1e1e1e")
            Chart.Line(x=[1.;10.],y=[fitB.Predict 1.;fitB.Predict 10.;],Name="TheilSen fit",LineColor=Color.fromHex "#ff7f0e")
        ]
        |> Chart.combine

    ]
    |> Chart.Grid(nRows=2,nCols=1)
    |> Chart.withTemplate ChartTemplates.lightMirrored
    |> Chart.withConfig (
        Config.init (ToImageButtonOptions = ConfigObjects.ToImageButtonOptions.init(Format = StyleParam.ImageFormat.SVG))
        )
    |> Chart.withDescription [description]

chart
|> GenericChart.toChartHTML
(***include-it-raw***)

(**

![](../img/odData_3.png)

_Fig. 3: Example of Plotly.NET combined graphs_


## References

  - [Plotly.NET documentation](https://plotly.net/)

  - [FSharp.Stats documentation](https://github.com/fslaborg/FSharp.Stats)


## Additional information

  - None

*)


