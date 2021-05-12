(**
---
title: Sequencing by Hybridization
category: projects
categoryindex: 1
index: 5
---
*)


(**
# Sequencing by Hybridization as an Eulerian Path Problem

**Interested?** Contact [muehlhaus@bio.uni-kl.de](mailto:muehlhaus@bio.uni-kl.de) or [ottj@rhrk.uni-kl.de](mailto:ottj@rhrk.uni-kl.de)

#### Table of contents

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

[Interactive Version]({{root}}img/EulerGraph.html)

### Hierholzer's algorithm


```
Input: Undirected graph G=(V,E), no or exactly two nodes have odd degree
Output: List of nodes in Eulerian cycle/path

BEGIN
    IF graph infeasible THEN END
    IF graph semi-Eulerian THEN
        start <- node with odd degree
    ELSE
        start <- arbitrary node
    subtour <- O
    tour <- {start}
    REPEAT
        start <-  node in tour with unvisited edge
        subtour <- {start}
        current = start
        DO
            {current, u} <- take unvisited edge leaving current
            subtour <- subtour U {u}
            current <- u
        WHILE start != current
        Integrate subtour in tour
    UNTIL tour is Eulerian cycle/path
END
```


*)

(**
## References

## Additional information


### Blog post

*)
