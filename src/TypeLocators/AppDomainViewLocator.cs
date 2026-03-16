using DevExpress.Mvvm.UI;
using System.Reflection;

namespace DevExpress.Mvvm;

public class AppDomainViewLocator : LocatorBase, IViewLocator
{
    public AppDomainViewLocator()
    {

    }

    protected override IEnumerable<Assembly> Assemblies => GetAssemblies();

    private static IEnumerable<Assembly> GetAssemblies()
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            yield return assembly;
        }
    }

    public string GetViewTypeName(Type type)
    {
        return ResolveTypeName(type, null);
    }

    public object ResolveView(string viewName)
    {
        var viewType = ResolveViewType(viewName);
        if (viewType != null)
        {
            return CreateInstance(viewType, viewName);
        }
        return CreateFallbackView(viewName);
    }

    public Type? ResolveViewType(string viewName)
    {
        return ResolveType(viewName, out _);
    }

    private static ViewLocatorExtensions.FallbackView CreateFallbackView(string documentType)
    {
        return new ViewLocatorExtensions.FallbackView() { Text = GetErrorMessage_CannotResolveViewType(documentType) };
    }

    private static string GetErrorMessage_CannotResolveViewType(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "ViewType is not specified.";
        if (ViewModelBase.IsInDesignMode)
            return $"[{name}]";
        return $"\"{name}\" type not found.";
    }
}
