using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DirectShapeFramework.Utils;

namespace DirectShapeFramework;

public static class Highlight
{
    private static View3D _defaultView;
    private static readonly string _defaultMarkPrefix = "DSF";
    private static readonly string _defaultViewName = "DSF View";

    public static DirectShape Geometry(Document doc, GeometryObject geometryObject)
    {
        //create direct shape and assign the sphere shape
        var ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

        ds.SetShape(new[] {geometryObject});
        ds.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.Set(GenerateMark(doc));
        return ds;
    }

    public static DirectShape Geometry(Document doc, IList<GeometryObject> geometryObjects)
    {
        var ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

        ds.SetShape(geometryObjects);
        ds.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.Set(GenerateMark(doc));
        return ds;
    }

    [UsedImplicitly]
    public static DirectShape RectangularPrism(Document doc, XYZ prismCenter, double d1, double d2, double d3)
    {
        var prism = GeometryUtils.CreateRectangularPrism(prismCenter, d1, d2, d3);

        return Geometry(doc, prism);
    }

    [UsedImplicitly]
    public static DirectShape Cube(Document doc, XYZ center, double edgeLength)
    {
        var prism = GeometryUtils.CreateCube(center, edgeLength);
        return Geometry(doc, prism);
    }

    [UsedImplicitly]
    public static DirectShape Sphere(Document doc, XYZ center, double radius)
    {
        var prism = GeometryUtils.CreateSphere(center, radius);
        return Geometry(doc, prism);
    }

    [UsedImplicitly]
    public static DirectShape Face(Document doc, Element element, Face face)
    {
        var g = GeometryUtils.GetGeometryFromFace(face, element);
        return Geometry(doc, g);
    }

    [UsedImplicitly]
    public static List<DirectShape> Vector(Document doc, XYZ vector, XYZ start)
    {
        var curve = Line.CreateBound(start, start + vector*2);
        var curveShapes= CurveByPoints(doc, curve);
        curveShapes.Add(Point(doc, start));
        return curveShapes;
    }

    public static DirectShape Point(Document doc, XYZ point, double radius = 0.3)
    {
        var g = GeometryUtils.CreateSphere(point, radius);
        return Geometry(doc, g);
    }

    public static List<DirectShape> CurveByPoints(Document doc, Curve curve, double step = 0.01)
    {
        var points = new List<XYZ>();
        for (double i = 0; i <= 1; i += step)
            points.Add(curve.Evaluate(i, true));
        var result = points.Select(point => Point(doc, point, 0.1)).ToList();
        result.Add(Geometry(doc, curve));
        return result;
    }

    [UsedImplicitly]
    public static DirectShape BoundingBox(Document doc, Element element)
    {
        var bBox = element.get_BoundingBox(null);
        return BoundingBox(doc, bBox);
    }

    [CanBeNull]
    public static DirectShape BoundingBox(Document doc, BoundingBoxXYZ bBox)
    {
        if (bBox == null)
        {
            MessageBox.Show("Element doesn't have BoundingBox", "DSF");
            return null;
        }

        var solid = GeometryUtils.CreateSolidFromBoundingBox(bBox);
        return Geometry(doc, solid);
    }

    [UsedImplicitly]
    public static void ClearAll(Document doc)
    {
        doc.Delete(new FilteredElementCollector(doc).OfClass(typeof(DirectShape)).ToElementIds());
        doc.Delete(_defaultView?.Id);
    }

    [CanBeNull]
    private static string GenerateMark(Document doc)
    {
        var existingElements = new FilteredElementCollector(doc).OfClass(typeof(DirectShape)).ToElements();
        var parameters = existingElements.Select(x => x.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.AsString()).ToList();
        var index = parameters.Count;
        string mark;
        do
        {
            mark = _defaultMarkPrefix + index++;
        } while (parameters.Contains(mark));

        return mark;
    }

    [UsedImplicitly]
    public static void OnView3D(UIDocument uiDoc)
    {
        if (_defaultView == null)
        {
            var doc = uiDoc.Document;
            var existingView = ViewUtils.GetView3DByName(doc, _defaultViewName);
            if (existingView == null)
            {
                using var t = new Transaction(uiDoc.Document, "DSF_GenerateView");
                t.Start();

                _defaultView = ViewUtils.GenerateDsfView(uiDoc.Document,_defaultViewName);
                ViewFilterUtils.AddDsfFilter(doc, _defaultView, _defaultMarkPrefix);

                t.Commit();
            }
        }
        uiDoc.ActiveView = _defaultView;
    }
}