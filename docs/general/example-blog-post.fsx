(**
---
title: Example blog post
category: general
categoryindex: 1
index: 3
---
*)

(**
# Blog post title

**Table of contents**

## Introduction to the field

This is placeholder text with a [link](google.com). 

This is **bold placeholder text**. 

This is _italic placeholder text_. 

This is _**bold and italic placeholder text**_. 

## Dividing the problem into subproblems

I identified the following sub problems:

1. this

2. is

3. a 

4. list

### Subproblem 1

Lets get started writing some code:
*)

#r "nuget: Plotly.NET, 2.0.0-preview.6"

open Plotly.NET

/// some example data. see how this text is active when hovering over data in the post?
let data = [
    0., 1.
    1., 3.
    2., 6.9
    3., 1.337
]

(**
Here is some rad visualization:
*)

let myChart =
    Chart.Spline(data)
    |> Chart.withTitle "My first chart!"
 
(***hide***)
myChart |> GenericChart.toChartHTML
(***include-it-raw***)