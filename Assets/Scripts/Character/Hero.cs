namespace DungeonDraws.Character
{
    public class Hero : ACharacter
    {
        private void Start()
        {
            Init();
        }

        public override int Faction => 1;
    }
}