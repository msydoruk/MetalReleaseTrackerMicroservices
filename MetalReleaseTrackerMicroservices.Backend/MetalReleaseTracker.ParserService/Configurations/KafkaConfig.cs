namespace MetalReleaseTracker.ParserService.Configurations;

public class KafkaConfig
{
    public string BootstrapServers { get; set; }

    public string ParserServiceTopic { get; set; }

    public KafkaSecurityConfig Security { get; set; }
}