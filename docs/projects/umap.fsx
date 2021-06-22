(**
---
title: Uniform Manifold Approximation and Projection 
category: projects
categoryindex: 1
index: 6
---
*)


(**
# Uniform Manifold Approximation and Projection (UMAP)

**Interested?** Contact [muehlhaus@bio.uni-kl.de](mailto:muehlhaus@bio.uni-kl.de) or [venn@bio.uni-kl.de](mailto:venn@bio.uni-kl.de)

#### Table of contents

- [Introduction](#Introduction) 
- [Aim for this project](#Aim-for-this-project)
- [Coding clues](#Coding-clues)
    - [Notes](#Notes)
    - [Pseudocode](#Pseudocode)
    - [Step 0](#0-sup-th-sup-step)
    - [Step 1](#1-sup-st-sup-step)
    - [Step 2](#2-sup-nd-sup-step)
    - [Step 3](#3-sup-rd-sup-step)
    - [Step 4](#4-sup-th-sup-step)
    - [Step 5](#5-sup-th-sup-step)
    - [Step 6](#6-sup-th-sup-step)
    - [Step 7](#7-sup-th-sup-step)
    - [Step 8 - Function implementation in F#](#8-sup-th-sup-step-Function-implementation-in-F)
- [References](#References)
- [Additional information](#Additional-information)
    - [Testing](#Testing)
    - [Blog post](#Blog-post)

## Introduction

  - UMAP is a dimensionality reduction method. It allows you to visualise a multi-dimensional dataset in 2 or 3 dimensional scatter plot. 
But what does that mean in practice? Imagine you measured height, weight, width, density, brightness, as well as magnetic, chemical, 
and physical properties of a bunch of objects. The simplest technique to summarize your measurements is a spreadsheet table in which each 
row represents an element, and each column represents a measured feature:


  |Object ID|height|weight|width|density|brightness|magnetic field|...|
  |---------|------|------|-----|-------|----------|--------------|---|
  |objectA|2|30|3|2|200|100000|...|
  |objectB|4|50|2|3|255|130000|...|
  |objectC|15|20|1|2|11|10000000|...|
  |...|...|...|...|...|...|...|...|
  

  - Note that the measured features span multiple orders of magnitude. A change of 1 in height for example has much more value than a change 
of 1 regarding the magnetic field. If now clusters of similar behaving objects should be identified, you are limited to inspect the data set 
column-wise by repetitive sorting. Just from the table you cannot create a meaningful graph, that allows you to perform a visual inspection of all features at once. 
Like principal component analysis (PCA), UMAP is a method for dimensionality reduction. It aggregates all features to a feature subset that 
allows a visual inspection of the complex data. It often is applied in image processing, image characterization and genomic data. 
  
  
  ![](../img/umap.png)
  Fig. 1: Idea of UMAP. Visualisation of a high dimensional data on a 2-dimensional scatter plot. 
  
## Aim for this project

1. Blog post introducing the method, its applications, and limitations.

2. Implement UMAP in FSharp.Stats.

  
## Coding clues

### Notes:

  - All 

### Pseudocode:

![](../img/umap_pc1.png)

![](../img/umap_pc2.png)

#### 0<sup>th</sup> step: 

  - Read the publication and visit further introduction material you can find below (References)
  
#### 1<sup>st</sup> step: 

  - create a F# script (.fsx), load and open ```FSharp.Stats```, ```FSharpAux```, and ```Plotly.NET```

  - import test data

    - You can find the classic clustering dataset "iris" [here](https://github.com/fslaborg/FSharp.Stats/tree/developer/docs/data).

*)

(******)

#r "nuget: FSharp.Stats, 0.4.1"
#r "nuget: Plotly.NET, 2.0.0-beta9"
    
open System.IO
open FSharp.Stats
open Plotly.NET

let fromFile filePath =     
    System.IO.File.ReadAllLines(filePath)
    |> Array.map (fun x -> Array.map float (x.Split ','))

//2D iris data from: FSharp.Stats
let iris    = fromFile (Directory.GetParent(__SOURCE_DIRECTORY__).FullName + "/content/iris.txt")
//3D mammoth data from: https://github.com/PAIR-code/understanding-umap/tree/master/raw_data
let mammoth = fromFile (Directory.GetParent(__SOURCE_DIRECTORY__).FullName + "/content/mammoth.txt")

let mammothChart = 
    let dataCoordinates = mammoth |> Array.map (fun x -> x.[0],x.[1],x.[2])
    Chart.Scatter3d(dataCoordinates,StyleParam.Mode.Markers)
    |> Chart.withMarkerStyle(Size=1)
    |> Chart.withTitle "Mammoth 3D representation for UMAP testing"

(***hide***)
mammothChart |> GenericChart.toChartHTML
(***include-it-raw***)

(**

#### 2<sup>nd</sup> step:

  - construct a weighted k-neighbour graph
  


#### 8<sup>th</sup> step: Function implementation in F#

  - create a function, that contains all necessary helper functions in its body and takes the following parameters (suggestion):
  
  |Parameter name|data type|description|
  |--------------|---------|-----------|
  |data|```matrix```|datamatrix (cols=features, rows=elements)|
  |dimensions|```int```|number of dimensions the final output data points have|
  |maxIter|```int```|maximal number of iterations|
  |minDist|```float```|inform yourself|
  |numNeighbours|```int```|inform yourself|

  - Default parameters should be given in the function description or as optional paramter.


## References

  - McInnes, L., Healy, J. & Melville, J. UMAP: Uniform manifold approximation and projection for dimension reduction. Stat. Mach. Learn. arXiv preprint arXiv:1802.03426 (2018).
  
    - Especially chapter 4

  - https://github.com/lmcinnes/umap

  - https://pair-code.github.io/understanding-umap/

  - https://www.r-bloggers.com/2019/06/running-umap-for-data-visualisation-in-r/

  - C# implementation with static minDist of 0.1: https://github.com/curiosity-ai/umap-sharp


## Additional information

### Testing

  - Apply UMAP to a dataset of your choice.

  - optional: Test your results against implementations in R/Python or in the best case against the datasets proposed in the original publication.

### Blog post 

  - What are the prerequisites of the data?
  
  - Don’t forget to describe the limits/weaknesses of the approach in your blog post.

  - How to handle/preprocess ties?

  - optional: Compare the method to PCA and tSNE.

*)


