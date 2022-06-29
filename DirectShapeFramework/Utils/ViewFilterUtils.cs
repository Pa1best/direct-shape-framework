using Autodesk.Revit.DB;

namespace DirectShapeFramework.Utils;

internal class ViewFilterUtils
{
    
    private static readonly string _defaultViewFilterName = "DSF Filter";

    internal static void AddDsfFilter(Document doc, View3D view, string elementMark)
    {
        
        var categories = new List<BuiltInCategory>(1) { BuiltInCategory.OST_GenericModel };

        var filter = FindFilter(doc,_defaultViewFilterName) 
                     ?? GenerateFilter(doc, _defaultViewFilterName, categories, GenerateContainsFilterRule(elementMark));

        view.AddFilter(filter.Id);
        //setting overrides to filter
        var graphicsSettings = new OverrideGraphicSettings();
        graphicsSettings = graphicsSettings.SetSurfaceTransparency(25);
        var elements = new FilteredElementCollector(doc);
        var solidFillPattern = elements.OfClass(typeof(FillPatternElement)).Cast<FillPatternElement>().First(a => a.GetFillPattern().IsSolidFill);
        graphicsSettings = graphicsSettings.SetSurfaceBackgroundPatternId(solidFillPattern.Id);
        graphicsSettings = graphicsSettings.SetSurfaceBackgroundPatternColor(new Color(0, 128, 128));
        view.SetFilterOverrides(filter.Id, graphicsSettings);
    }

    private static List<FilterRule> GenerateContainsFilterRule(string contains)
    {
        return new List<FilterRule>
        {
            ParameterFilterRuleFactory.CreateContainsRule(
                new ElementId(BuiltInParameter.ALL_MODEL_MARK),
                contains, false)
        };
    }

    [CanBeNull]
    private static ParameterFilterElement FindFilter(Document doc, string parameterFilterName)
    {
        return new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement))
            .FirstOrDefault(x => x.Name == parameterFilterName) as ParameterFilterElement;
    }

    private static ParameterFilterElement GenerateFilter(Document doc, string name, IEnumerable<BuiltInCategory> categories, IEnumerable<FilterRule> rules)
    {
        return ParameterFilterElement.Create(doc,
            name,
            categories.Select(x => new ElementId(x)).ToList(),
            FilterRulesToLogicalAndFilter(rules));
    }

    private static ElementFilter FilterRulesToLogicalAndFilter(IEnumerable<FilterRule> filterRules)
    {
        return new LogicalAndFilter(filterRules.Select(r => new ElementParameterFilter(r)).Cast<ElementFilter>().ToList());
    }
}