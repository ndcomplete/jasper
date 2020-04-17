using System;
using Jasper.Http.Model;
using LamarCodeGeneration;

namespace Jasper.Http.Routing.Codegen
{
    public class RelativePathFrame : RouteArgumentFrame
    {
        public RelativePathFrame(int position) : base(JasperRoute.RelativePath, position, typeof(string))
        {
        }

        public override void GenerateCode(GeneratedMethod method, ISourceWriter writer)
        {
            throw new NotImplementedException();
            // writer.Write(
            //     $"var {Variable.Usage} = {nameof(RouteHandler.ToRelativePath)}({Segments.Usage}, {Position});");
            // Next?.GenerateCode(method, writer);
        }
    }
}
