using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DirectShapeFramework.Demo.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class HighlightBiggestFaceCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDocument = commandData.Application.ActiveUIDocument;
        var document = uiDocument.Document;

        var instances = SelectFamilyInstances(uiDocument);
        if (instances == null)
        {
            MessageBox.Show("Select Family Instance(s)");
            return Result.Failed;
        }

        using var t = new Transaction(document, "DSF_Highlight Face");
        t.Start();

        var sdfIds = new List<ElementId>();

        foreach (var instance in instances)
        {
            var geometryInstance = instance.get_Geometry(new Options()).OfType<GeometryInstance>().FirstOrDefault();
            if (geometryInstance is null)
            {
                MessageBox.Show("This element doesn't have a geometry instance");
                return Result.Failed;
            }

            var instanceGeometry = geometryInstance.GetInstanceGeometry();
            var solids = new List<Solid>();
            foreach (var solid in instanceGeometry.OfType<Solid>())
            {
                solids.Add(solid);
            }

            var biggestFace = solids.SelectMany(x => x.Faces.OfType<Face>()).OrderBy(x => x.Area).Last();

            //Use this method inside transaction
            var dsf = Highlight.Face(document, biggestFace);
            sdfIds.Add(dsf.Id);
        }

        uiDocument.Selection.SetElementIds(sdfIds);

        t.Commit();

        //Use this method outside transaction
        // Highlight.OnView3D(uiDocument);

        return Result.Succeeded;
    }

    private List<FamilyInstance> SelectFamilyInstances(UIDocument uiDoc)
    {
        var collection = new List<FamilyInstance>();
        foreach (var elementId in uiDoc.Selection.GetElementIds())
            if (uiDoc.Document.GetElement(elementId) is FamilyInstance instance)
                collection.Add(instance);
        return collection;
    }
}