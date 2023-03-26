namespace DungeonDraws.Character
{
    public class Monster : ACharacter
    {
        private void Awake()
        {
            Init();
        }

        public override int Faction => 2;
    }
}