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
  - [Code examples](#code-examples)
  - [rendering charts](#rendering-charts)
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

### Indepth info for the interested

#### dotnet tools

#### fsdocs

## Creating content

### Markdown

### Literate F# scripts

### Code examples

### rendering charts

## Submission guidelines

### General content guidelines

### Submitting your blog post