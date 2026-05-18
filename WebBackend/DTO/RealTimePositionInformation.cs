namespace WebBackend.DTO
{
    public record realTimePositionInformation(
        double currentSeventhAxisPosition,
        List<double> toolPositionInUser,
        List<double> userPositionInWorld,
        List<double> userRotationInWorld,
        List<double> workpiecePosition
        )
    { };
}
