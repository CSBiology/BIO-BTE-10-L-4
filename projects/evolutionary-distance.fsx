(**
// can't yet format YamlFrontmatter (["title: Phylogenetic tree reconstruction based on evolutionary distance"; "category: projects"; "categoryindex: 1"; "index: 3"], Some { StartLine = 2 StartColumn = 0 EndLine = 6 EndColumn = 8 }) to pynb markdown

# Phylogenetic tree reconstruction based on evolutionary distance
    
**Interested?** Contact [muehlhaus@bio.uni-kl.de](mailto:muehlhaus@bio.uni-kl.de) or [schneike@bio.uni-kl.de](mailto:schneike@bio.uni-kl.de)

#### Table of contents

- [Introduction](#Introduction) 
    - [Phylogenetic trees](#Phylogenetic-trees)
    - [Evolutionary Distance of DNA sequences](#Evolutionary-Distance-of-DNA-sequences)
        - [Pairwise p distance](#Pairwise-p-distance)
    - [Distance Corrections based on evolutionary Models](#Distance-Corrections-based-on-evolutionary-Models)
        - [The Jukes-Cantor Model](#The-Jukes-Cantor-Model)
        - [The Kimura Model](#The-Kimura-Model)
        - [The Kimura 2-Parameter Model](#The-Kimura-2-Parameter-Model)
- [Aim for this project](#Aim-for-this-project)
- [Coding clues](#Coding-clues)
- [References](#References)
- [Additional information](#Additional-information)

## Introduction

### Phylogenetic trees

Phylogenetic trees are diagrams that visualize inferred evolutionary relationships between a set of organisms. Consider this tree diagram:

```
         ____ A
       _|
      | |____ B
______|
      |______ C
```

Ancestors are shown as nodes on the tree, while the leaves represent the respective organisms.
It tells you that A and B share a common ancestor, and that that ancestor shares a common ancestor with C. 
In other words, A and B are closer related to each other, than each of them to C.

A and B form a _clade_ together with their common ancestor (also known as a _monophyletic group_) - a group of organisms that includes a single ancestor 
and all of its descendents that represent unbroken lines of evolutionary descent. 

But based on what information do we construct such trees? There are different classes of approaches to this problem, but to stay beginner-friendy, 
only _distance-based_ methods will be discussed in the scope of this project. For sake of completeness, other approaches include _parsimony_, 
_maximum likelihood_ and _Bayesian approaches_ to searching the possible tree space.

The first step in any (distance-based) phylogenetic tree reconstruction is the selection of the characteristic to infer evolutionary relationships from, 
and subsequently the determination of the phylogenetic distance between the organisms of interest based on that characteristic.

---

### Evolutionary Distance of DNA sequences

Any kind of characteristic of organisms can be used to try to infer phylogenetic relationships - like for example beaks of Darwin finches -
but DNA sequences have proven to be incredibliy helpful to reconstruct phylogenetic trees, as the nucleotide alphabet is relatively 
simple and sequencing data has reached unparalleled throughput and accuracy. Likewise, there are a wide range of sophisticated methods to 
calculate phylogenetic distance based on DNA sequences. 

In the scope of this project, you will take a step back and look at some classic evolutionary models that can be used to model phylogenetic 
distance based on DNA sequences.

A few important bits of jargon for the following chapters:

- DNA substitution mutations can be classified by 2 types:
    - _Transitions_ are interchanges of two-ring purines (A <> G) or of one-ring pyrimidines (C <> T)
    - _Transversions_ are interchanges of purine for pyrimidine bases or vice versa (A <> T | A <> C | G <> T | G <> C)

---
---

#### Proportional distance

The pairwise proportional distance (or _p distance_) is the classical 'naive' approach to estimate pairwise distances between two sequences. 

It is simply the ratio between substitution sites and 

It does not make any correction for multiple substitutions at the same site, substitution rate biases (for example, differences in the transitional and transversional rates), or differences in evolutionary rates among sites.

### Distance Corrections based on evolutionary Models



---

#### The Jukes-Cantor Model



---

#### The Kimura 2-Parameter Model



---

## Aim for this project

You understand the following evolutionary distance models and are able to explain the differences between them (required in your final report)
Also, you implement them in F# for the BioFSharp library:

- Pairwise p Distance
- JC69 evolutionary distance based on the model by Jukes and Cantor
- K81 evolutionary distance based on the Kimura two-parameter model

As a demonstration of your implementations, as well as to show the differences between these models,
You construct at least 10 adequate test sequences of equal length, and construct phyologenetic trees from them. 
Investigate the most interesting and obvious differences, and relate them to the different model assumptions.

Finally, you choose adequate sequences of at least 6 organisms, perform a multiple alignment for them and repeat above process for real-world sequences.

## Coding clues

### Before you start 

- make sure you (re)familiarize yourself with the basics behind phylogenetic trees.

- 

### Scripting environment and necessary libraries

- create a F# script (.fsx), load and open the following libraries:
    - `FSharpAux`
    - `FSharpAux.IO`
    - `FSharp.Stats`
    - `BioFSharp`
    - `BioFSharp.IO`
    - `Plotly.NET`

the top of your script file should look like this:

*)
#r "nuget: FSharpAux"
#r "nuget: FSharpAux.IO"
#r "nuget: FSharp.Stats, 0.4.1" 
#r "nuget: BioFSharp, 2.0.0-beta5"
#r "nuget: BioFSharp.IO, 2.0.0-beta5"
#r "nuget: Plotly.NET, 2.0.0-beta9"

open FSharpAux
open FSharpAux.IO
open BioFSharp
open BioFSharp.IO
open FSharp.Stats
open Plotly.NET
(**
### General coding advice

- All pairwise distance functions should be able to operate on either `BioArray`, `BioList`, or `BioSeq`. 
You can use the fact that all of these sequence types are implementing `IEnumerable` and can only contain `IBioItem`s.

    The adequate functions therefore all should have the following signature: 

    ```seq<#IBioItem> -> seq<#IBioItem> -> float```

- To perform hierarchical clustering to reconstruct the phylogenetic trees, use the respective module from the `FSharp.Stats` library:
    - use your distance function as distance metric
    - use the `upgmaLwLinker` function as linker, this determines the function that is used to determine the distance between newly created ancestor nodes.
    - Documentation of these functions can be found [here](https://fslab.org/FSharp.Stats/Clustering.html#Hierarchical-clustering)

*)
open BioFSharp
open FSharp.Stats
open FSharp.Stats.ML
open FSharp.Stats.ML.Unsupervised

let yourDistance (seqA: seq<IBioItem>) (seqB:seq<IBioItem>) = ...

//construct a function that clusters input data based on your diostance function(s)
let clusterSequences (data:seq<seq<IBioItem>>) = 
    HierarchicalClustering.generate 
        yourDistance // your distance function for either p, jc69, or K81 distance
        HierarchicalClustering.Linker.upgmaLwLinker //use this premade linker function  
        data
(**
to perform Multiple sequence alignment between your real worl sequence examples, use the respective functions from `BioFSharp`:


- There are two ways of handling the gaps produced by alignments: _Complete-Deletion_ and _Pairwise Deletion_ inform yourself about them.

-

---

## References

Nei, M. & Zhang J. Evolutionary Distance: Estimation 2006 https://doi.org/10.1038/npg.els.0005108

https://en.wikipedia.org/wiki/Models_of_DNA_evolution

https://www.cs.rice.edu/~nakhleh/COMP571/Slides/Phylogenetics-DistanceMethods-Full.pdf

https://www.megasoftware.net/mega1_manual/Distance.html

## Additional information


*)