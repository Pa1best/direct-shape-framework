using System.Windows;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace DirectShapeFramework.Demo.Commands;

[UsedImplicitly]
[Transaction(TransactionMode.Manual)]
public class HighlightVectorCommand : IExternalCommand
{
    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    {
        var uiDocument = commandData.Application.ActiveUIDocument;
        var document = uiDocument.Document;

        var instances = SelectPointBasedFamilyInstances(uiDocument);
        if (instances == null)
        {
            MessageBox.Show("Select Family Instance(s)");
            return Result.Failed;
        }

        using (var t = new Transaction(document, "DSF_Highlight Vector"))
        {
            t.Start();

            foreach (var instance in instances)
            {
                //Use this method inside transaction
                Highlight.Vector(document, instance.Instance.FacingOrientation, instance.LocationPoint.Point);
            }

            t.Commit();
        }

        //Use this method outside transaction
        Highlight.OnView3D(uiDocument);

        return Result.Succeeded;
    }

    private List<(FamilyInstance Instance, LocationPoint LocationPoint)> SelectPointBasedFamilyInstances(
        UIDocument uiDoc)
    {
        var collection = new List<(FamilyInstance, LocationPoint)>();
        foreach (var elementId in uiDoc.Selection.GetElementIds())
            if (uiDoc.Document.GetElement(elementId) is FamilyInstance
                {
                    Location: LocationPoint locationPoint
                } instance)
                collection.Add((instance, locationPoint));
        return collection;
    }
}