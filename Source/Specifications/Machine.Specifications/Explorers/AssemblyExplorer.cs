using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Specifications.Factories;
using Machine.Specifications.Model;
using Machine.Specifications.Utility;

namespace Machine.Specifications.Explorers
{
  public class AssemblyExplorer
  {
    private readonly ContextFactory contextFactory;

    public AssemblyExplorer()
    {
      contextFactory = new ContextFactory();
    }

    public IEnumerable<Model.Context> FindContextsIn(Assembly assembly)
    {
      return EnumerateContextsIn(assembly).Select(x => CreateContextFrom(x));
    }

    public IEnumerable<Model.Context> FindContextsIn(Assembly assembly, string targetNamespace)
    {
      return EnumerateContextsIn(assembly)
        .Where(x => x.Namespace == targetNamespace)
        .Select(x => CreateContextFrom(x));
    }

    private Model.Context CreateContextFrom(Type type)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance);
    }

    private Model.Context CreateContextFrom(Type type, FieldInfo fieldInfo)
    {
      object instance = Activator.CreateInstance(type);
      return contextFactory.CreateContextFrom(instance, fieldInfo);
    }

    private static bool IsContext(Type type)
    {
      return HasSpecificationMembers(type);
    }

    private static bool HasSpecificationMembers(Type type)
    {
      return type.GetPrivateFieldsWith(typeof(It)).Any();
    }

    /*
    private static bool HasDescriptionAttribute(Type type)
    {
      return type.IsDefined(typeof(DescriptionAttribute), false);
    }
    */

    private static IEnumerable<Type> EnumerateContextsIn(Assembly assembly)
    {
      return assembly.GetExportedTypes().Where(IsContext);
    }

    public Model.Context FindContexts(Type type)
    {
      if (IsContext(type))
      {
        return CreateContextFrom(type);
      }

      return null;
    }

    public Model.Context FindContexts(FieldInfo info)
    {
      Type type = info.ReflectedType;
      if (IsContext(type))
      {
        return CreateContextFrom(type, info);
      }

      return null;
    }
  }
}
