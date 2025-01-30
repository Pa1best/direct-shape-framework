using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DirectShapeFramework.Utils;

namespace DirectShapeFramework;

/// <summary>
/// I will helps you to highlight invisible Geometry
/// </summary>
public static class Highlight
{
    private static View3D _defaultView;
    private static readonly string _defaultMarkPrefix = "DSF";
    private static readonly string _defaultViewName = "DSF View";

    /// <summary>
    /// Helps to highlight Geometry presented by single GeometryObject
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="geometryObject">GeometryObject to highlight</param>
    /// <returns>DirectShape object</returns>
    public static DirectShape Geometry(Document doc, GeometryObject geometryObject)
    {
        //create direct shape and assign the sphere shape
        var ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

        ds.SetShape(new[] {geometryObject});
        ds.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.Set(GenerateMark(doc));
        return ds;
    }

    /// <summary>
    /// Helps to highlight Geometry presented by List of GeometryObjects
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="geometryObjects">GeometryObjects to highlight</param>
    /// <returns>DirectShape object</returns>
    public static DirectShape Geometry(Document doc, IList<GeometryObject> geometryObjects)
    {
        var ds = DirectShape.CreateElement(doc, new ElementId(BuiltInCategory.OST_GenericModel));

        ds.SetShape(geometryObjects);
        ds.get_Parameter(BuiltInParameter.ALL_MODEL_MARK)?.Set(GenerateMark(doc));
        return ds;
    }

    /// <summary>
    /// Helps to highlight Rectangular Prism oriented in XYZ(0,0,0) coordinates
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="prismCenter">Central point of Prism. Is also is a location point</param>
    /// <param name="width"></param>
    /// <param name="depth"></param>
    /// <param name="height"></param>
    /// <returns>DirectShape object</returns>
    [UsedImplicitly]
    public static DirectShape RectangularPrism(Document doc, XYZ prismCenter, double width, double depth, double height)
    {
        var prism = GeometryUtils.CreateRectangularPrism(prismCenter, width, depth, height);

        return Geometry(doc, prism);
    }

    /// <summary>
    /// Helps to highlight Cube oriented in XYZ(0,0,0) coordinates
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="center">Central point of Cube. Is also is a location point</param>
    /// <param name="edgeLength"></param>
    /// <returns>DirectShape object</returns>
    [UsedImplicitly]
    public static DirectShape Cube(Document doc, XYZ center, double edgeLength)
    {
        var prism = GeometryUtils.CreateCube(center, edgeLength);
        return Geometry(doc, prism);
    }

    /// <summary>
    /// Helps to highlight Sphere
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="center">Central point of Sphere. Is also is a location point</param>
    /// <param name="radius"></param>
    /// <returns>DirectShape object</returns>
    [UsedImplicitly]
    public static DirectShape Sphere(Document doc, XYZ center, double radius)
    {
        var prism = GeometryUtils.CreateSphere(center, radius);
        return Geometry(doc, prism);
    }

    /// <summary>
    /// Helps to highlight element's Face
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="element">Host element</param>
    /// <param name="face">Face of host element to highlight</param>
    /// <returns>DirectShape object</returns>
    [UsedImplicitly]
    [Obsolete]
    public static DirectShape Face(Document doc, Element element, Face face)
    {
        var g = GeometryUtils.GetGeometryFromFace(face);
        return Geometry(doc, g);
    }
    
    /// <summary>
    /// Helps to highlight element's Face
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="face">Face of host element to highlight</param>
    /// <returns>DirectShape object</returns>
    [UsedImplicitly]
    public static DirectShape Face(Document doc, Face face)
    {
        var g = GeometryUtils.GetGeometryFromFace(face);
        return Geometry(doc, g);
    }

    /// <summary>
    /// Helps to highlight Plane
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="plane">Plane to highlight</param>
    /// <param name="width">represents width of generated geometry</param>
    /// <param name="height">represents height of generated geometry</param>
    /// <returns>DirectShape object</returns>
    [UsedImplicitly]
    public static DirectShape Plane(Document doc, Plane plane, double width = 10, double height = 10)
    {
        var g = GeometryUtils.CreatePlaneSolid(plane, width, height, 0.1);
        return Geometry(doc, g);
    }

    /// <summary>
    /// Creates a list of DirectShape spheres oriented according to chosen vector with chosen origin point
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="vector">Vector to highlight</param>
    /// <param name="start">Start point of this vector</param>
    /// <returns>List of DirectShape objects</returns>
    [UsedImplicitly]
    public static List<DirectShape> Vector(Document doc, XYZ vector, XYZ start)
    {
        var curve = Line.CreateBound(start, start + vector*2);
        var curveShapes= CurveByPoints(doc, curve);
        curveShapes.Add(Point(doc, start));
        return curveShapes;
    }

    /// <summary>
    /// Creates a DirectShape sphere with some radius (by default 0.3 feet)
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="point"></param>
    /// <param name="radius"></param>
    /// <returns>DirectShape object</returns>
    public static DirectShape Point(Document doc, XYZ point, double radius = 0.3)
    {
        var g = GeometryUtils.CreateSphere(point, radius);
        return Geometry(doc, g);
    }

    /// <summary>
    /// Creates a number of Direct Shape spheres with preselected steps (by default 0.01 feet)
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="curve"></param>
    /// <param name="step"></param>
    /// <returns>List of DirectShape objects</returns>
    public static List<DirectShape> CurveByPoints(Document doc, Curve curve, double step = 0.01)
    {
        var points = new List<XYZ>();
        for (double i = 0; i <= 1; i += step)
            points.Add(curve.Evaluate(i, true));
        var result = points.Select(point => Point(doc, point, 0.1)).ToList();
        result.Add(Geometry(doc, curve));
        return result;
    }

    /// <summary>
    /// Helps to highlight BoundingBox of some element
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="element">Element to retrieve Bounding Box</param>
    /// <returns>DirectShape object</returns>
    [UsedImplicitly]
    public static DirectShape BoundingBox(Document doc, Element element)
    {
        var bBox = element.get_BoundingBox(null);
        return BoundingBox(doc, bBox);
    }

    /// <summary>
    /// Helps to highlight BoundingBox 
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    /// <param name="bBox">BoundingBox to highlight</param>
    /// <returns>DirectShape object</returns>
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

    /// <summary>
    /// Removes all DirectShape elements and DSF default view from project
    /// </summary>
    /// <param name="doc">Revit Document you are working on</param>
    [UsedImplicitly]
    public static void ClearAll(Document doc)
    {
        doc.Delete(new FilteredElementCollector(doc).OfClass(typeof(DirectShape)).ToElementIds());
        if (_defaultView is {IsValidObject: true})
        {
            if (doc.ActiveView.Id == _defaultView.Id)
            {
                MessageBox.Show("This view can't be deleted while it is opened. Please open another view and try again");
                return;
            }
            doc.Delete(_defaultView.Id);
        }
    }

    /// <summary>
    /// Creates a special 3D View with predefined filters to show all DirectShapes with some color
    /// </summary>
    /// <param name="uiDoc"></param>
    [UsedImplicitly]
    public static void OnView3D(UIDocument uiDoc)
    {
        if (_defaultView is not {IsValidObject: true})
        {
            var doc = uiDoc.Document;
            var existingView = ViewUtils.GetView3DByName(doc, _defaultViewName);
            if (existingView == null)
            {
                using var t = new Transaction(uiDoc.Document, "DSF_Generate View");
                t.Start();

                _defaultView = ViewUtils.GenerateDsfView(uiDoc.Document,_defaultViewName);
                ViewFilterUtils.AddDsfFilter(doc, _defaultView, _defaultMarkPrefix);

                t.Commit();
            }
        }
        uiDoc.ActiveView = _defaultView;
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
}