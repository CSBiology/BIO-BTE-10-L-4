(**
---
title: Alignment of Peptide derived MS Spectra
category: projects
categoryindex: 1
index: 3
---
*)

(**
# Alignment of Peptide derived MS Spectra.
    
**Interested?** Contact [muehlhaus@bio.uni-kl.de](mailto:muehlhaus@bio.uni-kl.de) or [d_zimmer@rhrk.uni-kl.de](mailto:d_zimmer@rhrk.uni-kl.de)

#### Table of contents

- [Introduction](#Introduction) 
- [Aim for this project](#Aim-for-this-project)
- [Coding clues](#Coding-clues)
- [References](#References)
- [Additional information](#Additional-information)

## Introduction

Modern Proteomics relies on the use of mass spectrometry (MS). Since it is not feasible to analyze proteins directly, MS-based shotgun proteomics estimates protein abundances using a proxy: peptides. 
While the whole process is complex enough to fill whole textbooks, this project focuses on a very specific puzzle piece: 

**The alignment of peptide derived MS spectra.** 

Let us motivate this problem by visual examination of the fragment spectra*'* of two example peptides with the sequences 'PRTEIINNEE' (blue) and 'PRTEYINNEE' (orange).

*'*: For simplicity we will only consider b-ions at charge 1.
*)
(*** hide ***)
#r "nuget: FSharpAux"
#r "nuget: FSharpAux.IO"
#r "nuget: FSharp.Stats, 0.4.1" 
#r "nuget: BioFSharp, 2.0.0-beta5"
#r "nuget: BioFSharp.Mz, 0.1.5-beta"
#r "nuget: BioFSharp.IO, 2.0.0-beta5"
#r "nuget: Plotly.NET, 2.0.0-beta9"
open FSharpAux
open FSharpAux.IO
open BioFSharp
open BioFSharp.IO
open BioFSharp.Mz
open FSharp.Stats
open Plotly.NET

let PRTEIINNEE =   
    "PRTEIINNEE"
    |> BioFSharp.BioList.ofAminoAcidString
    |> Fragmentation.Series.bOfBioList BioFSharp.BioItem.monoisoMass
    |> List.map (fun x -> x.MainPeak.Mass,1.)

let PRTEYINNEE =   
    "PRTEYINNEE"
    |> BioFSharp.BioList.ofAminoAcidString
    |> Fragmentation.Series.bOfBioList BioFSharp.BioItem.monoisoMass
    |> List.map (fun x -> x.MainPeak.Mass,1.)

let specComp = 
    [
        PRTEIINNEE 
        |> Chart.Column
        |> Chart.withTraceName "PRTEIINNEE"
        PRTEYINNEE 
        |> Chart.Column
        |> Chart.withTraceName "PRTEYINNEE"
    ]    
    |> Chart.Combine
    
specComp
|> GenericChart.toChartHTML
(***include-it-raw***)    

(**
Even though that both peptides show very high sequence similarity and only differ by one amino acid, with 'I' at index 4 being substituted with 'Y', 
this small change in the sequence results in a big change on the spectrum level. As one can see, more than half of all peaks do not overlap. This is problematic since many
algorithms that compare spectra aim to maximize matching peaks - This project is designed to tackle this gap by solving the **'Spectral Alignment Problem'.**

Before we formulate the problem and discover a strategy to tackle it we change the perspective: The following figure plots the peak positions of 
the fragment spectra of the b-ions of 'PRTEIINNEE' versus the one of 'PRTEYINNEE'. 
The plot contains every possible pair of peaks (eg. the first peak of spectrum 1 and the second peak of spectrum 2) depicted as grey crosses.
Additionally, we added matching peaks as blue circles and non-matching peaks as orange triangles. You can see, that all matching peaks are placed
on the main diagonal (blue line), while the non-matching peaks are lined up at a constant offset, parallel to the main diagonal.
*)

(*** hide ***) 
let vs = 
    let vs = 
        (List.zip (PRTEIINNEE |> List.map fst) (PRTEYINNEE |> List.map fst))
    let matching = 
        vs |> List.filter (fun (x,y) -> x = y)
    let nonMatching = 
        (vs |> List.filter (fun (x,y) -> x <> y))
    let allPossibleMatchups = 
        let x,y = vs |> List.unzip
        [
            for i = 0 to x.Length-1 do 
                for j = 0 to y.Length-1 do 
                    yield (x.[i],y.[j])
        ]
    [
        Chart.Point(allPossibleMatchups)      
        |> Chart.withMarkerStyle(Size=5,Symbol=StyleParam.Symbol.Cross,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.grey)
        Chart.Point(matching)
        |> Chart.withMarkerStyle(Size=10,Symbol=StyleParam.Symbol.Circle,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.blue)
        Chart.Line([0.,0.;1400.,1400.])
        |> Chart.withMarkerStyle(Size=10,Symbol=StyleParam.Symbol.Circle,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.blue)
        Chart.Line(vs)
        |> Chart.withLineStyle(Dash=StyleParam.DrawingStyle.Dash,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.darkGreen)      
        Chart.Point(nonMatching)      
        |> Chart.withMarkerStyle(Size=10,Symbol=StyleParam.Symbol.TriangleUp,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.orange)
    ]    
    |> Chart.Combine
    |> Chart.withY_AxisStyle("PRTEIINNEE",MinMax=(1250.,0.))
    |> Chart.withX_AxisStyle("PRTEYINNEE",MinMax=(0.,1250.))
    |> Chart.withLegend false

    
vs
|> GenericChart.toChartHTML
(***include-it-raw***)      


(**
The pattern which we want to highlight here becomes even more evident if we raise the amount of amino acid mutations by one and plotting the b-ion spectrum of 'PRTEIINNEE' versus the one of 'PRTEYINYEE'. 
*)

(*** hide ***) 
let PRTEYINYEE =   
    "PRTEYINYEE"
    |> BioFSharp.BioList.ofAminoAcidString
    |> Fragmentation.Series.bOfBioList BioFSharp.BioItem.monoisoMass
    |> List.map (fun x -> x.MainPeak.Mass,1.)

let vs2 = 
    let vs = 
        (List.zip (PRTEIINNEE |> List.map fst) (PRTEYINYEE |> List.map fst))
    let matching = 
        [0.,0.]@(vs |> List.filter (fun (x,y) -> x = y))
    let nonMatching = 
        (vs |> List.filter (fun (x,y) -> x <> y))
    let allPossibleMatchups = 
        let x,y = vs |> List.unzip
        [
            for i = 0 to x.Length-1 do 
                for j = 0 to y.Length-1 do 
                    yield (x.[i],y.[j])
        ]
    [
        Chart.Point(allPossibleMatchups)      
        |> Chart.withMarkerStyle(Size=5,Symbol=StyleParam.Symbol.Cross,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.grey)
        Chart.Point(matching)
        |> Chart.withMarkerStyle(Size=10,Symbol=StyleParam.Symbol.Circle,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.blue)
        Chart.Line([0.,0.;1400.,1400.])
        |> Chart.withMarkerStyle(Size=10,Symbol=StyleParam.Symbol.Circle,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.blue)
        Chart.Line(vs)
        |> Chart.withLineStyle(Dash=StyleParam.DrawingStyle.Dash,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.darkGreen)      
        Chart.Point(nonMatching)      
        |> Chart.withMarkerStyle(Size=10,Symbol=StyleParam.Symbol.TriangleUp,Color=Plotly.NET.Colors.toWebColor Plotly.NET.Colors.Table.Office.orange)
    ]    
    |> Chart.Combine
    |> Chart.withY_AxisStyle("PRTEIINNEE",MinMax=(1350.,0.))
    |> Chart.withX_AxisStyle("PRTEYINYEE",MinMax=(0.,1350.))
    |> Chart.withLegend false

    
vs2
|> GenericChart.toChartHTML
(***include-it-raw***) 

(**
As one can see, the addition of another mutation has the effect that the 6 non-matching peaks are now lined up at 2 distinct offsets, resulting in three peaks per offset which are placed parallel to the main diagonal. Analyzing the plot carefully, you can derive that the green path is the **longest path** one can find with at most k=2 shifts from the main diagonal.
In practice this information can be used to identify spectra that are the result of modified or mutated peptides: Your main task will be to solve the following **longest path** problem and to provide an
efficient implementation: 

> **Spectral Alignment Problem:**
> Description: Find the k-similarity between two Spectra.
>
> Input: Spectrum $Sa$ , Spectrum $Sb$, number of shifts $k$  
>
> Output: The $k$ -similarity, $D(k)$, between $Sa$ and $Sb$ 
>
*)

(**
## Aim for this project

This project is your chance to dive into a topic of great relevance to modern biology: MS-based shotgun proteomics. 

- **By completing this assignment you will understand the basic principles of modern proteomics and gain a deep understanding of peptide identification.** 

- **Finally, you will implement an efficient version of spectral alignment to extend the BioFSharp.Mz library.**

*)
(**
## Coding clues
*)
(**
Fortunately, you can straight away start your journey since many functionalities are already implemented in the BioFSharp and BioFSharp.Mz libaries. The following snippet creates a function that returns
a fragment spectrum consisting of b-ions at intensity 1 based on a string encoded peptide sequence: 
*)

let calcBIonFragments pepSequence =   
    pepSequence
    |> BioFSharp.BioList.ofAminoAcidString
    |> Fragmentation.Series.bOfBioList BioFSharp.BioItem.monoisoMass
    |> List.map (fun x -> x.MainPeak.Mass,1.)

(**
This function can for example be used to recreate the first sample from above:
*)

let PRTEIINNEE' =   
    "PRTEIINNEE"
    |> calcBIonFragments
let PRTEYINNEE' =   
    "PRTEYINNEE"
    |> calcBIonFragments

let specComp' = 
    [
        PRTEIINNEE' 
        |> Chart.Column
        |> Chart.withTraceName "PRTEIINNEE"
        PRTEYINNEE' 
        |> Chart.Column
        |> Chart.withTraceName "PRTEYINNEE"
    ]    
    |> Chart.Combine
    
specComp'
|> GenericChart.toChartHTML

(**
To allow you a little head start we provide you with a **very naive** implementation of the spectral alignment problem*''*. 

**Disclaimer: You can use this implementation to benchmark against, or just familiarize yourself with the problem. You are by no means obligated to use this implementation as a blue print!**

In contrast to this naive implementation, the algorithm provided by you should be more efficient as well as applicable on mass spectra which consist of b and y ions*''* at charges up to 4.
Additionally, this naive implementation assumes that masses are measured as integers. Your algorithm should be able to take floating point values as an input.

*''*: Remember in the examples above we only considered b-ions at charge 1. 
*)

type Path = {
    K         : int
    PathIndices: list<int*int>
    }
   
// 
let findAllPaths maxK (m:Matrix<float>) = 
    let extendInAllPossibleDirections (m:Matrix<float>) maxK currentRow (path :Path) = 
        if path.K >= maxK then 
            let lastRow,lastCol = path.PathIndices.Head
            let colinearCol = currentRow-lastRow+lastCol
            if colinearCol > m.NumCols-1 then 
                [path]
            else
            let acc' =
                let v = m.[currentRow,colinearCol]
                if v = 1. then 
                    (currentRow,colinearCol)::path.PathIndices
                else
                    path.PathIndices 
            [{path with PathIndices = acc'}]
        else 
            let newV =
                let _,lastCol = path.PathIndices.Head 
                [
                    for j = (lastCol + 1) to m.NumCols-1 do
                        let v = m.[currentRow,j]
                        if v = 1. then yield (currentRow,j) 
                ]
            match newV with 
            | [] -> [path]
            | _ -> 
            let newPaths = 
                newV
                |> List.map (fun (currentRow,matchingColumn) -> (currentRow,matchingColumn)::path.PathIndices)
                |> List.map (fun acc' ->
                    let k' = 
                        match acc' with 
                        | [] -> path.K
                        | x::[] -> path.K
                        | x::y::t -> 
                            if (fst x - fst y) = (snd x - snd y) then 
                                path.K
                            else path.K + 1
                    {K=k';PathIndices=acc'}
                    )
            path::newPaths
    let rec loop paths currentRowIdx =
        if currentRowIdx > m.NumRows-1 then 
            paths
        else
            let expandedPaths = 
                paths
                |> List.collect (extendInAllPossibleDirections m maxK currentRowIdx)           
            loop expandedPaths (currentRowIdx+1)
    loop [{K=0;PathIndices=[0,0]}] 0

//
let calcSimilarityAt_simple k (specA:int[]) (specB:int[]) =
    let n :int = System.Math.Max(specA |> Array.max,specB |> Array.max)
    let vecA = 
        let tmp = FSharp.Stats.Vector.create (n+1) 0.
        specA
        |> Array.iter (fun p -> tmp.[p] <- 1.)
        tmp
    let vecB = 
        let tmp = FSharp.Stats.Vector.create (n+1) 0.
        specB
        |> Array.iter (fun p -> tmp.[p] <- 1.)
        tmp
    let m = vecA * vecB.Transpose
    let longestPath = 
        findAllPaths k m
        |> List.maxBy (fun p -> p.PathIndices.Length)
    longestPath.PathIndices.Length - 1


(**
Using this implementation we encourage you to read the paper "Mutation-Tolerant Protein Identification by Mass Spectrometry" (see References), 
which takes a deep dive into the topic. In the following code snippet we will apply our naive implementation to examples from the paper - something we also 
advise you to do - once you come up with your first implementation. 
*)
// Example 1
let sa = [|10; 20; 30; 40; 50; 60; 70; 80; 90; 100|]
let sb = [|10; 20; 30; 40; 50; 55; 65; 75; 85; 95|]
let sc = [|10; 15; 30; 35; 50; 55; 70; 75; 90; 95|]

// should be 10
let D_1_sa_sb =  calcSimilarityAt_simple 1 sa sb  

// should be 6
let D_1_sa_sc =  calcSimilarityAt_simple 1 sa sc  


// Example 2
let sd = [|7; 11; 15; 18; 21; 24; 30; 38; 43|]
let se = [|7; 11; 13; 19; 22; 25; 31; 33; 38|]

// the authors claim its 5.
let D_1_sd_se =  calcSimilarityAt_simple 1 sd se  

// should be 8
let D_2_sd_se =  calcSimilarityAt_simple 2 sd se  

(**
## References

Mutation-Tolerant Protein Identification by Mass Spectrometry, P. A. Pevzner et al. 2000

Efficiency of Database Search for Identification of Mutated and Modified Proteins via Mass Spectrometry, P. A. Pevzner et al. 2001

*)

(**
## Additional information

- We strongly advise to work through the paper "Mutation-Tolerant Protein Identification by Mass Spectrometry". It 
serves as a very good introduction into the topic and gives you many ideas on how to write an efficient implementation.

*)