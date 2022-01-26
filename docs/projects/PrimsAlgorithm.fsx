(**
---
title: Reduce complex graphs to the best paths
category: projects
categoryindex: 2
index: 4
---
*)

(**
# Reduce complex graphs to the best paths

**Interested?** Contact [muehlhaus@bio.uni-kl.de](mailto:muehlhaus@bio.uni-kl.de) or [weil@bio.uni-kl.de](mailto:weil@bio.uni-kl.de)

#### Table of contents

- [Introduction](#Introduction)
    - [Minimum spanning trees](#Minimum-spanning-trees) 
    - [Prim's algorithm](#Prim-s-algorithm) 
- [Aim for this project](#Aim-for-this-project)
- [References](#References)
- [Coding clues](#Coding-clues)
- [Additional information](#Additional-information)
    - [Testing](#Testing) 
    - [Blog post](#Blog-post) 

## Introduction

`Imagine you're elected Minister of Infrastructure and tasked to build a road grid. This grid is supposed to connect all cities. Your budget is pretty tight though so the combined length of the roads should be as low as possible`

Problem's like this are predetermined to be regarded as a graph problem. Graphs are structures that consist of two different kinds of components:
- Vertices are the entities of the graph
- Edges connect these vertices

Although simple in principal, graphs can become complex as they grow. Graph algorithms have emerged for all kinds of problems. One of them is finding minimum spanning trees which have been used to solve the problem described in the beginning.

### Minimum spanning trees

A minimum spanning tree (MST) or minimum weight spanning tree is a subset of the edges of a connected, edge-weighted undirected graph that connects all the vertices together, without any cycles and with the minimum possible total edge weight.

As indicated above, MSTs have applications everytime someone needs to design an efficient grid. Examples for this might be electric grids, road grids or water pipe grids. 
Besides that, MSTs can be used for studying epidemeology by finding the shortest paths trough which pathogens might traverse a population network.

In biological research, MSTs might be used for visualization or clustering purposes. An example for this can be seen in the picture below, where phylogenetic groups were found and visualized using an MST.

<img src="../img/Prims_MST.png" alt="drawing" width="45%"/>
<img src="../img/Prims_Phylo.png" alt="drawing" width="45%"/>

<br>

Several algorithms exist for finding MST, where often there is a tradeoff made between performance and quality of the result. Your task will be to implement one of them.

### Prim's algorithm

Prim's algorithm is a simple, greedy approach for finding the MST of a network. Greedy approaches always find the best solution in exchange for lower performance. 

In Prim's algorithm, you start a new graph by selecting a single vertex in the original graph. This new graph is repetitively grown by finding the closest connections to vertices not yet included in the MST.

![](../img/Prims_Demo.gif)

## Aim for this project

<br>

- Get a basic understanding of network science

- Implement Prim's algorithm for finding minimum spanning trees

- Write a blogpost entry 

## References

- [Introduction to graphs and networks](http://networksciencebook.com/chapter/2#networks-graphs)
- [Minimum spanning tree](https://en.wikipedia.org/wiki/Minimum_spanning_tree)
- [Prim's algorithm](https://en.wikipedia.org/wiki/Prim%27s_algorithm)
- [Graph Visualization](https://fslab.org/Cyjs.NET/)

## Coding clues

### General steps:

1. Initialize a tree with a single vertex, chosen arbitrarily from the graph.
2. Grow the tree by one edge: of the edges that connect the tree to vertices not yet in the tree, find the minimum-weight edge, and transfer it to the tree.
3. Repeat step 2 (until all vertices are in the tree).

### Using the graph library:

*)

#r "nuget: FSharp.FGL, 0.0.2"

open FSharp.FGL
open FSharp.FGL.Undirected

// Create a new graph
let g : Graph<int,string,float> = Graph.empty


// Add vertices 
let v1 = (1,"VertexNumeroUno")
let v2 = (2,"VertexNumeroDos")

let gWithVertices = 
    Vertices.add v1 g
    |> Vertices.add v2

// Add edges
let e = (1,2,1.)

let gWithEdge = 
    Edges.add e gWithVertices

// Show all edges to find the best
Edges.toEdgeList

// Remove vertex (Including its edges)
Vertices.remove (fst v1) gWithEdge

(**
### Visualize the graph:
*)

#r "nuget: Cyjs.NET, 0.0.4"

open Cyjs.NET

let inline toCyJS (g : Graph<'Vertex,'Label,float>) =
    let vertices = 
        g
        |> Vertices.toVertexList
        |> List.map (fun (id,name) -> Elements.node (string id) [CyParam.label (string name)])

    let edges =
        g
        |> Edges.toEdgeList
        |> List.map (fun (v1,v2,weight) -> Elements.edge (string v1 + string v2) (string v1) (string v2) [CyParam.weight (weight)])

    CyGraph.initEmpty ()
    |> CyGraph.withElements vertices
    |> CyGraph.withElements edges
    |> CyGraph.withStyle "node" [CyParam.content =. CyParam.label]
    |> CyGraph.withStyle "edge" [CyParam.content =. CyParam.weight]

(***do-not-eval***)
gWithEdge
|> toCyJS
|> CyGraph.show

(**
<br>

#### The visualized graph should look as follows:
*)

(*** hide ***)
gWithEdge
|> toCyJS
|> CyGraph.withSize(600, 400) 
|> Cyjs.NET.HTML.toEmbeddedHTML
(*** include-it-raw ***)

(** 
## Additional information

### Testing

For testing whether your implementation does return the correct results, you can use this website: 

https://algorithms.discrete.ma.tum.de/graph-algorithms/mst-prim/index_en.html

![](../img/Prims_Testing.png)

### Blog post

- What is a minimum spanning tree
- Classical application examples
- Advantages and drawbacks of Prim's algorithm
- Short description of the algorithm

*)