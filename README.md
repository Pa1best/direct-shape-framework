![logo](https://user-images.githubusercontent.com/68376046/177031470-779413b1-72bf-4b09-bce9-b8cee24c3bf9.png)

This light weight and easy-to-use library helps to visualize and analyze hidden Revit Geometry, Bounding Boxes, Vectors, lines, points and so on. It is extremely usefull for coordinators and Revit API programmers.

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
Creates a copy of selected element in the form of DirectShape element.

![Highlight Geometry](https://user-images.githubusercontent.com/68376046/201862275-df289d1a-49d2-4415-b44f-fa0bf599d6af.gif)

```c#
   IList<GeometryObject> geometry = element.get_Geometry(new Options()).Where(x=>x.IsElementGeometry).ToList();
   //Use this method inside transaction
   Highlight.Geometry(document, geometry);
   //overload for single GeometryObject
   Highlight.Geometry(document, geometry.First());
```

### Highlight Point
Placess a DirectShape point by entered coordinates. In this example element's location point is used.

![Highlight Point](https://user-images.githubusercontent.com/68376046/201863125-44dc9784-11ad-4867-987c-1ae14c7721f2.gif)

```c#
if (element is FamilyInstance familyInstance && familyInstance.Location is LocationPoint locationPoint)
   //Use this method inside transaction
   Highlight.Point(document, locationPoint.Point);
```

### Highlight Vector
Placess a DirectShape point by vectors start point and creates a visible line of predefined lenghth to show direction of vector. In this example element's FacingOrientation vector is used.

![Highlight Vector](https://user-images.githubusercontent.com/68376046/201863539-acdf291e-708e-4f8d-8e9d-a9c6a89e51ba.gif)

```c#
if (element is FamilyInstance instance && instance.Location is LocationPoint locationPoint)
    //Use this method inside transaction
    Highlight.Vector(document, instance.FacingOrientation, locationPoint.Point);
```

### <a id="highlight-face">Highlight Face (Experimental)
Places a DirectShape geometry to highlight selected Face.

![Highlight Face](https://user-images.githubusercontent.com/68376046/201866455-fae6d7ac-2ba2-44f4-9a45-ae30a457ee28.gif)

```c#
var wallFaces = HostObjectUtils.GetSideFaces(wall, ShellLayerType.Exterior);
var geometryObject = wall.GetGeometryObjectFromReference(wallFaces.First());
if (geometryObject is Face face)
   //Use this method inside transaction
   Highlight.Face(document, wall, face);
```

### Highlight Curve
Creates a number of DirectShape points to visualise a curve. In this example LocationCurve of wall was used.

![Highlight Curve](https://user-images.githubusercontent.com/68376046/201874166-b28c165f-d782-41c6-9ac8-4832ed9ac61d.gif)

```c#
if (instance.Location is LocationCurve locationCurve)
{
    Highlight.CurveByPoints(document, locationCurve.Curve);
}
```
### Highlight BoundingBox
Creates rectangular DirectShape element to visualise element's BoundingBox.

![Highlight BoundingBox](https://user-images.githubusercontent.com/68376046/201871396-b142ab80-3b9b-4fb6-ba4b-a9bf3060ae85.gif)

```c#
//Use this method inside transaction
Highlight.BoundingBox(document, instance.get_BoundingBox(null));
```

## Supported Revit versions

DirectShapeFramework is based on .Net Framework 4.7.2. It is supported by Revit 2019 or higher.

## Found a bug or want to contribute?

Please see [CONTRIBUTING.md](CONTRIBUTING.md) for more information about how to report a bug, suggest echancements and contribute.

## License

This project uses MIT license. Read [LICENSE.md](LICENSE.md) for more information.
