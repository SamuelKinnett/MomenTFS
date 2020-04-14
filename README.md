# MomenTFS

A C#/Eto application for viewing and exporting Digimon World .TFS map files.

## Features
- View TFS files and switch between palettes
- Export images in the following formats:
    - JPEG
    - PNG
    - BMP
- Scan disc images for TFS files

## Planned Features
- Read associated .MAP files to extract sprite overlays, collision boundaries etc.
- Mass export images
- TFS editing

## Building
If you're only interested in using the core library, you only need to worry about building the MomenTFS project. Otherwise, build whichever project corresponds to your target platform (i.e. if you want to build for windows then build the .wpf project). As far as I'm aware there shouldn't be anything you need to directly install but if I'm wrong please correct me!

## Credits
MomenTFS was based on code originally written by Lab 313 for their Delphi program TFSViewer. Without the hard work put in by them to reverse engineer the file format this would've been much more of a headache. So wherever you are, guys, thank you!

This project makes use of the following external libraries:
- [DiscUtils](https://github.com/DiscUtils/DiscUtils)
    - Copyright (c) 2008-2011, Kenneth Bell
    - Copyright (c) 2014, Quamotion
- [Eto](https://github.com/picoe/Eto)
    - Copyright (c) 2011-2019 Curtis Wensley
    - Copyright (c) 2012-2013 Vivek Jhaveri
