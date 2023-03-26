namespace DungeonDraws.Character
{
    public class Monster : ACharacter
    {
        private void Awake()
        {
            AwakeInternal();
        }

        private void Start()
        {
            StartInternal();
        }

        public override int Faction => 2;
    }
}