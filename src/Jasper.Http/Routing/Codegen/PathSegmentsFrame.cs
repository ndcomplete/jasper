using System;
using Jasper.Http.Model;
using LamarCodeGeneration;

namespace Jasper.Http.Routing.Codegen
{
    public class PathSegmentsFrame : RouteArgumentFrame
    {
        public PathSegmentsFrame(int position) : base(JasperRoute.PathSegments, position, typeof(string[]))
        {
        }

        public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
        {
            throw new NotImplementedException();

            // writer.Write(
            //     $"var {Variable.Usage} = {nameof(RouteHandler.ToPathSegments)}({Segments.Usage}, {Position});");
            // Next?.GenerateCode(method, writer);
        }
    }
}
