using System.Windows;
using Autodesk.Revit.DB;

namespace DirectShapeFramework.Utils;

internal static class GeometryUtils
{
    internal static Solid CreateSolidFromBoundingBox(BoundingBoxXYZ bBox)
    {
        // Corners in BBox coords

        var pt0 = new XYZ(bBox.Min.X, bBox.Min.Y, bBox.Min.Z);
        var pt1 = new XYZ(bBox.Max.X, bBox.Min.Y, bBox.Min.Z);
        var pt2 = new XYZ(bBox.Max.X, bBox.Max.Y, bBox.Min.Z);
        var pt3 = new XYZ(bBox.Min.X, bBox.Max.Y, bBox.Min.Z);

        // Edges in BBox coords

        var edge0 = Line.CreateBound(pt0, pt1);
        var edge1 = Line.CreateBound(pt1, pt2);
        var edge2 = Line.CreateBound(pt2, pt3);
        var edge3 = Line.CreateBound(pt3, pt0);

        // Create loop, still in BBox coords

        var edges = new List<Curve>();
        edges.Add(edge0);
        edges.Add(edge1);
        edges.Add(edge2);
        edges.Add(edge3);

        var height = bBox.Max.Z - bBox.Min.Z;

        var baseLoop = CurveLoop.Create(edges);

        var loopList = new List<CurveLoop> {baseLoop};

        var preTransformBox = GeometryCreationUtilities
            .CreateExtrusionGeometry(loopList, XYZ.BasisZ,
                height);

        var transformBox = SolidUtils.CreateTransformed(
            preTransformBox, bBox.Transform);

        return transformBox;
    }

    internal static Solid CreateRectangularPrism(XYZ center, double d1, double d2, double d3)
    {
        var profile = new List<Curve>();
        var profile00 = new XYZ(center.X - d1 / 2, center.Y - d2 / 2, center.Z - d3 / 2);
        var profile01 = new XYZ(center.X - d1 / 2, center.Y + d2 / 2, center.Z - d3 / 2);
        var profile11 = new XYZ(center.X + d1 / 2, center.Y + d2 / 2, center.Z - d3 / 2);
        var profile10 = new XYZ(center.X + d1 / 2, center.Y - d2 / 2, center.Z - d3 / 2);

        profile.Add(Line.CreateBound(profile00, profile01));
        profile.Add(Line.CreateBound(profile01, profile11));
        profile.Add(Line.CreateBound(profile11, profile10));
        profile.Add(Line.CreateBound(profile10, profile00));

        var curveLoop = CurveLoop.Create(profile);

        var options = new SolidOptions(
            ElementId.InvalidElementId,
            ElementId.InvalidElementId);

        return GeometryCreationUtilities
            .CreateExtrusionGeometry(
                new[] { curveLoop },
                XYZ.BasisZ, d3, options);
    }

    internal static Solid CreateCube(XYZ center, double d)
    {
        return CreateRectangularPrism(
            center, d, d, d);
    }

    [CanBeNull]
    internal static Solid CreateSphere(XYZ center, double radius)
    {
        var profile = new List<Curve>();
        var profilePlus = center + new XYZ(0, radius, 0);
        var profileMinus = center - new XYZ(0, radius, 0);

        profile.Add(Line.CreateBound(profilePlus, profileMinus));
        profile.Add(Arc.Create(profileMinus, profilePlus, center + new XYZ(radius, 0, 0)));

        var curveLoop = CurveLoop.Create(profile);
        var options = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

        var frame = new Frame(center, XYZ.BasisX, -XYZ.BasisZ, XYZ.BasisY);
        if (Frame.CanDefineRevitGeometry(frame)) return GeometryCreationUtilities.CreateRevolvedGeometry(frame, new[] {curveLoop}, 0, 2 * Math.PI, options);
        MessageBox.Show("Can't create Sphere with this parameters", "DSF");
        return null;
    }

    internal static IList<GeometryObject> GetGeometryFromFace(Face face, Element element)
    {
        var mesh = face.Triangulate();
        if (element is FamilyInstance familyInstance)
        {
            var t = familyInstance.GetTotalTransform();
            mesh = mesh.get_Transformed(t);
        }

        var builder = new TessellatedShapeBuilder();
        builder.OpenConnectedFaceSet(false);
        var triangleCorners = new XYZ[3];

        for (var i = 0; i < mesh.NumTriangles; ++i)
        {
            var triangle = mesh.get_Triangle(i);

            triangleCorners[0] = triangle.get_Vertex(0);
            triangleCorners[1] = triangle.get_Vertex(1);
            triangleCorners[2] = triangle.get_Vertex(2);

            var tessellatedFace = new TessellatedFace(triangleCorners, ElementId.InvalidElementId);

            if (builder.DoesFaceHaveEnoughLoopsAndVertices(tessellatedFace)) builder.AddFace(tessellatedFace);
        }

        builder.CloseConnectedFaceSet();
        builder.Target = TessellatedShapeBuilderTarget.AnyGeometry;
        builder.Fallback = TessellatedShapeBuilderFallback.Mesh;
        builder.Build();
        var result = builder.GetBuildResult();
        return result.GetGeometricalObjects();
    }
}
