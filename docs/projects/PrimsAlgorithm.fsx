(**
---
title: Reduce complex graphs to the best paths
category: projects
categoryindex: 1
index: 3
---
*)

(** 
# Reduce complex graphs to the best paths

`Imagine you're elected Minister of Infrastructure and tasked to build a road grid. This grid is supposed to connect all cities. Your budget is pretty tight though so the combined length of the roads should be as low as possible`


Problem's like this are predetermined to be regarded as a graph problem. Graphs are structures that consist of two different kinds of components:
- Vertices are the entities of the graph
- Edges connect these vertices

Although simple in principal, graphs can become complex as they grow. Graph algorithms have emerged for all kinds of problems. One of them is finding minimum spanning trees which have been used to solve the problem described in the beginning.



##### Interested?

Continue reading and write a mail to [Lukas](mailto:weil@bio.uni-kl.de) or [Timo](mailto:muehlhaus@bio.uni-kl.de)

## Content

1. Minimum spanning trees

2. Prim's algorithm

3. Aim for this project

4. References

5. Coding clues






## Minimum spanning trees

A minimum spanning tree (MST) or minimum weight spanning tree is a subset of the edges of a connected, edge-weighted undirected graph that connects all the vertices together, without any cycles and with the minimum possible total edge weight.

<img src="https://upload.wikimedia.org/wikipedia/commons/thumb/d/d2/Minimum_spanning_tree.svg/1200px-Minimum_spanning_tree.svg.png" alt="drawing" width="45%"/>
<img src="https://www.researchgate.net/profile/Michel-Fabre-2/publication/234090394/figure/fig11/AS:341854030712841@1458515760884/Minimum-spanning-tree-based-upon-whole-genome-SNP-analysis-The-tree-is-based-upon-13382.png" alt="drawing" width="45%"/>

Some more motivational words

## Prim's algorithm

Prim's algorithm is a simple, greedy approach for finding the MST of a network. Greedy approaches always find the best solution in exchange for lower performance. 

In Prim's algorithm, you start a new graph by selecting a single vertex in the original graph. This new graph is repetitively grown by finding the closest connections to vertices not yet included in the MST.

![prims](https://upload.wikimedia.org/wikipedia/commons/9/9b/PrimAlgDemo.gif)

## Aim for this project

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

#r "nuget: FSharp.FGL" 

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

#r "nuget: Cyjs.NET"

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

gWithEdge
|> toCyJS
|> CyGraph.show