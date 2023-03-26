namespace DungeonDraws.Character
{
    public class Hero : ACharacter
    {
        private void Awake()
        {
            Init();
        }

        public override int Faction => 1;
    }
}