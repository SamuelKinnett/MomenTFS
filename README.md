# MomenTFS

A C#/Eto application for viewing and exporting Digimon World [TFS map files](https://github.com/SamuelKinnett/MomenTFS/wiki/TFS-Files).

## Screenshots
![Screenshot3](https://github.com/SamuelKinnett/MomenTFS/blob/master/Assets/Screenshots/Screenshot3.png?raw=true)  
![Screenshot4](https://github.com/SamuelKinnett/MomenTFS/blob/master/Assets/Screenshots/Screenshot4.png?raw=true)

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
MomenTFS was based on code originally written by Lab 313 for their Delphi program TFSViewer.

Huge thanks also go to Romsstar and SydMontague for their amazing efforts in reverse engineering the MAP and TFS files. It's only thanks to them that the MAP reading is possible!

This project makes use of the following external libraries:
- [DiscUtils](https://github.com/DiscUtils/DiscUtils)
    - Copyright (c) 2008-2011, Kenneth Bell
    - Copyright (c) 2014, Quamotion
- [Eto](https://github.com/picoe/Eto)
    - Copyright (c) 2011-2019 Curtis Wensley
    - Copyright (c) 2012-2013 Vivek Jhaveri
