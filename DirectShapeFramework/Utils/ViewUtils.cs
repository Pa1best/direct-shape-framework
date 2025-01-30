using Autodesk.Revit.DB;

namespace DirectShapeFramework.Utils;

internal class ViewUtils
{
    internal static View3D GetView3DByName(Document doc, string name)
    {
        using var collector = new FilteredElementCollector(doc);
        var view3d = collector
            .OfClass(typeof(View3D))
            .Cast<View3D>()
            .FirstOrDefault(x => x.Name == name);
        return view3d;
    }

    internal static View3D GenerateDsfView(Document doc, string viewName)
    {
        var viewFamilyType3D = new FilteredElementCollector(doc)
            .OfClass(typeof(ViewFamilyType))
            .Cast<ViewFamilyType>()
            .First(x => ViewFamily.ThreeDimensional == x.ViewFamily);

        var newView3d = View3D.CreateIsometric(doc, viewFamilyType3D.Id);

        if (newView3d == null) return null;
        newView3d.Name = viewName;
        newView3d.DetailLevel = ViewDetailLevel.Fine;
        newView3d.DisplayStyle = DisplayStyle.HLR;

        return newView3d;
    }
}