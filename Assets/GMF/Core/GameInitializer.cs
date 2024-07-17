namespace GMF
{

    public static class GameInitializer
    {
        public static void Initialize()
        {
            var gsm = Core.Services.GetRequiredService<IGameStateManager>();
            gsm.ChangeState(new InitializationState());
        }
    }

}

