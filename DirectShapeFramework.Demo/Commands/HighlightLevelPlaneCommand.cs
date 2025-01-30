using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DirectShapeFramework.Demo.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class HighlightLevelPlaneCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDocument = commandData.Application.ActiveUIDocument;
        var document = uiDocument.Document;

        var levels = SelectLevels(uiDocument);
        if (levels == null)
        {
            MessageBox.Show("Select Family Instance(s)");
            return Result.Failed;
        }

        using var t = new Transaction(document, "DSF_Highlight Level Plane");
        t.Start();

        var sdfIds = new List<ElementId>();
        foreach (var level in levels)
        {
            var elevation = level.Elevation;

            // Create a horizontal plane in the XY direction
            var origin = new XYZ(0, 0, elevation); // Origin at the level's elevation
            var normal = XYZ.BasisZ; // Normal in the Z direction

            var levelPlane = Plane.CreateByNormalAndOrigin(normal, origin);
            Highlight.Plane(document, levelPlane, 100, 100);
        }

        uiDocument.Selection.SetElementIds(sdfIds);

        t.Commit();

        return Result.Succeeded;
    }

    private List<Level> SelectLevels(UIDocument uiDoc)
    {
        var collection = new List<Level>();
        foreach (var elementId in uiDoc.Selection.GetElementIds())
            if (uiDoc.Document.GetElement(elementId) is Level level)
                collection.Add(level);
        return collection;
    }
}