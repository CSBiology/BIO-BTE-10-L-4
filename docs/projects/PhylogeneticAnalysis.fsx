(**
---
title: Additive Phylogeny
category: projects
categoryindex: 2
index: 5
---
*)


(**
# Additive Phylogeny (no project to choose as WPB project)

This work-in-progress implementation is part of a contribution by Vanessa Leidel
that aims to implement Additive Phylogeny from Bioinformatics algorithms (Compeau & Pevzner). 
The first half has already been solved, but the back-stepping procedure is still missing.


## Introduction

Additive Phylogeny is a distance based Approach to construct phylogenetic trees from an additive distance matrix. A distancematrix is the representation of nodes (species) and edges (distance between species) as table and is additive,if you can recieve a unique simple unrooted tree fitting the distance matrix. In Real World Problems additive matrices are often not present, but parts of analysis are very similar to non-additive matrices. For this reason the implementation of the method additive Phylogeny is an important step in understanding computing evolutionary phylogenetic trees and solving special cases of real world problems.<br>
One of the central key points to recieve a additive phylogenetic tree is to  compute the limb length of an leaf j to its parent. (j represents the index of the species from interest) Since all matrixes have to be symmetric you can simply iter over it by using the rowindex, being equal to the corresponding columnindex. 
First you have to create a type Distancematrix for simulating the Matrix; consisting of a discription of the row and the column of matrix and the matrix itsself.


*)

(******)

#r "nuget: BioFSharp, 2.0.0-preview.3"
#r "nuget: BioFSharp.IO, 2.0.0-preview.3"
#r "nuget: FSharp.Stats, 0.5.0"
#r "nuget: FSharp.Data, 6.2.0"

open FSharp.Data
open BioFSharp
open BioFSharp.IO
open FSharp.Stats
open System.Collections.Generic
// Create Type DistanceMatrix with members species and DistanceM for representation of a distancematrix, connecting the different species by distances

type DistanceMatrix<'a> = { 
    Species : string [] 
    DistanceM : matrix
    } with 
        static member Create species distanceM = {Species=species; DistanceM=distanceM} 

// create testmatrix from Type Distancematrix, equal to matrix from the book "Bioinformatics Algorithm Chapter 7"
let myMatrixAdditive<'a> = 
    let distTest = 
        [| 
            [|0.;13.;21.;22.|] 
            [|13.;0.;12.;13.|] 
            [|21.;12.;0.;13.|]
            [|22.;13.;13.;0.|]
        |] 
        |> matrix
        
    DistanceMatrix<'a>.Create [|"i";"j";"k";"l"|] distTest


(**
## The limb length problem

<b>The first step</b> to get the corresponding phylogenetic tree of an additive distance matrix is to create a function Limblenthformula that computes the distance of a distinct leaf j to its parent leaf. Leaves are always present-day species. The formula needed for this function is:<br>
<b>Limblength(j) = (D(i,j)+D(j,k)-D(i,k))/2 </b> <br> where  i,j,k are three different species.

One further point you have to consider is that the specindex must be present in the matrix and not equal or bigger than the length of the Matrix, so you should introduce a error warning, by an if-clause, that automatically gives an error if the specIndex is not part of the matrix.

*)

// Formula for Limblength : Limblength(j) = D(i,j)+D(j,k)-D(i,k)/2
// specIndex 1 : present day species you want to know j , species 2 and 3: other Species in the tree  i and k 
// specCount: size of distanceMatrix --> number of species

let LimblengthFormula (distMat:matrix) (specIndex1:int) (specIndex2:int) (specIndex3:int) = 
    let specCount = distMat.NumRows
    if (specIndex1 >= specCount) || specIndex2 >=specCount || specIndex3 >= specCount then 
        failwithf "specIndex is not present in distance matrix" 
     
    (distMat.[specIndex2,specIndex1] + distMat.[specIndex1, specIndex3] - distMat.[specIndex2, specIndex3]) / 2. 


// Test of function
LimblengthFormula(myMatrixAdditive.DistanceM) 1  0 2 // 13 + 12 -21 / 2 = 2


(**
Because we want to get and compare all Limblength of a given species we have to use a for-loop which collect all Limblength for all possible Combinations of species. But you have to consider that the specIndexes have to be different, because the third node is a point to compare and is used as a reference. To get every possible interaction only once, because of the symmetry, you have to think about that the first index is always smaller than the 2nd. Furthermore choose always the smallest one limblength of a distinct species which represents the limblength of this species for the actual distancematrix
*)

// Determine all possible Limblength of given Species without taking a combination of species double --> can be done because symmetrie of matrix
// if the condition is fullfilled, you have to compute the Limblength stored in an Array. Last you only want the minima, 
// because this is the Limblength of the species for the matrix

let LimbLength (distMat: matrix) (specIndex: int) = 
    let specCount = distMat.NumRows
   
    [|for specIndex2 = 0 to (specCount-1) do 
        for specIndex3 = 0 to (specCount-1) do
            if (specIndex3 > specIndex2) && (specIndex3) <> (specIndex) && (specIndex) <> specIndex2 then 
                (LimblengthFormula (distMat) (specIndex) (specIndex2) (specIndex3))
    |]
    |>Array.min 

// test of Limblength
LimbLength myMatrixAdditive.DistanceM 3
(***include-it-raw***)

//LimbLength twentyNineMatrix.DistanceM 28


(**
## Trimming the tree
When we know the Limblength of any node in Tree(D), we can construct Tree (D) recursively using an algorithm described in the Chapter 7 of the Book "Bioinformatics Algorithms: An Active Learning Approach". <br>

<img src="../img/method.png" alt="drawing" width="30%"/> <br>

For this, you need an additive matrix representing Tree(D) and pick an arbitrary leaf j. The distances of leaf j to other present day specioes is then updated by reducing thr current distance in the corresponding column / row through the Limblength of j and create matrix D<sub>bald</sub>. D<sub>bald</sub> is the distancematrix, where the length of limb(j) is zero, because every part in the corresponding row /column is reduced by the limblength. This D<sub>bald</sub> matrix gives you informations about the length of edges between the species and where the Attachment point has to be. The Attachment of i and k to l is for example af the distance Dbald in line four of this picture and is an essential step during creation of matrix

*)


// Distance (species,j) - min Limblength(j)
// Construct the Dbald matrix where the row and column of the species from interest is modified by subtracting the value by the minimal Limblength

let getDbaldMatrixFormula (distMat:matrix)  (specIndexRow: int) (specIndexColumn: int) (specIndex: int)  =  
    if  distMat.[specIndexRow,specIndexColumn] = 0. then 
        0. 
    else
        distMat.[specIndexRow,specIndexColumn] - LimbLength (distMat) (specIndex)

// Test of function
getDbaldMatrixFormula (myMatrixAdditive.DistanceM) (1) (0) (1)
(***include-it-raw***)

(**
This function is used to change the row and column cooresponding to the species of this Limblength. For this a new matrix is build and filled with the old and the new values 
*)

//Connstruction of the Dbaldmatrix for every Value of the columnb / row
let dBaldMatrix (distMat:matrix) (specIndex: int)   = 
     let specCountRow = distMat.NumRows
     let specCountColumn = distMat.NumCols

     [|for x = 0 to specCountRow-1 do 
          [|for y = 0 to specCountColumn-1 do
               if x = specIndex || y = specIndex then 
                    getDbaldMatrixFormula distMat x y specIndex             
               else              
                    distMat.[x,y]            
          |]
     |]
     |> matrix     

// Test of function dBaldMatrix
dBaldMatrix myMatrixAdditive.DistanceM 1
(***include-it-raw***)

(**
The next step is removal of changed column and rows from the dBald matrix to get matrix D<sup>trimmed</sup> to get closer to the neighbor. For this the last edge / species has to be removed. The Construction of dBald and Dtrimmed is repeated until a length of 2*2 matrix is reached 
*)

// Trimming the row and column you have determined the Attached Point before to get a smaller distance matrix
let delete_species (distMat:matrix) (specIndex: int)  =
    distMat
    |> Matrix.removeColAt specIndex
    |> Matrix.removeRowAt specIndex 
    
// test of function
delete_species (dBaldMatrix (myMatrixAdditive.DistanceM) 1) 1
(***include-it-raw***)


(**
## Finding the attachment point


Another necessary function is attachmentPoint, where you try to identify the point ot attachment for a special leaf. For this especially the dBald matrix is necessary and the corresponding distances.
"To find the attachment point of a leaf j in TREE(Dtrimmed), consider TREE(Dbald), which is the same as TREE(D) except that LIMBLENGTH(j) = 0. From the Limb Length Theorem, we know that there must be leaves i and k in TREE(Dbald) such that (Dbald<sub>i,j</sub> + Dbald<sub>j,k</sub> - Dbald<sub>i,k</sub>) / 2 = 0 
This leads to the assumption that the attachment point for leaf j must be located at distance Dbald <sub>i,j</sub> from leaf i on the path connecting i and k in the trimmed tree. This attachment point may occur at an existing node, in which case we connect j to this node. On the other hand, the attachment point for j may occur along an edge, in which case we place a new node at the attachment point and connect j to it." This formula is also used in similar style at the recursive function in the end

Limblength : Dbald (i,k) = Dbald (i,j) + Dbald(j,k)   
*)

// Determine the Attachmentpoint for one species when it fullfills the defined conditions
let attachmentPoint (d_trimmed: matrix) (endpoint: int) =
    let specCount = d_trimmed.NumRows-1

    [|for i = 0 to specCount-1 do 
        for j = i+1 to specCount-1 do
            if d_trimmed.[i,j] = (d_trimmed.[i,endpoint] + d_trimmed.[endpoint,j]) then 
                   (i,j),d_trimmed.[i,(endpoint)]
    |]
    |> Array.groupBy (fun ((i,j),d) -> i)
    |> Array.maxBy fst
    |> snd
    |> Array.head
    
attachmentPoint (dBaldMatrix myMatrixAdditive.DistanceM 3) 3
(***include-it-raw***)


(**
## The four point condition

The four point condition is a simple way to determine if a matrix is additive, instead of looking if you can visit every node only once (p.52). It explains that a matrix is only additive, when the four point condition holds for every quartet of indices of this matrix. Because the following algorithm only works for additive matrices, you should test that before. The formula needed therefore is:

d<sub>i,j</sub> + d<sub>k,l</sub> < = d<sub>i,k</sub> + d<sub>j,l</sub> = d<sub>i,l</sub> + d<sub>j,k</sub>
*)

//  four-point condition, condition needed to be additive
let fourpoint_condition (distMat: matrix) (specIndex0:int) (specIndex1: int) (specIndex2: int) (specIndex3: int) = 
    let specCount = distMat.NumRows
    if (specIndex0 >= specCount) || specIndex1 >=specCount ||specIndex2 >= specCount ||specIndex3 >= specCount then 
        failwithf "specIndex is not present in distance matrix"
    
    let sum1 = distMat.[specIndex0,specIndex1] + distMat.[specIndex2,specIndex3]
    let sum2 = distMat.[specIndex0,specIndex2] + distMat.[specIndex1,specIndex3]
    let sum3 = distMat.[specIndex0,specIndex3] + distMat.[specIndex1,specIndex2]

    (sum1 = sum2 && sum1 > sum3) || (sum1 = sum3 && sum1 > sum2) || sum2 = sum3 && sum2 > sum1

(**
The dynamic version of testing Additivity looks as first condition, if we have a symmetric matrix, because this is necessary. Than you proof for every possible 4 points in the matrix if it fullfills the four point condition if not you should fail. To have every possible combination only once, you take care that the specIndex can always not be the same species
*)

// test if the matrix is additive for every possible four points in matrix
let testingAdditivity (distMat: matrix)   = 
    let specCount = distMat.NumRows
    let rowlength = distMat.NumRows
    let columnlength = distMat.NumCols

    if rowlength <> columnlength then failwith " Matrix isnt symmetric, but has to be to have a phylogenetic tree"
    
    for specIndex0 = 0 to (specCount-1) do 
            for specIndex1 = 0 to (specCount-1) do
                for specIndex2 = 0 to (specCount-1) do 
                    for specIndex3 = 0 to (specCount-1) do
                            if  
                                specIndex0 < specIndex1 
                                && specIndex0 < specIndex2 
                                && specIndex0 < specIndex3
                                && specIndex1 < specIndex2 
                                && specIndex1 < specIndex3
                                && specIndex2 < specIndex3
                            then 
                                if not (fourpoint_condition distMat specIndex0 specIndex1 specIndex2 specIndex3) then 
                                    failwithf "Matrix is non additive, but it has to be additive to fit a tree"

// test matrix of book for additivity
fourpoint_condition myMatrixAdditive.DistanceM 0 1 2 3
testingAdditivity myMatrixAdditive.DistanceM

(**
Here we test a 29*29 matrix for additivity to proof if the algorithm also works for bigger matrices
*)

let twentyNineMatrix<'a> =
    let test =
        let rawData = Http.RequestString @"https://bioinformaticsalgorithms.com/data/extradatasets/evolution/Additive_Phylogeny.txt"
        rawData.Split '\n'
        |> Array.skip 2
        |> Array.take 29
        |> Array.map (fun x ->
            x.Split ' '
            |> Array.take 29
            |> Array.map float    
        )  
        |> matrix

    DistanceMatrix<'a>.Create [|"i";"j";"k";"l";"m";"n";"o";"p";"q";"r";"s";"t";"u";"v";"w";"x";"y";"z";"a";"b";"c";"d";"e";"f";"g";"h";"ii";"jj";"kk"|] test

// Testing of Additivity from 29*29 matrix
testingAdditivity twentyNineMatrix.DistanceM
(***include-it-raw***)


(**
Testing of the algorithm with a nonadditive matrix
*)

// Create Non Additive Matrix for Testing
let myNonAdditiveAdditive<'a> = 
    let distTest = 
        [| 
            [|0.;3.;4.;3.|] 
            [|3.;0.;4.;5.|] 
            [|4.;4.;0.;2.|]
            [|3.;5.;2.;0.|]
        |] 
        |> matrix
        
    DistanceMatrix<'a>.Create [|"i";"j";"k";"l"|] distTest

// Test of an Non Additive Matrix 
fourpoint_condition myNonAdditiveAdditive.DistanceM 0 1 2 3
testingAdditivity myNonAdditiveAdditive.DistanceM 
(***include-it-raw***)

(**
##  An algorithm for distance-based phylogeny construction

After constructing all essential functions for the additive phylogeny, we can "engineer" a recursive algorithm <b> AdditivePhylogeny </b>, for finding a simple tree fitting the n*n additive distance matrix D. 
For the implemented function Limb (D,j), computing Limblebgth(j) you have now to consider that you use the last row in D instead of selecting a arbitrary leaf j from the Tree(D). As template for the recursive Algorithm,you can use the scheme on p.18

*)

type PhylTreeConnection = { 
    SourceToTargetIndex: (int*int)
    Distance: float
    } with 
        static member Create sourceToTargetIndex distance = { SourceToTargetIndex= sourceToTargetIndex; Distance=distance} 

(**
The steps to get succesfull the corresponding phylogenetic tree of a additive distancematrix are described in the following key points:
- testing of distanceMatrix: Call the function testingAdditivity for the original Matrix for testing if matrix is additive and cancel program if matrix is not additive
- define needed variables of Algorithm
- Start Algorithm describing the recursive function with original matrix and n discribing the size / number of colummns (remember in informatics, we always start with index 0)
    - When we just have two columns (n = 1), then we can easily determine the phylogenetic tree, because the distance between both species is known, so we can add that to the list resultlist and follwing conditions are skiped
    - if we have a bigger matrix, then we need to "analyze" the matrix in some steps:
        - put conditions in a while loop, which ends when n smaller one to reduce it until n smaller two
        - while loop computes for every matrix first the corresponding limnblength for the last column and updates then the distancematrix `dist_mat` by using the d-Bald function
            - Store for every distance matrix the `d_Bald` Matrix as well as size and limblength in a mutable dictionary graph
            - furthermore we store for every matrix the attachment points representing degenerate triples which satisfies Di,j = Di,n-1 + Dj,n-1 and store them in a 2nd mutable dictionary. This attachment points are needed to add possible 
            - in the end we update n, because n is needed to be equal to the size of matrix 
        - if n is now smaller two, than we add last the two* two matrix to the dictionary manually and take 0 as limblength, now we are able to reconstruct the phylogenetic tree. For zjid we also define path as the distance between i and j connected to each other
        - Next we need to integrate new nodes, if v is not already part of the path
*)

let addPhyl (distMat:matrix) =
    (* Hier wird getestet ob die in die Funktion gegebene distanzmatrix die Anforderungen einer additiven Matrix erfüllt, 
    da der Algorithmus nur für eine additive Matrix funktionsfähig ist. Dafür wird die Funktion testingAdditivity aufgerufen.
    *)
    testingAdditivity distMat

    // Hier werden verschiedene Variablen (zum Teil veränderbar) deklariert. Diese werden im Laufe des Algorithmus benötigt und müssen zum Teil veränderbar sein

    // veränderbare Variable, welche die aktuelle Version der Matrix beinhaltet, da die Matrix verändert werden muss (Stichwort Dbald)
    let mutable dist_Mat = distMat

    // veränderliche Varialble, die immer auf die letzte Reihe / Spalte der aktuellen Matrix verweist  
    let mutable n = dist_Mat.NumRows-1 

    // Unveränderliche Variable, welche auf die Länge der InputMatrix verweist
    let lengthOfMatrix = distMat.NumRows-1

    // Hier wird eine leere Liste vom Typ PhylTreeConnection deklariert in der die Pfade am Ende gelistet sind und der Tree beschrieben wird 
    let mutable resultList: PhylTreeConnection list = []

    (*  Hier werden twei mutable dictionaries deklariert. graph ist dafür notwendig, alle Matrixen zu speichern und damit rückwärts auflösen zu können  
        und enthält den Wert von n als Schlüssel ebensp wie der attachmentsstore der src,target und den attachmentPoint speichert. 
    *)
    let graph = new Dictionary<int,(float*matrix) >()
    let attachmentsStore = new Dictionary<int,((int*int)*float)>()

    // internalNodes ist die variable, welche den Index des einzufügenden Knoten deklariert
    let mutable internalNodes = 2*dist_Mat.NumRows-3
    
    
    (* Hier wird geprüft, ob eine 2*2 Matrix vorliegt, oder eine größere Matrix.
        Wenn eine 2*2 Matrix vorliegt, dann muss der Algorithmus nicht weiter durchlaufen werden 
        und es kann direkt der phylogenetische Baum, bestehend aus einer einfachen Verbindung der beiden Spezies erstellt werden.
        Die Länge der Verbindung entspricht der Distanz zwischen den beiden Spezies. Folglich wird  der Baum in der Liste 
        dargestellt durch den Eintrag von Source to Target (0,1) bzw. (1,0) und der Distanz zwischen beiden Spezies und die Liste wird zurückgegeben   
    *)
    if n = 1 then
        let distance = dist_Mat.[0,1]
        resultList <- {SourceToTargetIndex = (0,1); Distance = distance}::resultList
        resultList <- {SourceToTargetIndex = (1,0); Distance = distance}::resultList
        resultList

    // Diese Verzweigung wird ausgeführt falls keine 2*2 Matrix vorliegt
    
    else

    (*  
        Hier werden die in Zeile 24 und 25 deklarieren Dictionaries graph und attachmentsStore solange gefüllt bis eine Matrixgröße 2+2 erreicht ist.
        Das Dictionary graph beinhaltet dabei als Schlüssel n und ordnet diesem die jeweilige limbLength sowie die Distanzmatrix zu aus welcher die aktuelle Pfad Länge entnommen werden kann
        Im Dictionary attachmentsStore werden die berechneten attachmentpoints von zwei Spezies gespeichert sowie src und target
    *)
        while n >= 2 do
            let limblength = LimbLength (dist_Mat) n
            dist_Mat <- dBaldMatrix dist_Mat n 
            graph.Add (n,(limblength,dist_Mat))

       
           
             (*  printfn "n: %i" n
                printfn "limblength:  %f %A" limblength dist_Mat
                Beispielhafte Ausgabe wenn n = 3 erreicht ist, zur Ansicht was in dictionary graph gespeichert wird
             
             n: 3
             limblength: 409.000000 
                    0        1        2        3
                                                
            0 ->    0.000 3036.000 4777.000 1132.000
            1 -> 3036.000    0.000 6323.000 2678.000
            2 -> 4777.000 6323.000    0.000 3645.000
            3 -> 1132.000 2678.000 3645.000    0.000
             *)

            let attachment = attachmentPoint dist_Mat n
            let x = snd attachment 
            let (src,target) = fst attachment
            
            attachmentsStore.Add(n,((src,target),x))
            (* printfn " n: %i x: %f src: %i target:%i" n x src target 
            Beispielshafte Ausgabe wenn n = 3 erreicht ist in der 29*29 Matrix aus Rosalind
            n: 3 x: 2678.000000 src: 1 target:2
            *)
                
            // Hier wird die Matrix getrimmt und n geupdatet sodass n wieder dem Index der letzten Reihe / Spalte entspricht

            dist_Mat <- delete_species dist_Mat n
            n <- n-1
        
        (*
        Bis hier wurde die distanzmatrix iterativ reduziert und die jeweiligen Attachmentpoints und limblength in graph und attachmentsStore gespeichert
        Nun muss im TraceBack-Schritt in jeder Iteration ein initial graph bnestehen aus node 0 und 1 mit allen fehleden nodes vervollständigt werden. Diese werden 
        an den punkten aus attachmentstore angefügt mit der limblength, die in graph gespeichert ist. 
        Dafür wird zunächst der aktuelle Pfad der in der Matrix als Schnittpunkt 0,1 dargestellt ist in resultList eingefügt 
        *)

        resultList <- {SourceToTargetIndex = (0,1); Distance = distMat.[0,1]}::resultList
        resultList <- {SourceToTargetIndex = (1,0); Distance = distMat.[1,0]}::resultList

        // Hier wird n wieder angepasst sodass n = 2 ist und damit dem kleinsten Schlüssel in den Dictionaries entspricht
        n <- n+1

        (*  
            Im folgenden Schritt wird der traceback durchgeführt und nach und nach die bestimmten attachmentpoints eingefügt. 
            Dafür wird eine zweite While loop erstellt die solange wie n nicht dem maximalen Specindex entspricht den Traceback durchführt.
        *)
        let traceback =
            while n < lengthOfMatrix do (

            
                (*Hier wird die aktuelle resultList für jeden Durchgang geprintet:  printfn "resultList: %A" resultList, Bsp. zu Beginn und nach dem ersten Durchlauf  
                resultList zu Beginn: [{ SourceToTargetIndex = (1, 0) Distance = 3036.0 }; { SourceToTargetIndex = (0, 1) Distance = 3036.0 }]
                resultList nach Durchgang 1:[{ SourceToTargetIndex = (0, 55) Distance = 745.0 }; { SourceToTargetIndex = (55, 0) Distance = 745.0 }; 
                                             { SourceToTargetIndex = (55, 1) Distance = 2291.0 };
                                             { SourceToTargetIndex = (1, 55) Distance = 2291.0 }; { SourceToTargetIndex = (55, 2) Distance = 4032.0 }; 
                                             { SourceToTargetIndex = (2, 55) Distance = 4032.0 }]
                *) 


                // Take the currentGraphElement
                let (limblength,dMatrix) = graph.[n]
                // Take the currentAttachmentPoint
                let ((src,trg),distance) = attachmentsStore.[n]

        
                (* Hier kann Beispielshaft durch den Print Befehl überprüft werden; ob der  Attachmentpoint richtig übertragen wurde - Ausgabe für twentyNineMatrix

                    printfn "index: %i, src: %i, trg: %i, distance: %f"n src trg distance
                    index: 2, src: 0, trg: 1, distance: 745.000000
                    index: 3, src: 1, trg: 2, distance: 2678.000000

                *)


                (* Ab hier ist der Code noch nicht vollständig funcktionsfähig, da aktuell nur nach edgestomodify gesucht wird die genau dem src, target entsprechen, dies ist allerdings nicht ausreichend, da vorher bereits Pfade verändert wurden und im Pfad zwischen 1 und 2 z.B. schon eine internalNode eingefügt wurde
                    Problem: Es soll nodeindex 3 auf dem path zwischen nodes 1 und 2 angefügt werden, aber dieser path ist nicht verfügbar weil im vorigsen schritt schon 
                    ein artificail node (55) zwischen 1 und 2 eingefügt wurde. Deswegen müsste man resultlist jetzt untersuchen nach dem Path 1 -> 2 und die edge identifizieren an der 
                    der zusätzliche node (54) angefügt werden soll (aufgrund der distanzen 1-55: 2291 und 55-2: 4032 müsste node 54 demnach zwischen 55 und 2 in der Distanz 387 eingesetzt werden*)  

                let edgeToModify = 
                    resultList |> List.find (fun element -> 
                        element.SourceToTargetIndex = (src,trg) ||
                        element.SourceToTargetIndex = (trg,src) 
                        )

                // Hier soll der inverse Path von edge to Modify (Bsp. von 0,1 => 1,0) bestimmt und zugeordnet werden, da dieser auch aus der Liste entnommen werden muss
                let edgeToModifyInnverse = {SourceToTargetIndex = (snd (edgeToModify.SourceToTargetIndex),fst (edgeToModify.SourceToTargetIndex)); Distance = edgeToModify.Distance}
               
                (* Hier wird das im vorherigen Codeabschnitt bestimmte edgeToModify und der inverse Pfad bestimmt
                    printfn " edgetomodify: %A , edgetomodify inverse: %A"     edgeToModify edgeToModifyInnverse 
                    edgetomodify: { SourceToTargetIndex = (1, 0) Distance = 3036.0 } , edgetomodify inverse: { SourceToTargetIndex = (0, 1) Distance = 3036.0 }
                *)

                (* Hier werden die Pfade bestimmt, die in resultList hinzugefügt werden sollen und an welcher Stelle ein Knoten eingefügt werden soll
                    edgeAdditionA: Beschreibt den Pfad zwischen der Quelle und dem einzufügnden internal Node (z.B. 0 -55 ==> Distance 745 oder 55-54 ==>  distance 387 )
                    edgeAdditionB: Beschreibt den Pfad zwischen InternalNodes und dem target also von z.B. 55-1 ==> 2291 oder 54 -2 ==> 1354. 
                    Dafür muss die edgeToModiy Distanz von den einzelnen Distanzen ( Distanzen der beteiligten Teilpfade / Attachmentpoindistanz) abgezogen werden.
                    edgeAdditionC: Hier wird die aktuelle Spezies wieder zum Baum hinzugefügt im Abstand der limblength zum aktuellen InternalNode ( z.B. im ersten Schritt wird Spezies 2 wieder eingefüht)
                *)
                
                let edgeAdditionA = {SourceToTargetIndex = (src,internalNodes); Distance = distance}
                let edgeAdditionA_inverse = {SourceToTargetIndex = (internalNodes,src); Distance = distance}
                let edgeAdditionB = {SourceToTargetIndex = (internalNodes,trg); Distance = edgeToModify.Distance - distance}
                let edgeAdditionB_inverse = {SourceToTargetIndex = (trg, internalNodes); Distance = edgeToModify.Distance - distance}
                let edgeAdditionC = {SourceToTargetIndex = (internalNodes,n); Distance = limblength}
                let edgeAdditionC_inverse = {SourceToTargetIndex = (n,internalNodes); Distance = limblength}

                (* Beispielshafte Ausgabe nach dem ersten Durchlauf von edgeAddition A , edgeAddition B und edgeAddition C 
                    Edgeaddition A: { SourceToTargetIndex = (0, 55) Distance = 745.0 } 
                    edgeAdditionB: { SourceToTargetIndex = (55, 1) Distance = 2291.0 } 
                    edgeAdditionC: { SourceToTargetIndex = (55, 2) Distance = 4032.0 } 
                                                                                     *)
                // Aktualisieren von internal Nodes 

                internalNodes <- internalNodes-1

                 (* Hier wird der phylogenetische Baum  geupdated. 
                     Dafür werden die zu verändernden Pfade zunächst aus der Liste entferent und anschließend werden die vorher bestimmten Pfade der Liste hinzugefügt.
                     Zuletzt wird der übersichtlichkeithalber die Liste nach den Src sotiert und n geupdatet. Nach Beendigung der While Loop wird zuletzt die fertige resultList, 
                     welche den Baum beschreibt ausgegeben 
                *)

                let newResultList = 
                    resultList
                    |> List.where  (fun x -> x <> edgeToModify && x <> edgeToModifyInnverse)
                    |> List.append [edgeAdditionA;edgeAdditionA_inverse;edgeAdditionB;edgeAdditionB_inverse;edgeAdditionC;edgeAdditionC_inverse]
                    |>List.sortBy (fun x -> fst (x.SourceToTargetIndex))
                resultList <- newResultList
                n <- n+1
                )

        resultList
      
        
(* Beispielshafte Ausgabe zur Darstellung des Problems
printfn "resultList: %A" resultList
printfn "index: %i, src: %i, trg: %i, distance: %f"n src trg distance

resultList: [{ SourceToTargetIndex = (1, 0)
   Distance = 3036.0 }; { SourceToTargetIndex = (0, 1)
                          Distance = 3036.0 }]
index: 2, src: 0, trg: 1, distance: 745.000000
resultList: [{ SourceToTargetIndex = (0, 55)
   Distance = 745.0 }; { SourceToTargetIndex = (55, 0)
                         Distance = 745.0 }; { SourceToTargetIndex = (55, 1)
                                               Distance = 2291.0 };
 { SourceToTargetIndex = (1, 55)
   Distance = 2291.0 }; { SourceToTargetIndex = (55, 2)
                          Distance = 4032.0 }; { SourceToTargetIndex = (2, 55)
                                                 Distance = 4032.0 }]
index: 3, src: 1, trg: 2, distance: 2678.000000

*)

addPhyl twentyNineMatrix.DistanceM
addPhyl myMatrixAdditive.DistanceM


(**
## Test of some functions

Now we can construct the algorithm, which connects all prepared functions. This will be called addPhy and needs only the distanceMatrix and n as input. the distanceMatrix is called distMat and from type matrix, n is from type int and represents the species. First the distancematrix is tested for additivity if not you fail. Then you define dirst the length of matrix and assign it to speccount and you define the starting point for nodeName and assign it to internalNodelabel. For this the first internalNodelabel ist the double of matrixlength minus 3 e.g. for matrixAdditive with length 4 its 5.<br>
After definition of variables we can now call the recursive function `addPhyl_helper`, that need again distMat, n and internalNodelabel. This recursive function first proofs first if n is equal two, meaning that there is only two species. If this is fullfilled then you assign to distance the distance between this two matrices and add that to the dictionary. You can simply add that by using the Add function. When n is not equal two than, we directly determine the limblength and compute the d<sub>Bald</sub> Matrix. The `d_Bald` Matrix is now assigned to distMat and so the matrix is updated Next we need to assign the attachmentpoint to x. For this we use the function attachmentPoint, that find degenerate triples which satisfies Di,j = Di,n-1 + Dj,n-1. This formula takes dBald and n as input and wen need for x the attachmentPoint distance. Additionally we store in an Tuple (src,target) the coordinates of src and target to get attachment point of n. When this is done you call recursively the function you create and set the distanceMatrix as dBald and reduce n with one, also internalnodeLabel has to be reduced by one. By this step the graph (`d_trimmed`) is directly reduced and the result is assigned to the graph
Next we insert the new node on the path betwenn source and target determined during finding of attachment point. For this first the boolean array visited must be declared. At the Beginning of DFS all nodes first are false, for this reason an Array is created that has the size of the matrix and which has all values set false. Furthermore we call `insert_new_node`, which takes as input the actual dictionary, the src and target defined by finding the attachmentpoint and stored in a tuple, x and the actual array visited as well as internalNodelabel. Last we add leaf n-1 back to new node by creating a limb of length limbLength. for this the src node gets a new entry and also the internalNodelabel gets a new entry

## Algorithm translated in F# - Inser new nodes

To delete edges directly from the graph and not from matrix you have to work a little bit different. The function needs three different variables as input. Thia are `d_trimmed`, src and the target. `d_trimmed` represents an Dictionary consisting of an int value as Key and an List with tuples, describing the path where all species are visited once and highly interconected. This dictionarey is a discription of the distancematrix to reduce the graph during the recursive function. src represents the starting point and target the ending point of an path. To update the corresponding List, we use the <-  for updating this file and get no problems with the type of value

To insert internal nodes, which connect species and allow to compute back to the distancematrix, we need the function `get_path`, which is called by a further function get insert node. First we create the function `get_path` which has as input again `d_trimmed` the src and the target. Furthermore an Boolean array vsisited is the input. Visited represents s list, where you store for each distance, representing an path in the phylogenetic tree, if this path / edge is already visited or not. The function is divided in different steps. In General you want to compute the path by using the Depth first algorithm:<br>

<img src="../img/DFS.png" alt="drawing" width="40%"/> <br>

First you need to define two mutable variables:

- `path_list`: is a list, where you store the path for a distinct source and target and, where you can add the steps in the path simply

- `path_rec`: variable which gets the result of the recursive called function `get_path` and takes first None, describing that it has actually no value 

Then the function first remembers that the src, representing the starting point, is already visited and changes this field in the Array in an true then you need to iter over the complete list in the Dictionary at an special key, here the key representing the starting node you want. This iteration can be done by using an simple for loop iterating over every tuple and doing the following statements. 

- first you need to proof if v is already visited, because v represents now the next pointr and when its visited then the path is already known (avoid doubling of pathes)

- If you have a non visited point then the next condition is to proof if v is equal to the target  then you add to the pathlist both tuples and has the path of path

- next you simply call recursive again the function by assigning the src to v and call that there is some vale, last `path_list` is filled by the src and w an internal node


*)

// Return a path from src to dst by DFS Depth first search.
// Path will be like [(src, w1), (v1, w2), ..., (v_k-1, w_k), (dst, 0)].
//  Define the path between two nodes bx using DFS and determining all visited nodes

let rec get_path(d_trimmed:Dictionary<int,(int*int) list>) (src: int) (target: int) (visited: Boolean array) =
    let mutable path_list: list<(int*int)> = []
    let mutable path_rec = None

    visited.[src] <- true  

    for (v, w) in d_trimmed[src] do
        if not visited[v] then 
            if v = target then( 
                path_list <- (target,0):: path_list
                path_list <- (src,w):: path_list
            )
            
            path_rec <- Some (get_path (d_trimmed) (v) (target) (visited))

            path_list <- (src,w) :: path_list
            
    path_list        


(**
The snd formula needed is `insert_new_node`, which contains additionally to the variables of get_path the distance and an internalnodeLabel. distance discribes at the beginning the complete length of an path from source to target and is assigned to remainLength, that is mutable. internalnodeLabel represents the Nodename of the corresponding nodes and stores this list. Then you can create the function needed to insert the new node: 

- First we call the forward function by using the actual value
- Create mutable variables: 
    - curr gets first zero as value and is the pointer to the called node on the path
    - edgelength gets first the target node of the first tuple in the list, describing the length of the path / edge
    - remaininglength corresponds first to the complete distance between src and target 
- update values in dependence of that remainingLength is bigger than the edgeLength by using an while loop
- currNode and nextNode are defined by taking the snd value in the tuple representing the current node from interest and the next node (species)
- update field in the dictionary representing the node and how its connected , with help of List.append
- delete actual node and next node from the dictionary, representing later for every node how its connected     

 
*)

