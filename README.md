![logo](https://user-images.githubusercontent.com/68376046/177031470-779413b1-72bf-4b09-bce9-b8cee24c3bf9.png)

This light weight and easy-to-use library helps to visualize and analyze hidden Revit Geometry, Bounding Boxes, Vectors, lines, points and so on. 

## How it works?
This framework uses Revit API [DirectShape](https://www.revitapidocs.com/2017.1/bfbd137b-c2c2-71bb-6f4a-992d0dcf6ea8.htm#:~:text=This%20class%20is%20used%20to,may%20be%20assigned%20a%20category.) class to visualize hidden geometry as a real Generic Model instance. It helps you to analyze hidden geometry, check demensions and increase your performance in Revit API Geometry projects.

Click :point_up_2: on the image below :beginner: to watch a small gif demo of visualising element's Bounding Box using Direct Shape Framework.
![demo](https://user-images.githubusercontent.com/68376046/177167161-0eba6f1f-142d-45dd-89ae-1ca6442457c4.gif)

## Installation

For now you can download or copy local this project and build in release mode DirectShapeFramework library. Later I will set up this library as a .nuget package.

## Features

- [Highlight Geometry](#highlight-geometry)
- [Highlight Point](#highlight-point)
- [Highlight Vector](#highlight-vector)
- [Highlight Face](#highlight-face)
- [Highlight Curve](#highlight-curve)
- [Highlight BoundingBox](#highlight-boundingbox)


### Highlight Geometry
```c#
   IList<GeometryObject> geometry = element.get_Geometry(new Options()).Where(x=>x.IsElementGeometry).ToList();
   //Use this method inside transaction
   Highlight.Geometry(document, geometry);
   //overload for single GeometryObject
   Highlight.Geometry(document, geometry.First());
```

### Highlight Point
```c#
if (element is FamilyInstance familyInstance && familyInstance.Location is LocationPoint locationPoint)
   //Use this method inside transaction
   Highlight.Point(document, locationPoint.Point);
```

### Highlight Vector
```c#
if (element is FamilyInstance instance && instance.Location is LocationPoint locationPoint)
    //Use this method inside transaction
    Highlight.Vector(document, instance.FacingOrientation, locationPoint.Point);
```

### <a id="highlight-face">Highlight Face (Experimental)
```c#
var wallFaces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
var geometryObject = wall.GetGeometryObjectFromReference(wallFaces.First());
if (geometryObject is Face face)
   //Use this method inside transaction
   Highlight.Face(document, wall, face);
```

### Highlight Curve

### Highlight BoundingBox


## Supported Revit versions

DirectShapeFramework is based on .Net Framework 4.7.2. It is supported by Revit 2019 or higher.

## Found a bug or want to contribute?

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for more information about how to report a bug, suggest echancements and contribute.

## License

This project uses MIT license. Read [LICENSE.md](LICENSE.md) for more information.
