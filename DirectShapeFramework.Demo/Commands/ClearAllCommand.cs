using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DirectShapeFramework.Demo.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class ClearAllCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDocument = commandData.Application.ActiveUIDocument;
        var document = uiDocument.Document;

        using var t = new Transaction(document, "DSF_ClearAll");
        t.Start();

        Highlight.ClearAll(document);

        t.Commit();

        return Result.Succeeded;
    }

    private List<(FamilyInstance Instance, LocationPoint LocationPoint)> SelectPointBasedFamilyInstances(UIDocument uiDoc)
    {
        var collection = new List<(FamilyInstance, LocationPoint)>();
        foreach (var elementId in uiDoc.Selection.GetElementIds())
            if (uiDoc.Document.GetElement(elementId) is FamilyInstance { Location: LocationPoint locationPoint } instance)
                collection.Add((instance, locationPoint));
        return collection;
    }
}