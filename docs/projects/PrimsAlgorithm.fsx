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

## Coding clues

1. Initialize a tree with a single vertex, chosen arbitrarily from the graph.
2. Grow the tree by one edge: of the edges that connect the tree to vertices not yet in the tree, find the minimum-weight edge, and transfer it to the tree.
3. Repeat step 2 (until all vertices are in the tree).


<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>
<br>










1. Associate with each vertex v of the graph a number C[v] (the cheapest cost of a connection to v) and an edge E[v] (the edge providing that cheapest connection). To initialize these values, set all values of C[v] to +∞ (or to any number larger than the maximum edge weight) and set each E[v] to a special flag value indicating that there is no edge connecting v to earlier vertices.

2. Initialize an empty forest F and a set Q of vertices that have not yet been included in F (initially, all vertices).

3. Repeat the following steps until Q is empty:

    - Find and remove a vertex v from Q having the minimum possible value of C[v]
    - Add v to F and, if E[v] is not the special flag value, also add E[v] to F
    - Loop over the edges vw connecting v to other vertices w. For each such edge, if w still belongs to Q and vw has smaller weight than C[w], perform the following steps:
        - Set C[w] to the cost of edge vw
        - Set E[w] to point to edge vw.

4. Return F

*)

let f x = 1