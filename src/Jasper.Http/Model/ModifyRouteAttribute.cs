using System;
using Jasper.Configuration;
using LamarCodeGeneration;

namespace JasperHttp.Model
{
    /// <summary>
    ///     Base class for custom attributes that modify route configurations
    /// </summary>
    public abstract class ModifyRouteAttribute : Attribute, IModifyChain<RouteChain>
    {
        public abstract void Modify(RouteChain chain, GenerationRules rules);
    }
}
