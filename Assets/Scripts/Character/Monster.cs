namespace DungeonDraws.Character
{
    public class Monster : ACharacter
    {
        private void Start()
        {
            Init();
        }

        public override int Faction => 2;
    }
}