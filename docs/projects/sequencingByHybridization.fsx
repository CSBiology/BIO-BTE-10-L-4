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

### Eulerian Graph of l-mers

* Vertices correspond to (l-1)-mers

* Edges correspond to l-mers from the spectrum

![]({{root}}img/EulerGraph.png)

[How to use the graph library FSharp.FGL]({{root}}projects/PrimsAlgorithm.html#Using-the-graph-library)

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
