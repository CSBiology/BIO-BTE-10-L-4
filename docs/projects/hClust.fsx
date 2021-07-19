(**
---
title: Efficient agglomerative hierarchical clustering
category: projects
categoryindex: 2
index: 2
---
*)

(**
# Implementation of an efficient hierarchical agglomerative clustering algorithm

**Interested?** Contact [muehlhaus@bio.uni-kl.de](mailto:muehlhaus@bio.uni-kl.de) or [venn@bio.uni-kl.de](mailto:venn@bio.uni-kl.de)

#### Table of contents

- [Introduction](#Introduction) 
- [Aim for this project](#Aim-for-this-project)
- [Coding clues](#Coding-clues)
    - [Step 0](#0-sup-th-sup-step)
    - [Step 1](#1-sup-st-sup-step)
    - [Step 2 - generate priority queue](#2-sup-nd-sup-step-Generate-priority-queue)
    - [Step 3](#3-sup-rd-sup-step)
    - [Step 4](#4-sup-th-sup-step)
    - [Step 5](#5-sup-th-sup-step)
    - [Step 6 - Function implementation in F#](#6-sup-th-sup-step-Function-implementation-in-F)
    - [Step 7 - Further coding considerations](#7-sup-th-sup-step-Further-coding-considerations)
- [References](#References)
- [Additional information](#Additional-information)
    - [Testing](#Testing)
    - [Blog post](#Blog-post)


## Introduction

![](../img/overview.png)

Fig 1: Generating a hierarchical tree structure from a complex data set. Vertical thresholds (yellow, green, violet) generate different cluster numbers.

Clustering methods can be used to group elements of a huge data set based on their similarity. Elements sharing similar properties cluster together and can be 
reported as coherent group. These properties could be e.g. (a) similar gene expression kinetics in time series, (b) similar physicochemical properties, (c) genetic 
similarity measures for phylogenetic trees, etc.

Many clustering algorithms require a predefined cluster number, that has to be provided by the experimenter. The most common approach is _k_-means clustering, 
where _k_ stands for the user defined cluster number. This kind of user interaction can lead to results, that are not objective, but highly influenced by the 
view and expectation of the experimenter. 

Hierarchical clustering (_hClust_) does not require such cluster number definition. Instead, _hClust_ reports all possible cluster numbers 
(One big cluster with all elements to n clusters where every element is a singleton in its own cluster) in a tree structure (Fig 1). 
A _hClust_ tree has a single cluster (node) on its root and recursively splits up into clusters of elements that are more similar to each other than 
to elements of other clusters. For generating multiple cluster results with different number of clusters, the clustering has to performed only once. 
Subsequently the tree can be cut at any vertical line which will result in a defined number of clusters.

There are two types of _hClust_: 

  - Agglomerative (bottom-up): Each data point is in its own cluster and the nearest ones are merged recursively. It is referred to agglomerative hierarchical clustering (_HAC_)

  - Divisive (top-down): All data points are in the same cluster and you divide the cluster into two that are far away from each other.

  - The presented implementation is an agglomerative type.


There are several distance metrics, that can be used as distance function. The commonly used one probably is Euclidean distance. By inverting the distance, you end up with a similarity. High similarities indicate low distances, and vice versa. By calculating the similarities for every element pair, a similarity matrix can be generated.

![](../img/simMatrix.png)

Fig 2: Data matrix (left) with measurement types as columns and (biological) entities as rows. The data matrix can be converted into a similarity matrix, that contain the inverse of distances.

![](../img/workflow.png)

Fig 3: Workflow as proposed in pseudo code in Reference#2. 


## Aim for this project

1. Blog post introducing the method, its applications, and limitations.

2. Implement an efficient agglomerative hierarchical clustering in FSharp.Stats.


## Coding clues

### 0<sup>th</sup> step: 

  - Inform yourself about 

    - queues and priority queues (roughly)

    - similarity measurements such as Euclidean distance, Manhattan distance, the advantage to use the squared Euclidean distance

    - single linkage, complete linkage, and centroid based linkage types

  - Down below you can see the pseudo code (not F#!) the efficient agglomerative hierarchical clustering (_HAC_) is based on:

    ```
    // Generating priority queue
    Q = [] //priority queue 
    for n = 1 to N 
        for i = 1 to N 
            Q.enqueue(SIM(d[i], d[n]), (i, n)) 
    
    // iterative agglomerative clustering
    for k = 1 to N-1 
        <i,m> = Q.dequeue() 
        mergedCluster = merge((i,m)) 
    
        Q.remove((_,m)) //remove any similarity that includes m 
    
        for j = 1 to N 
            Q.update((i,j), SIM(mergedCluster, d[j])) 
    ```


### 1<sup>st</sup> step: 

  - create a F# script (.fsx), load and open ```FSharp.Stats```, ```FSharpAux``` and ```FSharpx.Collections```

  - import test data

    - You can find the classic clustering dataset "iris" [here](https://github.com/fslaborg/FSharp.Stats/tree/developer/docs/data).

  - An implementation of an priority queue is given below.
*)

(******)

#r "nuget: FSharp.Stats, 0.4.1"
#r "nuget: FSharpAux, 1.0.0"
#r "nuget: FSharpx.Collections, 2.1.3"
#r "nuget: Plotly.NET, 2.0.0-beta9"
            
open FSharp.Stats
open FSharpAux
open FSharpx.Collections
open Plotly.NET

                
let lables,data =
    let fromFileWithSep (separator:char) (filePath) =     
        // The function is implemented using a sequence expression
        seq {   let sr = System.IO.File.OpenText(filePath)
                while not sr.EndOfStream do 
                    let line = sr.ReadLine() 
                    let words = line.Split separator//[|',';' ';'\t'|] 
                    yield words }
    fromFileWithSep ',' (__SOURCE_DIRECTORY__ + "../content/irisData.csv")
    |> Seq.skip 1
    |> Seq.map (fun arr -> arr.[4], [| float arr.[0]; float arr.[1]; float arr.[2]; float arr.[3]; |])
    |> Seq.toArray
    |> FSharp.Stats.Array.shuffleFisherYates
    |> Array.mapi (fun i (lable,data) -> sprintf "%s_%i" lable i, data)
    |> Array.unzip

    
type PriorityQueue<'T when 'T : comparison>(values : 'T [], comparisonF : 'T -> float) = 
        
    let sort = Array.sortByDescending comparisonF
    let mutable data = sort values 
    
    new (comparisonF) = PriorityQueue(Array.empty,comparisonF)
    
    interface System.Collections.IEnumerable with
        member this.GetEnumerator() = data.GetEnumerator()
    
    member this.UpdateElement (t:'T) (newt:'T) =     
        let updated =
            data 
            |> Array.map (fun x -> if x = t then newt else t)
            |> sort
        data <- updated
          
    member this.Elements = data
        
    member this.RemoveElement (t:'T) = 
        let filtered = 
            Array.filter (fun x -> x <> t) data
        data <- filtered
    
    member this.GetHead :'T = 
        Array.head data
    
    member this.Dequeue() = 
        let head,tail = Array.head data, Array.tail data
        data <- tail
        head, this
    
    member this.Insert (t:'T) = 
        let newd = Array.append data [|t|] |> sort
        data <- newd

    member this.UpdateBy (updateElementFunction: 'T -> 'T) = 
        let newd = 
            Array.map updateElementFunction data 
            |> sort
        data <- newd

    member this.RemoveElementsBy (predicate: 'T -> bool) = 
        let newd = 
            Array.filter predicate data 
        data <- newd

(**
### 2<sup>nd</sup> step: Generate priority queue

  - For each data point calculate the distances to each of the other points. 

    - You can find different kinds of distance measures in ```ML.DistanceMetrics```

    - Similarity can be interpreted as inverse distance. The lower the distance, the higher the similarity and the faster the data points have to be merged. 
    An appropriate type to store the result could be the following:

*)

(******)

/// Type to store similarities
type Neighbour = {
    /// inverse of distance
    Similarity: float
    /// list of source cluster indices
    Source  : int list
    /// list of target cluster indices
    Target  : int list
    }
    with static
            member Create d s t = { Similarity = d; Source = s; Target = t}

//Example queue
let neighbours = 
    [|
    Neighbour.Create 1. [1]      [2]
    Neighbour.Create 2. [0]      [6]
    Neighbour.Create 5. [3]     [5]
    Neighbour.Create 2. [4;7;10] [8;9]
    Neighbour.Create 7. [1]      [2]
    |]

////// usage of PriorityQueue
let myPQueue = PriorityQueue(neighbours,fun x -> x.Similarity)
myPQueue.GetHead                                                                     // reports queues
myPQueue.RemoveElement (Neighbour.Create 5. [3] [5])                                 // removes element from queue
myPQueue.UpdateElement (Neighbour.Create 2. [0] [6]) (Neighbour.Create 200. [0] [6]) // update element in queue 
myPQueue.RemoveElementsBy (fun x -> not (List.contains 3 x.Source))                  // update element in queue 
myPQueue.UpdateBy (fun x -> if x.Similarity > 2. then Neighbour.Create 100. x.Source x.Target else x)// update elements in queue  by given function

////// usage of IntervalHeap
#r "nuget: C5, 2.5.3"
open C5
let myHeap : IntervalHeap<Neighbour> = IntervalHeap(MemoryType.Normal)

myHeap.AddAll(neighbours)                   // adds array of neighbours
let max = myHeap.FindMax()                  // finds max value entry
myHeap.DeleteMax()                          // deletes max value entry 
myHeap.Filter (fun x -> x.Similarity = 5.)  // filters entries based on predicate function


(**
  - Some example applications of the PriorityQueue type are shown above.
  
  - Generate a priority queue that is descending regarding the similarity. 


### 3<sup>rd</sup> step:
  - Create a clustering list, that contains information of the current clustering state. This could be an ```int list []``` where each of the lists contains indices of clustered data points. Since in the beginning all data points are in its own cluster the clustering list could look as follows: 

    - ```let clusteringList = [|[0];[1];[2];...[n-1]|]```

  - When cluster 1 and 2 merge, the clustering list may look like this:

    - ```let clusteringList = [|[0];[1;2];[];...[n-1]|]```

### 4<sup>th</sup> step:
  - Now the agglomeration starts. Since every data point is in its own cluster, you can perform n-1 agglomeration (merging) steps before you result in a single cluster that contains all data points.

  - For each merge (1..n-1) do

    - take the first entry of the priority queue (the most similar clusters)

      - source indices = [i] 

      - target indices = [j]

    - Create a new cluster, that contains the merged indices: [i;j]

    - Save the new cluster configuration in your clustering list. Therefore you can add j to the i<sup>th</sup> cluster, and you can remove j from the j<sup>th</sup> cluster.

    - Remove any entry from priority queue that contains j as target or source index.

    - Update all entries in priority queue that contain i as source or targe index:

      - j has to be added to every cluster that contains i

      - Replace the distances with new distances of the merged mergedCluster to all other clusters.

    - repeat cycle with next merge

### 5<sup>th</sup> step:

  - Clustering list now contains all possible cluster configurations. Convert the clustering list into
  a binary tree structure such as ```ML.Unsupervised.HierarchicalClustering.Cluster<'a>```


### 6<sup>th</sup> step: Function implementation in F#

  - create a function, that contains all necessary helper functions in its body and takes the following parameters (suggestion):

|Parameter name|data type|description|
|--------------|---------|-----------|
|data|```seq<'a>```|data|
|distFu|```'a->'a->float```|distance Function from ```FSharp.Stats.ML.DistanceMetrics```|
|linkageType|```Linker.LancWilliamsLinker``` or self defined|linkage type that is used during clustering|
||||
|output|```ML.Unsupervised.HierarchicalClustering.Cluster<'a>``` or cluster configuration list||

### 7<sup>th</sup> step: Further coding considerations

  - Removing elements from the priority queue is slow. Is there a better way to avoid the deletion? 
  
    - maybe a Map(int[],bool), or a nested priority queue (see Reference#2) would be beneficial

    - or another implementation of heap/priority queues like C5.IntervalHeap could be faster

## References

- https://www.youtube.com/watch?v=7xHsRkOdVwo

- https://github.com/srirambaskaran/efficient-hierarchical-clustering

- https://nlp.stanford.edu/IR-book/pdf/17hier.pdf

- https://medium.com/machine-learning-researcher/clustering-k-mean-and-hierarchical-cluster-fa2de08b4a4b


## Additional information

### Testing

  - apply hClust to a dataset of your choice

  - optional: Test your results against implementations in R/Python or in the best case against the datasets proposed in the original publication.

### Blog post

  - What is solved by the usage of hClust?
  
  - classical application examples
  
  - limitations/drawbacks of hClust

  - short description of the algorithm (maybe with flowchart visualization)


*)
