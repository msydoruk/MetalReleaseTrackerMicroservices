using Autofac;
using Autofac.Features.Metadata;
using MetalReleaseTracker.ParserService.Configurations;
using MetalReleaseTracker.ParserService.Parsers.Implementation;
using MetalReleaseTracker.ParserService.Parsers.Interfaces;
using MetalReleaseTracker.ParserService.Parsers.Models;

namespace MetalReleaseTracker.ParserService.ServiceExtensions;

public static class ParserRegistrationExtension
{
    public static void AddParsers(this ContainerBuilder builder)
    {
        builder.RegisterType<OsmoseProductionsParser>()
            .As<IParser>()
            .WithMetadata<ParserMetadata>(m => m.For(meta => meta.DistributorCode, DistributorCode.OsmoseProductions));

        builder.RegisterType<DrakkarParser>()
            .As<IParser>()
            .WithMetadata<ParserMetadata>(m => m.For(meta => meta.DistributorCode, DistributorCode.Drakkar));

        builder.RegisterType<DarkerThanBlackRecordsParser>()
            .As<IParser>()
            .WithMetadata<ParserMetadata>(m => m.For(meta => meta.DistributorCode, DistributorCode.DarkThanBlackRecords));

        builder.Register<Func<DistributorCode, IParser>>(context =>
        {
            var metaParsers = context.Resolve<IEnumerable<Meta<IParser, ParserMetadata>>>();
            return distributorCode =>
            {
                var parser = metaParsers.FirstOrDefault(p => p.Metadata.DistributorCode == distributorCode);

                return parser?.Value ?? throw new NotSupportedException($"Parser for distributor '{distributorCode}' not found.");
            };
        });
    }
}
