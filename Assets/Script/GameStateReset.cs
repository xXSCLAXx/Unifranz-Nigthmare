public static class GameStateReset
{
    public static void ResetAll()
    {
        GameOverController.ResetStatics();
        WinScreenController.ResetStatics();
        PCWindowController.ResetStatics();
        TaskNotesController.ResetStatics();
        PCTimer.ResetStatics();
        InformeController.ResetStatics();
        ComputerRoomController.ResetStatics();
        RouterController.ResetStatics();
        Aula306Controller.ResetStatics();
        WireMinigameController.ResetStatics();
    }
}
