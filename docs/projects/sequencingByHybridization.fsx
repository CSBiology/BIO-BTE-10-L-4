(**
---
title: Sequencing by Hybridization as an Eulerian Path Problem
category: projects
categoryindex: 1
index: 5
---
*)


(**
# Sequencing by Hybridization as an Eulerian Path Problem

**Interested?** Contact [muehlhaus@bio.uni-kl.de](mailto:muehlhaus@bio.uni-kl.de) or [ottj@rhrk.uni-kl.de](mailto:ottj@rhrk.uni-kl.de)

#### Table of contents

- [Introduction](#Introduction) 
- [Aim for this project](#Aim-for-this-project)
- [Coding clues](#Coding-clues)
- [References](#References)
- [Additional information](#Additional-information)
    - [Testing](#Testing)
    - [Blog post](#Blog-post)


## Introduction

Sequencing by Hybridization (SBH) involves building a miniature DNA array (or DNA chip), that contains thousands of short DNA fragments (probes). Each of these probes gives information about the presence 
of a known, but short, sequence in the unknown DNA sequence. All these pieces of information together should reveal the identity of the target DNA sequence. 
Given a short probe (an 8- to 30-nucleotide single-stranded synthetic DNA fragment) and a single-stranded target DNA fragment, the target will hybridize 
with the probe if the probe is a substring of the target's complement. When the probe and the target are mixed together, they form a 
weak chemical bond and stick together. For example, a probe ACCGTGGA will hybridize to a target CCC**TGGCACCT**A since it is complementary to the substring TGGCACCT of the target.

Given an unknown DNA sequence, an array provides information about all strings of length *l* (l-mer) that the sequence contains, but does not provide information 
about their positions in the sequence. For example, the 8-mer composition of CCCTGGCACCTA is {CCCTGGCA, CCTGGCAC, CTGGCACC, TGGCACCT,GGCACCTA} 
The reduction of the SBH problem to an [Eulerian Path](https://en.wikipedia.org/wiki/Eulerian_path) problem is to construct a graph whose edges, 
rather than vertices, correspond to those l-mers, and then to find a path in this graph visiting every edge exactly once. Paths visiting **ALL EDGES** correspond to sequence reconstructions.

## Aim for this project

1. Blog post introducing the method, its applications, and limitations.

2. Implement a function which creates a directed graph from a given set of l-mers

3. Implement [Hierholzer's algorithm](https://www-m9.ma.tum.de/graph-algorithms/hierholzer/index_en.html#tab_ti) for finding Eulerian paths in **directed** graphs


## Coding clues

### Setting up the environment

* Create a F# script file (.fsx) and paste the following text at the top of your file:

```
#r "nuget: FSharpAux"
#r "nuget: BioFSharp"
#r "nuget: FSharp.FGL"
#r "nuget: Cyjs.NET"

open FSharpAux
open BioFSharp
open FSharp.FGL
open FSharp.FGL.Directed
open Cyjs.NET
```

### Creating an Eulerian Graph of l-mers with FSharp.FGL

* Vertices correspond to (l-1)-mers
* Edges correspond to l-mers from the spectrum
* All functions should operate on either `BioArray`, `BioList` or `BioSeq`
*)
(***hide***)
#r "nuget: FSharpAux"
#r "nuget: BioFSharp"
#r "nuget: FSharp.FGL"
#r "nuget: Cyjs.NET"

open FSharpAux
open BioFSharp
open BioFSharp.Nucleotides
open FSharp.FGL
open FSharp.FGL.Directed
open Cyjs.NET
(**
<br>

* Start with any Nucleotide array

<br>
*)
let sampleSequence: array<Nucleotide> = 
    "ATGGCGTGCA"
    |> BioArray.ofNucleotideString
(**
<br>

* Compute the l-mers of the Nucleotide array (in this case 3-mers)

<br>
*)
let lMers3: array<array<Nucleotide>> = 
    [|[|A;T;G|]; [|T;G;G|]; [|T;G;C|]; [|G;T;G|]; [|G;G;C|]; [|G;C;A|]; [|G;C;G|]; [|C;G;T|]|]
(**
<br>

* Initialize a directed graph

<br>
*)
let graph: Graph<int,array<Nucleotide>,array<Nucleotide>> = Graph.empty
(**
<br>

* Create a list with vertices based on the l-mers and add them to the graph

<br>
*)
let vertices: list<int*array<Nucleotide>> =
    [(1, [|A; T|]); (2, [|T; G|]); (3, [|G; T|]); (4, [|G; G|]); (5, [|G; C|]);(6, [|C; G|]); (7, [|C; A|])]

let graphWithVertices: Graph<int,array<Nucleotide>,array<Nucleotide>> =
    Vertices.addMany vertices graph
(**
<br>

* Create a list with edges based in the vertices and l-mers and add them to the graph

<br>
*)
let edges: list<int*int*array<Nucleotide>>=
    [(1, 2, [|A; T; G|]); (2, 4, [|T; G; G|]); (2, 5, [|T; G; C|]); (3, 2, [|G; T; G|]);
    (4, 5, [|G; G; C|]); (5, 7, [|G; C; A|]); (5, 6, [|G; C; G|]); (6, 3, [|C; G; T|])]

let graphWithEdges: Graph<int,array<Nucleotide>,array<Nucleotide>> =
    Edges.addMany edges graphWithVertices
(**
<br>

* You can visualize the graph using Cyjs.NET

<br>
*)
let inline toCyJS (g : Graph<'Vertex,array<Nucleotide>,array<Nucleotide>>) =
    let vertices = 
        g
        |> Vertices.toVertexList
        |> List.map (fun (id,name) ->
            Elements.node (string id) [CyParam.label (name |> BioArray.toString)]
        )

    let edges =
        g
        |> Edges.toEdgeList
        |> List.map (fun (v1,v2,weight) -> 
            Elements.edge (string v1 + string v2) (string v1) (string v2) [CyParam.weight (weight |> BioArray.toString)]
        )

    CyGraph.initEmpty ()
    |> CyGraph.withElements vertices
    |> CyGraph.withElements edges
    |> CyGraph.withLayout (Layout.initBreadthfirst id)
    |> CyGraph.withStyle "node" [CyParam.content =. CyParam.label]
    |> CyGraph.withStyle "edge"     
                [
                    CyParam.Curve.style "bezier"
                    CyParam.opacity 0.666
                    CyParam.width <=. (CyParam.weight, 70, 100, 5, 5)
                    CyParam.Target.Arrow.shape "triangle"
                    CyParam.Line.color =. CyParam.color
                    CyParam.Target.Arrow.color =. CyParam.color
                    CyParam.Source.Arrow.color =. CyParam.color
                ]

graphWithEdges
|> toCyJS

(***hide***)
graphWithEdges |> toCyJS |> CyGraph.withSize(600, 400) |> Cyjs.NET.HTML.toEmbeddedHTML
(***include-it-raw***)    
(**
* This graph is semibalanced `(|indegree - outdegree| = 1)`. If a graph has an Eulerian path starting at vertex *s* and ending at vertex *t*, then all its vertices are balanced, 
with the possible exception of *s* and *t*, which may be semibalanced.

* The Eulerian path problem can be reduced to the Eulerian cycle problem by adding an edge between two semibalanced vertices.

### Hierholzer's algorithm

* Choose any starting vertex, and follow a path along the unused edges from that vertex until you return to it. You will alway return to the starting vertex in a balanced Eulerian graph. 
Since every vertex has `indegreee = outdegree`, there is always an unused edge to leave the current vertex. The path found by doing this is a closed tour, starting and ending at the same vertex, 
but not necessarily covering all vertices and edges.

* As long as there exists a vertex in the current closed tour that has unused edges, you can start finding a new closed tour and join it with the previously found tour.

* Since we assume the original graph is connected, repeating the previous step will cover all edges of the graph.


*)

(**
## References

* https://en.wikipedia.org/wiki/Eulerian_path
* https://www-m9.ma.tum.de/graph-algorithms/hierholzer/index_en.html#tab_ti
* https://www.geeksforgeeks.org/hierholzers-algorithm-directed-graph/

## Additional information

###Testing

* Implement a function that returns an array of l-mers for any given DNA-Sequence
* Generate an Eulerian Graph based on the returned set of l-mers
* Compare the reconstructed sequence to the original sequence
* Optional: Implement a function that performs the steps above for a large number of sequences with varying l-mer lengths

### Blog post

* Introduction about SBH, and how it is used today
* Describe the limits and weaknesses of the approach in your blog post
* Compare Hierholzer's algorithm to Fleury's algorithm

*)
