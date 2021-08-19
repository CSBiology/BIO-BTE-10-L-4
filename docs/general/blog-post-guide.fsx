(**
---
title: Blog post guide
category: general
categoryindex: 1
index: 1
---

# Blog post guide

This is a guide on how to create content for the blog post that marks the final submission for your project work.

**Table of contents**

- [Setup](#setup)
  - [Indepth info for the interested](#indepth-info-for-the-interested)
    - [dotnet tools](#dotnet-tools)
    - [fsdocs](#fsdocs)
- [Creating content](#creating-content)
  - [Markdown](#markdown)
  - [Literate F# scripts](#literate-f-scripts)
  - [Including output](#Including-output)
  - [Including values](#Including-values)
  - [rendering charts](#Rendering-charts)
- [Submission guidelines](#submission-guidelines)
  - [General content guidelines](#general-content-guidelines)
  - [Submitting your blog post](#submitting-your-blog-post)

_! This document is WIP. Please finish your project before writing the blog post to prevent usage of outdated information. !_

## Setup

At this point you should have the [.NET SDK](https://dotnet.microsoft.com/download/dotnet/5.0) installed, as it is a mandatory installation. 
Otherwise we strongly recommend to consider first writing some code and getting into the project before reading to deep into how to create a blog post about your results.

That said, follow these steps to set up the blogpost project:

1. Create a new folder for your blogpost
2. Open a terminal (e.g. powershell/cmd on windows, bash on linux/macOS). Pro tip: Visual Studio Code has a built in terminal, which is very handy.
3. Navigate to the new folder (type `cd replace/this/text/with/path/to/your/folder`)
4. use the `dotnet new tool-manifest` command to create a dotnet tool resgistry in the folder. This is basically a document that tracks what tools can be used in your project.
5. Install the fslab template for fsdocs via `dotnet new -i FsLab.DocumentationTemplate`. You can learn more about this template [here](https://fslab.org/docs-template/)
6. Initialize the template via `dotnet new fslab-docs`. When asked for permission to install the fsdocs tool, type `y` to continue. 
7. Test the template via `dotnet fsdocs watch --eval --clean`. Here is what this means in a bit more detail:
   - The `dotnet` command prefix always means "use the dotnet CLI(command line interface) to do the following:"
   - `fsdocs` means you will use the `fsdocs` tool
   - `watch` means that you will use `fsdocs` in watch mode, an interactive mode where you will have the live output of your documents open in a local browser and can see the changes live.
   - `--eval` means that you want to evaluate (meaning execute) any F# script content. More on that later.
   - `--clean` means that you want to clean up any leftover stuff from the last run before starting.
   - There may be a question for permission to start the local webserver. Allow it. You should now see this in your browser:
    ![]()
8. You have now set up an interactive blog post creation engine. changes to files in your folder will be reflected on the webpage that opened in your browser. 
9. To stop the tool, either press any button in the terminal you are running it, or interrupt the process by pressing `ctrl + c` in the terminal.

### Indepth info for the interested

#### dotnet tools

Coming soon<sup>TM</sup>

#### fsdocs

Coming soon<sup>TM</sup>

## Creating content

If you followed the setup steps, you have a folder set up to create content in. You can always preview your content by running `dotnet fsdocs watch --eval --clean`.

Content for fsdocs can be created in two kinds of files: `markdown` (`.md`) files and `literate F# scripts`. The core difference is that you can only write styled text in markdown, while you can add real code in literate F# scripts.

### Markdown

Markdown is a set of easy to use formatting rules to create structured text. In fact, the very document you are reading is written in markdown.

for example, this is how to create a heading:

```
# Hi, i am a large heading
```

will become:

# Hi, i am a large heading

You may have realized that your initialized template contains a markdown cheatsheat. Use this cheatsheet for all your markdown needs. an online version is also available [here]({{root}}general/markdown_cheatsheet.html).

You might also have realized that the styling of your markdown looks different than this page (see the screenshot below for reference as well). 
That is not a problem and depends on the styling that fsdocs uses. please use the default one that is coming with the template.

![cheatsheet_screenshot](/img/markdown_cheatsheet.png)

### Literate F# scripts

Literate F# scripts are a powerfull fusion of markdown with F# scripting and advanced formatting methods. 

With literate F# scripts you can tell the full story from the problem to your solution and show of how it works and what kinds of results it produces.

To write markdown, just put it in these parentheses ``(** ... *)` like this: `(** # i am a heading! *)`. you can also do that across multiple lines.

```
    (**
    # Hi!
    *markdown here :D*
    *)
```

But the real awesome thing is that you can write normal F# code that will be rendered beautifully on your generated webpage.

It also contains hover tooltips! Try it by hovering over some of the code below with your mouse!
*)

let a = 42

///try hovering over myFunction!
let myFunction someParameter = printfn $"i got some {someParameter}!"

(** 

### Including output

there are multiple ways of including output of function calls or values of bindings.

To include the output of a function that returns unit (so for example a `printfn` call), put `(***include-output***)` below the call:

```
    printfn "Hi"
    (***include-output***)
```

which will be rendered as:
*)

printfn "Hi"
(***include-output***)

(**

### Including values

To include the value of a binding, use `(***include-value: yourBindingname***)`:

so this:

```
    let x = 42
    (***include-value:x***)
```

becomes:

*)

let x = 42
(***include-value:x***)

(**

### Rendering charts

Chart rendering is a special value inclusion. Because Plotly.NET charts create html already, the raw string value of them (created by `Chart.toChartHTML`) has to be included via `(***include-it-raw***)` like this: 

```
    #r "nuget: Plotly.NET, 2.0.0-preview.6"
    open Plotly.NET

    let myChart = Chart.Line([1,42; 2,69; 3,1337])

    myChart
    |> GenericChart.toChartHTML
    (***include-it-raw***) 
```
 which will be rendered as:
*)

#r "nuget: Plotly.NET, 2.0.0-preview.6"
open Plotly.NET

let myChart = Chart.Line([1,42; 2,69; 3,1337])

myChart
|> GenericChart.toChartHTML
(***include-it-raw***) 

(** 
You can also hide blocks via `(***hide***)`, a good use case is the inclusion of charts:

```
    let myChart2 = Chart.Spline([1,42; 2,69; 3,1337])

    (***hide***)
    myChart2
    |> GenericChart.toChartHTML
    (***include-it-raw***) 
```

which will omit the part of the code block that is only there to display the chart:
*)

let myChart2 = Chart.Spline([1,42; 2,69; 3,1337])

(***hide***)
myChart2
|> GenericChart.toChartHTML
(***include-it-raw***) 

(**

## Submission guidelines

### General content guidelines

### Submitting your blog post
*)