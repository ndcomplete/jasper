using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Baseline;
using Baseline.Reflection;
using Jasper.Http.Routing.Codegen;
using Jasper.Util;
using LamarCodeGeneration;

namespace Jasper.Http.Routing
{
    // This is mostly tested through Storyteller specs
    public static class RouteBuilder
    {
        public static readonly IList<string> InputTypeNames = new List<string> {"input", "query", "message", "body"};

        public static IList<IRoutingRule> RoutingRules = new List<IRoutingRule>
            {new HomeEndpointRule(), new RootUrlRoutingRule(), new VerbMethodNames()};

        public static Route Build<T>(Expression<Action<T>> expression)
        {
            var method = ReflectionHelper.GetMethod(expression);
            return Build(typeof(T), method);
        }

        public static Route Build(Type handlerType, MethodInfo method)
        {
            var route = RoutingRules.FirstValue(x => x.DetermineRoute(handlerType, method));
            if (route == null)
                throw new InvalidOperationException(
                    $"Jasper does not know how to make an Http route from the method {handlerType.NameInCode()}.{method.Name}()");

            route.InputType = DetermineInputType(method);
            route.HandlerType = handlerType;
            route.Method = method;

            var hasPrimitives = method.GetParameters().Any(x =>
                x.ParameterType == typeof(string) || RoutingFrames.CanParse(x.ParameterType));

            if (hasPrimitives)
            {
                for (var i = 0; i < route.Segments.Count; i++)
                {
                    var current = route.Segments[i].SegmentPath;
                    var isParameter = current.StartsWith("{") && current.EndsWith("}");
                    var parameterName = current.TrimStart('{').TrimEnd('}');


                    var parameter = method.GetParameters().FirstOrDefault(x => x.Name == parameterName);
                    if (parameter != null)
                    {
                        var argument = new RouteArgument(parameter, i);
                        route.Segments[i] = argument;
                    }

                    if (isParameter && parameter == null)
                        throw new InvalidOperationException(
                            $"Required parameter '{current}' could not be resoved in method {handlerType.FullNameInCode()}.{method.Name}()");
                }
            }

            var spreads = method.GetParameters().Where(x => x.IsSpread()).ToArray();
            if (spreads.Length > 1)
                throw new InvalidOperationException(
                    $"An HTTP action method can only take in either '{Route.PathSegments}' or '{Route.RelativePath}', but not both. Error with action {handlerType.FullName}.{method.Name}()");

            var segments = route.Segments;
            if (spreads.Length == 1) segments = segments.Concat(new ISegment[] {new Spread(segments.Count)}).ToList();

            method.ForAttribute<RouteNameAttribute>(att => route.Name = att.Name);

            return route;
        }

        public static Type DetermineInputType(MethodInfo method)
        {
            var first = method.GetParameters().FirstOrDefault();
            if (first == null) return null;

            if (first.IsSpread()) return null;

            if (InputTypeNames.Contains(first.Name, StringComparer.OrdinalIgnoreCase)) return first.ParameterType;

            return first.ParameterType.IsInputTypeCandidate() ? first.ParameterType : null;
        }
    }
}
