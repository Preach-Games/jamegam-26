namespace DungeonDraws.Character
{
    public class Hero : ACharacter
    {
        private void Awake()
        {
            AwakeInternal();
        }

        private void Start()
        {
            StartInternal();
        }

        public override int Faction => 1;
    }
}