(**
---
title: t-Distributed Stochastic Neighbour Embedding
category: projects
categoryindex: 2
index: 1
---
*)


(**
# t-Distributed Stochastic Neighbour Embedding (tSNE)

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

  - tSNE is a dimensionality reduction method. It allows you to visualise a multi-dimensional dataset in 2 or 3 dimensional scatter plot. 
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
Like principal component analysis (PCA), tSNE is a method for dimensionality reduction. It aggregates all features to a feature subset that 
allows a visual inspection of the complex data. It often is applied in image processing, NLP, genomic data, and speech processing. 
  
  
  ![](../img/tSNE.png)
  Fig. 1: Idea of tSNE. Visualisation of a high dimensional data on a 2-dimensional scatter plot. 
  
## Aim for this project

1. Blog post introducing the method, its applications, and limitations.

2. Implement t-Distributed Stochastic Neighbour Embedding in FSharp.Stats.

  
## Coding clues

### Notes:

  - All functions below are taken from the original publication (van der Maaten and Hinton 2008).

  - Be aware, that the original work first describes SNE and later (section 3) describes the differences made to result in t-SNE!

  - Although variance is continually referred to as σ<sub>i</sub> in the paper, that is a repeated typo and should be σ<sub>i</sub><sup>2</sup>.

  - The data matrix has n rows (without header row). The first index defines the row, the second the column!

  - x<sub>i</sub> defines the i<sup>th</sup> row in the data matrix (a vector of measured features).

  - ||x|| indicates the vector norm, in this case it is the Euclidean distance between vector x<sub>i</sub> and y<sub>i</sub>. You can find distance metrics at ```FSharp.Stats.ML.DistanceMetrics```.

  - exp(t) indicates e<sup>t</sup>

  - A t distribution with degree of freedom = 1 is equal to 0.3183*(1+t²)-1 where the first constant part can be neglected if the constant term exists in all calculations.

### Pseudocode:

![](../img/tSNE_pc.png)

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
            
open FSharp.Stats
open Plotly.NET


let fromFileWithSep (separator:char) (filePath) =     
    // The function is implemented using a sequence expression
    seq {   let sr = System.IO.File.OpenText(filePath)
            while not sr.EndOfStream do 
                let line = sr.ReadLine() 
                let words = line.Split separator//[|',';' ';'\t'|] 
                yield words }

                
let lables,data =
    fromFileWithSep ',' (__SOURCE_DIRECTORY__ + "../content/irisData.csv")
    |> Seq.skip 1
    |> Seq.map (fun arr -> arr.[4], [| float arr.[0]; float arr.[1]; float arr.[2]; float arr.[3]; |])
    |> Seq.toArray
    |> Array.shuffleFisherYates
    |> Array.mapi (fun i (lable,data) -> sprintf "%s_%i" lable i, data)
    |> Array.unzip


(**

#### 2<sup>nd</sup> step:

  - Calculate a Euclidean distance matrix using ```ML.DistanceMetrics.euclidean```. The matrix’ dimensions are n x n.
  
  - Define functions that calculate similarity measures using the prior defined distance matrix:
    
    - (1) high dimensional affinity p (p<sub>i|j</sub>)(Equation 1)

      - Inform yourself how the variance is determined. If required define a Perplexity beforehand.

    - (2) low dimensional affinity q (q<sub>ij</sub>) (Equation 4)


#### 3<sup>rd</sup> step: 

  - Calculate the high dimensional affinity matrix between every data pair.

    - Note: p<sub>ij</sub> ≠ p<sub>i|j</sub>

    - p<sub>ij</sub> = (p<sub>j|i</sub> + p<sub>i|j</sub>) / 2n

  - The matrix has the dimensions n x n . The similarity of a point to itself is 0.


#### 4<sup>th</sup> step: 

  - Create an initial solution y(0) so that:

    - y(0) is a matrix (n x d)

    - y(0) contains as many rows as the original data matrix has rows (n)

    - The number of values in each row is the number of dimensions you want to obtain in the end (d; in most cases 1-3, but should be defined by user).

    - Each value is a randomly sampled from a normal distribution with mean = 0 and var = 0.0001.

*)

(******)

// defines a normal distribuiton with mean = 3 and stDev = 2
let normalDist = Distributions.Continuous.normal 3. 2.

let createInitialGuess n = Array.init n (fun x -> normalDist.Sample())

// see FSharp.Stats documentation for probability distributions in the first code block for details
// https://fslab.org/FSharp.Stats/Distributions.html#Normal-distribution)


(**


#### 5<sup>th</sup> step:

  - Recursively loop from t=1 to T (number of iterations)


  - calculate low dimensional affinities (q<sub>ij</sub> (Equation 4)) for all low dimensional result vectors from 3<sup>rd</sup> step. Collect results in a matrix (n x n).

  - compute gradient (Equation 5)

  - calculate the updated result y(t) and repeat.

#### 6<sup>th</sup> step:

  - report y(T) as final result

#### 7<sup>th</sup> step:
  
  - Use a 2D and 3D scatter plot from Plotly.NET to visualize your result.

#### 8<sup>th</sup> step: Function implementation in F#

  - create a function, that contains all necessary helper functions in its body and takes the following parameters (suggestion):
  
  |Parameter name|data type|description|
  |--------------|---------|-----------|
  |data|```matrix```|datamatrix (cols=features, rows=elements)|
  |dimensions|```int```|number of dimensions the final output data points have|
  |maxIter|```int```|maximal number of iterations|
  |perplexity|```float```|inform yourself if the perplexity should be defined by the user, or is calculated within the algorithm|
  |learnRate|```float```|inform yourself|
  |momentum|```float```|inform yourself|

  - Default parameters should be given in the function description or as optional paramter.


## References

  - van der Maaten & Hinton; Visualizing Data using t-SNE (2008) [PDF](https://lvdmaaten.github.io/publications/papers/JMLR_2008.pdf )

  - https://www.youtube.com/watch?v=NEaUSP4YerM 

  - https://cran.r-project.org/web/packages/Rtsne/Rtsne.pdf page 5

  - https://www.analyticsvidhya.com/blog/2017/01/t-sne-implementation-r-python/

    - Note: Inform yourself if the variance in Step 4 is in fact based on t distribution or if at this point of the algorithm a standard gaussian normal distribution is used!

  - https://www.datacamp.com/community/tutorials/introduction-t-sne


## Additional information

### Testing

  - Apply tSNE to a dataset of your choice.

  - optional: Test your results against implementations in R/Python or in the best case against the datasets proposed in the original publication.

### Blog post 

  - Don’t forget to describe the limits/weaknesses of the approach in your blog post.

  - How to handle/preprocess ties?

  - optional: Compare the method to PCA.

*)


